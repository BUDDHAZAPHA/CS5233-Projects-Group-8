using System;
using System.Collections.Generic;
using System.Linq;

using O2DESNet.Distributions;

namespace O2DESNet.Standard
{
    public class PatternGenerator : Sandbox<PatternGenerator.Statics>, IGenerator
    {
        public class Statics : IAssets
        {
            public string Id { get { return GetType().Name; } }
            /// <summary>
            /// By default it follow exponential distribution
            /// </summary>
            public double MeanHourlyRate { get; set; }
            /// <summary>
            /// A list of 24 seasonal factors, to be filled with 0s if not full
            /// All 0s or null means no seasonal effect
            /// </summary>
            public List<double> SeasonalFactorsHoursOfDay { get; set; }
            /// <summary>
            /// A list of 7 seasonal factors, to be filled with 0s if not full
            /// All 0s or null means no seasonal effect
            /// </summary>
            public List<double> SeasonalFactorsDaysOfWeek { get; set; }
            /// <summary>
            /// A list of 31 seasonal factors, to be filled with 0s if not full
            /// All 0s or null means no seasonal effect
            /// </summary>
            public List<double> SeasonalFactorsDaysOfMonth { get; set; }
            /// <summary>
            /// A list of 12 seasonal factors, to be filled with 0s if not full
            /// All 0s or null means no seasonal effect
            /// </summary>
            public List<double> SeasonalFactorsMonthsOfYear { get; set; }
            /// <summary>
            /// All 0s or null means no seasonal effect
            /// </summary>
            public List<double> SeasonalFactorsYears { get; set; }

            public List<(TimeSpan, List<double>)> CustomizedSeasonalFactors { get; set; }

            public PatternGenerator Sandbox(int seed = 0) { return new PatternGenerator(this, seed); }
        }

        #region Dyanmic Properties
        public DateTime? StartTime { get; private set; }
        public bool IsGeneratorOn { get; private set; }
        public int Count { get; private set; }
        private double PeakHourlyRate { get; set; }
        private List<double> AdjustedSeasonalFactorsHoursOfDay { get; set; }
        private List<double> AdjustedSeasonalFactorsDaysOfWeek { get; set; }
        private List<double> AdjustedSeasonalFactorsDaysOfMonth { get; set; }
        private List<double> AdjustedSeasonalFactorsMonthsOfYear { get; set; }
        private List<double> AdjustedSeasonalFactorsYears { get; set; }
        private List<(TimeSpan Interval, List<double> SeasonalFactors)> AdjustedCustomizedSeasonalFactors { get; set; }
        private double AdjMaxSeasonalFactorHoursOfDay { get; set; }
        private double AdjMaxSeasonalFactorDaysOfWeek { get; set; }
        private double AdjMaxSeasonalFactorDaysOfMonth { get; set; }
        private double AdjMaxSeasonalFactorMonthsOfYear { get; set; }
        private double AdjMaxSeasonalFactorYears { get; set; }
        private List<double> AdjMax_CustomizedSeasonalFactors { get; set; }
        private List<TimeSpan> CustomizedSeasonalRemainders { get; set; }
        #endregion

        #region Events
        public void Start()
        {
            if (!IsGeneratorOn)
            {
                Log("Start");
                IsGeneratorOn = true;
                StartTime = ClockTime;
                Count = 0;
                ScheduleToArrive();
            }
        }

        public void End()
        {
            if (IsGeneratorOn)
            {
                Log("End");
                IsGeneratorOn = false;
            }
        }

        private void ScheduleToArrive()
        {
            var time = ClockTime;
            while (true)
            {
                var hoursElapsed = Exponential.Sample(DefaultRS, 1 / PeakHourlyRate);
                var customizedIndices = new List<int>();
                for (int i = 0; i < AdjustedCustomizedSeasonalFactors.Count; i++)
                {
                    var (interval, factors) = AdjustedCustomizedSeasonalFactors[i];
                    var sum = CustomizedSeasonalRemainders[i].TotalHours + hoursElapsed;
                    var countIntervals = (int)Math.Floor(sum / interval.TotalHours);
                    CustomizedSeasonalRemainders[i] = TimeSpan.FromHours(sum % (interval.TotalHours * factors.Count));
                    customizedIndices.Add(countIntervals % factors.Count);
                }
                time = time.AddHours(hoursElapsed);
                if (DefaultRS.NextDouble() > AdjustedSeasonalFactorsHoursOfDay[time.Hour] / AdjMaxSeasonalFactorHoursOfDay) continue;
                if (DefaultRS.NextDouble() > AdjustedSeasonalFactorsDaysOfWeek[(int)time.DayOfWeek] / AdjMaxSeasonalFactorDaysOfWeek) continue;
                if (DefaultRS.NextDouble() > AdjustedSeasonalFactorsDaysOfMonth[time.Day - 1] * 31 / DateTime.DaysInMonth(time.Year, time.Month) / AdjMaxSeasonalFactorDaysOfMonth) continue;
                if (DefaultRS.NextDouble() > AdjustedSeasonalFactorsMonthsOfYear[time.Month - 1] / AdjMaxSeasonalFactorMonthsOfYear) continue;
                if (DefaultRS.NextDouble() > AdjustedSeasonalFactorsYears[(time.Year - 1) % AdjustedSeasonalFactorsYears.Count] / AdjMaxSeasonalFactorYears) continue;
                #region For customized seasonality
                bool reject = false;
                for (int i = 0; i < AdjustedCustomizedSeasonalFactors.Count; i++)
                {
                    var idx = customizedIndices[i];
                    var factors = AdjustedCustomizedSeasonalFactors[i].SeasonalFactors;
                    if (DefaultRS.NextDouble() > factors[idx] / AdjMax_CustomizedSeasonalFactors[i])
                    {
                        reject = true;
                        break;
                    }
                }
                if (reject) continue;
                #endregion
                Schedule(Arrive, time);
                break;
            }
        }

        private void Arrive()
        {
            if (IsGeneratorOn)
            {
                Log("Arrive");
                //Debug.WriteLine("{0}:\t{1}\tArrive", ClockTime, this);

                Count++;
                ScheduleToArrive();
                OnArrive.Invoke();
            }
        }

        public event Action OnArrive = () => { };
        #endregion

        public PatternGenerator(Statics assets, int seed = 0, string tag = null)
            : base(assets, seed, tag)
        {
            IsGeneratorOn = false;
            Count = 0;

            #region Normalize seasonal factors
            List<double> normalize(List<double> factors, int? nIntervals = null)
            {
                /// return default if undefined
                if (factors == null || factors.Sum() == 0)
                {
                    if (nIntervals != null) return Enumerable.Repeat(1d, nIntervals.Value).ToList();
                    else return new List<double> { 1 };
                }

                /// remove the negative part, replace with 0
                factors = factors.Select(f => Math.Max(0, f)).ToList();

                /// adjust the lenghth
                if (nIntervals != null)
                {
                    factors = factors.Take(nIntervals.Value).ToList();
                    while (factors.Count < nIntervals.Value) factors.Add(0);
                }

                /// standardize
                var sum = factors.Sum();
                return factors.Select(f => f / sum * factors.Count).ToList();
            }

            AdjustedSeasonalFactorsHoursOfDay = normalize(Assets.SeasonalFactorsHoursOfDay, 24);
            AdjustedSeasonalFactorsDaysOfWeek = normalize(Assets.SeasonalFactorsDaysOfWeek, 7);
            AdjustedSeasonalFactorsDaysOfMonth = normalize(Assets.SeasonalFactorsDaysOfMonth, 31);
            AdjustedSeasonalFactorsMonthsOfYear = normalize(Assets.SeasonalFactorsMonthsOfYear, 12);
            AdjustedSeasonalFactorsYears = normalize(Assets.SeasonalFactorsYears);
            AdjustedCustomizedSeasonalFactors = new List<(TimeSpan Interval, List<double> SeasonalFactors)>();
            if (Assets.CustomizedSeasonalFactors != null)
                foreach (var (interval, factors) in Assets.CustomizedSeasonalFactors)
                    AdjustedCustomizedSeasonalFactors.Add((interval, normalize(factors)));
            #endregion

            #region Set max factor and peak hourly rate
            AdjMaxSeasonalFactorHoursOfDay = AdjustedSeasonalFactorsHoursOfDay.Max();
            AdjMaxSeasonalFactorDaysOfWeek = AdjustedSeasonalFactorsDaysOfWeek.Max();
            AdjMaxSeasonalFactorDaysOfMonth = AdjustedSeasonalFactorsDaysOfMonth.Max();
            AdjMaxSeasonalFactorMonthsOfYear = AdjustedSeasonalFactorsMonthsOfYear.Max();
            AdjMaxSeasonalFactorYears = AdjustedSeasonalFactorsYears.Max();
            AdjMax_CustomizedSeasonalFactors = AdjustedCustomizedSeasonalFactors.Select(t => t.SeasonalFactors.Max()).ToList();
            PeakHourlyRate = Assets.MeanHourlyRate;
            PeakHourlyRate *= AdjMaxSeasonalFactorHoursOfDay;
            PeakHourlyRate *= AdjMaxSeasonalFactorDaysOfWeek;
            PeakHourlyRate *= AdjMaxSeasonalFactorDaysOfMonth;
            PeakHourlyRate *= AdjMaxSeasonalFactorMonthsOfYear;
            PeakHourlyRate *= AdjMaxSeasonalFactorYears;
            foreach (var max in AdjMax_CustomizedSeasonalFactors) PeakHourlyRate *= max;
            #endregion

            CustomizedSeasonalRemainders = AdjustedCustomizedSeasonalFactors.Select(t => new TimeSpan()).ToList();
        }

        protected override void WarmedUpHandler()
        {
            Count = 0;
        }

        public override void Dispose()
        {
            if (OnArrive != null)
                foreach (Action i in OnArrive.GetInvocationList()) OnArrive -= i;
        }

    }
}

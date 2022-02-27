using System;

namespace O2DESNet.Standard
{
    public class Generator : Sandbox<Generator.Statics>, IGenerator
    {
        /// <summary>
        /// Output event - Arrive
        /// </summary>
        public event Action OnArrive = () => { };

        public class Statics : IAssets
        {
            public string Id { get { return GetType().Name; } }
            public Func<Random, TimeSpan> InterArrivalTime { get; set; }
            public Generator Sandbox(int seed = 0) { return new Generator(this, seed); }
        }

        #region Dyanmic Properties
        /// <summary>
        /// Gets the start time.
        /// </summary>
        public DateTime? StartTime { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this generator is on.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this generator is on; otherwise, <c>false</c>.
        /// </value>
        public bool IsGeneratorOn { get; private set; }

        /// <summary>
        /// Number of loads generated
        /// </summary>
        public int Count { get; private set; } // number of loads generated   
        #endregion

        #region Events
        /// <summary>
        /// Input event - Start
        /// </summary>
        /// <exception cref="Exception">Inter-arrival time is null</exception>
        public void Start()
        {
            if (!IsGeneratorOn)
            {
                Log("Start");
                if (IsDebugMode) //Debug.WriteLine("{0}:\t{1}\tStart", ClockTime, this);
                if (Assets.InterArrivalTime == null) throw new Exception("Inter-arrival time is null");
                IsGeneratorOn = true;
                StartTime = ClockTime;
                Count = 0;
                ScheduleToArrive();
            }
        }

        /// <summary>
        /// Input event - End
        /// </summary>
        public void End()
        {
            if (IsGeneratorOn)
            {
                Log("End");
                if (IsDebugMode) //Debug.WriteLine("{0}:\t{1}\tEnd", ClockTime, this);
                IsGeneratorOn = false;
            }
        }

        private void ScheduleToArrive()
        {
            Schedule(Arrive, Assets.InterArrivalTime(DefaultRS));
        }

        private void Arrive()
        {
            if (IsGeneratorOn)
            {
                Log("Arrive");
                if (IsDebugMode) //Debug.WriteLine("{0}:\t{1}\tArrive", ClockTime, this);

                Count++;
                ScheduleToArrive();
                OnArrive.Invoke();
            }
        }


        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Generator"/> class.
        /// </summary>
        /// <param name="assets">The assets.</param>
        /// <param name="seed">The seed.</param>
        /// <param name="id">The identifier.</param>
        public Generator(Statics assets, int seed = 0, string id = null)
            : base(assets, seed, id)
        {
            IsGeneratorOn = false;
            Count = 0;
        }

        /// <summary>
        /// Warmeds up handler.
        /// </summary>
        protected override void WarmedUpHandler()
        {
            Count = 0;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (OnArrive != null)
                foreach (Action i in OnArrive.GetInvocationList()) OnArrive -= i;
        }

    }
}

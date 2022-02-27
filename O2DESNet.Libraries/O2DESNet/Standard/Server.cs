using System;
using System.Collections.Generic;
using System.Linq;

namespace O2DESNet.Standard
{
    public class Server : Sandbox<Server.Statics>, IServer
    {

        private readonly List<ILoad> _pendingToStartList = new List<ILoad>();
        private readonly HashSet<ILoad> _servingHSet = new HashSet<ILoad>();
        private readonly HashSet<ILoad> _pendingToDepartHSet = new HashSet<ILoad>();

        public class Statics : IAssets
        {
            /// <summary>
            /// Gets the identifier.
            /// </summary>
            public string Id => GetType().Name;

            /// <summary>
            /// Gets or sets the capacity.
            /// </summary>
            public double Capacity { get; set; }

            /// <summary>
            /// Gets or sets the service time.
            /// </summary>
            public Func<Random, ILoad, TimeSpan> ServiceTime { get; set; }
        }

        #region Dynamic Properties
        /// <summary>
        /// Gets the capacity.
        /// </summary>
        public double Capacity => Assets.Capacity;

        /// <summary>
        /// Gets the occupancy.
        /// </summary>
        public int Occupancy => _servingHSet.Count + _pendingToDepartHSet.Count;

        /// <summary>
        /// Gets the vacancy.
        /// </summary>
        public double Vacancy => Capacity - Occupancy;

        /// <summary>
        /// Gets the average n serving.
        /// </summary>
        public double AvgNServing => ServingHourCounter.AverageCount;

        /// <summary>
        /// Gets the average N occupying.
        /// </summary>
        public double AvgNOccupying => ServingHourCounter.AverageCount + PendingToDepartHourCounter.AverageCount;

        /// <summary>
        /// Utilization only consider serving loads (active)
        /// </summary>
        public double UtilServing => AvgNServing / Capacity;

        /// <summary>
        /// Utilization including both serving and served loads (active + passive)
        /// </summary>
        public double UtilOccupying => AvgNOccupying / Capacity;

        /// <summary>
        /// Gets the pending to start.
        /// </summary>
        public IReadOnlyList<ILoad> PendingToStart => _pendingToStartList.AsReadOnly();

        /// <summary>
        /// Gets the serving.
        /// </summary>
        public IReadOnlyList<ILoad> Serving => _servingHSet.ToList().AsReadOnly();

        /// <summary>
        /// Gets the pending to depart.
        /// </summary>
        public IReadOnlyList<ILoad> PendingToDepart => _pendingToDepartHSet.ToList().AsReadOnly();

        /// <summary>
        /// Gets or sets the serving hour counter.
        /// </summary>
        private HourCounter ServingHourCounter { get; set; }

        /// <summary>
        /// Gets or sets the pending to depart hour counter.
        /// </summary>
        private HourCounter PendingToDepartHourCounter { get; set; }

        #endregion

        #region Events
        /// <summary>
        /// Requests to start.
        /// </summary>
        /// <param name="load">The load.</param>
        public void RequestStart(ILoad load)
        {
            Log("Request to Start", load);
            if (IsDebugMode) //Debug.WriteLine("{0}:\t{1}\tRqstStart\t{2}", ClockTime, this, load);
            _pendingToStartList.Add(load);
            AttemptToStart();
        }

        private void AttemptToStart()
        {
            if (_pendingToStartList.Count > 0 && Vacancy > 0)
            {
                var load = _pendingToStartList.First();
                Log("Start", load);
                if (IsDebugMode) //Debug.WriteLine("{0}:\t{1}\tStart\t{2}", ClockTime, this, load);
                _pendingToStartList.RemoveAt(0);
                _servingHSet.Add(load);
                ServingHourCounter.ObserveChange(1, ClockTime);
                OnStarted.Invoke(load);
                Schedule(() => ReadyToDepart(load), Assets.ServiceTime(DefaultRS, load));
            }
        }

        private void ReadyToDepart(ILoad load)
        {
            Log("Ready to Depart", load);
            if (IsDebugMode) //Debug.WriteLine("{0}:\t{1}\tReadyToDepart\t{2}", ClockTime, this, load);
            _servingHSet.Remove(load);
            _pendingToDepartHSet.Add(load);
            ServingHourCounter.ObserveChange(-1, ClockTime);
            PendingToDepartHourCounter.ObserveChange(1, ClockTime);
            OnReadyToDepart.Invoke(load);
        }

        /// <summary>
        /// Departs the specified load.
        /// </summary>
        /// <param name="load">The load.</param>
        public void Depart(ILoad load)
        {
            if (_pendingToDepartHSet.Contains(load))
            {
                Log("Depart", load);
                if (IsDebugMode) //Debug.WriteLine("{0}:\t{1}\tDepart\t{2}", ClockTime, this, load);
                _pendingToDepartHSet.Remove(load);
                PendingToDepartHourCounter.ObserveChange(-1, ClockTime);
                AttemptToStart();
            }
        }

        public event Action<ILoad> OnStarted = Load => { };
        public event Action<ILoad> OnReadyToDepart = load => { };
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
        /// </summary>
        /// <param name="assets">The assets.</param>
        /// <param name="seed">The seed.</param>
        /// <param name="id">The identifier.</param>
        public Server(Statics assets, int seed = 0, string id = null)
            : base(assets, seed, id)
        {
            ServingHourCounter = AddHourCounter();
            PendingToDepartHourCounter = AddHourCounter();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (OnStarted != null)
                foreach (Action<ILoad> i in OnStarted.GetInvocationList()) OnStarted -= i;
            if (OnReadyToDepart != null)
                foreach (Action<ILoad> i in OnReadyToDepart.GetInvocationList()) OnReadyToDepart -= i;
        }
    }
}

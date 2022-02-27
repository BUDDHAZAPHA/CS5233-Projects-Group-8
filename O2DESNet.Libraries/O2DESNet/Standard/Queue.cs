using System;
using System.Collections.Generic;
using System.Linq;

namespace O2DESNet.Standard
{
    public class Queue : Sandbox, IQueue
    {
        private readonly List<ILoad> _queueingList = new List<ILoad>();
        private readonly List<ILoad> _pendingToEnqueueList = new List<ILoad>();

        public event Action<ILoad> OnEnqueued = load => { };

        #region Static Properties
        public double Capacity { get; private set; }
        #endregion

        #region Dynamic Properties        
        /// <summary>
        /// Gets the pending to enqueue.
        /// </summary>
        public IReadOnlyList<ILoad> PendingToEnqueue => _pendingToEnqueueList.AsReadOnly();

        /// <summary>
        /// Gets the queueing.
        /// </summary>
        public IReadOnlyList<ILoad> Queueing => _queueingList.AsReadOnly();

        /// <summary>
        /// Gets the occupancy.
        /// </summary>
        public int Occupancy => _queueingList.Count;

        /// <summary>
        /// Gets the vacancy.
        /// </summary>
        public double Vacancy => Capacity - Occupancy;

        /// <summary>
        /// Gets the utilization.
        /// </summary>
        public double Utilization => AvgNQueueing / Capacity;

        /// <summary>
        /// Average number of loads queueing
        /// </summary>
        public double AvgNQueueing => QueueingHourCounter.AverageCount;

        /// <summary>
        /// Gets or sets the queueing hour counter.
        /// </summary>
        private HourCounter QueueingHourCounter { get; set; }
        #endregion

        #region  Methods / Events
        /// <summary>
        /// Requests to enqueue.
        /// </summary>
        /// <param name="load">The load.</param>
        public void RequestEnqueue(ILoad load)
        {
            Log("RqstEnqueue");
            if (IsDebugMode) //Debug.WriteLine("{0}:\t{1}\tRqstEnqueue\t{2}", ClockTime, this, load);
            _pendingToEnqueueList.Add(load);
            AttemptToEnqueue();
        }

        /// <summary>
        /// Dequeues the specified load.
        /// </summary>
        /// <param name="load">The load.</param>
        public void Dequeue(ILoad load)
        {
            if (_queueingList.Contains(load))
            {
                Log("Dequeue", load);
                if (IsDebugMode) //Debug.WriteLine("{0}:\t{1}\tDequeue\t{2}", ClockTime, this, load);
                _queueingList.Remove(load);
                QueueingHourCounter.ObserveChange(-1, ClockTime);
                AttemptToEnqueue();
            }
        }

        private void AttemptToEnqueue()
        {
            if (_pendingToEnqueueList.Count > 0 && _queueingList.Count < Capacity)
            {
                var load = _pendingToEnqueueList.First();
                Log("Enqueue", load);
                if (IsDebugMode) //Debug.WriteLine("{0}:\t{1}\tEnqueue\t{2}", ClockTime, this, load);
                _queueingList.Add(load);
                _pendingToEnqueueList.RemoveAt(0);
                QueueingHourCounter.ObserveChange(1, ClockTime);
                OnEnqueued.Invoke(load);
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <param name="seed">The seed.</param>
        /// <param name="id">The identifier.</param>
        public Queue(double capacity, int seed = 0, string id = null)
            : base(seed, id)
        {
            Capacity = capacity;
            QueueingHourCounter = AddHourCounter();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (OnEnqueued != null)
                foreach (Action<ILoad> i in OnEnqueued.GetInvocationList()) OnEnqueued -= i;
        }
    }
}

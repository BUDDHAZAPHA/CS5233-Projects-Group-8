using System;
using System.Collections.Generic;

namespace O2DESNet
{
    public interface IQueue : ISandbox 
    {
        /// <summary>
        /// Gets the capacity.
        /// </summary>
        double Capacity { get; }

        /// <summary>
        /// Gets the pending to enqueue.
        /// </summary>
        IReadOnlyList<ILoad> PendingToEnqueue { get; }

        /// <summary>
        /// Gets the queueing.
        /// </summary>
        IReadOnlyList<ILoad> Queueing { get; }

        /// <summary>
        /// Gets the occupancy.
        /// </summary>
        int Occupancy { get; }

        /// <summary>
        /// Gets the vacancy.
        /// </summary>
        double Vacancy { get; }

        /// <summary>
        /// Gets the utilization.
        /// </summary>
        double Utilization { get; }

        /// <summary>
        /// Average number of loads queueing
        /// </summary>
        double AvgNQueueing { get; }

        /// <summary>
        /// Requests to enqueue.
        /// </summary>
        /// <param name="load">The load.</param>
        void RequestEnqueue(ILoad load);

        /// <summary>
        /// Dequeues the specified load.
        /// </summary>
        /// <param name="load">The load.</param>
        void Dequeue(ILoad load);

        /// <summary>
        /// Occurs when [on enqueued].
        /// </summary>
        event Action<ILoad> OnEnqueued;
    }
}

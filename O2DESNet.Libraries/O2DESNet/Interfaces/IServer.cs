using System;
using System.Collections.Generic;

namespace O2DESNet
{
    public interface IServer : ISandbox 
    {
        /// <summary>
        /// Gets the capacity.
        /// </summary>
        double Capacity { get; }

        /// <summary>
        /// Gets the occupancy.
        /// </summary>
        int Occupancy { get; }

        /// <summary>
        /// Gets the vacancy.
        /// </summary>
        double Vacancy { get; }

        /// <summary>
        /// Gets the average n serving.
        /// </summary>
        double AvgNServing { get; }

        /// <summary>
        /// Gets the average N occupying.
        /// </summary>
        double AvgNOccupying { get; }

        /// <summary>
        /// Utilization only consider serving loads (active)
        /// </summary>
        double UtilServing { get; }

        /// <summary>
        /// Utilization including both serving and served loads (active + passive)
        /// </summary>
        double UtilOccupying { get; }

        /// <summary>
        /// Gets the pending to start.
        /// </summary>
        IReadOnlyList<ILoad> PendingToStart { get; }

        /// <summary>
        /// Gets the serving.
        /// </summary>
        IReadOnlyList<ILoad> Serving { get; }

        /// <summary>
        /// Gets the pending to depart.
        /// </summary>
        IReadOnlyList<ILoad> PendingToDepart { get; }

        /// <summary>
        /// Requests to start.
        /// </summary>
        /// <param name="load">The load.</param>
        void RequestStart(ILoad load);

        /// <summary>
        /// Departs the specified load.
        /// </summary>
        /// <param name="load">The load.</param>
        void Depart(ILoad load);

        /// <summary>
        /// Occurs when [on started].
        /// </summary>
        event Action<ILoad> OnStarted;

        /// <summary>
        /// Occurs when [on ready to depart].
        /// </summary>
        event Action<ILoad> OnReadyToDepart;
    }
}

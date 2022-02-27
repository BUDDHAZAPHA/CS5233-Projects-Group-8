using Serilog;
using System;
using System.Collections.Generic;

namespace O2DESNet
{
    /// <summary>
    /// Sandbox Interface
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface ISandbox : ILoad, IDisposable
    {
        /// <summary>
        /// Gets the pointer.
        /// </summary>
        Pointer Pointer { get; }

        /// <summary>
        /// Gets the seed.
        /// </summary>
        int Seed { get; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        ISandbox Parent { get; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        IReadOnlyList<ISandbox> Children { get; }

        /// <summary>
        /// Gets the clock time.
        /// </summary>
        DateTime ClockTime { get; }

        /// <summary>
        /// Gets the head event time.
        /// </summary>
        DateTime? HeadEventTime { get; }

        /// <summary>
        /// Gets or sets the log file.
        /// </summary>
        //string LogFile { get; set; }

        /// <summary>
        /// Sets the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        void SetLogger(ILogger logger);

        /// <summary>
        /// Gets or sets a value indicating whether [debug mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [debug mode]; otherwise, <c>false</c>.
        /// </value>
        bool IsDebugMode { get; set; }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        /// <returns></returns>
        bool Run();

        /// <summary>
        /// Runs the specified event count.
        /// </summary>
        /// <param name="eventCount">The event count.</param>
        bool Run(int eventCount);

        /// <summary>
        /// Runs the specified terminate.
        /// </summary>
        /// <param name="terminate">The terminate.</param>
        bool Run(DateTime terminate);

        /// <summary>
        /// Runs the specified duration.
        /// </summary>
        /// <param name="duration">The duration.</param>
        bool Run(TimeSpan duration);

        /// <summary>
        /// Runs the specified speed.
        /// </summary>
        /// <param name="speed">The speed.</param>

        bool Run(double speed);

        /// <summary>
        /// Warms up.
        /// </summary>
        /// <param name="till">The till.</param>
        bool WarmUp(DateTime till);

        /// <summary>
        /// Warms up.
        /// </summary>
        /// <param name="period">The period.</param>
        bool WarmUp(TimeSpan period);

        /// <summary>
        /// Gets a value indicating whether this instance is simulation running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is simulation running; otherwise, <c>false</c>.
        /// </value>
        bool IsSimulationRunning { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is cancel request.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is cancel request; otherwise, <c>false</c>.
        /// </value>
        bool IsCancelRequest { get; }

        /// <summary>
        /// Resets this simulation.
        /// </summary>
        void Reset();

        /// <summary>
        /// Cancels this simulation.
        /// </summary>
        void Cancel();
    }
}

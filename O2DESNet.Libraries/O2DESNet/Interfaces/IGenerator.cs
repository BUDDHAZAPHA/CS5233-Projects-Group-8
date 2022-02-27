using System;

namespace O2DESNet
{
    public interface IGenerator : ISandbox
    {
        /// <summary>
        /// Gets the start time.
        /// </summary>
        DateTime? StartTime { get; }

        /// <summary>
        /// Gets a value indicating whether this generator is on.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this generator is on; otherwise, <c>false</c>.
        /// </value>
        bool IsGeneratorOn { get; }

        /// <summary>
        /// Number of loads generated
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Input event - Start
        /// </summary>
        void Start();

        /// <summary>
        /// Input event - End
        /// </summary>
        void End();

        /// <summary>
        /// Output event - Arrive
        /// </summary>
        event Action OnArrive;
    }
}

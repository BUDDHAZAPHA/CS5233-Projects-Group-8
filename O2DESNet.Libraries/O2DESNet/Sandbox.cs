using Serilog;

namespace O2DESNet
{
    /// <summary>
    /// Sandbox
    /// </summary>
    /// <seealso cref="O2DESNet.SandboxBase" />
    public class Sandbox : SandboxBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sandbox"/> class.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <param name="id">The identifier.</param>
        public Sandbox(int seed = 0, string id = null)
            : base(seed, id, Pointer.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sandbox"/> class.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="pointer">The pointer.</param>
        public Sandbox( int seed, string id, Pointer pointer)
            : base(seed, id, pointer) { }

    }

    /// <summary>
    /// Sandbox[TAssets]
    /// </summary>
    /// <typeparam name="TAssets">The type of the assets.</typeparam>
    /// <seealso cref="O2DESNet.SandboxBase" />
    public class Sandbox<TAssets> : SandboxBase<TAssets> where TAssets : IAssets
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sandbox{TAssets}"/> class.
        /// </summary>
        /// <param name="assets">The assets.</param>
        public Sandbox(ILogger logger, TAssets assets) : base(assets)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sandbox{TAssets}"/> class.
        /// </summary>
        /// <param name="assets">The assets.</param>
        /// <param name="seed">The seed.</param>
        public Sandbox(TAssets assets, int seed) :
            base(assets, seed, null, Pointer.Empty)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sandbox{TAssets}"/> class.
        /// </summary>
        /// <param name="assets">The assets.</param>
        /// <param name="seed">The seed.</param>
        /// <param name="id">The identifier.</param>
        public Sandbox(TAssets assets, int seed, string id) :
            base(assets, seed, id, Pointer.Empty)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sandbox{TAssets}"/> class.
        /// </summary>
        /// <param name="assets">The assets.</param>
        /// <param name="seed">The seed.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="pointer">The pointer.</param>
        public Sandbox(TAssets assets, int seed, string id, Pointer pointer) :
            base(assets, seed, id, pointer)
        { }
    }

}

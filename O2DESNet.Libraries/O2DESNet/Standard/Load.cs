namespace O2DESNet.Standard
{
    public class Load : ILoad
    {
        private static int _count = 0;

        /// <summary>
        /// Gets the index.
        /// </summary>
        public int Index { get; } = _count++;

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public virtual string Id => $"{GetType().Name}#{Index}";

        /// <summary>
        /// Resets the index.
        /// </summary>
        public void ResetIndex() => _count = 0;

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() { return Id; }
    }
}

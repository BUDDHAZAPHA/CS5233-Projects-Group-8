namespace O2DESNet.Drawing
{
    public struct Font
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="size">The size.</param>
        /// <param name="style">The style.</param>
        public Font(string name, float size, FontStyle style) : this()
        {
            Name = name;
            Size = size;

            Bold = (style & FontStyle.Bold) == FontStyle.Bold;
            Italic = (style & FontStyle.Italic) == FontStyle.Italic;
            Underline = (style & FontStyle.Underline) == FontStyle.Underline;
            Strikeout = (style & FontStyle.Strikeout) == FontStyle.Strikeout;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        public float Size { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Font"/> is strikeout.
        /// </summary>
        /// <value>
        ///   <c>true</c> if strikeout; otherwise, <c>false</c>.
        /// </value>
        public bool Strikeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Font"/> is bold.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bold; otherwise, <c>false</c>.
        /// </value>
        public bool Bold { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Font"/> is underline.
        /// </summary>
        /// <value>
        ///   <c>true</c> if underline; otherwise, <c>false</c>.
        /// </value>
        public bool Underline { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Font"/> is italic.
        /// </summary>
        /// <value>
        ///   <c>true</c> if italic; otherwise, <c>false</c>.
        /// </value>
        public bool Italic { get; set; }
    }
}

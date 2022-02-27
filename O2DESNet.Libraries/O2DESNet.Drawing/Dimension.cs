namespace O2DESNet.Drawing
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>
    /// Total dimension as (X, Y) in meters (including margin), 
    /// for which X is the direction on the bays, and Y is the direction on the rows
    /// </summary>
    [Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public struct Dimension
    {
        private double _x;
        private double _y;

        public static readonly Dimension Empty = new Dimension();

        /// <summary>
        /// Initializes a new instance of the <see cref="Dimension"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Dimension(double x, double y)
        {
            _x = x;
            _y = y;
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="dim1">The dim1.</param>
        /// <param name="dim2">The dim2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Dimension operator +(Dimension dim1, Dimension dim2)
        {
            return Add(dim1, dim2);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="dim1">The dim1.</param>
        /// <param name="dim2">The dim2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Dimension operator -(Dimension dim1, Dimension dim2)
        {
            return Subtract(dim1, dim2);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Dimension left, Dimension right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Dimension left, Dimension right)
        {
            return !(left == right);
        }

        private static Dimension Add(Dimension dim1, Dimension dim2)
        {
            return new Dimension(dim1.X + dim2.X, dim1.Y + dim2.Y);
        }

        private static Dimension Subtract(Dimension dim1, Dimension dim2)
        {
            return new Dimension(dim1.X - dim2.X, dim1.Y - dim2.Y);
        }

        /// <summary>
        /// Rounds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public static Dimension Round(Dimension value)
        {
            return new Dimension((int)Math.Round(value.X), (int)Math.Round(value.Y));
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        [Browsable(false)]
        public bool IsEmpty => _x == 0d && _y == 0d;

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        public double X { get => _x; set => _x = value; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        public double Y { get => _y; set => _y = value; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Dimension))
                return false;

            Dimension comp = (Dimension)obj;
            return comp.X == X &&
                   comp.Y == Y &&
                   comp.GetType().Equals(GetType());
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{X={0}, Y={1}}}", _x, _y);
        }
    }
}

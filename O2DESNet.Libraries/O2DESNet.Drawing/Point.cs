using System;
using System.ComponentModel;
using System.Globalization;

namespace O2DESNet.Drawing
{
    /// <summary>
    /// Represents a point in 2D coordinate space
    /// (float precision floating-point coordinates)
    /// </summary>
    [Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public struct Point
    {
        /// <summary>
        /// Empty Point
        /// </summary>
        public static readonly Point Empty = new Point();

        private float _x;
        private float _y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Point(float x, float y)
        {
            _x = x;
            _y = y;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        [Browsable(false)]
        public bool IsEmpty => _x == 0f && _y == 0f;

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        public float X
        {
            get => _x;
            set => _x = value;
        }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        public float Y
        {
            get => _y;
            set => _y = value;
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="pt">The pt.</param>
        /// <param name="sz">The sz.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Point operator +(Point pt, Size sz)
        {
            return Add(pt, sz);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Point operator +(Point point1, Point point2)
        {
            return Add(point1, point2);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="pt">The pt.</param>
        /// <param name="sz">The sz.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Point operator -(Point pt, Size sz)
        {
            return Subtract(pt, sz);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Point operator -(Point point1, Point point2)
        {
            return Subtract(point1, point2);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Point left, Point right)
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
        public static bool operator !=(Point left, Point right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Adds the specified point with size.
        /// </summary>
        /// <param name="pt">The pt.</param>
        /// <param name="sz">The sz.</param>
        public static Point Add(Point pt, Size sz)
        {
            return new Point(pt.X + sz.Width, pt.Y + sz.Height);
        }

        /// <summary>
        /// Adds the specified point with other point.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        public static Point Add(Point point1, Point point2)
        {
            return new Point(point1.X + point2.X, point1.Y + point2.Y);
        }

        /// <summary>
        /// Subtracts the specified pt.
        /// </summary>
        /// <param name="pt">The pt.</param>
        /// <param name="sz">The sz.</param>
        public static Point Subtract(Point pt, Size sz)
        {
            return new Point(pt.X - sz.Width, pt.Y - sz.Height);
        }

        /// <summary>
        /// Subtracts the specified point with other point.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>

        public static Point Subtract(Point point1, Point point2)
        {
            return new Point(point1.X - point2.X, point1.Y - point2.Y);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Point)) return false;
            Point comp = (Point)obj;
            return
            comp.X == X &&
            comp.Y == Y &&
            comp.GetType().Equals(GetType());
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
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

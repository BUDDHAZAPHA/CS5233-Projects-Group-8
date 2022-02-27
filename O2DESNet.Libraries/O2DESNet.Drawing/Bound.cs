using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace O2DESNet.Drawing
{
    /// <summary>
    /// Structure for creating a bounding rectangle.
    /// Bounding rectangle [ X, Y, Width, Height ]
    /// Bounding rectangle [ Point, Size ]
    /// </summary>
    [Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public struct Bound
    {
        public static readonly Bound Empty = new Bound();

        private float _x;
        private float _y;
        private float _width;
        private float _height;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bound"/> struct.
        /// </summary>
        /// <param name="yTop">The y top.</param>
        /// <param name="xRight">The x right.</param>
        /// <param name="yBottom">The y bottom.</param>
        /// <param name="xLeft">The x left.</param>
        public Bound(float x, float y, float width, float height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bound"/> struct.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="size">The size.</param>
        public Bound(Point point, Size size)
        {
            _x = point.X;
            _y = point.Y;
            _width = size.Width;
            _height = size.Height;
        }

        /// <summary>
        /// Gets or sets the x point.
        /// </summary>
        public float X { get => _x; set => _x = value; }

        /// <summary>
        /// Gets or sets the y point.
        /// </summary>
        public float Y { get => _y; set => _y = value; }

        /// <summary>
        /// Gets or sets the bounding width.
        /// </summary>
        public float Width { get => _width; set => _width = value; }

        /// <summary>
        /// Gets or sets the bounding height.
        /// </summary>
        public float Height { get => _height; set => _height = value; }

        /// <summary>
        /// Gets the bounding rectangle top left point.
        /// </summary>
        public Point TopLeft { get => new Point(_x, _y); }

        /// <summary>
        /// Gets the bounding rectangle bottom right point.
        /// </summary>
        public Point BottomRight { get => new Point(_x + _width, _y + _height); }

        /// <summary>
        /// Creates a intersection bound part between this Bound and otherBound.
        /// </summary>
        /// <param name="otherBound">The other bound.</param>
        public void Intersect(Bound otherBound)
        {
            Bound result = Bound.Intersect(otherBound, this);

            X = result.X;
            Y = result.Y;
            Width = result.Width;
            Height = result.Height;
        }

        /// <summary>
        /// Creates a intersection bound part between Bound a and Bound b.
        /// If there is no intersection, Bound.Empty is returned.
        /// </summary>
        /// <param name="a">The bound a.</param>
        /// <param name="b">The bound b.</param>
        [Pure]
        public static Bound Intersect(Bound a, Bound b)
        {
            float x1 = Math.Max(a.X, b.X);
            float x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            float y1 = Math.Max(a.Y, b.Y);
            float y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

            if (x2 >= x1 && y2 >= y1)
            {
                return new Bound(x1, y1, x2 - x1, y2 - y1);
            }

            return Bound.Empty;
        }

        /// <summary>
        /// Determines if this bound intersects with bound.
        /// </summary>
        /// <param name="otherBound">The other Bound.</param>
        /// <returns>
        /// [True] if the bound intersected with th other bound
        /// </returns>
        [Pure]
        public bool IntersectsWith(Bound otherBound)
        {
            return (otherBound.X < X + Width) &&
                   (X < (otherBound.X + otherBound.Width)) &&
                   (otherBound.Y < Y + Height) &&
                   (Y < otherBound.Y + otherBound.Height);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        [Browsable(false)]
        public bool IsEmpty => (Width <= 0) || (Height <= 0);

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left bound.</param>
        /// <param name="right">The right bound.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Bound left, Bound right)
        {
            return (left.X == right.X
                 && left.Y == right.Y
                 && left.Width == right.Width
                 && left.Height == right.Height);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left bound.</param>
        /// <param name="right">The right bound.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Bound left, Bound right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return
                "{X=" + X.ToString(CultureInfo.CurrentCulture) +
                ",Y=" + Y.ToString(CultureInfo.CurrentCulture) +
                ",Width=" + Width.ToString(CultureInfo.CurrentCulture) +
                ",Height=" + Height.ToString(CultureInfo.CurrentCulture) +
                "}";
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
            if (!(obj is Bound coord))
                return false;

            return (coord._x == _x) &&
                   (coord._width == _width) &&
                   (coord._y == _y) &&
                   (coord._height == _height);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return unchecked(
                (int)((UInt32)X ^
                (((UInt32)Y << 13) | ((UInt32)Y >> 19)) ^
                (((UInt32)Width << 26) | ((UInt32)Width >> 6)) ^
                (((UInt32)Height << 7) | ((UInt32)Height >> 25))));
        }
    }
}

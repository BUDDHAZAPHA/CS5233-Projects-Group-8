using System;
using System.ComponentModel;
using System.Globalization;

namespace O2DESNet.Drawing
{
    /// <summary>
    /// Respect to the origin of the Block, Y-top, X-right, Y-bottom, X-left
    /// </summary>
    [Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public struct Coord
    {
        public static readonly Coord Empty = new Coord();

        private float _yTop, _xRight, _yBottom, _xLeft;

        /// <summary>
        /// Initializes a new instance of the <see cref="Coord"/> struct.
        /// </summary>
        /// <param name="yTop">The y top.</param>
        /// <param name="xRight">The x right.</param>
        /// <param name="yBottom">The y bottom.</param>
        /// <param name="xLeft">The x left.</param>
        public Coord(float yTop, float xRight, float yBottom, float xLeft)
        {
            _yTop = yTop;
            _xRight = xRight;
            _yBottom = yBottom;
            _xLeft = xLeft;
        }

        /// <summary>
        /// Gets or sets the y top.
        /// </summary>
        public float YTop { get => _yTop; set => _yTop = value; }
        /// <summary>
        /// Gets or sets the x right.
        /// </summary>
        public float XRight { get => _xRight; set => _xRight = value; }
        /// <summary>
        /// Gets or sets the y bottom.
        /// </summary>
        public float YBottom { get => _yBottom; set => _yBottom = value; }
        /// <summary>
        /// Gets or sets the x left.
        /// </summary>
        public float XLeft { get => _xLeft; set => _xLeft = value; }

        /// <summary>
        /// Gets the width.
        /// </summary>
        public float Width { get => Math.Abs(_xRight - _xLeft); }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public float Height { get => Math.Abs(_yBottom - _yTop); }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        [Browsable(false)]
        public bool IsEmpty => (Width <= 0) || (Height <= 0);

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Coord left, Coord right)
        {
            return (left.XRight == right.XRight
                     && left.YBottom == right.YBottom
                     && left.Width == right.Width
                     && left.Height == right.Height);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Coord left, Coord right)
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
                "{X=" + XRight.ToString(CultureInfo.CurrentCulture) +
                ",Y=" + YBottom.ToString(CultureInfo.CurrentCulture) +
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
            if (!(obj is Coord coord))
                return false;

            return (coord._yTop == _yTop) &&
                   (coord._yBottom == _yBottom) &&
                   (coord._xRight == _xRight) &&
                   (coord._xLeft == _xLeft);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return unchecked((int)((UInt32)XRight ^
            (((UInt32)YBottom << 13) | ((UInt32)YBottom >> 19)) ^
            (((UInt32)Width << 26) | ((UInt32)Width >> 6)) ^
            (((UInt32)Height << 7) | ((UInt32)Height >> 25))));
        }
    }
}

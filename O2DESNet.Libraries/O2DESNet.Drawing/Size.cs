using System;
using System.ComponentModel;
using System.Globalization;

namespace O2DESNet.Drawing
{
    /// <summary>
    /// Represents a dimension in 2D coordinate space
    /// </summary>
    [Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public struct Size
    {
        /// <summary>
        /// Empty Size
        /// </summary>
        public static readonly Size Empty = new Size();

        private float _width;
        private float _height;

        /// <summary>
        /// Initializes a new instance of the <see cref="Size"/> struct.
        /// </summary>
        /// <param name="size">The size.</param>
        public Size(Size size)
        {
            _width = size._width;
            _height = size._height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Size"/> struct.
        /// </summary>
        /// <param name="pt">The pt.</param>
        public Size(Point pt)
        {
            _width = pt.X;
            _height = pt.Y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Size"/> struct.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Size(float width, float height)
        {
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="sz1">The SZ1.</param>
        /// <param name="sz2">The SZ2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Size operator +(Size sz1, Size sz2)
        {
            return Add(sz1, sz2);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="sz1">The SZ1.</param>
        /// <param name="sz2">The SZ2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Size operator -(Size sz1, Size sz2)
        {
            return Subtract(sz1, sz2);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="sz1">The SZ1.</param>
        /// <param name="sz2">The SZ2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Size sz1, Size sz2)
        {
            return sz1.Width == sz2.Width && sz1.Height == sz2.Height;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="sz1">The SZ1.</param>
        /// <param name="sz2">The SZ2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Size sz1, Size sz2)
        {
            return !(sz1 == sz2);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Size"/> to <see cref="Point"/>.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator Point(Size size)
        {
            return new Point(size.Width, size.Height);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        [Browsable(false)]
        public bool IsEmpty => _width == 0 && _height == 0;

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public float Width
        {
            get => _width;
            set => _width = value;
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public float Height
        {
            get => _height;
            set => _height = value;
        }

        /// <summary>
        /// Adds the specified SZ1.
        /// </summary>
        /// <param name="sz1">The SZ1.</param>
        /// <param name="sz2">The SZ2.</param>
        public static Size Add(Size sz1, Size sz2)
        {
            return new Size(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }

        /// <summary>
        /// Subtracts the specified SZ1.
        /// </summary>
        /// <param name="sz1">The SZ1.</param>
        /// <param name="sz2">The SZ2.</param>
        public static Size Subtract(Size sz1, Size sz2)
        {
            return new Size(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
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
            if (!(obj is Size))
                return false;

            Size comp = (Size)obj;

            return (comp.Width == Width) &&
            (comp.Height == Height) &&
            (comp.GetType().Equals(GetType()));
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
        /// Converts to point.
        /// </summary>
        public Point ToPoint()
        {
            return (Point)this;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "{Width=" + _width.ToString(CultureInfo.CurrentCulture) + ", Height=" + _height.ToString(CultureInfo.CurrentCulture) + "}";
        }
    }
}

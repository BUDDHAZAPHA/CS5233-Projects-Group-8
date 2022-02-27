using System;
using System.ComponentModel;
using System.Globalization;

namespace O2DESNet
{
    [Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public struct Pointer
    {
        /// <summary>
        /// Empty Pointer
        /// </summary>
        public static readonly Pointer Empty = new Pointer();

        private double _x;
        private double _y;
        private double _angle;
        private bool _flipped;

        /// <summary>
        /// Gets the x.
        /// </summary>
        public double X => _x;

        /// <summary>
        /// Gets the y.
        /// </summary>
        public double Y => _y;

        /// <summary>
        /// Gets the angle.
        /// </summary>
        public double Angle => _angle;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Pointer"/> is flipped.
        /// </summary>
        /// <value>
        ///   <c>true</c> if flipped; otherwise, <c>false</c>.
        /// </value>
        public bool Flipped => _flipped;

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        [Browsable(false)]
        public bool IsEmpty => _x == 0d && _y == 0d && _angle == 0d && _flipped == false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pointer"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="flipped">if set to <c>true</c> [flipped].</param>
        public Pointer(double x, double y, double angle, bool flipped)
        {
            _x = x;
            _y = y;
            _angle = angle;
            _flipped = flipped;
        }

        /// <summary>
        /// Super position of two pointer
        /// </summary>
        public static Pointer operator *(Pointer inner, Pointer outter)
        {
            var radius = outter.Angle / 180 * Math.PI;
            return new Pointer(
                x: inner.X * Math.Cos(radius) - inner.Y * Math.Sin(radius) + outter.X,
                y: inner.Y * Math.Cos(radius) + inner.X * Math.Sin(radius) + outter.Y,
                angle: (outter.Angle + inner.Angle) % 360,
                flipped: outter.Flipped ^ inner.Flipped
            );
        }

        /// <summary>
        /// Get the inner pointer
        /// </summary>
        public static Pointer operator /(Pointer product, Pointer outter)
        {
            return product * new Pointer(x: -outter.X, y: -outter.Y, 0, false)
                * new Pointer(0, 0, angle: -outter.Angle, flipped: outter.Flipped);
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
            if (!(obj is Pointer)) return false;
            Pointer comp = (Pointer)obj;
            return
            comp.X == X &&
            comp.Y == Y &&
            comp.Angle == Angle &&
            comp.Flipped == Flipped &&
            comp.GetType().Equals(GetType());
        }

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
            return string.Format(CultureInfo.CurrentCulture,
                "{{X={0}, Y={1}, Angle={2}, Flipped={3}}}", _x, _y, _angle, _flipped);
        }
    }
}
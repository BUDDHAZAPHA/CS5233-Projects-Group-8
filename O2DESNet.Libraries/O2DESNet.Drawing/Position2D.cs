using System;
using System.ComponentModel;
using System.Globalization;

namespace O2DESNet.Drawing
{
    /// <summary>
    /// Represents a position in 2D and 3D coordinate space
    /// (float precision floating-point coordinates)
    /// </summary>
    [Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public struct Position2D
    {
        /// <summary>
        /// Empty Position
        /// </summary>
        public static readonly Position2D Empty = new Position2D();

        private float _x;
        private float _y;
        private float _heading;

        /// <summary>
        /// Initializes a new instance of the <see cref="Position2D"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Position2D(float x, float y)
        {
            _x = x;
            _y = y;
            _heading = 0f;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Position2D" /> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="heading">The heading agnle in degrees/radians.</param>
        public Position2D(float x, float y, float heading)
        {
            _x = x;
            _y = y;
            _heading = heading;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        [Browsable(false)]
        public bool IsEmpty => _x == 0f && _y == 0f && _heading == 0f;

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
        /// Gets or sets the heading angle in degrees/radians.
        /// </summary>
        public float Heading
        {
            get => _heading;
            set => _heading = value;
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="pos1">The position 1.</param>
        /// <param name="pos2">The position 2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Position2D operator +(Position2D pos1, Position2D pos2)
        {
            return Add(pos1, pos2);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="pos1">The position 1.</param>
        /// <param name="pos2">The position 2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Position2D operator -(Position2D pos1, Position2D pos2)
        {
            return Subtract(pos1, pos2);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Position2D left, Position2D right)
        {
            return left.X == right.X && left.Y == right.Y && left.Heading == right.Heading;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Position2D left, Position2D right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Adds the specified pt.
        /// </summary>
        /// <param name="pos1">The position 1.</param>
        /// <param name="pos2">The position 2.</param>
        public static Position2D Add(Position2D pos1, Position2D pos2)
        {
            return new Position2D(pos1.X + pos2.X, pos1.Y + pos2.Y);
        }

        /// <summary>
        /// Subtracts the specified pt.
        /// </summary>
        /// <param name="pos1">The position 1.</param>
        /// <param name="pos2">The position 2.</param>
        public static Position2D Subtract(Position2D pos1, Position2D pos2)
        {
            return new Position2D(pos1.X - pos2.X, pos1.Y - pos2.Y);
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
            if (!(obj is Position2D)) return false;
            Position2D comp = (Position2D)obj;
            return
            comp.X == X &&
            comp.Y == Y &&
            comp.Heading == Heading &&
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
            return string.Format(CultureInfo.CurrentCulture, "{{X={0}, Y={1}, Heading={2}}}", _x, _y, _heading);
        }
    }
}

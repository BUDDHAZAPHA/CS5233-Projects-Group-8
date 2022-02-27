namespace O2DESNet.Drawing
{
    using System;
    using System.Text;

    [Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public struct Color
    {
        public static readonly Color Empty = new Color();

        private static short StateARGBValueValid = 0x0002;
        private static short StateValueMask = (short)(StateARGBValueValid);
        private static long NotDefinedValue = 0;

        /**
         * Shift count and bit mask for A, R, G, B components in ARGB mode!
         */
        private const int ARGBAlphaShift = 24;
        private const int ARGBRedShift = 16;
        private const int ARGBGreenShift = 8;
        private const int ARGBBlueShift = 0;

        // will contain standard 32bit sRGB (ARGB)
        //
        private readonly long _value;

        // implementation specific information
        //
        private readonly short _state;

        /// <summary>
        /// Gets the red.
        /// </summary>
        public byte Red => (byte)((Value >> ARGBRedShift) & 0xFF);
        /// <summary>
        /// Gets the green.
        /// </summary>
        public byte Green => (byte)((Value >> ARGBGreenShift) & 0xFF);
        /// <summary>
        /// Gets the blue.
        /// </summary>
        public byte Blue => (byte)((Value >> ARGBBlueShift) & 0xFF);
        /// <summary>
        /// Gets the alpha.
        /// </summary>
        public byte Alpha => (byte)((Value >> ARGBAlphaShift) & 0xFF);

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        public bool IsEmpty => _state == 0;

        private long Value
        {
            get
            {
                if ((_state & StateValueMask) != 0)
                {
                    return _value;
                }
                return NotDefinedValue;
            }
        }

        private Color(long value, short state)
        {
            _value = value;
            _state = state;
        }

        private static void CheckByte(int value)
        {
            if (value < 0 || value > 255)
                throw new ArgumentException("RBGA Color value should be 0-255");
        }

        private static long MakeArgb(byte alpha, byte red, byte green, byte blue)
        {
            return (long)(unchecked((uint)(red << ARGBRedShift |
                         green << ARGBGreenShift |
                         blue << ARGBBlueShift |
                         alpha << ARGBAlphaShift))) & 0xffffffff;
        }

        /// <summary>
        /// Froms the ARGB.
        /// </summary>
        /// <param name="argb">The ARGB.</param>
        /// <returns></returns>
        public static Color FromArgb(int argb)
        {
            return new Color((long)argb & 0xffffffff, StateARGBValueValid);
        }

        /// <summary>
        /// Froms the ARGB.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public static Color FromArgb(int alpha, Color color)
        {
            alpha = alpha << 24;
            int c = color.ToArgb() & 0xffffff;
            return new Color(alpha | c, StateARGBValueValid);
        }

        /// <summary>
        /// Froms the ARGB.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        /// <returns></returns>
        public static Color FromArgb(int alpha, int red, int green, int blue)
        {
            CheckByte(alpha);
            CheckByte(red);
            CheckByte(green);
            CheckByte(blue);
            return new Color(MakeArgb((byte)alpha, (byte)red, (byte)green, (byte)blue), StateARGBValueValid);
        }

        /// <summary>
        /// Froms the ARGB.
        /// </summary>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        /// <returns></returns>
        public static Color FromArgb(int red, int green, int blue)
        {
            return FromArgb(255, red, green, blue);
        }

        /// <summary>
        ///       Returns the Hue-Saturation-Lightness (HSL) lightness
        /// </summary>
        public float GetBrightness()
        {
            float r = (float)Red / 255.0f;
            float g = (float)Green / 255.0f;
            float b = (float)Blue / 255.0f;

            float max, min;

            max = r; min = r;

            if (g > max) max = g;
            if (b > max) max = b;

            if (g < min) min = g;
            if (b < min) min = b;

            return (max + min) / 2;
        }

        /// <summary>
        ///       Returns the Hue-Saturation-Lightness (HSL) hue
        ///       value, in degrees, for this <see cref='System.Drawing.Color'/> .  
        ///       If R == G == B, the hue is meaningless, and the return value is 0.
        /// </summary>
        public Single GetHue()
        {
            if (Red == Green && Green == Blue)
                return 0; // 0 makes as good an UNDEFINED value as any

            float r = (float)Red / 255.0f;
            float g = (float)Green / 255.0f;
            float b = (float)Blue / 255.0f;

            float max, min;
            float delta;
            float hue = 0.0f;

            max = r; min = r;

            if (g > max) max = g;
            if (b > max) max = b;

            if (g < min) min = g;
            if (b < min) min = b;

            delta = max - min;

            if (r == max)
            {
                hue = (g - b) / delta;
            }
            else if (g == max)
            {
                hue = 2 + (b - r) / delta;
            }
            else if (b == max)
            {
                hue = 4 + (r - g) / delta;
            }
            hue *= 60;

            if (hue < 0.0f)
            {
                hue += 360.0f;
            }
            return hue;
        }

        /// <summary>
        ///   The Hue-Saturation-Lightness (HSL) saturation for this
        /// </summary>
        public float GetSaturation()
        {
            float r = (float)Red / 255.0f;
            float g = (float)Green / 255.0f;
            float b = (float)Blue / 255.0f;

            float max, min;
            float l, s = 0;

            max = r; min = r;

            if (g > max) max = g;
            if (b > max) max = b;

            if (g < min) min = g;
            if (b < min) min = b;

            // if max == min, then there is no color and
            // the saturation is zero.
            //
            if (max != min)
            {
                l = (max + min) / 2;

                if (l <= .5)
                {
                    s = (max - min) / (max + min);
                }
                else
                {
                    s = (max - min) / (2 - max - min);
                }
            }
            return s;
        }

        /// <summary>
        /// Converts to argb.
        /// </summary>
        public int ToArgb()
        {
            return unchecked((int)Value);
        }

        /// <summary>
        /// Converts to systemdrawingcolor.
        /// </summary>
        public System.Drawing.Color ToSystemDrawingColor()
        {
            return System.Drawing.Color.FromArgb(unchecked((int)Value));
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(32);
            sb.Append(GetType().Name);
            sb.Append(" [");

            if ((_state & StateValueMask) != 0)
            {
                sb.Append("A=");
                sb.Append(Alpha);
                sb.Append(", R=");
                sb.Append(Red);
                sb.Append(", G=");
                sb.Append(Green);
                sb.Append(", B=");
                sb.Append(Blue);
            }
            else
            {
                sb.Append("Empty");
            }

            sb.Append("]");

            return sb.ToString();
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Color left, Color right)
        {
            return left._value == right._value && left._state == right._state;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Color left, Color right)
        {
            return !(left == right);
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
            return obj is Color right && _value == right._value && _state == right._state;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return unchecked(_value.GetHashCode() ^ _state.GetHashCode());
        }
    }
}

using System;
using System.Drawing;

namespace Svg
{
    public sealed class SvgColourServer : SvgPaintServer
    {
        /// <summary>
        /// An unspecified <see cref="SvgPaintServer"/>.
        /// </summary>
        public static readonly SvgPaintServer NotSet = new SvgColourServer();
        /// <summary>
        /// A <see cref="SvgPaintServer"/> that should inherit from its parent.
        /// </summary>
        public static readonly SvgPaintServer Inherit = new SvgColourServer();

        public SvgColourServer()
            : this(System.Drawing.Color.Black)
        {
        }

        public SvgColourServer(Color colour)
        {
            this._colour = colour;
        }

        private Color _colour;

        public Color Colour
        {
            get { return this._colour; }
            set { this._colour = value; }
        }

        public override Brush GetBrush(SvgVisualElement styleOwner, ISvgRenderer renderer, float opacity, bool forStroke = false)
        {
            // is none?
            if (this == SvgPaintServer.None) return new SolidBrush(System.Drawing.Color.Transparent);

            // default fill color is black, default stroke color is none
            if (this == SvgColourServer.NotSet && forStroke) return new SolidBrush(System.Drawing.Color.Transparent);

            int alpha = (int)Math.Round((opacity * (this.Colour.A / 255.0)) * 255);
            Color colour = System.Drawing.Color.FromArgb(alpha, this.Colour);

            return new SolidBrush(colour);
        }

        public override string ToString()
        {
            if (this == SvgPaintServer.None)
                return "none";
            else if (this == SvgColourServer.NotSet)
                return string.Empty;
            else if (this == SvgColourServer.Inherit)
                return "inherit";

            Color c = this.Colour;
#if !NETSTANDARD20
            // Return the name if it exists
            if (c.IsKnownColor)
                return c.Name;
#endif
            // Return the hex value
            return String.Format("#{0}", c.ToArgb().ToString("x").Substring(2));
        }


        public override SvgElement DeepCopy()
        {
            return DeepCopy<SvgColourServer>();
        }

        public override SvgElement DeepCopy<T>()
        {
            if (this == None || this == Inherit || this == NotSet)
                return this;

            var newObj = base.DeepCopy<T>() as SvgColourServer;
            newObj.Colour = this.Colour;
            return newObj;
        }

        public override bool Equals(object obj)
        {
            var objColor = obj as SvgColourServer;
            if (objColor == null)
                return false;

            if ((this == SvgPaintServer.None && obj != SvgPaintServer.None) ||
                (this != SvgPaintServer.None && obj == SvgPaintServer.None) ||
                (this == SvgColourServer.NotSet && obj != SvgColourServer.NotSet) ||
                (this != SvgColourServer.NotSet && obj == SvgColourServer.NotSet) ||
                (this == SvgColourServer.Inherit && obj != SvgColourServer.Inherit) ||
                (this != SvgColourServer.Inherit && obj == SvgColourServer.Inherit)) return false;

            return this.GetHashCode() == objColor.GetHashCode();
        }

        public override int GetHashCode()
        {
            return _colour.GetHashCode();
        }
    }
}

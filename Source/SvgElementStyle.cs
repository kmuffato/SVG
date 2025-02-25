﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using Svg.DataTypes;
using System.Text.RegularExpressions;
using System.Linq;

namespace Svg
{
    public partial class SvgElement
    {
        private bool _dirty;

        /// <summary>
        /// Gets or sets a value indicating whether this element's 'Path' is dirty.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the path is dirty; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool IsPathDirty
        {
            get { return this._dirty; }
            set { this._dirty = value; }
        }

        /// <summary>
        /// Force recreation of the paths for the element and it's children.
        /// </summary>
        public void InvalidateChildPaths()
        {
            this.IsPathDirty = true;
            foreach (SvgElement element in this.Children)
            {
                element.InvalidateChildPaths();
            }
        }

        protected static float FixOpacityValue(float value)
        {
            const float max = 1.0f;
            const float min = 0.0f;
            return Math.Min(Math.Max(value, min), max);
        }

        /// <summary>
        /// Gets or sets the fill <see cref="SvgPaintServer"/> of this element.
        /// </summary>
        [SvgAttribute("fill")]
        public virtual SvgPaintServer Fill
        {
            get { return GetAttribute<SvgPaintServer>("fill", Inherited, SvgColourServer.NotSet); }
            set { Attributes["fill"] = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="SvgPaintServer"/> to be used when rendering a stroke around this element.
        /// </summary>
        [SvgAttribute("stroke")]
        public virtual SvgPaintServer Stroke
        {
            get { return GetAttribute<SvgPaintServer>("stroke", Inherited); }
            set { Attributes["stroke"] = value; }
        }

        [SvgAttribute("fill-rule")]
        public virtual SvgFillRule FillRule
        {
            get { return GetAttribute("fill-rule", Inherited, SvgFillRule.NonZero); }
            set { Attributes["fill-rule"] = value; }
        }

        /// <summary>
        /// Gets or sets the opacity of this element's <see cref="Fill"/>.
        /// </summary>
        [SvgAttribute("fill-opacity")]
        public virtual float FillOpacity
        {
            get { return GetAttribute("fill-opacity", Inherited, 1f); }
            set { Attributes["fill-opacity"] = FixOpacityValue(value); }
        }

        /// <summary>
        /// Gets or sets the width of the stroke (if the <see cref="Stroke"/> property has a valid value specified.
        /// </summary>
        [SvgAttribute("stroke-width")]
        public virtual SvgUnit StrokeWidth
        {
            get { return GetAttribute<SvgUnit>("stroke-width", Inherited, 1f); }
            set { Attributes["stroke-width"] = value; }
        }

        [SvgAttribute("stroke-linecap")]
        public virtual SvgStrokeLineCap StrokeLineCap
        {
            get { return GetAttribute("stroke-linecap", Inherited, SvgStrokeLineCap.Butt); }
            set { Attributes["stroke-linecap"] = value; }
        }

        [SvgAttribute("stroke-linejoin")]
        public virtual SvgStrokeLineJoin StrokeLineJoin
        {
            get { return GetAttribute("stroke-linejoin", Inherited, SvgStrokeLineJoin.Miter); }
            set { Attributes["stroke-linejoin"] = value; }
        }

        [SvgAttribute("stroke-miterlimit")]
        public virtual float StrokeMiterLimit
        {
            get { return GetAttribute("stroke-miterlimit", Inherited, 4f); }
            set { Attributes["stroke-miterlimit"] = value; }
        }

        [SvgAttribute("stroke-dasharray")]
        public virtual SvgUnitCollection StrokeDashArray
        {
            get { return GetAttribute<SvgUnitCollection>("stroke-dasharray", Inherited); }
            set { Attributes["stroke-dasharray"] = value; }
        }

        [SvgAttribute("stroke-dashoffset")]
        public virtual SvgUnit StrokeDashOffset
        {
            get { return GetAttribute("stroke-dashoffset", Inherited, SvgUnit.Empty); }
            set { Attributes["stroke-dashoffset"] = value; }
        }

        /// <summary>
        /// Gets or sets the opacity of the stroke, if the <see cref="Stroke"/> property has been specified. 1.0 is fully opaque; 0.0 is transparent.
        /// </summary>
        [SvgAttribute("stroke-opacity")]
        public virtual float StrokeOpacity
        {
            get { return GetAttribute("stroke-opacity", Inherited, 1f); }
            set { Attributes["stroke-opacity"] = FixOpacityValue(value); }
        }

        /// <summary>
        /// Gets or sets the colour of the gradient stop.
        /// </summary>
        /// <remarks>Apparently this can be set on non-sensical elements.  Don't ask; just check the tests.</remarks>
        [SvgAttribute("stop-color")]
        [TypeConverter(typeof(SvgPaintServerFactory))]
        public virtual SvgPaintServer StopColor
        {
            get { return GetAttribute<SvgPaintServer>("stop-color", Inherited); }
            set { Attributes["stop-color"] = value; }
        }

        /// <summary>
        /// Gets or sets the opacity of the element. 1.0 is fully opaque; 0.0 is transparent.
        /// </summary>
        [SvgAttribute("opacity")]
        public virtual float Opacity
        {
            get { return GetAttribute("opacity", Inherited, 1f); }
            set { Attributes["opacity"] = FixOpacityValue(value); }
        }

        /// <summary>
        /// Refers to the AnitAlias rendering of shapes.
        /// </summary>
        [SvgAttribute("shape-rendering")]
        public virtual SvgShapeRendering ShapeRendering
        {
            get { return GetAttribute<SvgShapeRendering>("shape-rendering", Inherited); }
            set { Attributes["shape-rendering"] = value; }
        }

        /// <summary>
        /// Gets or sets the text anchor.
        /// </summary>
        [SvgAttribute("text-anchor")]
        public virtual SvgTextAnchor TextAnchor
        {
            get { return GetAttribute<SvgTextAnchor>("text-anchor", Inherited); }
            set { Attributes["text-anchor"] = value; IsPathDirty = true; }
        }

        /// <summary>
        /// Specifies dominant-baseline positioning of text.
        /// </summary>
        [SvgAttribute("baseline-shift")]
        public virtual string BaselineShift
        {
            get { return GetAttribute<string>("baseline-shift", Inherited); }
            set { Attributes["baseline-shift"] = value; IsPathDirty = true; }
        }

        /// <summary>
        /// Indicates which font family is to be used to render the text.
        /// </summary>
        [SvgAttribute("font-family")]
        public virtual string FontFamily
        {
            get { return GetAttribute<string>("font-family", Inherited); }
            set { Attributes["font-family"] = value; IsPathDirty = true; }
        }

        /// <summary>
        /// Refers to the size of the font from baseline to baseline when multiple lines of text are set solid in a multiline layout environment.
        /// </summary>
        [SvgAttribute("font-size")]
        public virtual SvgUnit FontSize
        {
            get { return GetAttribute("font-size", Inherited, SvgUnit.Empty); }
            set { Attributes["font-size"] = value; IsPathDirty = true; }
        }

        /// <summary>
        /// Refers to the style of the font.
        /// </summary>
        [SvgAttribute("font-style")]
        public virtual SvgFontStyle FontStyle
        {
            get { return GetAttribute("font-style", Inherited, SvgFontStyle.All); }
            set { Attributes["font-style"] = value; IsPathDirty = true; }
        }

        /// <summary>
        /// Refers to the varient of the font.
        /// </summary>
        [SvgAttribute("font-variant")]
        public virtual SvgFontVariant FontVariant
        {
            get { return GetAttribute("font-variant", Inherited, SvgFontVariant.Inherit); }
            set { Attributes["font-variant"] = value; IsPathDirty = true; }
        }

        /// <summary>
        /// Refers to the boldness of the font.
        /// </summary>
        [SvgAttribute("text-decoration")]
        public virtual SvgTextDecoration TextDecoration
        {
            get { return GetAttribute("text-decoration", Inherited, SvgTextDecoration.Inherit); }
            set { Attributes["text-decoration"] = value; IsPathDirty = true; }
        }

        /// <summary>
        /// Refers to the boldness of the font.
        /// </summary>
        [SvgAttribute("font-weight")]
        public virtual SvgFontWeight FontWeight
        {
            get { return GetAttribute("font-weight", Inherited, SvgFontWeight.Inherit); }
            set { Attributes["font-weight"] = value; IsPathDirty = true; }
        }

        /// <summary>
        /// Refers to the text transformation.
        /// </summary>
        [SvgAttribute("text-transform")]
        public virtual SvgTextTransformation TextTransformation
        {
            get { return GetAttribute("text-transform", Inherited, SvgTextTransformation.Inherit); }
            set { Attributes["text-transform"] = value; IsPathDirty = true; }
        }

        private enum FontParseState
        {
            fontStyle,
            fontVariant,
            fontWeight,
            fontSize,
            fontFamilyNext,
            fontFamilyCurr
        }

        /// <summary>
        /// Set all font information.
        /// </summary>
        [SvgAttribute("font")]
        public virtual string Font
        {
            get { return GetAttribute("font", Inherited, string.Empty); }
            set
            {
                var state = FontParseState.fontStyle;
                var parts = value.Split(' ');

                SvgFontStyle fontStyle;
                SvgFontVariant fontVariant;
                SvgFontWeight fontWeight;
                SvgUnit fontSize;

                bool success;
                string[] sizes;
                string part;

                for (var i = 0; i < parts.Length; i++)
                {
                    part = parts[i];
                    success = false;
                    while (!success)
                    {
                        switch (state)
                        {
                            case FontParseState.fontStyle:
                                success = Enums.TryParse(part, out fontStyle);
                                if (success) FontStyle = fontStyle;
                                state++;
                                break;
                            case FontParseState.fontVariant:
                                success = Enums.TryParse(part, out fontVariant);
                                if (success) FontVariant = fontVariant;
                                state++;
                                break;
                            case FontParseState.fontWeight:
                                success = Enums.TryParse(part, out fontWeight);
                                if (success) FontWeight = fontWeight;
                                state++;
                                break;
                            case FontParseState.fontSize:
                                sizes = part.Split('/');
                                try
                                {
                                    fontSize = (SvgUnit)(new SvgUnitConverter().ConvertFromInvariantString(sizes[0]));
                                    success = true;
                                    FontSize = fontSize;
                                }
                                catch { }
                                state++;
                                break;
                            case FontParseState.fontFamilyNext:
                                state++;
                                success = true;
                                break;
                        }
                    }

                    switch (state)
                    {
                        case FontParseState.fontFamilyNext:
                            FontFamily = string.Join(" ", parts, i + 1, parts.Length - (i + 1));
                            i = int.MaxValue - 2;
                            break;
                        case FontParseState.fontFamilyCurr:
                            FontFamily = string.Join(" ", parts, i, parts.Length - (i));
                            i = int.MaxValue - 2;
                            break;
                    }

                }

                Attributes["font"] = value;
                IsPathDirty = true;
            }
        }

        /// <summary>
        /// Get the font information based on data stored with the text object or inherited from the parent.
        /// </summary>
        /// <returns></returns>
        internal IFontDefn GetFont(ISvgRenderer renderer)
        {
            // Get the font-size
            float fontSize;
            var fontSizeUnit = this.FontSize;
            if (fontSizeUnit == SvgUnit.None || fontSizeUnit == SvgUnit.Empty)
            {
                fontSize = new SvgUnit(SvgUnitType.Em, 1.0f);
            }
            else
            {
                fontSize = fontSizeUnit.ToDeviceValue(renderer, UnitRenderingType.Vertical, this);
            }

            var family = ValidateFontFamily(this.FontFamily, this.OwnerDocument);
            var sFaces = family as IEnumerable<SvgFontFace>;

            if (sFaces == null)
            {
                var fontStyle = System.Drawing.FontStyle.Regular;

                // Get the font-weight
                switch (this.FontWeight)
                {
                    //Note: Bold is not listed because it is = W700.
                    case SvgFontWeight.Bolder:
                    case SvgFontWeight.W600:
                    case SvgFontWeight.W700:
                    case SvgFontWeight.W800:
                    case SvgFontWeight.W900:
                        fontStyle |= System.Drawing.FontStyle.Bold;
                        break;
                }

                // Get the font-style
                switch (this.FontStyle)
                {
                    case SvgFontStyle.Italic:
                    case SvgFontStyle.Oblique:
                        fontStyle |= System.Drawing.FontStyle.Italic;
                        break;
                }

                // Get the text-decoration
                switch (this.TextDecoration)
                {
                    case SvgTextDecoration.LineThrough:
                        fontStyle |= System.Drawing.FontStyle.Strikeout;
                        break;
                    case SvgTextDecoration.Underline:
                        fontStyle |= System.Drawing.FontStyle.Underline;
                        break;
                }

                var ff = family as FontFamily;
                if (!ff.IsStyleAvailable(fontStyle))
                {
                    // Do Something
                }

                // Get the font-family
                return new GdiFontDefn(new System.Drawing.Font(ff, fontSize, fontStyle, System.Drawing.GraphicsUnit.Pixel));
            }
            else
            {
                var font = sFaces.First().Parent as SvgFont;
                if (font == null)
                {
                    var uri = sFaces.First().Descendants().OfType<SvgFontFaceUri>().First().ReferencedElement;
                    font = OwnerDocument.IdManager.GetElementById(uri) as SvgFont;
                }
                return new SvgFontDefn(font, fontSize, OwnerDocument.Ppi);
            }
        }

        public static System.Drawing.Text.PrivateFontCollection PrivateFonts = new System.Drawing.Text.PrivateFontCollection();
        public static object ValidateFontFamily(string fontFamilyList, SvgDocument doc)
        {
            // Split font family list on "," and then trim start and end spaces and quotes.
            var fontParts = (fontFamilyList ?? string.Empty).Split(new[] { ',' }).Select(fontName => fontName.Trim(new[] { '"', ' ', '\'' }));
            FontFamily family;
            IEnumerable<SvgFontFace> sFaces;

            // Find a the first font that exists in the list of installed font families.
            //styles from IE get sent through as lowercase.
            foreach (var f in fontParts)
            {
                if (doc != null && doc.FontDefns().TryGetValue(f, out sFaces)) return sFaces;
                family = SvgFontManager.FindFont(f);
                if (family != null) return family;
                family = PrivateFonts.Families.FirstOrDefault(ff => string.Equals(ff.Name, f, StringComparison.OrdinalIgnoreCase));
                if (family != null) return family;
                switch (f.ToLower())
                {
                    case "serif":
                        return System.Drawing.FontFamily.GenericSerif;
                    case "sans-serif":
                        return System.Drawing.FontFamily.GenericSansSerif;
                    case "monospace":
                        return System.Drawing.FontFamily.GenericMonospace;
                }
            }

            // No valid font family found from the list requested.
            return System.Drawing.FontFamily.GenericSansSerif;
        }
    }
}

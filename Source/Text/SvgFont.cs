﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Svg
{
    [SvgElement("font")]
    public class SvgFont : SvgElement
    {
        [SvgAttribute("horiz-adv-x")]
        public float HorizAdvX
        {
            get { return GetAttribute("horiz-adv-x", Inherited, 0f); }
            set { Attributes["horiz-adv-x"] = value; }
        }
        [SvgAttribute("horiz-origin-x")]
        public float HorizOriginX
        {
            get { return GetAttribute("horiz-origin-x", Inherited, 0f); }
            set { Attributes["horiz-origin-x"] = value; }
        }
        [SvgAttribute("horiz-origin-y")]
        public float HorizOriginY
        {
            get { return GetAttribute("horiz-origin-y", Inherited, 0f); }
            set { Attributes["horiz-origin-y"] = value; }
        }
        [SvgAttribute("vert-adv-y")]
        public float VertAdvY
        {
            get { return GetAttribute("vert-adv-y", Inherited, Children.OfType<SvgFontFace>().First().UnitsPerEm); }
            set { Attributes["vert-adv-y"] = value; }
        }
        [SvgAttribute("vert-origin-x")]
        public float VertOriginX
        {
            get { return GetAttribute("vert-origin-x", Inherited, HorizAdvX / 2); }
            set { Attributes["vert-origin-x"] = value; }
        }
        [SvgAttribute("vert-origin-y")]
        public float VertOriginY
        {
            get
            {
                var defaultValue = Children.OfType<SvgFontFace>().First().Attributes["ascent"] as float? ?? 0f;
                return GetAttribute("vert-origin-y", Inherited, defaultValue);
            }
            set { Attributes["vert-origin-y"] = value; }
        }

        public override SvgElement DeepCopy()
        {
            return base.DeepCopy<SvgFont>();
        }

        protected override void Render(ISvgRenderer renderer)
        { }
    }
}

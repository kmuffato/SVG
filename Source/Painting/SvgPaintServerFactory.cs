using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace Svg
{
    internal class SvgPaintServerFactory : TypeConverter
    {
#if !NETSTANDARD20
        private static readonly SvgColourConverter _colourConverter;

        static SvgPaintServerFactory()
        {
            _colourConverter = new SvgColourConverter();
        }
#endif
        public static SvgPaintServer Create(string value, SvgDocument document)
        {
            // If it's pointing to a paint server
            if (string.IsNullOrEmpty(value))
            {
                return SvgColourServer.NotSet;
            }
            else if (value == "inherit")
            {
                return SvgColourServer.Inherit;
            }
            else if (value == "currentColor")
            {
                return new SvgDeferredPaintServer(value);
            }
            else
            {
                var servers = new List<SvgPaintServer>();

                while (!string.IsNullOrEmpty(value))
                {
                    if (value.StartsWith("url("))
                    {
                        var nextIndex = value.IndexOf(')', 4) + 1;
                        var uri = new Uri(value.Substring(0, nextIndex), UriKind.RelativeOrAbsolute);
                        value = value.Substring(nextIndex).Trim();
                        servers.Add((SvgPaintServer)document.IdManager.GetElementById(uri));
                    }
                    // If referenced to to a different (linear or radial) gradient
                    else if (document.IdManager.GetElementById(value) != null && document.IdManager.GetElementById(value).GetType().BaseType == typeof(SvgGradientServer))
                    {
                        return (SvgPaintServer)document.IdManager.GetElementById(value);
                    }
                    else if (value.StartsWith("#")) // Otherwise try and parse as colour
                    {
                        switch (CountHexDigits(value, 1))
                        {
#if !NETSTANDARD20
                            case 3:
                                servers.Add(new SvgColourServer((Color)_colourConverter.ConvertFrom(value.Substring(0, 4))));
                                value = value.Substring(4).Trim();
                                break;
                            case 6:
                                servers.Add(new SvgColourServer((Color)_colourConverter.ConvertFrom(value.Substring(0, 7))));
                                value = value.Substring(7).Trim();
                                break;
#endif
                            default:
                                return new SvgDeferredPaintServer(value);
                        }
                    }
#if !NETSTANDARD20
                    else
                    {
                        return new SvgColourServer((Color)_colourConverter.ConvertFrom(value.Trim()));
                    }
#endif
                }

                if (servers.Count > 1)
                {
                    return new SvgFallbackPaintServer(servers[0], servers.Skip(1));
                }
                return servers[0];
            }
        }

        private static int CountHexDigits(string value, int start)
        {
            int i = Math.Max(start, 0);
            int count = 0;
            while (i < value.Length &&
                   ((value[i] >= '0' && value[i] <= '9') ||
                    (value[i] >= 'a' && value[i] <= 'f') ||
                    (value[i] >= 'A' && value[i] <= 'F')))
            {
                count++;
                i++;
            }
            return count;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                var s = ((string)value).Trim();
                if (String.Equals(s, "none", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(s))
                    return SvgPaintServer.None;
                else
                    return Create(s, (SvgDocument)context);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                // check for none
                if (value == SvgPaintServer.None || value == SvgColourServer.None) return "none";
                if (value == SvgColourServer.Inherit) return "inherit";
                if (value == SvgColourServer.NotSet) return "";
#if !NETSTANDARD20
                var colourServer = value as SvgColourServer;
                if (colourServer != null)
                {
                    return new SvgColourConverter().ConvertTo(colourServer.Colour, typeof(string));
                }
#endif
                var deferred = value as SvgDeferredPaintServer;
                if (deferred != null)
                {
                    return deferred.ToString();
                }

                if (value != null)
                {
                    return string.Format(CultureInfo.InvariantCulture, "url(#{0})", ((SvgPaintServer)value).ID);
                }
                else
                {
                    return "none";
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

using System;
using osu.Framework.Graphics.Colour;
using osuTK.Graphics;

namespace osu.Framework.Design.Markup.ValueConverters
{
    public class Color4Converter : IValueConverter
    {
        public Type ConvertingType => typeof(Color4);

        public void Serialize(object value, Type type, out string data) => data = Serialize((Color4)value);
        public static string Serialize(Color4 c)
        {
            var r = (byte)(c.R * byte.MaxValue);
            var g = (byte)(c.G * byte.MaxValue);
            var b = (byte)(c.B * byte.MaxValue);
            var a = (byte)(c.A * byte.MaxValue);

            return $"rgba({r}, {g}, {b}, {a})";
        }

        public void Deserialize(string data, Type type, out object value) => value = Deserialize(data);
        public static Color4 Deserialize(string data)
        {
            data = data.Trim();

            if (data.Length == 7 && data.StartsWith('#'))
                return new Color4(
                    Convert.ToByte(data.Substring(1, 2), 16),
                    Convert.ToByte(data.Substring(3, 2), 16),
                    Convert.ToByte(data.Substring(5, 2), 16),
                    255);

            if (data.StartsWith("rgb(", StringComparison.OrdinalIgnoreCase) && data.EndsWith(')'))
            {
                var parts = data.Substring(4, data.Length - 5).SplitByComma();

                if (parts.Length == 3)
                    return new Color4(
                        Convert.ToByte(parts[0]),
                        Convert.ToByte(parts[1]),
                        Convert.ToByte(parts[2]),
                        255);
            }

            if (data.StartsWith("rgba(", StringComparison.OrdinalIgnoreCase) && data.EndsWith(')'))
            {
                var parts = data.Substring(5, data.Length - 6).SplitByComma();

                if (parts.Length == 4)
                    return new Color4(
                        Convert.ToByte(parts[0]),
                        Convert.ToByte(parts[1]),
                        Convert.ToByte(parts[2]),
                        Convert.ToByte(parts[3]));
            }

            throw new FormatException($"Unrecognized color '{data}'.");
        }
    }

    public class SRGBColourConverter : IValueConverter
    {
        public Type ConvertingType => typeof(SRGBColour);

        public void Serialize(object value, Type type, out string data) => data = Color4Converter.Serialize((SRGBColour)value);
        public void Deserialize(string data, Type type, out object value) => value = (SRGBColour)Color4Converter.Deserialize(data);
    }

    public class ColourInfoConverter : IValueConverter
    {
        public Type ConvertingType => typeof(ColourInfo);

        public void Serialize(object value, Type type, out string data) => data = Serialize((ColourInfo)value);
        public static string Serialize(ColourInfo c)
        {
            // SingleColour
            if (c.TopLeft.Equals(c.TopRight) && c.TopLeft.Equals(c.BottomRight) && c.TopLeft.Equals(c.BottomLeft))
                return Color4Converter.Serialize(c.TopLeft);

            // GradientHorizontal
            if (c.TopLeft.Equals(c.BottomLeft) && c.TopRight.Equals(c.BottomRight) && !c.TopLeft.Equals(c.TopRight))
                return $"gradient(horizontal, {Color4Converter.Serialize(c.TopLeft)}, {Color4Converter.Serialize(c.TopRight)})";

            // GradientVertical
            if (c.TopLeft.Equals(c.TopRight) && c.BottomLeft.Equals(c.BottomRight) && !c.TopLeft.Equals(c.BottomLeft))
                return $"gradient(vertical, {Color4Converter.Serialize(c.TopLeft)}, {Color4Converter.Serialize(c.BottomRight)})";

            // Full serialization
            var tl = Color4Converter.Serialize(c.TopLeft);
            var tr = Color4Converter.Serialize(c.TopRight);
            var br = Color4Converter.Serialize(c.BottomRight);
            var bl = Color4Converter.Serialize(c.BottomLeft);

            return $"{tl}, {tr}, {br}, {bl}";
        }

        public void Deserialize(string data, Type type, out object value) => value = Deserialize(data);
        public static ColourInfo Deserialize(string data)
        {
            data = data.Trim();

            string[] parts;

            if (data.StartsWith("gradient(", StringComparison.OrdinalIgnoreCase) && data.EndsWith(')'))
            {
                parts = data.Substring(9, data.Length - 10).SplitByComma();
                var direction = parts[0];

                if (direction.Equals("vertical", StringComparison.OrdinalIgnoreCase))
                    return ColourInfo.GradientVertical(
                        c1: Color4Converter.Deserialize(parts[1]),
                        c2: Color4Converter.Deserialize(parts[2]));

                if (direction.Equals("horizontal", StringComparison.OrdinalIgnoreCase))
                    return ColourInfo.GradientHorizontal(
                        c1: Color4Converter.Deserialize(parts[1]),
                        c2: Color4Converter.Deserialize(parts[2]));
            }

            parts = data.SplitByComma();

            if (parts.Length == 1)
                return ColourInfo.SingleColour(Color4Converter.Deserialize(parts[0]));

            if (parts.Length == 4)
                return new ColourInfo
                {
                    TopLeft = Color4Converter.Deserialize(parts[0]),
                    TopRight = Color4Converter.Deserialize(parts[1]),
                    BottomRight = Color4Converter.Deserialize(parts[2]),
                    BottomLeft = Color4Converter.Deserialize(parts[3])
                };

            throw new FormatException($"Unrecognized {nameof(ColourInfo)} '{data}'.");
        }
    }
}
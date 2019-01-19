using System;
using System.Collections.Generic;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace osu.Framework.Design.Markup.ValueConverters
{
    public class EdgeEffectConverter : IValueConverter
    {
        public Type ConvertingType => typeof(EdgeEffectParameters);

        public void Serialize(object value, Type type, out string data) => data = Serialize((EdgeEffectParameters)value);
        public static string Serialize(EdgeEffectParameters p)
        {
            if (p.Type == EdgeEffectType.None)
                return "None";

            var list = new List<string>();

            if (p.Hollow)
                list.Add("true");
            if (p.Offset != default || list.Count > 0)
                list.Add($"({Vector2Converter.Serialize(p.Offset)})");
            if (p.Roundness != default || list.Count > 0)
                list.Add(p.Roundness.ToString());
            if (p.Radius != default || list.Count > 0)
                list.Add(p.Radius.ToString());

            list.Add(Color4Converter.Serialize(p.Colour));
            list.Add(p.Type.ToString());
            list.Reverse();

            return string.Join(", ", list);
        }

        public void Deserialize(string data, Type type, out object value) => value = Deserialize(data);
        public static EdgeEffectParameters Deserialize(string data)
        {
            var parts = data.SplitByComma();

            if (parts.Length == 0)
                throw new MarkupException($"Unrecognized {nameof(EdgeEffectParameters)}.");

            var edge = new EdgeEffectParameters
            {
                Type = Enum.Parse<EdgeEffectType>(parts[0], ignoreCase: true),
            };

            if (parts.Length > 1)
                edge.Colour = Color4Converter.Deserialize(parts[1]);
            if (parts.Length > 2)
                edge.Radius = float.Parse(parts[2]);
            if (parts.Length > 3)
                edge.Roundness = float.Parse(parts[3]);
            if (parts.Length > 4)
                edge.Offset = Vector2Converter.Deserialize(parts[4].Substring(1, parts[4].Length - 2));
            if (parts.Length > 5)
                edge.Hollow = bool.Parse(parts[5]);
            if (parts.Length > 6)
                throw new MarkupException($"Unknown {nameof(EdgeEffectParameters)} component '{parts[6]}'.");

            return edge;
        }
    }
}
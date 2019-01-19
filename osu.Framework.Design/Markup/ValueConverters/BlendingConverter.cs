using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace osu.Framework.Design.Markup.ValueConverters
{
    public class BlendingConverter : IValueConverter
    {
        public Type ConvertingType => typeof(BlendingParameters);

        public void Serialize(object value, Type type, out string data) => data = Serialize((BlendingParameters)value);
        public static string Serialize(BlendingParameters b)
        {
            var list = new List<string>();
            list.Add(b.Mode.ToString());

            if (b.AlphaEquation != default)
                list.Add($"alpha({b.AlphaEquation})");
            if (b.RGBEquation != default)
                list.Add($"rgb({b.RGBEquation})");

            return string.Join(", ", list);
        }

        public void Deserialize(string data, Type type, out object value) => value = Deserialize(data);
        public BlendingParameters Deserialize(string data)
        {
            var parts = data.SplitByComma();

            if (parts.Length == 0)
                throw new MarkupException($"Unrecognized {nameof(BlendingParameters)}.");

            var blending = new BlendingParameters
            {
                Mode = Enum.Parse<BlendingMode>(parts[0], ignoreCase: true)
            };

            BlendingEquation? alpha = null;
            BlendingEquation? rgb = null;

            void parseEquation(string part)
            {
                if (part.StartsWith("alpha(", StringComparison.OrdinalIgnoreCase) && part.EndsWith(')'))
                {
                    if (alpha.HasValue)
                        throw new MarkupException($"Cannot specify multiple alpha equations.");

                    alpha = Enum.Parse<BlendingEquation>(part.Substring(6, part.Length - 7), ignoreCase: true);
                }
                else if (part.StartsWith("rgb(", StringComparison.OrdinalIgnoreCase) && part.EndsWith(')'))
                {
                    if (rgb.HasValue)
                        throw new MarkupException($"Cannot specify multiple RGB equations.");

                    rgb = Enum.Parse<BlendingEquation>(part.Substring(4, part.Length - 5), ignoreCase: true);
                }
            }

            if (parts.Length > 1)
                parseEquation(parts[1]);
            if (parts.Length > 2)
                parseEquation(parts[2]);
            if (parts.Length > 3)
                throw new MarkupException($"Unknown {nameof(BlendingParameters)} component '{parts[3]}'.");

            blending.AlphaEquation = alpha ?? BlendingEquation.Inherit;
            blending.RGBEquation = rgb ?? BlendingEquation.Inherit;

            return blending;
        }
    }
}
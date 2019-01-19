using System;
using osu.Framework.Graphics;

namespace osu.Framework.Design.Markup.ValueConverters
{
    public class MarginPaddingConverter : IValueConverter
    {
        public Type ConvertingType => typeof(MarginPadding);

        public void Serialize(object value, Type type, out string data) => data = Serialize((MarginPadding)value);
        public static string Serialize(MarginPadding m)
        {
            if (m.Top == m.Left && m.Top == m.Right && m.Top == m.Bottom)
                return m.Top.ToString();

            return $"{m.Top}, {m.Right}, {m.Bottom}, {m.Left}";
        }

        public void Deserialize(string data, Type type, out object value) => value = Deserialize(data);
        public static MarginPadding Deserialize(string data)
        {
            var parts = data.SplitByComma();

            if (parts.Length == 1)
                return new MarginPadding(float.Parse(parts[0]));

            return new MarginPadding
            {
                Top = float.Parse(parts[0]),
                Right = float.Parse(parts[1]),
                Bottom = float.Parse(parts[2]),
                Left = float.Parse(parts[3])
            };
        }
    }
}
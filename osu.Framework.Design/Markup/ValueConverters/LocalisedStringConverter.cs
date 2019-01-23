using System;
using osu.Framework.Localisation;

namespace osu.Framework.Design.Markup.ValueConverters
{
    public class LocalisedStringConverter : IValueConverter
    {
        public Type ConvertingType => typeof(LocalisedString);

        public void Serialize(object value, Type type, out string data) => data = Serialize((LocalisedString)value);
        public static string Serialize(LocalisedString str) => str.Text.Original ?? str.Text.Fallback;

        public void Deserialize(string data, Type type, out object value) => value = new LocalisedString(data);
    }
}
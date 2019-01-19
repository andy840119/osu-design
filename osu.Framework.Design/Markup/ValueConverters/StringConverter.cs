using System;

namespace osu.Framework.Design.Markup.ValueConverters
{
    public class StringConverter : IValueConverter
    {
        public Type ConvertingType => typeof(string);

        public void Serialize(object value, Type type, out string data) => data = value as string ?? value.ToString();
        public void Deserialize(string data, Type type, out object value) => value = data;
    }
}
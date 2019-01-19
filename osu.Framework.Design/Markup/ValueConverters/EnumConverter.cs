using System;
using System.Collections.Generic;

namespace osu.Framework.Design.Markup.ValueConverters
{
    public class EnumConverter : IValueConverter
    {
        public Type ConvertingType => typeof(Enum);

        public void Serialize(object value, Type type, out string data) => data = value.ToString();
        public void Deserialize(string data, Type type, out object value) => value = Enum.Parse(type, data, ignoreCase: true);
    }
}
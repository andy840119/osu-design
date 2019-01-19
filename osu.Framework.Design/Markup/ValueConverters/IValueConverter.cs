using System;

namespace osu.Framework.Design.Markup.ValueConverters
{
    public interface IValueConverter
    {
        Type ConvertingType { get; }

        void Serialize(object value, Type type, out string data);
        void Deserialize(string data, Type type, out object value);
    }
}
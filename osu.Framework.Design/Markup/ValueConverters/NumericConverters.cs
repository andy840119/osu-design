using System;

namespace osu.Framework.Design.Markup.ValueConverters
{
    public class BoolConverter : IValueConverter
    {
        public Type ConvertingType => typeof(bool);

        public void Serialize(object value, Type type, out string data) => data = (bool)value ? "true" : "false";
        public void Deserialize(string data, Type type, out object value) => value = bool.Parse(data);
    }

    public class ByteConverter : IValueConverter
    {
        public Type ConvertingType => typeof(byte);

        public void Serialize(object value, Type type, out string data) => data = value.ToString();
        public void Deserialize(string data, Type type, out object value) => value = byte.Parse(data);
    }

    public class Int16Converter : IValueConverter
    {
        public Type ConvertingType => typeof(short);

        public void Serialize(object value, Type type, out string data) => data = value.ToString();
        public void Deserialize(string data, Type type, out object value) => value = short.Parse(data);
    }

    public class Int32Converter : IValueConverter
    {
        public Type ConvertingType => typeof(int);

        public void Serialize(object value, Type type, out string data) => data = value.ToString();
        public void Deserialize(string data, Type type, out object value) => value = int.Parse(data);
    }

    public class Int64Converter : IValueConverter
    {
        public Type ConvertingType => typeof(long);

        public void Serialize(object value, Type type, out string data) => data = value.ToString();
        public void Deserialize(string data, Type type, out object value) => value = long.Parse(data);
    }

    public class SingleConverter : IValueConverter
    {
        public Type ConvertingType => typeof(float);

        public void Serialize(object value, Type type, out string data) => data = value.ToString();
        public void Deserialize(string data, Type type, out object value) => value = float.Parse(data);
    }

    public class DoubleConverter : IValueConverter
    {
        public Type ConvertingType => typeof(double);

        public void Serialize(object value, Type type, out string data) => data = value.ToString();
        public void Deserialize(string data, Type type, out object value) => value = double.Parse(data);
    }
}
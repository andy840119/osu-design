using System;
using System.Xml.Linq;

namespace osu.Framework.Design.Markup.Converters
{
    public class BoolConverter : IConverter
    {
        public Type ConvertingType => typeof(bool);
        public bool PreferStringSerialization => true;

        public object DeserializeFromElement(XElement element, Type type) => bool.Parse(element.Value);
        public object DeserializeFromString(string data, Type type) => bool.Parse(data);

        public void SerializeAsElement(object value, XElement element) => element.Value = value.ToString();
        public void SerializeAsString(object value, out string data) => data = value.ToString();
    }

    public class ByteConverter : IConverter
    {
        public Type ConvertingType => typeof(byte);
        public bool PreferStringSerialization => true;

        public object DeserializeFromElement(XElement element, Type type) => byte.Parse(element.Value);
        public object DeserializeFromString(string data, Type type) => byte.Parse(data);

        public void SerializeAsElement(object value, XElement element) => element.Value = value.ToString();
        public void SerializeAsString(object value, out string data) => data = value.ToString();
    }

    public class Int16Converter : IConverter
    {
        public Type ConvertingType => typeof(short);
        public bool PreferStringSerialization => true;

        public object DeserializeFromElement(XElement element, Type type) => short.Parse(element.Value);
        public object DeserializeFromString(string data, Type type) => short.Parse(data);

        public void SerializeAsElement(object value, XElement element) => element.Value = value.ToString();
        public void SerializeAsString(object value, out string data) => data = value.ToString();
    }

    public class Int32Converter : IConverter
    {
        public Type ConvertingType => typeof(int);
        public bool PreferStringSerialization => true;

        public object DeserializeFromElement(XElement element, Type type) => int.Parse(element.Value);
        public object DeserializeFromString(string data, Type type) => int.Parse(data);

        public void SerializeAsElement(object value, XElement element) => element.Value = value.ToString();
        public void SerializeAsString(object value, out string data) => data = value.ToString();
    }

    public class Int64Converter : IConverter
    {
        public Type ConvertingType => typeof(long);
        public bool PreferStringSerialization => true;

        public object DeserializeFromElement(XElement element, Type type) => long.Parse(element.Value);
        public object DeserializeFromString(string data, Type type) => long.Parse(data);

        public void SerializeAsElement(object value, XElement element) => element.Value = value.ToString();
        public void SerializeAsString(object value, out string data) => data = value.ToString();
    }

    public class SingleConverter : IConverter
    {
        public Type ConvertingType => typeof(float);
        public bool PreferStringSerialization => true;

        public object DeserializeFromElement(XElement element, Type type) => float.Parse(element.Value);
        public object DeserializeFromString(string data, Type type) => float.Parse(data);

        public void SerializeAsElement(object value, XElement element) => element.Value = value.ToString();
        public void SerializeAsString(object value, out string data) => data = value.ToString();
    }

    public class DoubleConverter : IConverter
    {
        public Type ConvertingType => typeof(double);
        public bool PreferStringSerialization => true;

        public object DeserializeFromElement(XElement element, Type type) => double.Parse(element.Value);
        public object DeserializeFromString(string data, Type type) => double.Parse(data);

        public void SerializeAsElement(object value, XElement element) => element.Value = value.ToString();
        public void SerializeAsString(object value, out string data) => data = value.ToString();
    }
}
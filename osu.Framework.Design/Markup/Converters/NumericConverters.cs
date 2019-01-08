using System;
using System.Xml.Linq;

namespace osu.Framework.Design.Markup.Converters
{
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
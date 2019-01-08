using System;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace osu.Framework.Design.Markup.Converters
{
    public class EnumConverter : IConverter
    {
        public Type ConvertingType => typeof(Enum);
        public bool PreferStringSerialization => true;

        public object DeserializeFromElement(XElement element, Type type) => Enum.Parse(type, element.Value);
        public object DeserializeFromString(string data, Type type) => Enum.Parse(type, data);

        public void SerializeAsElement(object value, XElement element) => element.Value = value.ToString();
        public void SerializeAsString(object value, out string data) => data = value.ToString();

        public SyntaxNode GenerateInstantiation(object value, SyntaxGenerator g)
        {
            return g.IdentifierName($"{value.GetType()}.{value}");
        }
    }
}
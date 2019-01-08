using System;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace osu.Framework.Design.Markup.Converters
{
    public interface IConverter
    {
        Type ConvertingType { get; }

        bool PreferStringSerialization { get; }

        void SerializeAsString(object value, out string data);
        void SerializeAsElement(object value, XElement element);

        object DeserializeFromString(string data, Type type);
        object DeserializeFromElement(XElement element, Type type);

        SyntaxNode GenerateInstantiation(object value, SyntaxGenerator g);
    }
}
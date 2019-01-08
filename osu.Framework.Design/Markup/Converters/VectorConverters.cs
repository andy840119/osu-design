using System;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using osuTK;

namespace osu.Framework.Design.Markup.Converters
{
    public class Vector2Converter : IConverter
    {
        public Type ConvertingType => typeof(Vector2);
        public bool PreferStringSerialization => true;

        public object DeserializeFromElement(XElement element, Type type)
        {
            var x = float.Parse(element.Attribute("X").Value);
            var y = float.Parse(element.Attribute("Y").Value);

            return new Vector2(x, y);
        }
        public object DeserializeFromString(string data, Type type)
        {
            var array = data.Split(',', 2);
            var x = float.Parse(array[0]);
            var y = float.Parse(array[1]);

            return new Vector2(x, y);
        }

        public void SerializeAsElement(object value, XElement element)
        {
            if (value is Vector2 v)
            {
                element.SetAttributeValue("X", v.X);
                element.SetAttributeValue("Y", v.Y);
            }
        }
        public void SerializeAsString(object value, out string data)
        {
            if (value is Vector2 v)
                data = $"{v.X},{v.Y}";
            else
                data = null;
        }

        public SyntaxNode GenerateInstantiation(object value, SyntaxGenerator g)
        {
            var v = (Vector2)value;

            return g.ObjectCreationExpression(
                namedType: g.IdentifierName(typeof(Vector2).FullName),
                arguments: new[]
                {
                    g.LiteralExpression(v.X),
                    g.LiteralExpression(v.Y)
                }
            );
        }
    }

    public class Vector3Converter : IConverter
    {
        public Type ConvertingType => typeof(Vector3);
        public bool PreferStringSerialization => true;

        public object DeserializeFromElement(XElement element, Type type)
        {
            var x = float.Parse(element.Attribute("X").Value);
            var y = float.Parse(element.Attribute("Y").Value);
            var z = float.Parse(element.Attribute("Z").Value);

            return new Vector3(x, y, z);
        }
        public object DeserializeFromString(string data, Type type)
        {
            var array = data.Split(',', 3);
            var x = float.Parse(array[0]);
            var y = float.Parse(array[1]);
            var z = float.Parse(array[2]);

            return new Vector3(x, y, z);
        }

        public void SerializeAsElement(object value, XElement element)
        {
            if (value is Vector3 v)
            {
                element.SetAttributeValue("X", v.X);
                element.SetAttributeValue("Y", v.Y);
                element.SetAttributeValue("Z", v.Z);
            }
        }
        public void SerializeAsString(object value, out string data)
        {
            if (value is Vector3 v)
                data = $"{v.X},{v.Y},{v.Z}";
            else
                data = null;
        }

        public SyntaxNode GenerateInstantiation(object value, SyntaxGenerator g)
        {
            var v = (Vector3)value;

            return g.ObjectCreationExpression(
                namedType: g.IdentifierName(typeof(Vector3).FullName),
                arguments: new[]
                {
                    g.LiteralExpression(v.X),
                    g.LiteralExpression(v.Y),
                    g.LiteralExpression(v.Z)
                }
            );
        }
    }

    public class Vector4Converter : IConverter
    {
        public Type ConvertingType => typeof(Vector4);
        public bool PreferStringSerialization => true;

        public object DeserializeFromElement(XElement element, Type type)
        {
            var x = float.Parse(element.Attribute("X").Value);
            var y = float.Parse(element.Attribute("Y").Value);
            var z = float.Parse(element.Attribute("Z").Value);
            var w = float.Parse(element.Attribute("W").Value);

            return new Vector4(x, y, z, w);
        }
        public object DeserializeFromString(string data, Type type)
        {
            var array = data.Split(',', 4);
            var x = float.Parse(array[0]);
            var y = float.Parse(array[1]);
            var z = float.Parse(array[2]);
            var w = float.Parse(array[3]);

            return new Vector4(x, y, z, w);
        }

        public void SerializeAsElement(object value, XElement element)
        {
            if (value is Vector4 v)
            {
                element.SetAttributeValue("X", v.X);
                element.SetAttributeValue("Y", v.Y);
                element.SetAttributeValue("Z", v.Z);
                element.SetAttributeValue("W", v.W);
            }
        }
        public void SerializeAsString(object value, out string data)
        {
            if (value is Vector4 v)
                data = $"{v.X},{v.Y},{v.Z},{v.W}";
            else
                data = null;
        }

        public SyntaxNode GenerateInstantiation(object value, SyntaxGenerator g)
        {
            var v = (Vector4)value;

            return g.ObjectCreationExpression(
                namedType: g.IdentifierName(typeof(Vector4).FullName),
                arguments: new[]
                {
                    g.LiteralExpression(v.X),
                    g.LiteralExpression(v.Y),
                    g.LiteralExpression(v.Z),
                    g.LiteralExpression(v.W)
                }
            );
        }
    }
}
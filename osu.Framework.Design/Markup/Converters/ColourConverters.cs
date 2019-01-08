using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using osu.Framework.Graphics.Colour;
using osuTK.Graphics;

namespace osu.Framework.Design.Markup.Converters
{
    public class SRGBColourConverter : IConverter
    {
        public Type ConvertingType => typeof(SRGBColour);
        public bool PreferStringSerialization => true;

        public object DeserializeFromElement(XElement element, Type type)
        {
            var r = float.Parse(element.Attribute("R").Value) / byte.MaxValue;
            var g = float.Parse(element.Attribute("G").Value) / byte.MaxValue;
            var b = float.Parse(element.Attribute("B").Value) / byte.MaxValue;
            var a = float.Parse(element.Attribute("A").Value) / byte.MaxValue;

            return new Color4(r, g, b, a);
        }
        public object DeserializeFromString(string data, Type type)
        {
            return FromHex(data);
        }
        public static SRGBColour FromHex(string hex)
        {
            if (!hex.StartsWith('#') || hex.Length != 7)
                throw new ArgumentException($"Hexadecimal colour '{hex}' is invalid.");

            return new Color4(
                Convert.ToByte(hex.Substring(1, 2), 16),
                Convert.ToByte(hex.Substring(3, 2), 16),
                Convert.ToByte(hex.Substring(5, 2), 16),
                255
            );
        }

        public void SerializeAsElement(object value, XElement element)
        {
            if (value is SRGBColour c)
            {
                element.SetElementValue("R", c.Linear.R * byte.MaxValue);
                element.SetElementValue("G", c.Linear.G * byte.MaxValue);
                element.SetElementValue("B", c.Linear.B * byte.MaxValue);
                element.SetElementValue("A", c.Linear.A * byte.MaxValue);
            }
        }
        public void SerializeAsString(object value, out string data)
        {
            if (value is SRGBColour c)
                data = ToHex(c);
            else
                data = null;
        }
        public static string ToHex(SRGBColour c)
        {
            var r = Convert.ToString((byte)(c.Linear.R * byte.MaxValue), 16);
            var g = Convert.ToString((byte)(c.Linear.G * byte.MaxValue), 16);
            var b = Convert.ToString((byte)(c.Linear.B * byte.MaxValue), 16);

            return $"#{r}{g}{b}";
        }

        public SyntaxNode GenerateInstantiation(object value, SyntaxGenerator g)
        {
            var c = (SRGBColour)value;

            return GenerateInstantiation(c, g);
        }
        public static SyntaxNode GenerateInstantiation(SRGBColour c, SyntaxGenerator g)
        {
            return g.CastExpression(
                type: g.IdentifierName(typeof(SRGBColour).FullName),
                expression: g.ObjectCreationExpression(
                    namedType: g.IdentifierName(typeof(Color4).FullName),
                    arguments: new[]
                    {
                        g.LiteralExpression(c.Linear.R),
                        g.LiteralExpression(c.Linear.G),
                        g.LiteralExpression(c.Linear.B),
                        g.LiteralExpression(c.Linear.A)
                    }
                )
            );
        }
    }

    public class ColourInfoConverter : IConverter
    {
        public Type ConvertingType => typeof(ColourInfo);
        public bool PreferStringSerialization => false;

        public object DeserializeFromElement(XElement element, Type type)
        {
            if (!element.HasAttributes)
                return ColourInfo.SingleColour(SRGBColourConverter.FromHex(element.Value));

            else if (element.Attributes().Count() == 2)
            {
                if (element.Attribute("Top") != null)
                    return ColourInfo.GradientVertical(
                        c1: SRGBColourConverter.FromHex(element.Attribute("Top").Value),
                        c2: SRGBColourConverter.FromHex(element.Attribute("Bottom").Value)
                    );
                else if (element.Attribute("Left") != null)
                    return ColourInfo.GradientHorizontal(
                        c1: SRGBColourConverter.FromHex(element.Attribute("Left").Value),
                        c2: SRGBColourConverter.FromHex(element.Attribute("Right").Value)
                    );
            }

            var tl = SRGBColourConverter.FromHex(element.Attribute("TopLeft").Value);
            var tr = SRGBColourConverter.FromHex(element.Attribute("TopRight").Value);
            var bl = SRGBColourConverter.FromHex(element.Attribute("BottomLeft").Value);
            var br = SRGBColourConverter.FromHex(element.Attribute("BottomRight").Value);

            return new ColourInfo
            {
                TopLeft = tl,
                TopRight = tr,
                BottomLeft = bl,
                BottomRight = br,
                HasSingleColour = false
            };
        }
        public object DeserializeFromString(string data, Type type)
        {
            throw new NotSupportedException($"{nameof(ColourInfo)} cannot be deserialized from plain string.");
        }

        public void SerializeAsElement(object value, XElement element)
        {
            if (value is ColourInfo c)
            {
                element.SetAttributeValue("TopLeft", SRGBColourConverter.ToHex(c.TopLeft));
                element.SetAttributeValue("TopRight", SRGBColourConverter.ToHex(c.TopRight));
                element.SetAttributeValue("BottomLeft", SRGBColourConverter.ToHex(c.BottomLeft));
                element.SetAttributeValue("BottomRight", SRGBColourConverter.ToHex(c.BottomRight));
            }
        }
        public void SerializeAsString(object value, out string data)
        {
            throw new NotSupportedException($"{nameof(ColourInfo)} cannot be serialized into plain string.");
        }

        public SyntaxNode GenerateInstantiation(object value, SyntaxGenerator g)
        {
            var c = (ColourInfo)value;

            return g.ObjectCreationExpression(
                namedType: g.IdentifierName(typeof(ColourInfo).FullName),
                arguments: new[]
                {
                    SRGBColourConverter.GenerateInstantiation(c.TopLeft, g),
                    SRGBColourConverter.GenerateInstantiation(c.BottomLeft, g),
                    SRGBColourConverter.GenerateInstantiation(c.TopRight, g),
                    SRGBColourConverter.GenerateInstantiation(c.BottomRight, g)
                }
            );
        }
    }
}
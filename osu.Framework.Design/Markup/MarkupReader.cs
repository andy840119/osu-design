using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using osu.Framework.Design.Markup.Converters;
using osu.Framework.Graphics;

namespace osu.Framework.Design.Markup
{
    public class MarkupReader
    {
        public readonly Dictionary<string, Dictionary<string, Type>> ImportedTypes = new Dictionary<string, Dictionary<string, Type>>();

        public DrawableData Parse(TextReader reader)
        {
            var doc = XDocument.Load(reader);
            var root = doc.Root;

            return recursiveParseElement(root);
        }

        DrawableData recursiveParseElement(XElement element)
        {
            var d = new DrawableData();

            // Parse drawable type
            d.DrawableType = getTypeFromName(element.Name);

            // Parse attributes
            foreach (var attr in element.Attributes().Where(a => a.Name.Namespace == XNamespace.None))
            {
                if (attr.Name.LocalName.Equals("id", StringComparison.OrdinalIgnoreCase))
                    d.Id = attr.Value;
                else
                    d.Attributes[attr.Name.LocalName] = parseAttribute(attr, d.DrawableType);
            }

            foreach (var elem in element.Elements().Where(e => e.Name.LocalName.StartsWith('_') && e.Name.Namespace == XNamespace.None))
                d.Attributes[elem.Name.LocalName.Substring(1)] = parseNestedAttribute(elem, d.DrawableType);

            // Parse children recursively
            d.Children = element
                .Elements()
                .Where(e => !e.Name.LocalName.StartsWith('_'))
                .Select(recursiveParseElement)
                .ToList();

            return d;
        }

        DrawableAttribute parseAttribute(XAttribute xattr, Type drawableType)
        {
            var property = drawableType.GetProperty(xattr.Name.LocalName);

            if (property == null)
                throw new KeyNotFoundException($"'{drawableType}' does not contain a property named '{xattr.Name.LocalName}'.");

            var attr = new DrawableAttribute(property);
            attr.Value = attr.Converter.DeserializeFromString(xattr.Value, property.PropertyType);
            attr.ParseAsNested = false;
            return attr;
        }

        DrawableAttribute parseNestedAttribute(XElement elem, Type drawableType)
        {
            var property = drawableType.GetProperty(elem.Name.LocalName.Substring(1));

            if (property == null)
                throw new KeyNotFoundException($"'{drawableType}' does not contain a property named '{elem.Name.LocalName}'.");

            var attr = new DrawableAttribute(property);
            attr.Value = attr.Converter.DeserializeFromElement(elem, property.PropertyType);
            attr.ParseAsNested = false;
            return attr;
        }

        IConverter getConverter(PropertyInfo p) => p.PropertyType.IsEnum ? ConverterFactory.Get<Enum>() : ConverterFactory.Get(p.PropertyType);

        Type getTypeFromName(XName name)
        {
            if (!ImportedTypes.TryGetValue(name.NamespaceName, out var types))
                types = importNamespace(name.NamespaceName);

            return types[name.LocalName];
        }

        Dictionary<string, Type> importNamespace(string resource)
        {
            var assembly = Assembly.Load(resource.Substring(0, resource.IndexOf(':')));
            var matchPattern = new Regex($"^{Regex.Escape(resource.Substring(resource.IndexOf(':') + 1)).Replace("\\?", ".").Replace("\\*", ".*")}$",
                RegexOptions.Singleline | RegexOptions.Compiled);

            return ImportedTypes[resource] = assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Drawable)) && t.GetConstructor(new Type[0]) != null && matchPattern.IsMatch(t.FullName))
                .ToDictionary(t => t.Name, t => t);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Framework.Design.Markup
{
    public class DrawableNode : List<DrawableNode>
    {
        public DrawableDocument Document { get; private set; }

        public Type DrawableType { get; set; }
        public string GivenName
        {
            get
            {
                var property = GetProperty("Name");

                if (property == null)
                    return (DrawableType?.Name ?? nameof(Drawable)) + GetHashCode();

                if (property is EmbeddedDrawableProperty embeddedProperty)
                    return embeddedProperty.RawValue;

                throw new MarkupException($"Property 'Name' must either be embedded or unspecified.");
            }
        }
        public bool IsNameSpecified => GetProperty("Name") != null;

        public bool IsDrawable => typeof(IDrawable).IsAssignableFrom(DrawableType);
        public bool IsContainer => typeof(IContainer).IsAssignableFrom(DrawableType);

        public ICollection<DrawableProperty> Properties { get; } = new List<DrawableProperty>();

        public DrawableProperty GetProperty(string name) => Properties.SingleOrDefault(p => p.Name == name);
        public void SetProperty(string name, DrawableProperty property)
        {
            var lastProperty = GetProperty(name);
            if (lastProperty != null)
                Properties.Remove(lastProperty);

            Properties.Add(property);
        }

        public void Load(XElement element, DrawableDocument document)
        {
            Document = document;

            // Parse type
            DrawableType = Document.GetDrawableType(
                import: ImportNamespaceInfo.Parse(element.Name.NamespaceName),
                localName: element.Name.LocalName
            );

            reloadProperties(element);
            reloadChildren(element);
        }

        void reloadProperties(XElement element)
        {
            // Ensure we are drawables
            if (!IsDrawable)
                throw new MarkupException($"Type '{DrawableType}' is not a drawable.");

            // Parse properties
            Properties.Clear();

            foreach (var attribute in element.Attributes())
            {
                // Ignore namespace attributes
                if (attribute.Name.LocalName == "xmlns" ||
                    element.GetPrefixOfNamespace(attribute.Name.Namespace) == "xmlns")
                    continue;

                var property = DrawableType.GetFieldOrProperty(attribute.Name.LocalName);

                if (property == null)
                    throw new MarkupException($"Drawable '{DrawableType}' [{GivenName}] does not contain a property named '{attribute.Name.LocalName}'.");

                try
                {
                    Properties.Add(DrawableProperty.Parse(property, attribute.Value));
                }
                catch (Exception e)
                {
                    throw new MarkupException($"Could not parse property '{attribute.Name}' of Drawable '{DrawableType}' [{GivenName}].", e);
                }
            }
        }

        void reloadChildren(XElement element)
        {
            // Ensure non-containers don't have children
            if (!IsContainer && element.Elements().Any())
                throw new MarkupException($"Drawable '{DrawableType}' [{GivenName}] cannot contain children as it is not a container.");

            // Parse children recursively
            Clear();

            foreach (var childElement in element.Elements())
            {
                var child = new DrawableNode();

                try
                {
                    child.Load(childElement, Document);
                }
                catch (Exception e)
                {
                    throw new MarkupException($"Could not parse child '{childElement.Name.LocalName}' of Drawable '{DrawableType}' [{GivenName}].", e);
                }

                Add(child);
            }
        }
    }
}
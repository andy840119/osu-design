using System;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Framework.Design.Markup
{
    public static class DrawableConverter
    {
        public static Drawable CreateDrawable(this DrawableNode node)
        {
            var arguments = node.DrawableType
                .GetConstructors()
                .First(c => c
                    .GetParameters()
                    .All(p => p.IsOptional))
                .GetParameters()
                .Select(p => p.DefaultValue)
                .ToArray();

            var drawable = Activator.CreateInstance(node.DrawableType, arguments) as Drawable;

            // Apply properties
            foreach (var property in node.Properties.OfType<EmbeddedDrawableProperty>())
                try
                {
                    property.PropertyInfo.SetValue(drawable, property.ParsedValue);
                }
                catch (Exception e)
                {
                    throw new MarkupException($"Could not parse value '{property.RawValue}' for property '{property.Name}' of Drawable '{node.DrawableType}' [{node.GivenName}].", e);
                }

            // Recursively create children
            if (drawable is IContainerCollection<Drawable> container)
                container.AddRange(node.Select(CreateDrawable));

            return drawable;
        }
    }
}
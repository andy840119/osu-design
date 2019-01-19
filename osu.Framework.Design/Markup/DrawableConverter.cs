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
            var drawable = Activator.CreateInstance(node.DrawableType) as Drawable;

            // Apply properties
            foreach (var property in node.Properties.OfType<EmbeddedDrawableProperty>())
                property.PropertyInfo.SetValue(drawable, property.ParsedValue);

            // Recursively create children
            if (drawable is IContainerCollection<Drawable> container)
                container.AddRange(node.Select(CreateDrawable));

            return drawable;
        }
    }
}
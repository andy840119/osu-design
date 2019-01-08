using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Framework.Design.Markup
{
    public class DrawableData
    {
        public string Id { get; set; }

        public Type DrawableType { get; set; }

        public Dictionary<string, DrawableAttribute> Attributes { get; set; } = new Dictionary<string, DrawableAttribute>();

        public List<DrawableData> Children { get; set; } = new List<DrawableData>();

        public void ApplyAttributes(Drawable d)
        {
            // Apply attribute values
            foreach (var attr in Attributes.Values)
                attr.Property.SetValue(d, attr.Value);
        }

        public Drawable CreateDrawable()
        {
            // Activate
            var d = (Drawable)Activator.CreateInstance(DrawableType);

            // Apply attributes to the drawable
            ApplyAttributes(d);

            // Create children as well, if we are a container
            if (d is IContainerCollection<Drawable> cont)
                cont.Children = Children.Select(c => c.CreateDrawable()).ToList();

            return d;
        }
    }
}
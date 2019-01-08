using System;
using System.Reflection;

namespace osu.Framework.Design.Markup
{
    public class DrawableAttribute
    {
        public string Name => Property.Name;
        public PropertyInfo Property { get; }

        public object Value { get; }
        public bool ParseAsNested { get; }

        public DrawableAttribute(PropertyInfo property, object value, bool nested)
        {
            Property = property;
            Value = value;
            ParseAsNested = nested;
        }
    }
}
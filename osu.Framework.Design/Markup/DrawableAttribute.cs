using System;
using System.Reflection;
using osu.Framework.Design.Markup.Converters;

namespace osu.Framework.Design.Markup
{
    public class DrawableAttribute
    {
        public string Name => Property.Name;
        public PropertyInfo Property { get; }
        public Type Type => Property.PropertyType;

        public object Value { get; }
        public bool ParseAsNested { get; }

        public DrawableAttribute(PropertyInfo property, object value, bool nested)
        {
            Property = property;
            Value = value;
            ParseAsNested = nested;
        }

        public IConverter Converter => Type.IsEnum ? ConverterFactory.Get<Enum>() : ConverterFactory.Get(Type);
    }
}
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

        public object Value { get; set; }
        public bool ParseAsNested { get; set; }

        public DrawableAttribute(PropertyInfo property)
        {
            Property = property;
        }

        public IConverter Converter => Type.IsEnum ? ConverterFactory.Get<Enum>() : ConverterFactory.Get(Type);
    }
}
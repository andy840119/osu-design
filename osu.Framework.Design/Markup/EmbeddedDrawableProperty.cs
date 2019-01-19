using osu.Framework.Design.CodeGeneration.ValueExpressionGenerators;
using osu.Framework.Design.Markup.ValueConverters;

namespace osu.Framework.Design.Markup
{
    public class EmbeddedDrawableProperty : DrawableProperty
    {
        public static new EmbeddedDrawableProperty Parse(IPropertyInfo property, string value) => new EmbeddedDrawableProperty(property)
        {
            RawValue = value
        };

        public string RawValue { get; set; }
        public object ParsedValue
        {
            get
            {
                Converter.Deserialize(RawValue, PropertyInfo.PropertyType, out var value);
                return value;
            }
            set
            {
                Converter.Serialize(value, PropertyInfo.PropertyType, out var data);
                RawValue = data;
            }
        }

        public IValueConverter Converter
        {
            get
            {
                var converter = ValueConverterFactory.Get(PropertyInfo.PropertyType);

                if (converter == null)
                    throw new MarkupException($"Could not find {nameof(IValueConverter)} for property of type '{PropertyInfo.PropertyType}'.");

                return converter;
            }
        }
        public IValueExpressionGenerator ExpressionGenerator
        {
            get
            {
                var converter = ValueExpressionGeneratorFactory.Get(PropertyInfo.PropertyType);

                if (converter == null)
                    throw new MarkupException($"Could not find {nameof(IValueExpressionGenerator)} for property of type '{PropertyInfo.PropertyType}'.");

                return converter;
            }
        }

        public EmbeddedDrawableProperty(IPropertyInfo property) : base(property)
        {
        }
    }
}
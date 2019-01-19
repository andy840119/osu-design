using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Framework.Design.Markup.ValueConverters
{
    public static class ValueConverterFactory
    {
        static readonly Dictionary<Type, IValueConverter> _converters = typeof(IValueConverter).Assembly
            .GetExportedTypes()
            .Where(t => !t.IsAbstract && typeof(IValueConverter).IsAssignableFrom(t))
            .Select(Activator.CreateInstance)
            .Cast<IValueConverter>()
            .ToDictionary(c => c.ConvertingType, c => c);

        public static void Register(IValueConverter converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            _converters[converter.ConvertingType] = converter;
        }

        public static IValueConverter Get<T>() => Get(typeof(T));
        public static IValueConverter Get(Type t)
        {
            // Enum is a special snowflake
            if (t.IsEnum)
                t = typeof(Enum);

            if (_converters.TryGetValue(t, out var converter))
                return converter;

            return null;
        }
    }
}
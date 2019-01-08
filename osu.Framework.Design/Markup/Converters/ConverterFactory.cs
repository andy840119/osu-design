using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Framework.Design.Markup.Converters
{
    public static class ConverterFactory
    {
        static readonly Dictionary<Type, IConverter> _converters = typeof(MarkupReader).Assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && typeof(IConverter).IsAssignableFrom(t))
            .Select(Activator.CreateInstance)
            .Cast<IConverter>()
            .ToDictionary(c => c.ConvertingType, c => c);

        public static void Register(IConverter converter) => _converters[converter.ConvertingType] = converter;

        public static IConverter Get<T>() => Get(typeof(T));
        public static IConverter Get(Type t) => _converters[t];
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Framework.Design.CodeGeneration.ValueExpressionGenerators
{
    public static class ValueExpressionGeneratorFactory
    {
        static readonly Dictionary<Type, IValueExpressionGenerator> _converters = typeof(IValueExpressionGenerator).Assembly
            .GetExportedTypes()
            .Where(t => !t.IsAbstract && typeof(IValueExpressionGenerator).IsAssignableFrom(t))
            .Select(Activator.CreateInstance)
            .Cast<IValueExpressionGenerator>()
            .ToDictionary(c => c.GeneratingType, c => c);

        public static void Register(IValueExpressionGenerator converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            _converters[converter.GeneratingType] = converter;
        }

        public static IValueExpressionGenerator Get<T>() => Get(typeof(T));
        public static IValueExpressionGenerator Get(Type t)
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
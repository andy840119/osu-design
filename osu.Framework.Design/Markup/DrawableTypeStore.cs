using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNet.Globbing;
using osu.Framework.Graphics;

namespace osu.Framework.Design.Markup
{
    public static class DrawableTypeStore
    {
        static readonly object _lock = new object();

        static readonly Dictionary<string, Type[]> _loadedTypes = new Dictionary<string, Type[]>();

        public static Type GetDrawable(ImportNamespaceInfo import, string localName)
        {
            lock (_lock)
            {
                if (!_loadedTypes.TryGetValue(import.AssemblyName, out var typeDict))
                    typeDict = _loadedTypes[import.AssemblyName] = Assembly
                        .Load(import.AssemblyName)
                        .GetExportedTypes()
                        .Where(t => t.IsClass &&
                                    t.IsSubclassOf(typeof(Drawable)))
                        .ToArray();

                var pattern = Glob.Parse(import.ImportPattern);
                var matchingTypes = typeDict
                    .Where(t => pattern.IsMatch(t.FullName) &&
                                localName.Equals(t.Name))
                    .ToArray();

                if (matchingTypes.Length == 1)
                {
                    var matchingType = matchingTypes[0];

                    // Ensure type is not abstract
                    if (matchingType.IsAbstract)
                        throw new MarkupException($"Drawable '{matchingType}' is abstract and cannot be used.");

                    // Ensure type can be created
                    if (!matchingType.GetConstructors().Any(c => c.GetParameters().All(p => p.IsOptional)))
                        throw new MarkupException($"Drawable '{matchingType}' does not have a suitable constructor.");

                    return matchingType;
                }

                if (matchingTypes.Length == 0)
                    throw new KeyNotFoundException($"Type '{localName}' could not be found in assembly '{import.AssemblyName}'.");

                throw new AmbiguousMatchException(
                    $"Drawable '{localName}' is ambiguous between the following types: " +
                    string.Join(", ", matchingTypes.Select(t => t.FullName))
                );
            }
        }
    }
}
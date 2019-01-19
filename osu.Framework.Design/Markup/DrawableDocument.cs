using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DotNet.Globbing;
using osu.Framework.Graphics;

namespace osu.Framework.Design.Markup
{
    public class DrawableDocument : DrawableNode
    {
        public string Namespace { get; set; }

        readonly Dictionary<ImportNamespaceInfo, Dictionary<string, Type>> _loadedTypes = new Dictionary<ImportNamespaceInfo, Dictionary<string, Type>>();

        public Type GetDrawableType(ImportNamespaceInfo import, string localName)
        {
            if (!_loadedTypes.TryGetValue(import, out var typeDict))
            {
                var assembly = Assembly.Load(import.AssemblyName);
                var pattern = Glob.Parse(import.ImportPattern);

                typeDict = _loadedTypes[import] = assembly
                    .GetExportedTypes()
                    .Where(t => t.IsClass &&
                                !t.IsAbstract &&
                                t.IsSubclassOf(typeof(Drawable)) &&
                                pattern.IsMatch(t.FullName))
                    .ToDictionary(t => t.Name, t => t);
            }

            if (!typeDict.TryGetValue(localName, out var type))
                throw new MarkupException($"Could not find drawable '{localName}' within assembly '{import.AssemblyName}'.");

            return type;
        }

        public void Load(string text) => Load(XElement.Parse(text));
        public void Load(XElement element) => Load(element, this);
        public void LoadFrom(TextReader reader) => Load(XElement.Load(reader));
    }
}
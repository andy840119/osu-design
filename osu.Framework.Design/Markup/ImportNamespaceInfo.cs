using System;

namespace osu.Framework.Design.Markup
{
    public class ImportNamespaceInfo : IEquatable<ImportNamespaceInfo>
    {
        public const string Scheme = "osufx://";

        public static ImportNamespaceInfo Parse(string uri)
        {
            if (!uri.StartsWith(Scheme))
                throw new NotSupportedException($"'{uri}' is not a valid osufx scheme.");

            var parts = uri.Substring(Scheme.Length).Split('/');

            if (parts.Length != 2)
                throw new FormatException($"'{uri}' is malformed.");

            return new ImportNamespaceInfo(parts[0], parts[1]);
        }

        public string AssemblyName { get; }
        public string ImportPattern { get; }

        public ImportNamespaceInfo(string assemblyName, string importPattern)
        {
            AssemblyName = assemblyName;
            ImportPattern = importPattern;
        }

        public override string ToString() => $"{Scheme}{AssemblyName}/{ImportPattern}";
        public override int GetHashCode() => HashCode.Combine(AssemblyName, ImportPattern);
        public override bool Equals(object obj)
        {
            if (obj is ImportNamespaceInfo import)
                return Equals(import);
            return false;
        }
        public bool Equals(ImportNamespaceInfo other) =>
            AssemblyName == other.AssemblyName &&
            ImportPattern == other.ImportPattern;
    }
}
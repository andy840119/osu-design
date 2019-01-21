using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace osu.Framework.Design
{
    public static class Extensions
    {
        public static int CountStart(this string str, char character, int startIndex = 0)
        {
            for (var i = startIndex; i < str.Length; i++)
                if (str[i] != character)
                    return i - startIndex;

            return 0;
        }

        static readonly Regex _spaceRegex = new Regex(@"\s", RegexOptions.Compiled);

        public static string RemoveAllSpaces(this string str) => _spaceRegex.Replace(str, "");

        // Thank you Michael Liu
        // https://stackoverflow.com/questions/2055927/ienumerable-and-recursion-using-yield-return
        public static IEnumerable<TSource> RecursiveSelect<TSource>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TSource>> childSelector)
        {
            var stack = new Stack<IEnumerator<TSource>>();
            var enumerator = source.GetEnumerator();

            try
            {
                while (true)
                {
                    if (enumerator.MoveNext())
                    {
                        TSource element = enumerator.Current;
                        yield return element;

                        stack.Push(enumerator);
                        enumerator = childSelector(element).GetEnumerator();
                    }
                    else if (stack.Count > 0)
                    {
                        enumerator.Dispose();
                        enumerator = stack.Pop();
                    }
                    else
                    {
                        yield break;
                    }
                }
            }
            finally
            {
                enumerator.Dispose();

                while (stack.Count > 0) // Clean up in case of an exception.
                {
                    enumerator = stack.Pop();
                    enumerator.Dispose();
                }
            }
        }

        static readonly Regex _splitRegex = new Regex(@",(?![^(]*\))", RegexOptions.Compiled);

        public static string[] SplitByComma(this string str) => _splitRegex.Split(str).Select(s => s.Trim()).ToArray();

        public static IPropertyInfo GetFieldOrProperty(this Type type, string name)
        {
            var field = type.GetField(name);
            if (field != null)
                return new FieldInfoHelper(field);

            var property = type.GetProperty(name);
            if (property != null)
                return new PropertyInfoHelper(property);

            return null;
        }

        public static DirectoryInfoBase GetCurrentDirectory(this IFileSystem fileSystem) =>
            fileSystem.DirectoryInfo.FromDirectoryName(fileSystem.Directory.GetCurrentDirectory());

        public static FileInfoBase GetFile(this DirectoryInfoBase dir, string name) =>
            dir.FileSystem.FileInfo.FromFileName(dir.FileSystem.Path.Combine(dir.FullName, name));

        public static DirectoryInfoBase GetDirectory(this DirectoryInfoBase dir, string name) =>
            dir.FileSystem.DirectoryInfo.FromDirectoryName(dir.FileSystem.Path.Combine(dir.FullName, name));

        public static string NameRelativeTo(this FileSystemInfoBase from, FileSystemInfoBase to)
        {
            var fromName = from is DirectoryInfoBase fromDir
                ? fromDir.FullName + from.FileSystem.Path.DirectorySeparatorChar
                : from.FullName;
            var toName = to is DirectoryInfoBase toDir
                ? toDir.FullName + to.FileSystem.Path.DirectorySeparatorChar
                : to.FullName;

            var fromUri = new Uri(fromName);
            var toUri = new Uri(toName);

            var relativeUri = toUri.MakeRelativeUri(fromUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            return relativePath;
        }

        public static FileSystemWatcherBase CreateWatcher(this DirectoryInfoBase dir)
        {
            var watcher = dir.FileSystem.FileSystemWatcher.CreateNew();
            watcher.Path = dir.FullName;
            return watcher;
        }
    }
}
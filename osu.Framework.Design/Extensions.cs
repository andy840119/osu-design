using System;
using System.IO;
using System.IO.Abstractions;

namespace osu.Framework.Design
{
    static class Extensions
    {
        public static DirectoryInfoBase GetCurrentDirectory(this IFileSystem fileSystem) => fileSystem.DirectoryInfo.FromDirectoryName(fileSystem.Directory.GetCurrentDirectory());

        public static FileInfoBase GetFile(this DirectoryInfoBase dir, string name) =>
            dir.FileSystem.FileInfo.FromFileName(dir.FileSystem.Path.Combine(dir.FullName, name));

        public static DirectoryInfoBase GetDirectory(this DirectoryInfoBase dir, string name) =>
            dir.FileSystem.DirectoryInfo.FromDirectoryName(dir.FileSystem.Path.Combine(dir.FullName, name));

        public static string NameRelativeTo(this FileSystemInfoBase from, FileSystemInfoBase to) => GetRelativePath(to.FullName, from.FullName).TrimEnd('\\', '/');

        public static string GetRelativePath(string fromPath, string toPath)
        {
            var fromUri = new Uri(AppendDirectorySeparatorChar(fromPath));
            var toUri = new Uri(AppendDirectorySeparatorChar(toPath));

            if (fromUri.Scheme != toUri.Scheme)
                return toPath;

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            return relativePath;
        }

        static string AppendDirectorySeparatorChar(string path)
        {
            if (Path.HasExtension(path) ||
                path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                return path;

            // Append a slash only if the path is a directory and does not have a slash.
            return path + Path.DirectorySeparatorChar;
        }
    }
}
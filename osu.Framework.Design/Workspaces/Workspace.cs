using System;
using System.IO.Abstractions;

namespace osu.Framework.Design.Workspaces
{
    public class Workspace : IDisposable
    {
        public DirectoryInfoBase Directory { get; }

        public Workspace(DirectoryInfoBase baseDirectory)
        {
            Directory = baseDirectory;
        }

        public void Dispose()
        {
        }
    }
}
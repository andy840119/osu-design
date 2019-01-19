using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading;

namespace osu.Framework.Design.Tests.Helpers
{
    public class MockFileSystemWatcher : FileSystemWatcherBase
    {
        public override bool IncludeSubdirectories { get; set; }
        public override bool EnableRaisingEvents { get; set; }
        public override string Filter { get; set; }
        public override int InternalBufferSize { get; set; }
        public override NotifyFilters NotifyFilter { get; set; }
        public override string Path { get; set; }

        public override WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType)
        {
            Thread.Sleep(Timeout.Infinite);

            return new WaitForChangedResult();
        }

        public override WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType, int timeout)
        {
            Thread.Sleep(timeout);

            throw new Exception();
        }
    }

    public class MockFileSystemWatcherFactory : IFileSystemWatcherFactory
    {
        public FileSystemWatcherBase CreateNew() => new MockFileSystemWatcher();
        public FileSystemWatcherBase FromPath(string path) => new MockFileSystemWatcher
        {
            Path = path
        };
    }
}
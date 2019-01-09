using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using osu.Framework.Configuration;
using osu.Framework.Threading;

namespace osu.Framework.Design.Solution
{
    public class Workspace : IDisposable
    {
        public readonly Scheduler Scheduler = new Scheduler();

        public Bindable<DirectoryInfoBase> Directory { get; } = new Bindable<DirectoryInfoBase>();
        public BindableList<Document> Documents { get; } = new BindableList<Document>();
        public Bindable<Document> CurrentDocument { get; } = new Bindable<Document>();

        public Workspace()
        {
            Directory.ValueChanged += handleDirectoryChange;
        }

        FileSystemWatcherBase _watcher;

        void handleDirectoryChange(DirectoryInfoBase dir)
        {
            CurrentDocument.Value = null;

            // Dispose last watcher
            if (_watcher != null)
            {
                _watcher.Created -= handleCreated;
                _watcher.Changed -= handleChanged;
                _watcher.Deleted -= handleDeleted;
                _watcher.Dispose();
            }

            if (dir == null)
                return;

            _watcher = dir.FileSystem.FileSystemWatcher.CreateNew();
            _watcher.Path = dir.FullName;
            _watcher.IncludeSubdirectories = true;
            _watcher.EnableRaisingEvents = true;
            _watcher.Created += handleCreated;
            _watcher.Changed += handleChanged;
            _watcher.Deleted += handleDeleted;

            Documents.Clear();
            Documents.AddRange(dir
                .EnumerateFiles("*", SearchOption.AllDirectories)
                .Select(f => new Document(this, f))
                .OrderBy(d => d.FullName));
        }

        void handleCreated(object sender, FileSystemEventArgs e)
        {
            Scheduler.Add(() =>
            {
                var document = new Document(this, Directory.Value.FileSystem.FileInfo.FromFileName(e.FullPath));

                // TODO: Add in sorted order, once BinarySearch is implemented in BindableList
                // var index = Files.BinarySearch(file);
                // if (index < 0) index = ~index;
                // Files.Insert(index, file);

                Documents.Add(document);
            });
        }

        void handleChanged(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath != CurrentDocument.Value?.File.FullName)
                return;

            Scheduler.Add(() =>
            {
                var last = CurrentDocument.Value;
                CurrentDocument.Value = null;
                CurrentDocument.Value = last;
            });
        }

        void handleDeleted(object sender, FileSystemEventArgs e)
        {
            Scheduler.Add(() =>
            {
                Documents.RemoveAll(d => d.File.FullName == e.FullPath);
            });
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }
    }
}
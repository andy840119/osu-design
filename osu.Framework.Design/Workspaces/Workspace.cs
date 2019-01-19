using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using osu.Framework.Configuration;
using osu.Framework.Threading;

namespace osu.Framework.Design.Workspaces
{
    public class Workspace : IDisposable
    {
        /// <summary>
        /// Scheduler is used as file system events could be triggered on a different thread.
        /// </summary>
        /// <returns></returns>
        public Scheduler Scheduler { get; } = new Scheduler();

        public DirectoryInfoBase Directory { get; }
        public FileSystemWatcherBase Watcher { get; }

        readonly BindableList<Document> _docs = new BindableList<Document>();
        public BindableList<Document> Documents
        {
            get => _docs;
            set
            {
                _docs.UnbindBindings();
                _docs.BindTo(value);
            }
        }

        public Workspace(DirectoryInfoBase baseDirectory)
        {
            Directory = baseDirectory;

            Watcher = baseDirectory.CreateWatcher();
            Watcher.Created += handleFileCreated;
            Watcher.Deleted += handleFileDeleted;
            Watcher.Renamed += handleFileRenamed;
            Watcher.Changed += handleFileChanged;

            foreach (var file in Directory.EnumerateFiles("*", SearchOption.AllDirectories))
                Documents.Add(new Document(this, file.NameRelativeTo(Directory)));
        }

        void handleFileCreated(object sender, FileSystemEventArgs e)
        {
            var fullName = Directory.FileSystem.FileInfo.FromFileName(e.FullPath).NameRelativeTo(Directory);

            Scheduler.Add(() =>
            {
                if (!_docs.Any(d => d.FullName == fullName))
                    _docs.Add(new Document(this, fullName));
            });
        }
        void handleFileDeleted(object sender, FileSystemEventArgs e)
        {
            var fullName = Directory.FileSystem.FileInfo.FromFileName(e.FullPath).NameRelativeTo(Directory);

            Scheduler.Add(() =>
            {
                var doc = GetDocument(fullName);

                if (doc != null)
                    _docs.Remove(doc);
            });
        }
        void handleFileRenamed(object sender, RenamedEventArgs e)
        {
            var fullName = Directory.FileSystem.FileInfo.FromFileName(e.FullPath).NameRelativeTo(Directory);

            Scheduler.Add(() =>
            {
                var doc = GetDocument(fullName);

                if (doc != null)
                    doc.FullName.Value = fullName;
            });
        }
        void handleFileChanged(object sender, FileSystemEventArgs e)
        {
            var fullName = Directory.FileSystem.FileInfo.FromFileName(e.FullPath).NameRelativeTo(Directory);

            Scheduler.Add(() =>
            {
                var doc = GetDocument(fullName);

                if (doc != null)
                    doc.LastWriteTime.Value = doc.File.LastWriteTime;
            });
        }

        public Document CreateDocument(string fullName)
        {
            var doc = new Document(this, Directory.GetFile(fullName).NameRelativeTo(Directory));

            _docs.Add(doc);
            doc.Exists.Value = true;

            return doc;
        }
        public void DeleteDocument(string fullName)
        {
            var doc = GetDocument(fullName);

            if (doc == null)
                return;

            _docs.Remove(doc);
            doc.Exists.Value = false;
        }
        public Document GetDocument(string fullName) => _docs.FirstOrDefault(d => d.FullName == fullName);

        public void Dispose()
        {
        }
    }
}
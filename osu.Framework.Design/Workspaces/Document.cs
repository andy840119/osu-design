using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using osu.Framework.Configuration;

namespace osu.Framework.Design.Workspaces
{
    public class Document
    {
        public Workspace Workspace { get; }
        public FileInfoBase File { get; }

        readonly Bindable<string> _fullName = new Bindable<string>();
        public Bindable<string> FullName
        {
            get => _fullName;
            set
            {
                _fullName.UnbindBindings();
                _fullName.BindTo(value);
            }
        }
        public string Name => File.FileSystem.Path.GetFileName(_fullName);

        readonly Bindable<bool> _exists = new Bindable<bool>();
        public Bindable<bool> Exists
        {
            get => _exists;
            set
            {
                _exists.UnbindBindings();
                _exists.BindTo(value);
            }
        }

        readonly Bindable<DateTime> _lastWriteTime = new Bindable<DateTime>();
        public Bindable<DateTime> LastWriteTime
        {
            get => _lastWriteTime;
            set
            {
                _lastWriteTime.UnbindBindings();
                _lastWriteTime.BindTo(value);
            }
        }

        public IEnumerable<DirectoryInfoBase> Folders
        {
            get
            {
                var parts = _fullName.Value.Split(File.FileSystem.Path.DirectorySeparatorChar);

                for (var i = 0; i < parts.Length - 1; i++)
                    yield return Workspace.Directory.GetDirectory(File.FileSystem.Path.Combine(parts.Take(i + 1).ToArray()));
            }
        }

        public DocumentType Type
        {
            get
            {
                switch (File.Extension.ToLowerInvariant())
                {
                    default: return DocumentType.Unknown;
                    case ".cs":
                    case ".csx": return DocumentType.CSharp;
                    case ".osuml": return DocumentType.osuML;
                    case ".csproj":
                    case ".vbproj":
                    case ".xml": return DocumentType.XML;
                    case ".jpg":
                    case ".png": return DocumentType.Texture;
                    case ".mp3": return DocumentType.Audio;
                }
            }
        }

        public Document(Workspace workspace, string fullName)
        {
            Workspace = workspace;
            File = Workspace.Directory.GetFile(fullName);

            _fullName.Value = fullName;
            _fullName.ValueChanged += n =>
            {
                File.Refresh();
                if (File.Exists)
                    File.MoveTo(Workspace.Directory.GetFile(n).FullName);
            };

            _exists.Value = File.Exists;
            _exists.ValueChanged += e =>
            {
                File.Refresh();
                if (e && !File.Exists)
                {
                    File.Create().Dispose();
                    _lastWriteTime.Value = DateTime.Now;
                }
                else if (!e && File.Exists)
                    File.Delete();
            };

            if (File.Exists)
                _lastWriteTime.Value = File.LastWriteTime;
            _lastWriteTime.ValueChanged += t =>
            {
                File.Refresh();
                if (File.Exists)
                    File.LastWriteTime = t;
            };
        }

        public Stream OpenRead()
        {
            if (!Exists)
                throw new InvalidOperationException($"Cannot open nonexistent file '{FullName}'.");

            return File.FileSystem.FileStream.Create(File.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        public TextReader OpenReader() => new StreamReader(OpenRead());

        public Stream OpenWrite()
        {
            if (!Exists)
                throw new InvalidOperationException($"Cannot open nonexistent file '{FullName}'.");

            return File.FileSystem.FileStream.Create(File.FullName, FileMode.Create, FileAccess.Write, FileShare.Read);
        }
        public TextWriter OpenWriter() => new StreamWriter(OpenWrite());

        public WorkingDocument CreateWorkingDocument() => new WorkingDocument(this);
    }
}
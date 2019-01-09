using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace osu.Framework.Design.Solution
{
    public class Document
    {
        public Workspace Workspace { get; }
        public FileInfoBase File { get; }

        public string Name => File.Name;
        public string FullName => File.NameRelativeTo(Workspace.Directory);

        public Document(Workspace workspace, FileInfoBase file)
        {
            Workspace = workspace;
            File = file;
        }

        public IEnumerable<DirectoryInfoBase> Folders => enumerateParents().Reverse();

        IEnumerable<DirectoryInfoBase> enumerateParents()
        {
            var parent = File.Directory;
            var workspace = Workspace.Directory.Value;

            while (parent.FullName != workspace.FullName)
            {
                yield return parent;
                parent = parent.Parent;
            }
        }

        public Stream OpenRead() => File.OpenRead();
        public Stream OpenWrite() => File.OpenWrite();

        public DocumentType Type
        {
            get
            {
                switch (File.Extension)
                {
                    default:
                        return DocumentType.Unknown;
                    case ".txt":
                        return DocumentType.Text;
                    case ".cs":
                        return DocumentType.Source;
                    case ".xml":
                        return DocumentType.Markup;
                    case ".csproj":
                    case ".vbproj":
                        return DocumentType.Project;
                    case ".jpg":
                    case ".png":
                    case ".bmp":
                        return DocumentType.Image;
                    case ".mp3":
                    case ".ogg":
                        return DocumentType.Audio;
                }
            }
        }
    }
}
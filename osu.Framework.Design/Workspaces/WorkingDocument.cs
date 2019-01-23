using System;
using Microsoft.CodeAnalysis;
using osu.Framework.Configuration;
using osu.Framework.Design.CodeGeneration;
using osu.Framework.Design.Markup;

namespace osu.Framework.Design.Workspaces
{
    public class WorkingDocument : IDisposable
    {
        public Document Document { get; }

        readonly Bindable<string> _content = new Bindable<string>();
        public Bindable<string> Content
        {
            get => _content;
            set
            {
                _content.UnbindBindings();
                _content.BindTo(value);
            }
        }

        readonly Bindable<DateTime> _lastWriteTime;

        public WorkingDocument(Document doc)
        {
            Document = doc;

            _lastWriteTime = doc.LastWriteTime.GetBoundCopy();
            _lastWriteTime.BindValueChanged(handleWrite, runOnceImmediately: true);

            _content.ValueChanged += c => IsSynchronized = false;
        }

        public bool IsSynchronized { get; private set; } = true;

        void handleWrite(DateTime writeTime)
        {
            if (IsSynchronized)
            {
                Reload();

                // Generate drawable if we are osuML
                if (Document.Type == DocumentType.osuML)
                    generateDrawable(Document.Workspace
                        .CreateDocument(Document.File.FileSystem.Path
                        .ChangeExtension(Document.FullName, ".Designer.cs")));
            }
        }

        void generateDrawable(Document doc)
        {
            var node = new DrawableNode();

            // Parse document
            try { node.Load(Content); }
            catch { return; }

            // Generate syntax
            var syntax = node
                .GenerateClassSyntax()
                .NormalizeWhitespace();

            // Save syntax
            using (var writer = doc.OpenWriter())
                syntax.WriteTo(writer);
        }

        public void Reload()
        {
            using (var reader = Document.OpenReader())
                _content.Value = reader.ReadToEnd();

            IsSynchronized = true;
        }
        public void Save()
        {
            using (var writer = Document.OpenWriter())
                writer.Write(_content.Value);

            IsSynchronized = true;

            Document.LastWriteTime.Value = DateTime.Now;
        }

        public void Dispose()
        {
            _content.UnbindAll();
            _lastWriteTime.UnbindAll();
        }

        public override string ToString() => Document.ToString();
    }
}
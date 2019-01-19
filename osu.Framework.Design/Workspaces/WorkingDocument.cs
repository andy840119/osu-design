using System;
using osu.Framework.Configuration;

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
                Reload();
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
        }

        public void Dispose()
        {
            _content.UnbindAll();
            _lastWriteTime.UnbindAll();
        }
    }
}
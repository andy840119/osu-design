using osu.Framework.Configuration;

namespace osu.Framework.Design.Workspaces
{
    public class WorkingDocument
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

        public WorkingDocument(Document doc)
        {
            Document = doc;
        }

        public void Reload()
        {
            using (var reader = Document.OpenReader())
                _content.Value = reader.ReadToEnd();
        }
        public void Save()
        {
            using (var writer = Document.OpenWriter())
                writer.Write(_content.Value);
        }
    }
}
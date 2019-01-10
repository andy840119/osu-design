using System.IO.Abstractions;
using System.Threading.Tasks;
using osu.Framework.Configuration;

namespace osu.Framework.Design.Solution
{
    public class WorkingDocument
    {
        public Document Document { get; }

        public Bindable<string> Content { get; } = new Bindable<string>();

        internal WorkingDocument(Document doc)
        {
            Document = doc;
        }

        public void UpdateContent()
        {
            using (var reader = Document.OpenReader())
                Content.Value = reader.ReadToEnd();
        }

        public async Task UpdateContentAsync()
        {
            using (var reader = Document.OpenReader())
                Content.Value = await reader.ReadToEndAsync();
        }
    }
}
using osu.Framework.Allocation;
using osu.Framework.Design.CodeEditor;
using osu.Framework.Design.CodeEditor.Highlighters;
using osu.Framework.Design.Workspaces;
using osu.Framework.Graphics;

namespace osu.Framework.Design.Designer
{
    public class DesignerWindow : ComponentWindow
    {
        public readonly Document Document;

        PreviewContainer _preview;
        DrawableEditor _editor;

        public DesignerWindow(Document doc) : base("Designer")
        {
            Document = doc;
        }

        DependencyContainer _dependencies;
        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            _dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        WorkingDocument _document;

        [BackgroundDependencyLoader]
        void load()
        {
            // Working document for all components of this designer
            _dependencies.Cache(_document = Document.CreateWorkingDocument());

            if (Document.Type == DocumentType.osuML)
            {
                Child = new HalvedContainer(
                    Direction.Vertical,
                    _preview = new PreviewContainer
                    {
                        Height = 500
                    },
                    _editor = new DrawableEditor()
                )
                {
                    RelativeSizeAxes = Axes.Both
                };
            }
            else
            {
                Child = _editor = new DrawableEditor
                {
                    RelativeSizeAxes = Axes.Both
                };
            }

            switch (Document.Type)
            {
                case DocumentType.CSharp:
                    _editor.SyntaxHighlighter.Value = new CSharpSyntaxHighlighter();
                    break;

                case DocumentType.osuML:
                case DocumentType.XML:
                    _editor.SyntaxHighlighter.Value = new XMLSyntaxHighlighter();
                    break;
            }

            _editor.Current.BindTo(_document.Content);
        }
    }
}
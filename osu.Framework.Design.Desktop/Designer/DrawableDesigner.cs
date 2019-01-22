using osu.Framework.Allocation;
using osu.Framework.Design.CodeEditor;
using osu.Framework.Design.CodeEditor.Highlighters;
using osu.Framework.Design.Workspaces;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace osu.Framework.Design.Designer
{
    public class DrawableDesigner : CompositeDrawable
    {
        public readonly Document Document;

        PreviewContainer _preview;
        DrawableEditor _editor;

        public DrawableDesigner(Document doc)
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

            var editorContainer = new Container
            {
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = DesignerColours.Editor
                    },
                    _editor = new DrawableEditor
                    {
                        RelativeSizeAxes = Axes.Both
                    }
                }
            };

            if (Document.Type == DocumentType.osuML)
            {
                InternalChild = new HalvedContainer(
                    Direction.Vertical,
                    _preview = new PreviewContainer
                    {
                        RelativeSizeAxes = Axes.Y,
                        Height = 0.4f
                    },
                    editorContainer
                )
                {
                    RelativeSizeAxes = Axes.Both
                };
            }
            else
            {
                InternalChild = editorContainer;
                editorContainer.RelativeSizeAxes = Axes.Both;
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
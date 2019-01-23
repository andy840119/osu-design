using osu.Framework.Allocation;
using osu.Framework.Design.CodeEditor;
using osu.Framework.Design.CodeEditor.Highlighters;
using osu.Framework.Design.Workspaces;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace osu.Framework.Design.Designer
{
    public class DrawableDesigner : CompositeDrawable
    {
        public readonly WorkingDocument WorkingDocument;

        PreviewContainer _preview;
        DrawableEditor _editor;

        public DrawableDesigner(WorkingDocument doc)
        {
            WorkingDocument = doc;
        }

        DependencyContainer _dependencies;
        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            _dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        [BackgroundDependencyLoader]
        void load()
        {
            // Working document for all components of this designer
            _dependencies.Cache(WorkingDocument);

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

            if (WorkingDocument.Document.Type == DocumentType.osuML)
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

            switch (WorkingDocument.Document.Type)
            {
                case DocumentType.CSharp:
                    _editor.SyntaxHighlighter.Value = new CSharpSyntaxHighlighter();
                    break;

                case DocumentType.osuML:
                case DocumentType.XML:
                    _editor.SyntaxHighlighter.Value = new XMLSyntaxHighlighter();
                    break;
            }

            _editor.Current.BindTo(WorkingDocument.Content);
        }

        public override void Show()
        {
            this.FadeIn(duration: 200);

            Schedule(_editor.Focus);
        }
        public override void Hide()
        {
            this.FadeOut(duration: 200);

            Schedule(_editor.Unfocus);
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            base.OnKeyDown(e);

            switch (e.Key)
            {
                case Key.S:
                    if (e.ControlPressed)
                    {
                        WorkingDocument.Save();
                        break;
                    }
                    return false;
                default:
                    return false;
            }

            return true;
        }
    }
}
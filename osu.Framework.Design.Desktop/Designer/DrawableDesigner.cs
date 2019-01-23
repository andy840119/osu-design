using osu.Framework.Allocation;
using osu.Framework.Design.CodeEditor;
using osu.Framework.Design.CodeEditor.Highlighters;
using osu.Framework.Design.Workspaces;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
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

            if (WorkingDocument.Document.Type == DocumentType.Unknown)
            {
                InternalChild = new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = $"osu!design does not support editing of file type '{WorkingDocument.Document.File.Extension}'.",
                    TextSize = 18,
                    Font = "Inconsolata"
                };
                return;
            }

            Drawable editorContainer = new Container
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

            switch (WorkingDocument.Document.Type)
            {
                case DocumentType.osuML:
                    editorContainer = new HalvedContainer(
                        Direction.Vertical,
                        _preview = new PreviewContainer
                        {
                            RelativeSizeAxes = Axes.Y,
                            Height = 0.4f
                        },
                        editorContainer
                    );

                    _editor.SyntaxHighlighter.Value = new XMLSyntaxHighlighter();
                    break;

                case DocumentType.XML:
                    _editor.SyntaxHighlighter.Value = new XMLSyntaxHighlighter();
                    break;

                case DocumentType.CSharp:
                    _editor.SyntaxHighlighter.Value = new CSharpSyntaxHighlighter();
                    break;
            }

            editorContainer.RelativeSizeAxes = Axes.Both;
            InternalChild = editorContainer;

            _editor.Current.BindTo(WorkingDocument.Content);
        }

        public override void Show()
        {
            this.FadeIn(duration: 200);

            if (_editor != null)
                Schedule(_editor.Focus);
        }
        public override void Hide()
        {
            this.FadeOut(duration: 200);

            if (_editor != null)
                Schedule(_editor.Unfocus);
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            base.OnKeyDown(e);

            if (_editor == null)
                return false;

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
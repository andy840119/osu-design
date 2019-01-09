using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Design.CodeEditor;
using osu.Framework.Design.Solution;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;

namespace osu.Framework.Design.Designer
{
    public class WorkspaceScreen : Screen
    {
        readonly SolutionBrowser _browser;
        readonly PreviewContainer _preview;
        readonly DrawableEditor _editor;

        public WorkspaceScreen()
        {
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = DesignerColours.Editor
                },
                _browser = new SolutionBrowser
                {
                    RelativeSizeAxes = Axes.Both,
                    Width = 0.2f,
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft
                },
                _preview = new PreviewContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Width = 0.8f,
                    Height = 0.6f,
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight
                },
                _editor = new DrawableEditor
                {
                    RelativeSizeAxes = Axes.Both,
                    Width = 0.8f,
                    Height = 0.4f,
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight
                }
            };
        }

        Bindable<WorkingDocument> _document;
        Bindable<string> _documentContent;

        [BackgroundDependencyLoader]
        void load(Workspace workspace)
        {
            _document = workspace.CurrentDocument.GetBoundCopy();
            _document.BindValueChanged(d =>
            {
                _documentContent?.UnbindAll();

                if (d == null)
                    return;

                _documentContent = d.Content.GetBoundCopy();
            }, true);

            _editor.Model.Lines.ItemsAdded += _ => _documentContent.Value = _editor.Model.Text;
            _editor.Model.Lines.ItemsRemoved += _ => _documentContent.Value = _editor.Model.Text;
        }
    }
}
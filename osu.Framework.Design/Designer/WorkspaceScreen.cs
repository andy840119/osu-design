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
                // _preview = new PreviewContainer
                // {
                //     RelativeSizeAxes = Axes.Both,
                //     Width = 0.8f,
                //     Anchor = Anchor.TopRight,
                //     Origin = Anchor.TopRight
                // },
                _editor = new DrawableEditor
                {
                    RelativeSizeAxes = Axes.Both,
                    Width = 0.8f,
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight
                }
            };
        }

        Bindable<Document> _doc;

        [BackgroundDependencyLoader]
        void load(Workspace workspace)
        {
            _doc = workspace.CurrentDocument;
            _doc.BindValueChanged(d =>
            {
                _editor.Model.Lines.Clear();

                if (d == null)
                    return;

                using (var reader = d.OpenReader())
                    _editor.Model.Set(reader.ReadToEnd());
            });
        }
    }
}
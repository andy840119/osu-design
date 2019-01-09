using osu.Framework.Design.CodeEditor;
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

            _editor.Insert(0, "test hello");
        }
    }
}
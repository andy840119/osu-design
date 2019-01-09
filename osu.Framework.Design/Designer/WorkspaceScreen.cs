using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace osu.Framework.Design.Designer
{
    public class WorkspaceScreen : Screen
    {
        readonly SolutionBrowser _browser;
        readonly PreviewContainer _preview;

        public WorkspaceScreen()
        {
            Children = new Drawable[]
            {
                _browser = new SolutionBrowser
                {
                    RelativeSizeAxes = Axes.Y,
                    Width = 200,
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft
                },
                _preview = new PreviewContainer
                {
                    RelativeSizeAxes = Axes.Y,
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight
                }
            };
        }

        protected override void Update()
        {
            base.Update();

            _preview.Width = DrawWidth - _browser.DrawWidth;
        }
    }
}
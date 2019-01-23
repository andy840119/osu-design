using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;

namespace osu.Framework.Design.Designer
{
    public class WorkspaceScreen : Screen
    {
        WorkspaceBrowser _browser;
        DesignerWindow _designer;

        [BackgroundDependencyLoader]
        void load()
        {
            Child = new HalvedContainer(
                Direction.Horizontal,
                _browser = new WorkspaceBrowser
                {
                    RelativeSizeAxes = Axes.X,
                    Width = 0.2f,
                    OpenDocument = d => _designer.SelectDocument(d)
                },
                _designer = new DesignerWindow()
            )
            {
                RelativeSizeAxes = Axes.Both
            };
        }
    }
}
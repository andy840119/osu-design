using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;

namespace osu.Framework.Design.Designer
{
    public class WorkspaceScreen : Screen
    {
        SolutionBrowser _browser;
        Container<DesignerWindow> _designerContainer;

        [BackgroundDependencyLoader]
        void load()
        {
            Child = new HalvedContainer(
                Direction.Horizontal,
                _browser = new SolutionBrowser
                {
                    Width = 240,
                    OpenDocument = d =>
                    {
                        _designerContainer.Clear();
                        _designerContainer.Add(new DesignerWindow(d)
                        {
                            RelativeSizeAxes = Axes.Both
                        });
                    }
                },
                _designerContainer = new Container<DesignerWindow>()
            )
            {
                RelativeSizeAxes = Axes.Both
            };
        }
    }
}
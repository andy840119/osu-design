using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace osu.Framework.Design.Designer
{
    public abstract class ComponentWindow : Container
    {
        readonly Drawable _headContainer;
        readonly Container _content;

        protected Container Head { get; }

        protected override Container<Drawable> Content => _content;

        public ComponentWindow(string name)
        {
            Name = name;

            InternalChildren = new Drawable[]
            {
                _headContainer = new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = DesignerColours.SideHead
                        },
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Padding = new MarginPadding(8),
                            Spacing = new Vector2(8),
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Children = new Drawable[]
                            {
                                new SpriteText
                                {
                                    Text = name?.ToUpperInvariant(),
                                    TextSize = 18,
                                    Font = "Nunito-Bold",
                                    Colour = DesignerColours.SideForeground,
                                    Shadow = true
                                },
                                Head = new Container
                                {
                                    AutoSizeAxes = Axes.Both
                                }
                            }
                        }
                    },
                    Alpha = string.IsNullOrWhiteSpace(name) ? 0 : 1
                },
                _content = new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft
                }
            };
        }

        protected override void Update()
        {
            base.Update();

            if (_headContainer.IsPresent)
                _content.Height = DrawHeight - _headContainer.DrawHeight;
            else
                _content.Height = DrawHeight;
        }
    }
}
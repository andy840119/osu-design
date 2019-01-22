using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace osu.Framework.Design.Designer
{
    public abstract class ComponentWindow : Container
    {
        Drawable _headContainer;
        Container _content;

        protected Container Head { get; private set; }

        protected override Container<Drawable> Content => _content;

        public ComponentWindow(string name)
        {
            Name = name;
        }

        [BackgroundDependencyLoader]
        void load()
        {
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
                            Direction = FillDirection.Horizontal,
                            Children = new Drawable[]
                            {
                                new SpriteText
                                {
                                    Text = Name?.ToUpperInvariant(),
                                    TextSize = 18,
                                    Font = "Nunito-Bold",
                                    Colour = DesignerColours.SideForeground,
                                    Shadow = true,
                                    Padding = new MarginPadding
                                    {
                                        Top = 10,
                                        Right = 14,
                                        Bottom = 10,
                                        Left = 14
                                    },
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft
                                },
                                Head = new Container
                                {
                                    RelativeSizeAxes = Axes.Both
                                }
                            }
                        }
                    },
                    Alpha = string.IsNullOrEmpty(Name) ? 0 : 1
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
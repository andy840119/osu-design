using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;

namespace osu.Framework.Design.Designer
{
    public abstract class ComponentWindow : Container
    {
        readonly Drawable _head;
        readonly Container _content;

        protected override Container<Drawable> Content => _content;

        public ComponentWindow(string name)
        {
            Name = name;

            InternalChildren = new Drawable[]
            {
                _head = new Container
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
                        new SpriteText
                        {
                            Padding = new MarginPadding(8),
                            Text = name?.ToUpperInvariant(),
                            TextSize = 18,
                            Font = "Nunito-Bold",
                            Colour = DesignerColours.SideForeground,
                            Shadow = true
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

            if (_head.IsPresent)
                _content.Height = DrawHeight - _head.DrawHeight;
            else
                _content.Height = DrawHeight;
        }
    }
}
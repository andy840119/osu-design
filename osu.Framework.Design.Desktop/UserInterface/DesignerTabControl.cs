using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace osu.Framework.Design.UserInterface
{
    public class DesignerTabControl<T> : TabControl<T>
    {
        public DesignerTabControl()
        {
            TabContainer.Spacing = new Vector2(10f, 0f);
        }

        protected override Dropdown<T> CreateDropdown() => new DesignerTabDropdown();
        protected override TabItem<T> CreateTabItem(T value) => new DesignerTabItem(value);

        public class DesignerTabDropdown : DesignerDropdown<T>
        {
            public DesignerTabDropdown()
            {
                RelativeSizeAxes = Axes.X;
            }

            protected override DropdownMenu CreateMenu() => new DesignerTabDropdownMenu();
            protected override DropdownHeader CreateHeader() => new DesignerTabDropdownHeader
            {
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight
            };

            public class DesignerTabDropdownMenu : DesignerDropdownMenu
            {
                public DesignerTabDropdownMenu()
                {
                    Anchor = Anchor.TopRight;
                    Origin = Anchor.TopRight;

                    MaxHeight = 400;
                }

                protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item) => new DrawableDesignerTabDropdownMenuItem(item);

                private class DrawableDesignerTabDropdownMenuItem : DrawableDesignerDropdownMenuItem
                {
                    public DrawableDesignerTabDropdownMenuItem(MenuItem item) : base(item)
                    {
                        ForegroundColourHover = Color4.Black;
                    }
                }
            }

            public class DesignerTabDropdownHeader : DesignerDropdownHeader
            {
                public DesignerTabDropdownHeader()
                {
                    RelativeSizeAxes = Axes.None;
                    AutoSizeAxes = Axes.X;

                    Background.Height = 0.5f;
                    Background.CornerRadius = 5;
                    Background.Masking = true;

                    Foreground.RelativeSizeAxes = Axes.None;
                    Foreground.AutoSizeAxes = Axes.X;
                    Foreground.RelativeSizeAxes = Axes.Y;
                    Foreground.Margin = new MarginPadding(5);

                    Foreground.Children = new Drawable[]
                    {
                        new SpriteText
                        {
                            Text = "More"
                        }
                    };

                    Padding = new MarginPadding(5);
                }

                protected override bool OnHover(HoverEvent e)
                {
                    Foreground.Colour = BackgroundColour;
                    return base.OnHover(e);
                }

                protected override void OnHoverLost(HoverLostEvent e)
                {
                    Foreground.Colour = BackgroundColourHover;
                    base.OnHoverLost(e);
                }
            }
        }

        public class DesignerTabItem : TabItem<T>
        {
            readonly SpriteText _text;
            readonly Box _bar;

            protected override bool OnHover(HoverEvent e)
            {
                if (!Active)
                    OnActivated();

                return true;
            }
            protected override void OnHoverLost(HoverLostEvent e)
            {
                if (!Active)
                    OnDeactivated();
            }

            public DesignerTabItem(T value) : base(value)
            {
                AutoSizeAxes = Axes.X;
                RelativeSizeAxes = Axes.Y;

                Children = new Drawable[]
                {
                    _text = new SpriteText
                    {
                        Origin = Anchor.CentreLeft,
                        Anchor = Anchor.CentreLeft,
                        Text = value.ToString(),
                        TextSize = 18,
                        Font = "Nunito",
                    },
                    _bar = new Box
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 2,
                        Alpha = 0,
                        Colour = Color4.White,
                        Origin = Anchor.BottomLeft,
                        Anchor = Anchor.BottomLeft,
                    }
                };

                Active.BindValueChanged(a => _text.Font = a ? @"Nunito-Bold" : @"Nunito", runOnceImmediately: true);
            }

            protected override void OnActivated()
            {
                _bar.FadeTo(IsHovered ? 0.5f : 1, duration: 200);
                _text.FadeColour(Color4.White, duration: 200);
            }
            protected override void OnDeactivated()
            {
                _bar.FadeOut(duration: 200);
                _text.FadeColour(DesignerColours.Highlight, duration: 200);
            }
        }
    }
}
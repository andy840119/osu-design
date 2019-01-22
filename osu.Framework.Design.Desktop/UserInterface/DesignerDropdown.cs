using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace osu.Framework.Design.UserInterface
{
    public class DesignerDropdown<T> : Dropdown<T>
    {
        protected override DropdownHeader CreateHeader() => new DesignerDropdownHeader();
        protected override DropdownMenu CreateMenu() => new DesignerDropdownMenu();

        public class DesignerDropdownHeader : DropdownHeader
        {
            readonly SpriteText _text;

            protected override string Label
            {
                get => _text.Text;
                set => _text.Text = value;
            }

            public DesignerDropdownHeader()
            {
                Foreground.Padding = new MarginPadding(4);

                AutoSizeAxes = Axes.None;
                Margin = new MarginPadding { Bottom = 4 };
                CornerRadius = 4;
                Height = 40;

                Foreground.Child = _text = new SpriteText
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                };

                BackgroundColourHover = DesignerColours.Highlight;
            }
        }

        public class DesignerDropdownMenu : DropdownMenu
        {
            public override bool HandleNonPositionalInput => State == MenuState.Open;

            public DesignerDropdownMenu()
            {
                CornerRadius = 4;
                BackgroundColour = DesignerColours.Side;

                ItemsContainer.Padding = new MarginPadding(5);
            }

            protected override void AnimateOpen() => this.FadeIn(duration: 200);
            protected override void AnimateClose() => this.FadeOut(duration: 200);

            protected override void UpdateSize(Vector2 newSize)
            {
                if (Direction == Direction.Vertical)
                {
                    Width = newSize.X;
                    this.ResizeHeightTo(newSize.Y, duration: 200, Easing.OutQuint);
                }
                else
                {
                    Height = newSize.Y;
                    this.ResizeWidthTo(newSize.X, duration: 200, Easing.OutQuint);
                }
            }

            protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item) => new DrawableDesignerDropdownMenuItem(item);

            public class DrawableDesignerDropdownMenuItem : DrawableDropdownMenuItem
            {
                public override bool HandlePositionalInput => true;

                public DrawableDesignerDropdownMenuItem(MenuItem item)
                    : base(item)
                {
                    Masking = true;
                    CornerRadius = 6;

                    Foreground.Padding = new MarginPadding(2);

                    BackgroundColourHover = DesignerColours.Highlight.Opacity(0.6f);
                    BackgroundColourSelected = DesignerColours.Highlight;

                    UpdateBackgroundColour();
                    UpdateForegroundColour();
                }

                protected override Drawable CreateContent() => new SpriteText();
            }
        }
    }
}
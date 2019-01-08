using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Threading;

namespace osu.Framework.Design
{
    public class DesignGame : Game
    {
        Container _content;
        TextBox _textbox;
        SpriteText _updating;

        protected override Container<Drawable> Content => _content;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            InternalChildren = new Drawable[]
            {
                _content = new Container
                {
                    RelativeSizeAxes = Axes.Both
                },
                _textbox = new TextBox
                {
                    RelativeSizeAxes = Axes.X,
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    Height = 40
                },
                _updating = new SpriteText
                {
                    TextSize = 20,
                    Text = "Updating...",
                    Alpha = 0
                }
            };

            _textbox.Current.ValueChanged += handleTextChanged;
        }

        ScheduledDelegate _updateDebounce;

        void handleTextChanged(string t)
        {
            _updating.ClearTransforms();
            _updating.Alpha = 1;

            _updateDebounce?.Cancel();
            _updateDebounce = Scheduler.AddDelayed(() =>
            {
                _updating.FadeOut(500);

                try
                {
                    var parser = new Markup.MarkupReader();

                    using (var reader = new System.IO.StringReader(t))
                        Child = parser.Parse(reader).CreateDrawable();

                    _content.ClearTransforms();
                    _content.Alpha = 1;
                }
                catch
                {
                    _content.FadeTo(0.5f, 500);
                }
            }, timeUntilRun: 1000);
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Design.Markup;
using osu.Framework.Design.Workspaces;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

namespace osu.Framework.Design.Designer
{
    public class PreviewContainer : Container
    {
        SpriteText _statusText;
        ParserErrorDisplay _errorDisplay;
        Container _content;

        protected override Container<Drawable> Content => _content;

        Bindable<string> _documentContent;

        [BackgroundDependencyLoader]
        void load(WorkingDocument doc)
        {
            InternalChildren = new Drawable[]
            {
                _content = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true
                },
                _errorDisplay = new ParserErrorDisplay
                {
                    RelativeSizeAxes = Axes.Both,
                    Height = 0.5f,
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft
                },
                _statusText = new SpriteText
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    TextSize = 18,
                    Font = "Inconsolata",
                    Shadow = true,
                    Text = "Waiting..."
                }
            };

            _error = _errorDisplay.Current.GetBoundCopy();

            _documentContent = doc.Content.GetBoundCopy();
            _documentContent.BindValueChanged(handleChange, true);
        }

        Bindable<Exception> _error;

        Task _updateTask;
        CancellationTokenSource _updateSource;

        public double UpdateDebounceTime { get; set; } = 500;

        void handleChange(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                _content.Clear();
                return;
            }

            _statusText.Text = "Parsing...";
            _statusText.FadeColour(DesignerColours.Highlight, 200);

            _content.FadeTo(0.6f, 200);

            // Cancel last update
            if (_updateTask != null)
            {
                _updateSource.Cancel();
                _updateSource.Dispose();
            }

            _updateSource = new CancellationTokenSource();
            _updateTask = Task.Run(updateAsync, _updateSource.Token);
        }

        async Task updateAsync()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(UpdateDebounceTime));

            try
            {
                // Read and parse markup
                var node = new DrawableNode();
                node.Load(_documentContent);

                // Create drawable from markup
                var drawable = node.CreateDrawable();

                Schedule(() =>
                {
                    _content.Child = drawable;
                    _content.FadeIn(30);

                    _statusText.Text = "Waiting...";
                    _statusText.FadeColour(Color4.White, 200);

                    _errorDisplay.FadeOut(200);
                });
            }
            catch (Exception e)
            {
                Schedule(() =>
                {
                    _statusText.Text = "Error!";
                    _statusText.FadeColour(DesignerColours.Error, 200);

                    _error.Value = e;
                    _errorDisplay.FadeIn(200);
                });
            }
        }
    }
}
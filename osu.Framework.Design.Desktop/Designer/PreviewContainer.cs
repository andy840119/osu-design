using System;
using System.IO;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Design.Markup;
using osu.Framework.Design.Solution;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Threading;
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

        ScheduledDelegate _updateTask;
        Bindable<Exception> _error;

        public double UpdateDebounceMilliseconds { get; set; } = 600;

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

            _updateTask?.Cancel();
            _updateTask = Scheduler.AddDelayed(
                task: () =>
                {
                    try
                    {
                        // Read and parse markup
                        DrawableData data;

                        using (var reader = new StringReader(content))
                            data = new MarkupReader().Read(reader);

                        // Create drawable from markup
                        Child = data.CreateDrawable();

                        _content.FadeIn(30);

                        _statusText.Text = "Waiting...";
                        _statusText.FadeColour(Color4.White, 200);

                        _errorDisplay.FadeOut(30);
                    }
                    catch (Exception e)
                    {
                        _statusText.Text = "Error!";
                        _statusText.FadeColour(DesignerColours.Error, 30);

                        _error.Value = e;
                        _errorDisplay.FadeIn(200);
                    }
                },
                timeUntilRun: UpdateDebounceMilliseconds
            );
        }
    }
}
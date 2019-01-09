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
        readonly SpriteText _statusText;
        readonly ParserErrorDisplay _errorDisplay;
        readonly Container _content;

        protected override Container<Drawable> Content => _content;

        public PreviewContainer()
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
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight,
                    TextSize = 24,
                    Shadow = true,
                    Text = "Waiting..."
                }
            };

            _error = _errorDisplay.Current.GetBoundCopy();
        }

        Bindable<Document> _document;

        [BackgroundDependencyLoader]
        void load(Workspace workspace)
        {
            _document = workspace.CurrentDocument.GetBoundCopy();
            _document.BindValueChanged(handleChange, true);
        }

        ScheduledDelegate _updateTask;
        Bindable<Exception> _error;

        public double UpdateDebounceMilliseconds { get; set; } = 600;

        void handleChange(Document doc)
        {
            _content.FadeTo(0.2f, 200);
            _error.Value = null;

            if (doc?.Type != DocumentType.Markup)
            {
                _statusText.Text = "Not markup";
                _statusText.FadeColour(Color4.White, 200);
                return;
            }

            _statusText.Text = "Parsing...";
            _statusText.FadeColour(DesignerColours.Highlight, 200);

            _updateTask?.Cancel();
            _updateTask = Scheduler.AddDelayed(
                task: () =>
                {
                    try
                    {
                        // Read and parse markup
                        DrawableData data;

                        using (var stream = doc.OpenRead())
                        using (var reader = new StreamReader(stream))
                            data = new MarkupReader().Read(reader);

                        // Create drawable from markup
                        Child = data.CreateDrawable();

                        _content.FadeIn(30);

                        _statusText.Text = "Waiting...";
                        _statusText.FadeColour(Color4.White, 200);
                    }
                    catch (Exception e)
                    {
                        _statusText.Text = "Error!";
                        _statusText.FadeColour(DesignerColours.Error, 30);

                        _error.Value = e;
                    }
                },
                timeUntilRun: UpdateDebounceMilliseconds
            );
        }
    }
}
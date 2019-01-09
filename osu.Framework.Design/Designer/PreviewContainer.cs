using System;
using System.IO;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Design.Markup;
using osu.Framework.Design.Solution;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Threading;

namespace osu.Framework.Design.Designer
{
    public class PreviewContainer : Container
    {
        readonly ParserErrorDisplay _errorDisplay;
        readonly Container _content;

        protected override Container<Drawable> Content => _content;

        public PreviewContainer()
        {
            InternalChildren = new Drawable[]
            {
                _errorDisplay = new ParserErrorDisplay
                {
                    RelativeSizeAxes = Axes.Both,
                    Height = 0.5f,
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft
                },
                _content = new Container
                {
                    RelativeSizeAxes = Axes.Both
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
            _content.FadeTo(0.4f, 200);

            if (doc?.Type != DocumentType.Markup)
                return;

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

                        _error.Value = null;
                    }
                    catch (Exception e)
                    {
                        _error.Value = e;
                    }
                },
                timeUntilRun: UpdateDebounceMilliseconds
            );
        }
    }
}
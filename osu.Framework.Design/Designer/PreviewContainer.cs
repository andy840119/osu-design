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
        readonly Container _content;

        protected override Container<Drawable> Content => _content;

        public PreviewContainer()
        {
            InternalChild = _content = new Container
            {
                RelativeSizeAxes = Axes.Both
            };
        }

        Bindable<Document> _document;

        [BackgroundDependencyLoader]
        void load(Workspace workspace)
        {
            _document = workspace.CurrentDocument.GetBoundCopy();
            _document.BindValueChanged(handleChange, true);
        }

        ScheduledDelegate _updateTask;

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
                            data = new MarkupReader().Parse(reader);

                        // Create drawable from markup
                        Child = data.CreateDrawable();

                        _content.FadeIn(30);
                    }
                    catch
                    {
                    }
                },
                timeUntilRun: UpdateDebounceMilliseconds
            );
        }
    }
}
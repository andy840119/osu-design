using System;
using osu.Framework.Configuration;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osuTK.Graphics;

namespace osu.Framework.Design.Designer
{
    public class ParserErrorDisplay : CompositeDrawable, IHasCurrentValue<Exception>
    {
        readonly TextFlowContainer _flow;

        readonly Bindable<Exception> _current = new Bindable<Exception>();

        public Bindable<Exception> Current
        {
            get => _current;
            set
            {
                _current.UnbindBindings();
                _current.BindTo(value);
            }
        }

        public ParserErrorDisplay()
        {
            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black.Opacity(0.8f)
                },
                new ScrollContainer(Direction.Vertical)
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = _flow = new TextFlowContainer(text =>
                    {
                        text.TextSize = 18;
                        text.Font = "Nunito";
                    })
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        ParagraphSpacing = 1,
                        Padding = new MarginPadding(5)
                    }
                }
            };

            Current.BindValueChanged(handleChange, true);
        }

        void handleChange(Exception e)
        {
            _flow.Clear(true);

            if (e == null)
                return;

            _flow.AddText($"{e.Message}\n", t =>
            {
                t.Colour = DesignerColours.Error;
                t.TextSize = 24;
                t.Font = "Nunito-Bold";
            });
            _flow.AddText($"{e.Source}\n", t =>
            {
                t.Colour = DesignerColours.Highlight;
            });
            _flow.AddText($"Trace:\n{e.StackTrace}", t =>
            {
                t.Alpha = 0.7f;
                t.Font = "Inconsolata";
            });
        }
    }
}
using System;
using osu.Framework.Configuration;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace osu.Framework.Design.Designer
{
    public class ParserErrorDisplay : CompositeDrawable
    {
        readonly TextFlowContainer _flow;

        public Bindable<Exception> Current { get; }

        public ParserErrorDisplay()
        {
            Current = new Bindable<Exception>();
            Current.ValueChanged += handleChange;

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
                        text.TextSize = 20;
                    })
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        ParagraphSpacing = 1
                    }
                }
            };
        }

        void handleChange(Exception e)
        {
            _flow.Clear(true);

            if (e == null)
            {
                _flow.FadeOut(30);
                return;
            }
            else
                _flow.FadeIn(200);

            _flow.AddText($"{e.Message}\n", t =>
            {
                t.Colour = Color4.Red;
                t.TextSize = 26;
            });
            _flow.AddText($"Source: {e.Source}\n", t =>
            {
                t.Colour = Color4.LightGreen;
            });
            _flow.AddText($"Trace:\n{e.StackTrace}", t =>
            {
                t.Alpha = 0.8f;
            });
        }
    }
}
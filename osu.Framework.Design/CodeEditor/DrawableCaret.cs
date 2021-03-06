using System;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace osu.Framework.Design.CodeEditor
{
    public class DrawableCaret : CompositeDrawable
    {
        readonly SelectionRange _selection;

        public DrawableCaret(SelectionRange selection)
        {
            _selection = selection;

            Masking = true;
            CornerRadius = 1.5f;

            Size = new Vector2(3, 20);

            InternalChild = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.White.Opacity(0.8f)
            };
        }

        DrawableEditor _editor;
        BindableFloat _fontSize;
        BindableInt _lineNumberWidth;
        BindableInt _selectionEnd;

        [BackgroundDependencyLoader]
        void load(DrawableEditor editor)
        {
            _editor = editor;

            _fontSize = editor.FontSize.GetBoundCopy() as BindableFloat;
            _fontSize.BindValueChanged(s => Scheduler.AddOnce(() =>
            {
                Size = new Vector2(3, s);
                ResetFlicker();
            }));

            _lineNumberWidth = editor.LineNumberWidth.GetBoundCopy() as BindableInt;
            _lineNumberWidth.BindValueChanged(w => Scheduler.AddOnce(ResetFlicker));

            _selectionEnd = _selection.End.GetBoundCopy() as BindableInt;
            _selectionEnd.BindValueChanged(i => Scheduler.AddOnce(ResetFlicker));

            // This updates the caret
            _fontSize.TriggerChange();
        }

        public void ResetFlicker()
        {
            FinishTransforms();

            Position = _editor.GetPositionAtIndex(_selectionEnd);

            this.FadeIn(30)
                .Delay(500)
                .FadeTo(0.4f, 200)
                .Delay(300)
                .Loop();
        }
    }
}
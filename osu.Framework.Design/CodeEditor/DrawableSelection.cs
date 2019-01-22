using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace osu.Framework.Design.CodeEditor
{
    public class DrawableSelection : CompositeDrawable
    {
        readonly SelectionRange _selection;

        public DrawableSelection(SelectionRange selection)
        {
            _selection = selection;

            RelativeSizeAxes = Axes.Both;
        }

        DrawableEditor _editor;
        BindableFloat _fontSize;
        BindableInt _lineNumberWidth;
        BindableInt _selectionStart;
        BindableInt _selectionEnd;

        [BackgroundDependencyLoader]
        void load(DrawableEditor editor)
        {
            _editor = editor;

            _fontSize = editor.FontSize.GetBoundCopy() as BindableFloat;
            _fontSize.BindValueChanged(s => Scheduler.AddOnce(ResetFlicker));

            _lineNumberWidth = editor.LineNumberWidth.GetBoundCopy() as BindableInt;
            _lineNumberWidth.BindValueChanged(w => Scheduler.AddOnce(ResetFlicker));

            _selectionStart = _selection.Start.GetBoundCopy() as BindableInt;
            _selectionStart.BindValueChanged(i => Scheduler.AddOnce(ResetFlicker));

            _selectionEnd = _selection.End.GetBoundCopy() as BindableInt;
            _selectionEnd.BindValueChanged(i => Scheduler.AddOnce(ResetFlicker));

            _selectionEnd.TriggerChange();
        }

        public void ResetFlicker()
        {
            FinishTransforms();

            updateDrawables();

            this.FadeIn(30)
                .Delay(500)
                .FadeTo(0.9f, 200)
                .Delay(300)
                .Loop();
        }

        void updateDrawables()
        {
            var ranges = getBoxRanges().ToArray();

            for (var i = 0; i < ranges.Length; i++)
            {
                if (i == InternalChildren.Count)
                    AddInternal(new Selection());

                var range = ranges[i];
                var position = _editor.GetPositionAtIndex(range.start);
                var size = _editor.GetPositionAtIndex(range.end, ignoreEnd: false) + new Vector2(0, _fontSize) - position;

                var selectionDrawable = InternalChildren[i];

                selectionDrawable.Position = position;
                selectionDrawable.Size = size;
            }

            // Remove unused selections
            for (var i = InternalChildren.Count; i > ranges.Length; i--)
                RemoveInternal(InternalChildren[ranges.Length]);
        }

        IEnumerable<(int start, int end)> getBoxRanges()
        {
            var length = _selection.Length;

            if (length == 0)
                yield break;

            int line;
            int start;

            if (length < 0)
            {
                // Swap selection start with and end if negative length
                length = -length;
                _editor.GetLineAtIndex(_selectionStart.Value - length, out line, out start);
            }
            else
                _editor.GetLineAtIndex(_selectionStart, out line, out start);

            for (; length > 0; line++)
            {
                var lineDrawable = _editor[line];
                var remaining = Math.Min(lineDrawable.Length - start, length);

                var startIndex = lineDrawable.StartIndex + start;
                var endIndex = startIndex + remaining;

                if (startIndex != endIndex)
                    yield return (startIndex, endIndex);

                start = 0;
                length -= remaining;
            }
        }

        public sealed class Selection : Box
        {
            public Selection()
            {
                Colour = DesignerColours.Highlight.Opacity(0.3f);
            }
        }
    }
}
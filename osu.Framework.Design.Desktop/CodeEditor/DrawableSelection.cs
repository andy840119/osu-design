using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace osu.Framework.Design.CodeEditor
{
    public class DrawableSelection : CompositeDrawable
    {
        readonly SelectionRange _selectionRange;

        public DrawableSelection(SelectionRange selection)
        {
            _selectionRange = selection;

            RelativeSizeAxes = Axes.Both;
        }

        DrawableEditor _editor;
        BindableFloat _fontSize;
        BindableInt _lineNumberWidth;
        BindableInt _selectionStart;
        BindableInt _selectionLength;

        [BackgroundDependencyLoader]
        void load(DrawableEditor editor)
        {
            _editor = editor;

            _fontSize = editor.FontSize.GetBoundCopy() as BindableFloat;
            _fontSize.BindValueChanged(s => updateDrawables());

            _lineNumberWidth = editor.LineNumberWidth.GetBoundCopy() as BindableInt;
            _lineNumberWidth.BindValueChanged(w => updateDrawables());

            _selectionStart = _selectionRange.Start.GetBoundCopy() as BindableInt;
            _selectionStart.BindValueChanged(i => updateDrawables());

            _selectionLength = _selectionRange.Length.GetBoundCopy() as BindableInt;
            _selectionLength.BindValueChanged(i => updateDrawables());

            updateDrawables();
        }

        void updateDrawables()
        {
            var ranges = getBoxRanges().ToArray();

            for (var i = 0; i < ranges.Length; i++)
            {
                if (i == InternalChildren.Count)
                    AddInternal(new Selection());

                var range = ranges[i];
                var startPos = _editor.GetPositionAtIndex(range.start);
                var endPos = _editor.GetPositionAtIndex(range.end) + new Vector2(0, _fontSize);

                var selectionDrawable = InternalChildren[i];

                selectionDrawable.Position = startPos;
                selectionDrawable.Size = endPos - startPos;
            }

            // Remove unused selections
            while (InternalChildren.Count > ranges.Length)
                RemoveInternal(InternalChildren[ranges.Length]);
        }

        IEnumerable<(int start, int end)> getBoxRanges()
        {
            var length = _selectionLength.Value;

            _editor.GetLineAtIndex(_selectionStart, out var line, out var start);

            for (; length > 0; line++)
            {
                var lineDrawable = _editor[line];
                var remaining = Math.Min(lineDrawable.Length - start, length);

                var startIndex = lineDrawable.StartIndex + start;
                var endIndex = startIndex + remaining;

                if (length >= remaining)
                    endIndex--;

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
                Alpha = 0.4f;
                Colour = Color4.Purple;
            }
        }
    }
}
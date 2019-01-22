using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osuTK;

namespace osu.Framework.Design.CodeEditor
{
    public class DrawableLine : CompositeDrawable
    {
        public int Length { get; private set; }

        FillFlowContainer<DrawableWord> _flow;

        public DrawableLineNumber LineNumber { get; private set; }

        [BackgroundDependencyLoader]
        void load()
        {
            AutoSizeAxes = Axes.Both;
            InternalChild = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(5),
                Children = new Drawable[]
                {
                    LineNumber = new DrawableLineNumber(),
                    _flow = new FillFlowContainer<DrawableWord>
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal
                    }
                }
            };
        }

        public float TextStartOffset => _flow.ToSpaceOfOtherDrawable(Vector2.Zero, this).X;

        public int StartIndex { get; private set; }
        public int EndIndex => StartIndex + Length;

        public int Count => _flow.Count;

        public void Set(string[] parts, int startIndex)
        {
            var j = 0;
            var index = startIndex;

            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];

                if (string.IsNullOrEmpty(part))
                    continue;

                if (j == _flow.Count)
                    _flow.Add(new DrawableWord());

                _flow[j++].Set(part, index);

                index += part.Length;
            }

            // Remove unused words
            while (_flow.Count > j)
                _flow.Remove(_flow[j]);

            StartIndex = startIndex;
            Length = parts.Sum(p => p.Length);
        }

        public DrawableWord GetWordAtIndex(int index, out int wordIndex, out int indexInWord)
        {
            for (var i = 0; i < _flow.Count; i++)
            {
                var word = _flow[i];

                if (index >= word.Length && i != _flow.Count - 1)
                {
                    index -= word.Length;
                    continue;
                }

                wordIndex = i;
                indexInWord = index;
                return word;
            }

            wordIndex = -1;
            indexInWord = -1;
            return null;
        }

        public DrawableWord this[int index] => _flow[index];
    }
}
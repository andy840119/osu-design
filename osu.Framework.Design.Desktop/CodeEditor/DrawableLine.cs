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
        DrawableWord _placeholder;
        FillFlowContainer<DrawableWord> _flow;

        public int Length => _flow.Sum(w => w.Length);
        public string Text => string.Concat(_flow.Select(w => w.Current.Value));

        float fixedWidth;

        [BackgroundDependencyLoader]
        void load(DrawableEditor editor, FontStore fonts)
        {
            AutoSizeAxes = Axes.Both;
            InternalChildren = new Drawable[]
            {
                // Placeholder to hold the line height when empty
                _placeholder = new DrawableWord(),

                _flow = new FillFlowContainer<DrawableWord>
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    LayoutEasing = Easing.OutQuint,
                    LayoutDuration = 50,
                    Child = new DrawableWord()
                }
            };

            _placeholder.Set(" ");
        }

        public DrawableWord GetWordAtIndex(int index, out int wordIndex, out int charIndex)
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
                charIndex = index;
                return word;
            }

            throw new ArgumentOutOfRangeException(nameof(index));
        }

        public void Set(string text)
        {
            _flow.Child = new DrawableWord();
            Insert(text, 0);
        }

        static readonly Regex _splitRegex = new Regex(@"(?<= )(?=\S)", RegexOptions.Compiled);

        public void Insert(string value, int index)
        {
            var word = GetWordAtIndex(index, out var wordIndex, out var charIndex);

            var parts = _splitRegex.Split(value).ToList();
            var wordParts = _splitRegex.Split(word.Current.Value.Insert(charIndex, parts[0]));

            word.Set(wordParts[0]);

            parts.RemoveAt(0);
            parts.InsertRange(0, wordParts.Skip(1));

            var drawables = _flow.ToList();

            for (var i = 0; i < parts.Count; i++)
            {
                var drawable = new DrawableWord();
                _flow.Add(drawable);

                drawable.Set(parts[i]);
                drawables.Insert(wordIndex + i + 1, drawable);
            }

            foreach (var drawable in _flow)
                _flow.SetLayoutPosition(drawable, drawables.IndexOf(drawable));
        }

        public void Remove(int count, int index)
        {
            var drawables = _flow.ToList();

            while (count > 0)
            {
                var word = GetWordAtIndex(index, out var wordIndex, out var charIndex);
                var removeable = Math.Min(count, word.Length - charIndex);

                if (removeable == 0)
                    break;

                if (word.Length == removeable)
                {
                    _flow.Remove(word);
                    drawables.RemoveAt(wordIndex);
                }
                else
                    word.Remove(removeable, charIndex);

                count -= removeable;
                charIndex = 0;
            }

            foreach (var drawable in _flow)
                _flow.SetLayoutPosition(drawable, drawables.IndexOf(drawable));

            // We want to keep at least one word
            if (_flow.Count == 0)
                _flow.Add(new DrawableWord());
        }
    }
}
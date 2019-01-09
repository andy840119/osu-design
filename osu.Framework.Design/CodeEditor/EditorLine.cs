using System;
using System.Linq;
using System.Text.RegularExpressions;
using osu.Framework.Configuration;

namespace osu.Framework.Design.CodeEditor
{
    public class EditorLine
    {
        public int Length { get; private set; }

        public BindableList<EditorWord> Words { get; } = new BindableList<EditorWord>();

        public string Text => string.Concat(Words.Select(w => w.Text.Value));

        static readonly Regex _splitRegex = new Regex(@"(?<= )", RegexOptions.Compiled);

        public void Insert(int startIndex, string value)
        {
            if (startIndex < 0 || startIndex > Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (string.IsNullOrEmpty(value))
                return;

            var parts = _splitRegex.Split(Text.Insert(startIndex, value));

            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];

                if (Words.Count == i)
                    Words.Add(new EditorWord(part));
                else
                    Words[i].Text.Value = part;
            }

            Length += value.Length;
        }

        public void Remove(int startIndex, int count)
        {
            if (startIndex < 0 || startIndex > Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (count < 0 || startIndex + count > Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (count == 0)
                return;

            var parts = _splitRegex.Split(Text.Remove(startIndex, count));

            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];

                Words[i].Text.Value = part;
            }

            // Remove redundant words
            while (Words.Count != parts.Length)
                Words.RemoveAt(parts.Length);

            Length -= count;
        }
    }
}
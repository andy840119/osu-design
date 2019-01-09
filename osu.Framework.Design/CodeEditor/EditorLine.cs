using System;
using System.Linq;
using System.Text.RegularExpressions;
using osu.Framework.Configuration;

namespace osu.Framework.Design.CodeEditor
{
    public class EditorLine
    {
        public int Length => Words.Sum(w => w.Length) + 1;

        public BindableList<EditorWord> Words { get; } = new BindableList<EditorWord>();

        public string Text => string.Concat(Words.Select(w => w.Text.Value)) + '\n';

        public EditorLine(string value = "")
        {
            Set(value);
        }

        public void Insert(int startIndex, string value)
        {
            if (startIndex < 0 || startIndex > Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (string.IsNullOrEmpty(value))
                return;

            Set(Text.Insert(startIndex, value));
        }

        public void Remove(int startIndex, int count)
        {
            if (startIndex < 0 || startIndex > Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (count < 0 || startIndex + count > Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (count == 0)
                return;

            Set(Text.Remove(startIndex, count));
        }

        static readonly Regex _splitRegex = new Regex(@"(?<= )", RegexOptions.Compiled);

        public void Set(string value)
        {
            var parts = _splitRegex.Split(value);

            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];

                if (Words.Count == i)
                    Words.Add(new EditorWord(part));
                else
                    Words[i].Set(part);
            }

            // Remove redundant words
            while (Words.Count != parts.Length)
                Words.RemoveAt(parts.Length);
        }
    }
}
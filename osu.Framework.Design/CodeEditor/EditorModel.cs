using System;
using System.Linq;
using System.Text.RegularExpressions;
using osu.Framework.Configuration;

namespace osu.Framework.Design.CodeEditor
{
    public class EditorModel
    {
        public int Length => Lines.Sum(l => l.Length);

        public BindableList<EditorLine> Lines { get; } = new BindableList<EditorLine>();

        public string Text => string.Concat(Lines.Select(l => l.Text));

        public EditorModel(string value = "")
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

        static readonly Regex _splitRegex = new Regex(@"\r\n|\r|\n", RegexOptions.Compiled);

        public void Set(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Lines.Clear();
                return;
            }

            var parts = _splitRegex.Split(value);

            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];

                if (Lines.Count == i)
                    Lines.Add(new EditorLine(part));
                else
                    Lines[i].Set(part);
            }

            // Remove redundant words
            while (Lines.Count != parts.Length)
                Lines.RemoveAt(parts.Length);
        }
    }
}
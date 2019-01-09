using System;
using System.Text.RegularExpressions;
using osu.Framework.Configuration;

namespace osu.Framework.Design.CodeEditor
{
    public class EditorModel
    {
        public int Length { get; private set; }

        public BindableList<EditorLine> Lines { get; } = new BindableList<EditorLine>();

        static readonly Regex _splitRegex = new Regex(@"(?<=\n)", RegexOptions.Compiled);

        /// <remarks>
        /// All newlines in value are discarded.
        /// </remarks>
        public void Insert(int startIndex, string value)
        {
            if (startIndex < 0 || startIndex > Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            value = value
                .Replace("\r", "")
                .Replace("\n", "");

            if (string.IsNullOrEmpty(value))
                return;

            for (var i = 0; i < Lines.Count; i++)
            {
                var line = Lines[i];

                if (startIndex > line.Length)
                {
                    startIndex -= line.Length;
                    continue;
                }

                line.Insert(startIndex, value);
                break;
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

            for (var i = 0; i < Lines.Count; i++)
            {
                var line = Lines[i];

                if (startIndex > line.Length)
                {
                    startIndex -= line.Length;
                    continue;
                }

                var removeable = Math.Min(line.Length - startIndex, count);

                count -= removeable;

                line.Remove(startIndex, removeable);

                // Handle complete deletion of a single line
                if (line.Length == 0)
                    Lines.RemoveAt(i--);

                if (count <= 0)
                    break;

                startIndex = 0;
            }
        }
    }
}
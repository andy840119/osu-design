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

        public void Insert(int startIndex, string value)
        {
            // Ensure startIndex is valid
            if (startIndex < 0 || startIndex >= Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
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

                var valueParts = _splitRegex.Split(value);

                Length += line.Length;
                break;
            }
        }
    }
}
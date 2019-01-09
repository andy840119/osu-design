using System;
using System.Text.RegularExpressions;
using osu.Framework.Configuration;

namespace osu.Framework.Design.CodeEditor
{
    public class EditorLine
    {
        public int Length { get; private set; }

        public BindableList<EditorWord> Words { get; } = new BindableList<EditorWord>();

        static readonly Regex _splitRegex = new Regex(@"(?<= )", RegexOptions.Compiled);

        public void Insert(int startIndex, string value)
        {
            // Ensure startIndex is valid
            if (startIndex < 0 || startIndex >= Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (string.IsNullOrEmpty(value))
                return;

            for (var i = 0; i < Words.Count; i++)
            {
                var word = Words[i];

                if (startIndex >= word.Length)
                {
                    startIndex -= word.Length;
                    continue;
                }

                var valueParts = _splitRegex.Split(value);

                // Inserting at the begging
                if (startIndex == 0)
                {
                    for (var j = 0; j < valueParts.Length - 1; j++)
                        Words.Insert(i + j, new EditorWord(valueParts[j]));

                    word.Insert(0, valueParts[valueParts.Length - 1]);
                }

                // Inserting in the middle
                else
                {

                }

                var valuePartIndex = 0;

                if (!word.Text.Value.EndsWith(' '))
                    word.Insert(startIndex, valueParts[valuePartIndex++]);

                var j = 1;

                for (; valuePartIndex < valueParts.Length - 1; j++)
                    Words.Insert(i + j, new EditorWord(valueParts[valuePartIndex++]));

                if (valuePartIndex == valueParts.Length - 1)
                {
                    if (valueParts[valuePartIndex].EndsWith(' '))
                        Words.Insert(i + j, new EditorWord(valueParts[valuePartIndex]));
                    else
                        Words[i + j].Insert(0, valueParts[valuePartIndex]);
                }

                Length += value.Length;
                break;
            }
        }
    }
}
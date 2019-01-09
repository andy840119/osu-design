using System;
using System.Linq;
using System.Text.RegularExpressions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace osu.Framework.Design.CodeEditor
{
    public class DrawableLine : FillFlowContainer<DrawableWord>
    {
        public int Length { get; private set; }

        public DrawableLine(string initialValue = "")
        {
            AutoSizeAxes = Axes.Both;
            Direction = FillDirection.Horizontal;

            Add(new DrawableWord());
            Insert(0, initialValue);
        }

        public void Insert(int startIndex, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            // Ensure startIndex is valid
            startIndex = Math.Clamp(startIndex, 0, Length);

            var words = this.ToList();

            for (var i = 0; i < words.Count; i++)
            {
                var word = words[i];

                if (value != null)
                {
                    // Find word that contains startIndex
                    if (startIndex > word.Length)
                    {
                        startIndex -= word.Length;
                        continue;
                    }

                    // Split the value by spaces
                    var valueWords = _splitRegex.Split(value);

                    // First part appends to our matching word
                    word.Insert(startIndex, valueWords[0]);

                    // Add parts
                    for (var j = 1; j < valueWords.Length; j++)
                    {
                        var valueWord = new DrawableWord(valueWords[j]);
                        words.Insert(i + j, valueWord);

                        Add(valueWord);
                    }

                    Length += value.Length;
                    value = null;
                }

                // Update depth to reflect the insertion
                ChangeChildDepth(word, i);
            }
        }

        public void Remove(int startIndex, int count)
        {
        }
    }
}
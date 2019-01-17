using System;
using System.Linq;
using osu.Framework.Configuration;

namespace osu.Framework.Design.CodeEditor
{
    public class TextModel
    {
        public int Length => Lines.Sum(l => l.Length);

        public BindableList<LineModel> Lines { get; } = new BindableList<LineModel>();

        void ensureLineExists()
        {
            if (Lines.Count == 0)
                Lines.Add(new LineModel());
        }

        public LineModel GetLine(int index, out int indexInLine)
        {
            ensureLineExists();

            foreach (var line in Lines)
            {
                if (index >= line.Length + 1)
                {
                    index -= line.Length - 1;
                    continue;
                }

                indexInLine = index;
                return line;
            }

            throw new ArgumentOutOfRangeException(nameof(index));
        }
    }

    public class LineModel
    {
        public int Length => Words.Sum(w => w.Length);

        public BindableList<WordModel> Words { get; } = new BindableList<WordModel>();
    }

    public class WordModel
    {
        public int Length => Current.Value.Length;

        public Bindable<string> Current = new Bindable<string>();
        public Bindable<string> FontFamily = new Bindable<string>();
    }
}
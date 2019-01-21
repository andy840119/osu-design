using System;
using osu.Framework.Configuration;

namespace osu.Framework.Design.CodeEditor
{
    public class SelectionRange
    {
        public int IndexInLine;

        public BindableInt Start { get; } = new BindableInt();
        public BindableInt End { get; } = new BindableInt();

        public int Length => End - Start;

        readonly DrawableEditor _editor;

        public SelectionRange(DrawableEditor editor)
        {
            _editor = editor;
        }
    }
}
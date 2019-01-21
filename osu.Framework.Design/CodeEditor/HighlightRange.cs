namespace osu.Framework.Design.CodeEditor
{
    public struct HighlightRange
    {
        public int Start { get; set; }
        public int End { get; set; }
        public HighlightStyle Style { get; set; }

        public HighlightRange(int start, int end, HighlightStyle style)
        {
            Start = start;
            End = end;
            Style = style;
        }
    }
}
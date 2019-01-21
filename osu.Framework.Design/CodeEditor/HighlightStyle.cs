namespace osu.Framework.Design.CodeEditor
{
    public struct HighlightStyle
    {
        public readonly HighlightType Type;
        public readonly HighlightFont Font;

        public HighlightStyle(HighlightType type, HighlightFont font)
        {
            Type = type;
            Font = font;
        }
    }
}
using osuTK.Graphics;

namespace osu.Framework.Design.CodeEditor
{
    public struct HighlightStyle
    {
        public readonly Color4 Color;
        public readonly HighlightFont Font;

        public HighlightStyle(Color4 color, HighlightFont font)
        {
            Color = color;
            Font = font;
        }
    }
}
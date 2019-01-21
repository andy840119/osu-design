using System;
using osuTK.Graphics;

namespace osu.Framework.Design.CodeEditor
{
    public struct HighlightStyle : IEquatable<HighlightStyle>
    {
        public readonly Color4 Color;
        public readonly HighlightFont Font;

        public HighlightStyle(Color4 color, HighlightFont font)
        {
            Color = color;
            Font = font;
        }

        public override bool Equals(object obj) => obj is HighlightStyle style ? Equals(style) : false;
        public bool Equals(HighlightStyle other) =>
            Color == other.Color &&
            Font == other.Font;

        public override int GetHashCode() => HashCode.Combine(Color, Font);
        public override string ToString() => $"{Color} ({Font})";
    }
}
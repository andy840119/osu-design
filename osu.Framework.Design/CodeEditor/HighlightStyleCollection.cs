using System.Collections.Generic;
using osuTK.Graphics;

namespace osu.Framework.Design.CodeEditor
{
    public class HighlightStyleCollection : Dictionary<HighlightType, HighlightStyle>
    {
        public static HighlightStyleCollection Default => new HighlightStyleCollection
        {
            { HighlightType.String, new HighlightStyle(new Color4(209, 124, 58, 255), HighlightFont.Normal) },
            { HighlightType.SinglelineComment, new HighlightStyle(new Color4(77, 112, 77, 255), HighlightFont.Italic) },
            { HighlightType.MultilineComment, new HighlightStyle(new Color4(77, 112, 77, 255) , HighlightFont.Italic)},
            { HighlightType.Number, new HighlightStyle(new Color4(211, 120, 232, 255), HighlightFont.Normal) },
            { HighlightType.Attribute, new HighlightStyle(new Color4(106, 209, 173, 255), HighlightFont.Normal) },
            { HighlightType.Keyword, new HighlightStyle(new Color4(110, 124, 229, 255), HighlightFont.Normal) },
            { HighlightType.Entity, new HighlightStyle(new Color4(239, 237, 107, 255), HighlightFont.Bold) }
        };
    }
}
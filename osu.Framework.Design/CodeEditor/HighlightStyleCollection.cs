using System.Collections.Generic;
using osuTK.Graphics;

namespace osu.Framework.Design.CodeEditor
{
    public class HighlightStyleCollection : Dictionary<HighlightType, HighlightStyle>
    {
        public static HighlightStyleCollection Default => new HighlightStyleCollection
        {
            { HighlightType.String, new HighlightStyle(new Color4(119, 66, 25, 255), HighlightFont.Normal) },
            { HighlightType.SinglelineComment, new HighlightStyle(new Color4(51, 183, 49, 255), HighlightFont.Normal) },
            { HighlightType.MultilineComment, new HighlightStyle(new Color4(51, 183, 49, 255) , HighlightFont.Normal)},
            { HighlightType.Number, new HighlightStyle(new Color4(180, 73, 204, 255), HighlightFont.Normal) },
            { HighlightType.Attribute, new HighlightStyle(new Color4(83, 158, 132, 255), HighlightFont.Normal) },
            { HighlightType.Keyword, new HighlightStyle(new Color4(91, 105, 216, 255), HighlightFont.Normal) },
            { HighlightType.Entity, new HighlightStyle(new Color4(211, 209, 91, 255), HighlightFont.Bold) }
        };
    }
}
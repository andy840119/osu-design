using System.Collections.Generic;
using osuTK.Graphics;

namespace osu.Framework.Design.CodeEditor
{
    public class EditorColours : Dictionary<HighlightType, Color4>
    {
        public static EditorColours Default => new EditorColours
        {
            { HighlightType.String, new Color4(119, 66, 25, 255) },
            { HighlightType.SinglelineComment, new Color4(51, 183, 49, 255) },
            { HighlightType.MultilineComment, new Color4(51, 183, 49, 255) },
            { HighlightType.Number, new Color4(180, 73, 204, 255) },
            { HighlightType.Attribute, new Color4(83, 158, 132, 255) },
            { HighlightType.Keyword, new Color4(91, 105, 216, 255) },
            { HighlightType.Entity, new Color4(211, 209, 91, 255) }
        };
    }
}
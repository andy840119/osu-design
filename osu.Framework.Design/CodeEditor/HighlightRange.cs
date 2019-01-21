using System.Text.RegularExpressions;

namespace osu.Framework.Design.CodeEditor
{
    public struct HighlightRange
    {
        public int Start { get; set; }
        public int End { get; set; }
        public HighlightType Type { get; set; }

        public HighlightRange(Match match, HighlightType type)
        {
            Start = match.Index;
            End = match.Index + match.Length;
            Type = type;
        }
        public HighlightRange(Group group, HighlightType type)
        {
            Start = group.Index;
            End = group.Index + group.Length;
            Type = type;
        }
        public HighlightRange(int start, int end, HighlightType type)
        {
            Start = start;
            End = end;
            Type = type;
        }
    }
}
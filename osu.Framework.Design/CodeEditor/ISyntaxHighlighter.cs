using System.Collections.Generic;

namespace osu.Framework.Design.CodeEditor
{
    public interface ISyntaxHighlighter
    {
        string Name { get; }

        string[] Tokenize(string text);
        IEnumerable<HighlightRange> Highlight(string text);
    }
}
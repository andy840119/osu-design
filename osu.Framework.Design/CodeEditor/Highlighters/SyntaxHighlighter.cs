using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace osu.Framework.Design.CodeEditor.Highlighters
{
    public class SyntaxHighlighter : ISyntaxHighlighter
    {
        public string Name => "Default Highlighter";

        const string symbols = @"[.,\/';\[\]\-=!@#$%^&*()_+{}:""<>?`~\\|]";
        static readonly Regex _tokenizerRegex = new Regex($@"(?={symbols})|(?<={symbols})|(?<=\s)(?!\s)|(?=\s)(?<!\s)", RegexOptions.Compiled);

        public virtual string[] Tokenize(string text) => _tokenizerRegex.Split(text);

        public virtual IEnumerable<HighlightRange> Highlight(string text) => Enumerable.Empty<HighlightRange>();
    }
}
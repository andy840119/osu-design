using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace osu.Framework.Design.CodeEditor.Highlighters
{
    public class CSharpSyntaxHighlighter : SyntaxHighlighter
    {
        static readonly Regex _stringRegex = new Regex(
            @"'(?>(?:\\[^\r\n]|[^'\r\n])*)'?|(?<verbatim>@)?""(?>(?:(?(verbatim)""""|\\.)|[^""])*)""",
            options: RegexOptions.ExplicitCapture | RegexOptions.Singleline | RegexOptions.Compiled
        );
        static readonly Regex _singlelineCommentRegex = new Regex(
            @"//.*$",
            options: RegexOptions.Multiline | RegexOptions.Compiled
        );
        static readonly Regex _multilineCommentRegex = new Regex(
            @"(/\*.*?\*/)|(/\*.*)",
            options: RegexOptions.Singleline | RegexOptions.Compiled
        );
        static readonly Regex _numberRegex = new Regex(
            @"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b",
            options: RegexOptions.Compiled
        );
        static readonly Regex _attributeRegex = new Regex(
            @"^\s*(?<range>\[.+?\])\s*$",
            options: RegexOptions.Multiline | RegexOptions.Compiled
        );
        static readonly Regex _keywordRegex = new Regex(
            @"\b(abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|while|add|alias|ascending|descending|dynamic|from|get|global|group|into|join|let|orderby|partial|remove|select|set|value|var|where|yield)\b|#region\b|#endregion\b",
            options: RegexOptions.Compiled
        );
        static readonly Regex _entityRegex = new Regex(
            @"\b(class|struct|enum|interface)\s+(?<range>\w+?)\b",
            options: RegexOptions.Compiled
        );

        public override IEnumerable<HighlightRange> Highlight(string text)
        {
            foreach (Match match in _stringRegex.Matches(text))
                yield return new HighlightRange(match, HighlightType.String);

            foreach (Match match in _singlelineCommentRegex.Matches(text))
                yield return new HighlightRange(match, HighlightType.SinglelineComment);

            foreach (Match match in _multilineCommentRegex.Matches(text))
                yield return new HighlightRange(match, HighlightType.MultilineComment);

            foreach (Match match in _numberRegex.Matches(text))
                yield return new HighlightRange(match, HighlightType.Number);

            foreach (Match match in _attributeRegex.Matches(text))
                yield return new HighlightRange(match, HighlightType.Attribute);

            foreach (Match match in _keywordRegex.Matches(text))
                yield return new HighlightRange(match, HighlightType.Keyword);

            foreach (Match match in _entityRegex.Matches(text))
                yield return new HighlightRange(match, HighlightType.Entity);
        }
    }
}
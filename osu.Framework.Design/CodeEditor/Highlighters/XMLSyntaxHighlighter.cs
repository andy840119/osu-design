using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace osu.Framework.Design.CodeEditor.Highlighters
{
    public class XMLSyntaxHighlighter : SyntaxHighlighter
    {
        static readonly Regex _attributeValueRegex = new Regex(
            @"[\w\d\-]+?=(?<range>'[^']*')|[\w\d\-]+[ ]*=[ ]*(?<range>""[^""]*"")|[\w\d\-]+[ ]*=[ ]*(?<range>[\w\d\-]+)",
            options: RegexOptions.Compiled
        );
        static readonly Regex _commentRegex = new Regex(
            @"(<!--.*?-->)|(<!--.*)",
            options: RegexOptions.Singleline | RegexOptions.Compiled
        );
        static readonly Regex _attributeRegex = new Regex(
            @"(?<range>[\w\d\-\:]+)[ ]*=[ ]*'[^']*'|(?<range>[\w\d\-\:]+)[ ]*=[ ]*""[^""]*""|(?<range>[\w\d\-\:]+)[ ]*=[ ]*[\w\d\-\:]+",
            options: RegexOptions.Compiled
        );
        static readonly Regex _tagRegex = new Regex(
            @"<\/|\/>|<\?|\?>|<|>",
            options: RegexOptions.Compiled
        );
        static readonly Regex _tagNameRegex = new Regex(
            @"<[?](?<range1>[x][m][l]{1})|<\/?(?<range>[!\w:]+)",
            options: RegexOptions.Compiled
        );

        public override IEnumerable<HighlightRange> Highlight(string text)
        {
            foreach (Match match in _attributeValueRegex.Matches(text))
                yield return new HighlightRange(match.Groups["range"], HighlightType.String);

            foreach (Match match in _commentRegex.Matches(text))
                yield return new HighlightRange(match, HighlightType.MultilineComment);

            foreach (Match match in _attributeRegex.Matches(text))
                yield return new HighlightRange(match.Groups["range"], HighlightType.Attribute);

            foreach (Match match in _tagRegex.Matches(text))
                yield return new HighlightRange(match, HighlightType.Keyword);

            foreach (Match match in _tagNameRegex.Matches(text))
                yield return new HighlightRange(match, HighlightType.Entity);
        }
    }
}
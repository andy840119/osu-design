using System.Text.RegularExpressions;

namespace osu.Framework.Design.Markup
{
    public class LocalBoundDrawableProperty : DrawableProperty
    {
        public static readonly Regex Syntax = new Regex(@"^\{\s*Binding\s+Local=(?<name>\S+)\s*\}$", RegexOptions.Compiled);

        public static new LocalBoundDrawableProperty Parse(IPropertyInfo property, string value)
        {
            var match = Syntax.Match(value);

            return new LocalBoundDrawableProperty(property)
            {
                BindableName = match.Groups["name"].Value
            };
        }

        public string BindableName { get; set; }

        public LocalBoundDrawableProperty(IPropertyInfo property) : base(property)
        {
        }
    }
}
using System.Text.RegularExpressions;

namespace osu.Framework.Design.Markup
{
    public class ConfigBoundDrawableProperty : DrawableProperty
    {
        public static readonly Regex Syntax = new Regex(@"^\{\s*Binding\s+Config=(?<name>\S+)\s*\}$", RegexOptions.Compiled);

        public static new ConfigBoundDrawableProperty Parse(IPropertyInfo property, string value)
        {
            var match = Syntax.Match(value);

            return new ConfigBoundDrawableProperty(property)
            {
                ConfigurationName = match.Groups["name"].Value
            };
        }

        public string ConfigurationName { get; set; }

        public ConfigBoundDrawableProperty(IPropertyInfo property) : base(property)
        {
        }
    }
}
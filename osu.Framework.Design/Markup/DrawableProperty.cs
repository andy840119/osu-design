namespace osu.Framework.Design.Markup
{
    public abstract class DrawableProperty
    {
        public static DrawableProperty Parse(IPropertyInfo property, string value)
        {
            if (LocalBoundDrawableProperty.Syntax.IsMatch(value))
                return LocalBoundDrawableProperty.Parse(property, value);

            if (ConfigBoundDrawableProperty.Syntax.IsMatch(value))
                return ConfigBoundDrawableProperty.Parse(property, value);

            return EmbeddedDrawableProperty.Parse(property, value);
        }

        public string Name => PropertyInfo.Name;
        public IPropertyInfo PropertyInfo { get; }

        protected DrawableProperty(IPropertyInfo property)
        {
            PropertyInfo = property;
        }
    }
}
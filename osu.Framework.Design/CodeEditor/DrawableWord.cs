using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

namespace osu.Framework.Design.CodeEditor
{
    // We simply derive from spritetext because it is already heavily optimized using drawnodes
    public class DrawableWord : SpriteText
    {
        public int Length => Current.Value.Length;

        public Bindable<HighlightStyle> Style { get; } = new Bindable<HighlightStyle>();

        public DrawableWord()
        {
            FixedWidth = true;
            Shadow = true;
        }

        DrawableEditor _editor;
        Bindable<string> _fontFamily;
        Bindable<float> _fontSize;

        [BackgroundDependencyLoader]
        void load(DrawableEditor editor)
        {
            _editor = editor;

            _fontFamily = editor.FontFamily.GetBoundCopy();
            _fontFamily.BindValueChanged(f => Font = f, runOnceImmediately: true);

            _fontSize = editor.FontSize.GetBoundCopy();
            _fontSize.BindValueChanged(s => TextSize = s, runOnceImmediately: true);

            Style.BindValueChanged(updateStyle);
            ResetStyle();
        }

        public void ResetStyle() => Style.BindTo(_editor.DefaultHighlightStyle);
        public void SetStyle(HighlightStyle style)
        {
            Style.UnbindBindings();
            Style.Value = style;
        }

        void updateStyle(HighlightStyle style)
        {
            this.FadeColour(style.Color, duration: 50);
        }

        public int StartIndex { get; private set; }
        public int EndIndex => StartIndex + Length;

        public void Set(string value, int startIndex)
        {
            Current.Value = value;

            StartIndex = startIndex;
        }

        protected override bool UseFixedWidthForCharacter(char c) => true;
    }
}
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
        public DrawableWord()
        {
            FixedWidth = true;
            Shadow = true;
        }

        Bindable<string> _fontFamily;
        Bindable<float> _fontSize;

        [BackgroundDependencyLoader]
        void load(DrawableEditor editor)
        {
            _fontFamily = editor.FontFamily.GetBoundCopy();
            _fontFamily.BindValueChanged(f => Font = f, runOnceImmediately: true);

            _fontSize = editor.FontSize.GetBoundCopy();
            _fontSize.BindValueChanged(s => TextSize = s, runOnceImmediately: true);
        }

        public int StartIndex { get; private set; }

        public void Set(string value, int startIndex)
        {
            Current.Value = value;

            StartIndex = startIndex;
        }

        protected override bool UseFixedWidthForCharacter(char c) => true;
    }
}
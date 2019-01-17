using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

namespace osu.Framework.Design.CodeEditor
{
    // We simply derive from spritetext because it is already heavily optimised using drawnodes
    public class DrawableWord : SpriteText
    {
        public int Length => Current.Value.Length;

        public Bindable<SRGBColour> ColourBindable { get; } = new Bindable<SRGBColour>();
        public Bindable<string> FontFamily { get; } = new Bindable<string>();
        public BindableFloat FontSize { get; } = new BindableFloat();

        public DrawableWord()
        {
            FixedWidth = true;
            Shadow = true;

            Set("");
        }

        [BackgroundDependencyLoader]
        void load(DrawableEditor editor)
        {
            ColourBindable.Value = Color4.White;
            ColourBindable.BindValueChanged(c => this.FadeColour(c, 200), true);

            FontFamily.BindTo(editor.FontFamily);
            FontFamily.BindValueChanged(f => Font = f, true);

            FontSize.BindTo(editor.FontSize);
            FontSize.BindValueChanged(s => TextSize = s, true);
        }

        public void Insert(string value, int index) => Set(Current.Value.Insert(index, value));
        public void Remove(int count, int index) => Set(Current.Value.Remove(index, count));

        public void Set(string text) => Current.Value = text;

        protected override bool UseFixedWidthForCharacter(char c) => true;
    }
}
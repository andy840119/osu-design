using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Sprites;

namespace osu.Framework.Design.CodeEditor
{
    // We simply derive from spritetext because it is already heavily optimised using drawnodes
    public class DrawableWord : SpriteText
    {
        public EditorWord Model { get; }

        public DrawableWord(EditorWord model)
        {
            Model = model;
            Current.BindTo(Model.Text);

            FixedWidth = true;
            Shadow = true;
        }

        Bindable<SRGBColour> _colour;
        Bindable<string> _font;
        Bindable<float> _fontSize;

        [BackgroundDependencyLoader]
        void load(DrawableEditor editor)
        {
            _colour = Model.Colour.GetBoundCopy();
            _font = editor.Font.GetBoundCopy();
            _fontSize = editor.FontSize.GetBoundCopy();

            _colour.BindValueChanged(c => this.FadeColour(c, 200), true);
            _font.BindValueChanged(f => Font = f, true);
            _fontSize.BindValueChanged(s => TextSize = s, true);
        }

        protected override bool UseFixedWidthForCharacter(char c) => true;
    }
}
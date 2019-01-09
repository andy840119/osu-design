using osu.Framework.Graphics;
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
            Model.Colour.ValueChanged += c => this.FadeColour(c, 200);

            FixedWidth = true;
            Shadow = true;
            Font = "Consolas";
        }
    }
}
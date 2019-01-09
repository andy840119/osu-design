using osu.Framework.Configuration;
using osu.Framework.Graphics.Colour;
using osuTK.Graphics;

namespace osu.Framework.Design.CodeEditor
{
    public class EditorWord
    {
        public Bindable<string> Text { get; } = new Bindable<string>();
        public Bindable<SRGBColour> Colour { get; } = new Bindable<SRGBColour>(Color4.White);

        public int Length => Text.Value.Length;

        public EditorWord(string initialValue)
        {
            Text.Value = initialValue;
        }

        public void Set(string value) => Text.Value = value;
    }
}
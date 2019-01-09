using osu.Framework.Configuration;
using osu.Framework.Graphics.Colour;

namespace osu.Framework.Design.CodeEditor
{
    public class EditorWord
    {
        public Bindable<string> Text { get; } = new Bindable<string>();
        public Bindable<SRGBColour> Colour { get; } = new Bindable<SRGBColour>();

        public int Length => Text.Value.Length;

        public EditorWord(string initialValue)
        {
            Text.Value = initialValue;
        }
    }
}
using osu.Framework.Graphics.Sprites;

namespace osu.Framework.Design.CodeEditor
{
    // We simply derive from spritetext because it is already heavily optimised using drawnodes
    public class DrawableWord : SpriteText
    {
        public int Length { get; private set; }

        public DrawableWord(string initialValue = "")
        {
            FixedWidth = true;
            Shadow = true;

            Current.Value = initialValue;
            Length = initialValue.Length;
        }

        public void Insert(int startIndex, string value)
        {
            Current.Value = Current.Value.Insert(startIndex, value);

            Length += value.Length;
        }

        public void Remove(int startIndex, int count)
        {
            Current.Value = Current.Value.Remove(startIndex, count);

            Length -= count;
        }
    }
}
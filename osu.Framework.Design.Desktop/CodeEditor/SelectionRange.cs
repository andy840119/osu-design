using osu.Framework.Configuration;

namespace osu.Framework.Design.CodeEditor
{
    public class SelectionRange
    {
        public BindableInt Start { get; } = new BindableInt
        {
            MinValue = 0
        };
        public BindableInt Length { get; } = new BindableInt
        {
            MinValue = 0
        };

        public int End => Start + Length;
    }
}
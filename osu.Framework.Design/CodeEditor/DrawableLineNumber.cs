using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Framework.Design.CodeEditor
{
    public class DrawableLineNumber : CompositeDrawable
    {
        DrawableWord _numberWord;

        int _value;
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                updateText();
            }
        }

        BindableInt _lineNumberWidth;

        [BackgroundDependencyLoader]
        void load(DrawableEditor editor)
        {
            AutoSizeAxes = Axes.Both;
            InternalChild = _numberWord = new DrawableWord
            {
                Alpha = 0.6f
            };

            _lineNumberWidth = editor.LineNumberWidth.GetBoundCopy() as BindableInt;
            _lineNumberWidth.BindValueChanged(w => updateText(), runOnceImmediately: true);
        }

        void updateText() => _numberWord.Text = Value.ToString().PadLeft(_lineNumberWidth);
    }
}
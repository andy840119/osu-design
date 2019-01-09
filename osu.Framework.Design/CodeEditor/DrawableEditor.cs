using System;
using System.Linq;
using System.Text.RegularExpressions;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osuTK;
using osuTK.Input;

namespace osu.Framework.Design.CodeEditor
{
    public class DrawableEditor : CompositeDrawable
    {
        readonly ScrollContainer _scroll;
        readonly FillFlowContainer<DrawableLine> _flow;
        readonly DrawableCaret _caret;

        public Bindable<int> CaretPosition { get; } = new Bindable<int>();
        public Bindable<string> CurrentLine { get; } = new Bindable<string>();
        public Bindable<string> SelectedText { get; } = new Bindable<string>();

        public DrawableEditor()
        {
            InternalChildren = new Drawable[]
            {
                _scroll = new ScrollContainer(Direction.Vertical)
                {
                    RelativeSizeAxes = Axes.Both,
                    ClampExtension = 0,
                    DistanceDecayDrag = 0.01,
                    DistanceDecayScroll = 0.1,
                    DistanceDecayJump = 0.2,
                    Child = new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            _flow = new FillFlowContainer<DrawableLine>
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                // Need one line to start with
                                Child = new DrawableLine()
                            },
                            _caret = new DrawableCaret()
                        }
                    }
                }
            };

            CaretPosition.ValueChanged += handleCaretMove;
        }

        ITextInputSource _textInput;

        [BackgroundDependencyLoader]
        void load(GameHost host)
        {
            _textInput = host.GetTextInput();
            _textInput.Activate(this);
        }

        void handleCaretMove(int index)
        {
            for (var i = 0; i < _flow.Count; i++)
            {
                var line = _flow[i];

                // Find line that contains index
                if (index > line.Length)
                {
                    index -= line.Length;
                    continue;
                }

                _caret.MoveTo(line.ToSpaceOfOtherDrawable(
                    new Vector2(9, 0) * index,
                    _flow
                ), 100);
            }
        }

        protected override void Update()
        {
            base.Update();

            var pending = _textInput.GetPendingText();

            if (!string.IsNullOrEmpty(pending))
            {
                Insert(pending);
                CaretPosition.Value += pending.Length;
            }
        }

        public int Length { get; private set; }

        static readonly Regex _splitRegex = new Regex(@"(?<=\n)", RegexOptions.Compiled);

        public void Insert(string text) => Insert(CaretPosition.Value, text);
        public void Insert(int startIndex, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            // Ensure startIndex is valid
            startIndex = Math.Clamp(startIndex, 0, Length);

            var lines = _flow.ToList();

            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                if (value != null)
                {
                    // Find line that contains startIndex
                    if (startIndex > line.Length)
                    {
                        startIndex -= line.Length;
                        continue;
                    }

                    // Split value by lines
                    var valueLines = _splitRegex.Split(value);

                    // First part appends to our matching line
                    line.Insert(startIndex, valueLines[i]);

                    // All other parts are added as new words
                    for (var j = 1; j < valueLines.Length; j++)
                    {
                        var valueLine = new DrawableLine(valueLines[j]);
                        lines.Insert(i + j, valueLine);

                        _flow.Add(valueLine);
                    }

                    Length += value.Length;
                    value = null;
                }

                // Update depth to reflect the insertion
                _flow.ChangeChildDepth(line, i);
            }
        }

        public void Remove(int count) => Remove(CaretPosition.Value, count);
        public void Remove(int startIndex, int count)
        {
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            base.OnKeyDown(e);

            switch (e.Key)
            {
                case Key.Left:
                    CaretPosition.Value--;
                    break;
                case Key.Right:
                    CaretPosition.Value++;
                    break;
                case Key.BackSpace:
                    Remove(CaretPosition.Value - 1, 1);
                    CaretPosition.Value--;
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}
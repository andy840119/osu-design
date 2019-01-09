using System;
using System.Collections.Generic;
using System.Linq;
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
        public EditorModel Model { get; }

        readonly ScrollContainer _scroll;
        readonly FillFlowContainer<DrawableLine> _flow;
        readonly DrawableCaret _caret;

        public Bindable<int> CaretPosition { get; } = new Bindable<int>();
        public Bindable<string> CurrentLine { get; } = new Bindable<string>();
        public Bindable<string> SelectedText { get; } = new Bindable<string>();

        public DrawableEditor()
        {
            Model = new EditorModel();
            Model.Lines.ItemsAdded += handleLinesAdded;
            Model.Lines.ItemsRemoved += handleLinesRemoved;

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
                                Direction = FillDirection.Vertical
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

        void handleLinesAdded(IEnumerable<EditorLine> models)
        {
            foreach (var model in models)
                _flow.Add(new DrawableLine(model));

            updateLineDepths();
        }

        void handleLinesRemoved(IEnumerable<EditorLine> models)
        {
            // For faster removal performance, convert to an array
            var modelsArray = models.ToArray();

            _flow.RemoveAll(l => Array.IndexOf(modelsArray, l.Model) != -1);

            updateLineDepths();
        }

        void updateLineDepths()
        {
            foreach (var line in _flow)
                _flow.ChangeChildDepth(line, Model.Lines.IndexOf(line.Model));
        }

        void handleCaretMove(int index)
        {
            for (var i = 0; i < _flow.Count; i++)
            {
                var line = _flow[i];

                // Find line that contains index
                if (index > line.Model.Length)
                {
                    index -= line.Model.Length;
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

            // Handle text input
            var pending = _textInput.GetPendingText();

            if (!string.IsNullOrEmpty(pending))
            {
                Model.Insert(CaretPosition.Value, pending);
                CaretPosition.Value += pending.Length;
            }
        }

        public void AdvanceCaret(int count = 1) => CaretPosition.Value = Math.Min(CaretPosition.Value + count, Model.Length);
        public void RetreatCaret(int count = 1) => CaretPosition.Value = Math.Max(CaretPosition.Value - count, 0);

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            base.OnKeyDown(e);

            switch (e.Key)
            {
                case Key.Left:
                    RetreatCaret();
                    break;
                case Key.Right:
                    AdvanceCaret();
                    break;
                case Key.BackSpace:
                    if (CaretPosition.Value > 0)
                    {
                        Model.Remove(CaretPosition.Value - 1, 1);
                        RetreatCaret();
                    }
                    break;
                case Key.Enter:
                case Key.KeypadEnter:
                    Model.InsertLine(Math.Clamp(CaretPosition.Value, 0, Model.Length));
                    AdvanceCaret();
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}
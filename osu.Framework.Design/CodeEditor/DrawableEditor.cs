using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.IO.Stores;
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
        readonly MouseInputReceptor _mouse;

        public Bindable<string> Font { get; } = new Bindable<string>("Inconsolata");
        public Bindable<float> FontSize { get; } = new Bindable<float>(20);

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
                    ClampExtension = 100,
                    DistanceDecayDrag = 0.008,
                    DistanceDecayScroll = 0.056,
                    DistanceDecayJump = 0.056,
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
                                LayoutEasing = Easing.OutQuint,
                                LayoutDuration = 200
                            },
                            _caret = new DrawableCaret()
                        }
                    }
                },
                _mouse = new MouseInputReceptor
                {
                    RelativeSizeAxes = Axes.Both
                }
            };

            _mouse.MouseDown += handleMouseDown;

            handleLinesAdded(Model.Lines);
        }

        sealed class MouseInputReceptor : Drawable
        {
            public event Func<MouseDownEvent, bool> MouseDown;

            protected override bool OnMouseDown(MouseDownEvent e) => MouseDown?.Invoke(e) ?? false;
        }

        DependencyContainer _dependencies;
        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            _dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        ITextInputSource _textInput;

        [BackgroundDependencyLoader]
        void load(GameHost host, FontStore fonts)
        {
            _textInput = host.GetTextInput();
            _textInput.Activate(this);

            // Cache this as dependency for our children only
            _dependencies.CacheAs(this);

            // Need font store to get fixed width size
            Font.BindValueChanged(f => fixedWidth = fonts.GetCharacter(f, 'D').DisplayWidth, true);

            // Bind caret event and update it
            CaretPosition.BindValueChanged(handleCaretMove, true);
        }

        bool handleMouseDown(MouseDownEvent e)
        {
            var pos = _flow.ToLocalSpace(e.ScreenSpaceMouseDownPosition);

            var index = 0;

            for (var i = 0; i < _flow.Count; i++)
            {
                var line = _flow[i];

                if (pos.Y > line.DrawHeight)
                {
                    index += line.Model.Length + 1;

                    pos.Y -= line.DrawHeight;
                    continue;
                }

                index += Math.Clamp((int)Math.Round(pos.X / fixedWidth / FontSize.Value), 0, line.Model.Length);

                CaretPosition.Value = index;
                break;
            }

            return true;
        }

        void handleLinesAdded(IEnumerable<EditorLine> models)
        {
            if (LoadState != LoadState.Loaded)
            {
                Schedule(() => handleLinesAdded(models));
                return;
            }

            foreach (var model in models)
                _flow.Add(new DrawableLine(model));

            updateLinePositions();
        }

        void handleLinesRemoved(IEnumerable<EditorLine> models)
        {
            if (LoadState != LoadState.Loaded)
            {
                Schedule(() => handleLinesRemoved(models));
                return;
            }

            foreach (var model in models)
                _flow.Remove(_flow.First(l => l.Model == model));

            updateLinePositions();
        }

        void updateLinePositions()
        {
            foreach (var line in _flow)
                _flow.SetLayoutPosition(line, Model.Lines.IndexOf(line.Model));
        }

        float fixedWidth;

        void handleCaretMove(int index)
        {
            for (var i = 0; i < _flow.Count; i++)
            {
                var line = _flow[i];

                // Find line that contains index
                if (index >= line.Model.Length + 1)
                {
                    index -= line.Model.Length + 1;
                    continue;
                }

                var y = i * FontSize.Value;
                var x = index * fixedWidth * FontSize.Value;

                _caret.MoveTo(new Vector2(x, y), 50, Easing.Out);
                break;
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

        public void MoveCaret(int position) => CaretPosition.Value = Math.Clamp(position, 0, Model.Length);
        public void AdvanceCaret(int count = 1) => MoveCaret(CaretPosition.Value + count);
        public void RetreatCaret(int count = 1) => MoveCaret(CaretPosition.Value - count);

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
                case Key.Delete:
                    if (CaretPosition.Value < Model.Length)
                    {
                        Model.Remove(CaretPosition.Value, 1);
                    }
                    break;
                case Key.Enter:
                case Key.KeypadEnter:
                    Model.Insert(Math.Clamp(CaretPosition.Value, 0, Model.Length), "\n");
                    AdvanceCaret();
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osuTK;
using osuTK.Input;

namespace osu.Framework.Design.CodeEditor
{
    public class DrawableEditor : CompositeDrawable, IHasCurrentValue<string>
    {
        ScrollContainer _scroll;
        FillFlowContainer<DrawableLine> _flow;
        MouseInputReceptor _mouse;
        DrawableCaret _caret;

        ITextInputSource _textInput;
        FontStore _fontStore;

        readonly Bindable<string> _text = new Bindable<string>(string.Empty);
        public Bindable<string> Current
        {
            get => _text;
            set
            {
                _text.UnbindBindings();
                _text.BindTo(value);
            }
        }

        public int Length => Current.Value.Length;

        public Bindable<string> FontFamily { get; } = new Bindable<string>("Inconsolata");
        public BindableFloat FontSize { get; } = new BindableFloat(20);
        public BindableInt LineNumberWidth { get; } = new BindableInt(5);

        public BindableInt SelectionStart { get; } = new BindableInt();

        sealed class MouseInputReceptor : Drawable
        {
            public Func<MouseDownEvent, bool> MouseDown;

            protected override bool OnMouseDown(MouseDownEvent e) => MouseDown?.Invoke(e) ?? false;
        }

        DependencyContainer _dependencies;
        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            _dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        [BackgroundDependencyLoader]
        void load(GameHost host, FontStore fonts)
        {
            _dependencies.CacheAs(this);

            _textInput = host.GetTextInput();
            _fontStore = fonts;

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
                                Direction = FillDirection.Vertical
                            }
                        }
                    }
                },
                _caret = new DrawableCaret(),
                _mouse = new MouseInputReceptor
                {
                    RelativeSizeAxes = Axes.Both,
                    MouseDown = handleMouseDown
                }
            };

            Current.BindValueChanged(updateText, runOnceImmediately: true);

            FontFamily.BindValueChanged(f => updateFontCache(), runOnceImmediately: true);
            FontSize.BindValueChanged(f => updateFontCache(), runOnceImmediately: true);

            SelectionStart.BindValueChanged(p => updateCaretPosition(), runOnceImmediately: true);
            LineNumberWidth.BindValueChanged(w => updateCaretPosition(), runOnceImmediately: true);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            _textInput.Activate(this);
        }

        protected override void Update()
        {
            base.Update();

            // Handle pending text input
            var pending = _textInput.GetPendingText();

            if (!string.IsNullOrEmpty(pending))
            {
                Insert(pending);
                AdvanceCaret();
            }
        }

        static readonly Regex _lineSplitRegex = new Regex(@"(?<=[\r\n])", RegexOptions.Compiled);
        static readonly Regex _wordSplitRegex = new Regex(@"(?<= )(?=\S)", RegexOptions.Compiled);

        void updateText(string text)
        {
            var lines = _lineSplitRegex.Split(text);

            // Add new lines or set existing ones
            var index = 0;

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var parts = _wordSplitRegex.Split(line);

                if (i == _flow.Count)
                    _flow.Add(new DrawableLine());

                var lineDrawable = _flow[i];
                lineDrawable.Set(parts, index);
                lineDrawable.LineNumber.Value = i + 1;

                index += parts.Length;
            }

            // Remove unused lines
            while (_flow.Count > lines.Length)
                _flow.Remove(_flow[lines.Length]);
        }

        public void Insert(string value) => Insert(value, SelectionStart.Value);
        public void Insert(string value, int index)
        {
            if (string.IsNullOrEmpty(value))
                return;

            index = Math.Clamp(index, 0, Length);

            Current.Value = Current.Value.Insert(index, value);
        }

        public void Remove(int count) => Remove(count, SelectionStart.Value);
        public void Remove(int count, int index)
        {
            if (index >= Length)
                return;

            count = Math.Clamp(count, 0, Length - index);

            if (count == 0)
                return;

            Current.Value = Current.Value.Remove(index, count);
        }

        float _charWidth;

        void updateFontCache() => _charWidth = _fontStore.GetCharacter(FontFamily, 'D').DisplayWidth * FontSize;

        void updateCaretPosition()
        {
            var index = SelectionStart.Value;

            for (var i = 0; i < _flow.Count; i++)
            {
                var line = _flow[i];

                if (index >= line.Length && i != _flow.Count - 1)
                {
                    index -= line.Length;
                    continue;
                }

                _caret.MoveTo(new Vector2(
                    x: _charWidth * index + line.TextStartOffset,
                    y: FontSize.Value * i
                ), 50, Easing.OutQuint);
                break;
            }
        }

        bool handleMouseDown(MouseDownEvent e)
        {
            var pos = _flow.ToLocalSpace(e.ScreenSpaceMouseDownPosition);

            return true;
        }

        public bool MoveCaret(int position)
        {
            var clamped = Math.Clamp(position, 0, Length);
            SelectionStart.Value = clamped;

            return clamped == position;
        }
        public bool AdvanceCaret(int count = 1) => MoveCaret(SelectionStart.Value + count);
        public bool RetreatCaret(int count = 1) => MoveCaret(SelectionStart.Value - count);

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
                    if (RetreatCaret())
                        Remove(1);
                    break;
                case Key.Delete:
                    Remove(1);
                    break;
                case Key.Enter:
                case Key.KeypadEnter:
                    Insert("\n");
                    AdvanceCaret();
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}
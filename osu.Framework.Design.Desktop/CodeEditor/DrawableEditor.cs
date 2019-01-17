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
    public class DrawableEditor : CompositeDrawable
    {
        ScrollContainer _scroll;
        FillFlowContainer<DrawableLine> _flow;
        MouseInputReceptor _mouse;
        DrawableCaret _caret;

        public int Length => _flow.Sum(l => l.Length) + _flow.Count;
        public string Text => string.Concat(_flow.Select(l => l.Text + '\n'));

        ITextInputSource _textInput;

        public Bindable<string> FontFamily { get; } = new Bindable<string>("Inconsolata");
        public BindableFloat FontSize { get; } = new BindableFloat(20);

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
            _textInput = host.GetTextInput();

            _dependencies.CacheAs(this);

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
                                LayoutDuration = 200,
                                Child = new DrawableLine()
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

            FontFamily.BindValueChanged(f =>
            {
                // Need font store to get fixed width size
                _fixedWidth = fonts.GetCharacter(f, 'D').DisplayWidth;
            }, true);
            SelectionStart.BindValueChanged(updateCaretPosition, true);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            _textInput.Activate(this);
        }

        protected override void Update()
        {
            base.Update();

            // Handle text input
            var pending = _textInput.GetPendingText();

            if (!string.IsNullOrEmpty(pending))
            {
                Insert(pending);
                AdvanceCaret();
            }
        }

        public DrawableLine GetLineAtIndex(int index, out int lineIndex, out int charIndex)
        {
            for (var i = 0; i < _flow.Count; i++)
            {
                var line = _flow[i];

                if (index >= line.Length + 1 && i != _flow.Count - 1)
                {
                    index -= line.Length - 1;
                    continue;
                }

                lineIndex = i;
                charIndex = index;
                return line;
            }

            throw new ArgumentOutOfRangeException(nameof(index));
        }

        static readonly Regex _splitRegex = new Regex(@"[\r\n]", RegexOptions.Compiled);

        public void Insert(string value) => Insert(value, SelectionStart.Value);
        public void Insert(string value, int index)
        {
            var line = GetLineAtIndex(index, out var lineIndex, out var charIndex);

            // We can skip newline handling if we don't have one
            if (!_splitRegex.IsMatch(value))
            {
                line.Insert(value, charIndex);
                return;
            }

            var parts = _splitRegex.Split(value).ToList();
            var wordParts = _splitRegex.Split(line.Text.Insert(charIndex, parts[0] + '\n'));

            line.Set(wordParts[0]);

            parts.RemoveAt(0);
            parts.InsertRange(0, wordParts.Skip(1));

            var drawables = _flow.ToList();

            for (var i = 0; i < parts.Count - 1; i++)
            {
                var drawable = new DrawableLine();
                _flow.Add(drawable);

                drawable.Set(parts[i]);
                drawables.Insert(lineIndex + i + 1, drawable);
            }

            drawables[lineIndex + parts.Count - 1].Insert(parts[parts.Count - 1], 0);

            foreach (var drawable in _flow)
                _flow.SetLayoutPosition(drawable, drawables.IndexOf(drawable));
        }

        public void Remove(int count) => Remove(count, SelectionStart.Value);
        public void Remove(int count, int index)
        {
            var drawables = _flow.ToList();

            while (count > 0)
            {
                var line = GetLineAtIndex(index, out var lineIndex, out var charIndex);
                var removeable = Math.Min(count, line.Length + 1 - charIndex);

                if (removeable == 0)
                    break;

                if (line.Length == removeable)
                {
                    _flow.Remove(line);
                    drawables.RemoveAt(lineIndex);
                }
                else
                    line.Remove(removeable, charIndex);

                count -= removeable;
                charIndex = 0;
            }

            foreach (var drawable in _flow)
                _flow.SetLayoutPosition(drawable, drawables.IndexOf(drawable));

            // We want to keep at least one line
            if (_flow.Count == 0)
                _flow.Add(new DrawableLine());
        }

        public void Set(string text)
        {
            _flow.Child = new DrawableLine();
            Insert(text, 0);
        }

        float _fixedWidth;

        void updateCaretPosition(int index)
        {
            var line = GetLineAtIndex(index, out var lineIndex, out var wordIndex);

            _caret.MoveTo(new Vector2(_fixedWidth * FontSize.Value * wordIndex, lineIndex * FontSize.Value), 50, Easing.OutQuint);
        }

        bool handleMouseDown(MouseDownEvent e)
        {
            var pos = _flow.ToLocalSpace(e.ScreenSpaceMouseDownPosition);

            return true;
        }

        public void MoveCaret(int position) => SelectionStart.Value = Math.Clamp(position, 0, Length);
        public void AdvanceCaret(int count = 1) => MoveCaret(SelectionStart.Value + count);
        public void RetreatCaret(int count = 1) => MoveCaret(SelectionStart.Value - count);

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
                    RetreatCaret();
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
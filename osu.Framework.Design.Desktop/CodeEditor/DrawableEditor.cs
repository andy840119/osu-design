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

        public BindableList<SelectionRange> Selections { get; } = new BindableList<SelectionRange>();

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
                    Child = _flow = new FillFlowContainer<DrawableLine>
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical
                    }
                },
                _mouse = new MouseInputReceptor
                {
                    RelativeSizeAxes = Axes.Both,
                    MouseDown = handleMouseDown
                }
            };

            Current.BindValueChanged(updateText, runOnceImmediately: true);

            FontFamily.BindValueChanged(f => updateFontCache(), runOnceImmediately: true);
            FontSize.BindValueChanged(f => updateFontCache(), runOnceImmediately: true);

            Selections.ItemsAdded += handleSelectionsAdded;
            Selections.ItemsRemoved += handleSelectionsRemoved;
            Selections.Add(new SelectionRange());
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

                index += line.Length;
            }

            // Remove unused lines
            while (_flow.Count > lines.Length)
                _flow.Remove(_flow[lines.Length]);
        }

        public void Insert(string value) => Insert(value, Selections[0].Start);
        public void Insert(string value, int index)
        {
            if (string.IsNullOrEmpty(value))
                return;

            index = Math.Clamp(index, 0, Length);

            Current.Value = Current.Value.Insert(index, value);
        }

        public void Remove(int count) => Remove(count, Selections[0].Start);
        public void Remove(int count, int index)
        {
            if (index >= Length)
                return;

            count = Math.Clamp(count, 0, Length - index);

            if (count == 0)
                return;

            Current.Value = Current.Value.Remove(index, count);
        }

        public float CharWidth { get; private set; }

        void updateFontCache() => CharWidth = _fontStore.GetCharacter(FontFamily, 'D').DisplayWidth * FontSize;

        public Vector2 GetPositionAtIndex(int index)
        {
            for (var i = 0; i < _flow.Count; i++)
            {
                var line = _flow[i];

                if (index >= line.Length && i != _flow.Count - 1)
                {
                    index -= line.Length;
                    continue;
                }

                return new Vector2(
                    x: CharWidth * index + line.TextStartOffset,
                    y: FontSize.Value * i
                );
            }

            return Vector2.Zero;
        }

        public int GetIndexAtPosition(Vector2 pos)
        {
            for (var i = 0; i < _flow.Count; i++)
            {
                if (pos.Y > FontSize && i != _flow.Count - 1)
                {
                    pos.Y -= FontSize;
                    continue;
                }

                var line = _flow[i];
                pos.X -= line.TextStartOffset;

                var j = (int)Math.Clamp(Math.Round(pos.X / CharWidth), 0, line.Length - 1);

                return line.StartIndex + j;
            }

            return 0;
        }

        public DrawableLine GetLineAtIndex(int index, out int lineIndex, out int indexInLine)
        {
            for (var i = 0; i < _flow.Count; i++)
            {
                var line = _flow[i];

                if (index >= line.Length && i != _flow.Count - 1)
                {
                    index -= line.Length;
                    continue;
                }

                lineIndex = i;
                indexInLine = index;
                return line;
            }

            lineIndex = -1;
            indexInLine = -1;
            return null;
        }

        public DrawableLine this[int index] => _flow[index];

        bool handleMouseDown(MouseDownEvent e)
        {
            var index = GetIndexAtPosition(_flow.ToLocalSpace(e.ScreenSpaceMouseDownPosition));

            Selections.RemoveAll(s => true);

            // New selection is automatically added
            var selection = Selections[0];

            selection.Start.Value = index;
            selection.Length.Value = 0;

            return true;
        }


        readonly Dictionary<SelectionRange, DrawableCaret> _caretDrawables = new Dictionary<SelectionRange, DrawableCaret>();
        readonly Dictionary<SelectionRange, DrawableSelection> _selectionDrawables = new Dictionary<SelectionRange, DrawableSelection>();

        void handleSelectionsAdded(IEnumerable<SelectionRange> selections)
        {
            foreach (var selection in selections)
            {
                // Caret drawable
                _scroll.ScrollContent.Add(_caretDrawables[selection] = new DrawableCaret(selection)
                {
                    Depth = -_caretDrawables.Count
                });

                // Selection drawable
                _scroll.ScrollContent.Add(_selectionDrawables[selection] = new DrawableSelection(selection)
                {
                    Depth = _selectionDrawables.Count
                });
            }
        }
        void handleSelectionsRemoved(IEnumerable<SelectionRange> selections)
        {
            foreach (var selection in selections)
            {
                // Remove caret
                _scroll.ScrollContent.Remove(_caretDrawables[selection]);
                _caretDrawables.Remove(selection);

                // Remove selection
                _scroll.ScrollContent.Remove(_selectionDrawables[selection]);
                _selectionDrawables.Remove(selection);
            }

            // Must have at least one selection
            if (Selections.Count == 0)
                Selections.Add(new SelectionRange());
        }

        public bool AdvanceCaret(int count = 1, bool resetSelection = true)
        {
            var advanced = false;

            foreach (var selection in Selections)
                if (selection.Start + count < Length)
                {
                    selection.Start.Value += count;

                    if (resetSelection)
                        selection.Length.Value = 0;
                    else
                        selection.Length.Value -= count;

                    advanced = true;
                }

            return advanced;
        }

        public bool RetreatCaret(int count = 1, bool resetSelection = true)
        {
            var retreated = false;

            foreach (var selection in Selections)
                if (selection.Start - count >= 0)
                {
                    selection.Start.Value -= count;

                    if (resetSelection)
                        selection.Length.Value = 0;
                    else
                        selection.Length.Value += count;

                    retreated = true;
                }

            return retreated;
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            base.OnKeyDown(e);

            switch (e.Key)
            {
                case Key.Left:
                    RetreatCaret(resetSelection: !e.ShiftPressed);
                    break;
                case Key.Right:
                    AdvanceCaret(resetSelection: !e.ShiftPressed);
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
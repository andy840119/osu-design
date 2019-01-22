using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Design.CodeEditor.Highlighters;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Framework.Threading;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace osu.Framework.Design.CodeEditor
{
    public class DrawableEditor : CompositeDrawable, IHasCurrentValue<string>
    {
        ScrollContainer _scroll;
        FillFlowContainer<DrawableLine> _flow;
        MouseInputHandler _mouse;

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
        public Bindable<ISyntaxHighlighter> SyntaxHighlighter { get; } = new Bindable<ISyntaxHighlighter>(new SyntaxHighlighter());
        public Bindable<HighlightStyleCollection> HighlightStyles { get; } = new Bindable<HighlightStyleCollection>(HighlightStyleCollection.Default);
        public Bindable<HighlightStyle> DefaultHighlightStyle { get; } = new Bindable<HighlightStyle>(new HighlightStyle(Color4.White, HighlightFont.Normal));
        public BindableInt TabSize { get; } = new BindableInt(4)
        {
            MinValue = 1
        };
        public BindableBool UseSpacesAsTabs { get; } = new BindableBool(true);
        public BindableList<SelectionRange> Selections { get; } = new BindableList<SelectionRange>();
        public BindableBool AutoIndent { get; } = new BindableBool(true);

        sealed class MouseInputHandler : Drawable
        {
            DrawableEditor _editor;

            [BackgroundDependencyLoader]
            void load(DrawableEditor editor)
            {
                _editor = editor;
            }

            int getIndexAtPosition(MouseEvent e) =>
                _editor.GetIndexAtPosition(_editor._scroll.ScrollContent.ToLocalSpace(e.ScreenSpaceMousePosition));

            int _clickIndex;
            bool _doubleClicking;

            protected override bool OnMouseDown(MouseDownEvent e)
            {
                base.OnMouseDown(e);

                _clickIndex = getIndexAtPosition(e);

                var selection = _editor.EnsureOneSelection();

                selection.Start.Value = _clickIndex;
                selection.End.Value = _clickIndex;
                _editor.GetLineAtIndex(selection.End, out _, out selection.IndexInLine);

                return true;
            }

            protected override bool OnMouseUp(MouseUpEvent e)
            {
                base.OnMouseUp(e);

                _clickIndex = -1;
                _doubleClicking = false;

                return true;
            }

            protected override bool OnClick(ClickEvent e)
            {
                base.OnClick(e);
                return true;
            }

            protected override bool OnDoubleClick(DoubleClickEvent e)
            {
                base.OnDoubleClick(e);

                var selection = _editor.EnsureOneSelection();
                var word = _editor.GetLineAtIndex(_clickIndex, out _, out var index).GetWordAtIndex(index, out _, out index);

                selection.Start.Value = word.StartIndex;
                selection.End.Value = word.EndIndex;
                _editor.GetLineAtIndex(selection.End, out _, out selection.IndexInLine);

                _doubleClicking = true;
                return true;
            }

            protected override bool OnDragStart(DragStartEvent e)
            {
                base.OnDragStart(e);
                return true;
            }

            protected override bool OnDragEnd(DragEndEvent e)
            {
                base.OnDragEnd(e);
                return true;
            }

            protected override bool OnDrag(DragEvent e)
            {
                base.OnDrag(e);

                var index = getIndexAtPosition(e);
                var selection = _editor.EnsureOneSelection();

                if (_doubleClicking)
                {
                    var currentWord = _editor.GetLineAtIndex(index, out _, out index).GetWordAtIndex(index, out _, out index);
                    var startWord = _editor.GetLineAtIndex(_clickIndex, out _, out var clickIndex).GetWordAtIndex(clickIndex, out _, out clickIndex);

                    if (currentWord.StartIndex < startWord.StartIndex)
                    {
                        selection.Start.Value = startWord.EndIndex;
                        selection.End.Value = currentWord.StartIndex;
                    }
                    else
                    {
                        selection.Start.Value = startWord.StartIndex;
                        selection.End.Value = currentWord.EndIndex;
                    }
                }
                else
                    selection.End.Value = index;

                _editor.GetLineAtIndex(selection.End, out _, out selection.IndexInLine);

                return true;
            }
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
                _mouse = new MouseInputHandler
                {
                    RelativeSizeAxes = Axes.Both
                }
            };

            FontFamily.BindValueChanged(f => Scheduler.AddOnce(updateFontCache));
            FontSize.BindValueChanged(f => Scheduler.AddOnce(updateFontCache));
            Scheduler.AddOnce(updateFontCache);

            Selections.ItemsAdded += handleSelectionsAdded;
            Selections.ItemsRemoved += handleSelectionsRemoved;

            SyntaxHighlighter.BindValueChanged(h => Scheduler.AddOnce(updateText));
            Current.BindValueChanged(t => Scheduler.AddOnce(updateText));
            Scheduler.AddOnce(updateText);

            ScheduleAfterChildren(() => EnsureOneSelection());
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
                AdvanceCaret(pending.Length);
            }
        }

        static readonly Regex _lineSplitRegex = new Regex(@"(?<=\n)", RegexOptions.Compiled);

        void updateText()
        {
            if (Current.Value.Contains('\r'))
            {
                Current.Value = Current.Value.Replace("\r", "");
                return;
            }

            var lines = _lineSplitRegex.Split(Current);

            // Add new lines or set existing ones
            var index = 0;

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var tokens = SyntaxHighlighter.Value.Tokenize(line);

                if (i == _flow.Count)
                    _flow.Add(new DrawableLine());

                var lineDrawable = _flow[i];
                lineDrawable.Set(tokens, index);
                lineDrawable.LineNumber.Value = i + 1;

                index += line.Length;
            }

            // Remove unused lines
            while (_flow.Count > lines.Length)
                _flow.Remove(_flow[lines.Length]);

            foreach (var selection in Selections)
                updateSelectionLimits(selection);

            scheduleHighlight();
        }

        void updateSelectionLimits(SelectionRange selection)
        {
            selection.Start.MinValue = 0;
            selection.Start.MaxValue = Length;

            selection.End.MinValue = 0;
            selection.End.MaxValue = Length;

            _caretDrawables[selection].ResetFlicker();
        }

        ScheduledDelegate _highlightTask;

        void scheduleHighlight()
        {
            _highlightTask?.Cancel();
            _highlightTask = Scheduler.AddDelayed(highlightCurrent, timeUntilRun: 200);
        }

        void highlightCurrent()
        {
            var ranges = SyntaxHighlighter.Value.Highlight(Current).ToList();

            for (var i = 0; i < _flow.Count; i++)
            {
                var line = _flow[i];

                for (var j = 0; j < line.Count; j++)
                {
                    var word = line[j];
                    var rangeIndex = ranges.FindIndex(r => r.Start <= word.StartIndex && word.EndIndex <= r.End);

                    if (rangeIndex == -1)
                    {
                        word.ResetStyle();
                        continue;
                    }

                    var range = ranges[rangeIndex];

                    if (HighlightStyles.Value.TryGetValue(range.Type, out var style))
                        word.SetStyle(style);
                    else
                        word.ResetStyle();

                    if (word.EndIndex == range.End)
                        ranges.RemoveAt(rangeIndex);
                }
            }
        }

        public void Insert(string value)
        {
            var offset = 0;

            foreach (var selection in Selections.OrderBy(s => s.End.Value))
            {
                var index = (selection.Length > 0 ? selection.Start : selection.End) + offset;
                var text = Current.Value;

                if (selection.Length != 0)
                    text = text.Remove(index, Math.Abs(selection.Length));

                text = text.Insert(index, value);

                Current.Value = text;
                selection.End.Value = selection.Start.Value = index;

                offset += value.Length;
            }
        }
        public void Insert(string value, int index)
        {
            if (string.IsNullOrEmpty(value))
                return;

            index = Math.Clamp(index, 0, Length);

            Current.Value = Current.Value.Insert(index, value);
        }

        public void Remove(bool retreat = false)
        {
            var offset = 0;

            foreach (var selection in Selections.OrderBy(s => s.End.Value))
            {
                var index = (selection.Length > 0 ? selection.Start : selection.End) + offset;
                var text = Current.Value;

                if (selection.Length != 0)
                {
                    var length = Math.Abs(selection.Length);

                    text = text.Remove(index, length);
                    offset -= length;
                }
                else if (retreat && index > 0)
                {
                    text = text.Remove(--index, 1);
                    offset -= 1;
                }
                else if (!retreat && index < text.Length)
                {
                    text = text.Remove(index, 1);
                    offset -= 1;
                }

                GetLineAtIndex(index, out _, out selection.IndexInLine);

                Current.Value = text;
                selection.Start.Value = selection.End.Value = index;
            }
        }
        public void Remove(int count, int index)
        {
            if (index >= Length)
                return;

            count = Math.Clamp(count, 0, Length - index);

            if (count == 0)
                return;

            Current.Value = Current.Value.Remove(index, count);
        }

        public SelectionRange EnsureOneSelection()
        {
            if (Selections.Count == 0)
                Selections.Add(new SelectionRange(this));

            Selections.RemoveAll(s => Selections.Count != 1);

            return Selections[0];
        }

        public float CharWidth { get; private set; }

        void updateFontCache() => CharWidth = _fontStore.GetCharacter(FontFamily, 'D').DisplayWidth * FontSize;

        public Vector2 GetPositionAtIndex(int index, bool ignoreEnd = true)
        {
            for (var i = 0; i < _flow.Count; i++)
            {
                var line = _flow[i];

                if ((index > line.Length || ignoreEnd && index == line.Length) && i != _flow.Count - 1)
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

                var j = (int)Math.Clamp(Math.Round(pos.X / CharWidth), 0, line.Length);

                if (i != _flow.Count - 1)
                    j = Math.Min(j, line.Length - 1);

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

                updateSelectionLimits(selection);
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
                Selections.Add(new SelectionRange(this));
        }

        public bool AdvanceCaret(int count = 1, bool roundToWord = false, bool keepSelection = false)
        {
            var advanced = false;

            foreach (var selection in Selections)
            {
                var before = selection.End.Value;

                if (roundToWord)
                {
                    var line = GetLineAtIndex(selection.End + count, out _, out var indexInLine);
                    var word = line.GetWordAtIndex(indexInLine, out _, out _);

                    selection.End.Value = count > 0 ? word.EndIndex : word.StartIndex;
                }
                else
                    selection.End.Value += count;

                if (!keepSelection)
                    selection.Start.Value = selection.End;

                GetLineAtIndex(selection.End, out _, out selection.IndexInLine);

                advanced |= before != selection.End;
            }

            return advanced;
        }

        public bool RetreatCaret(int count = 1, bool roundToWord = false, bool keepSelection = false) => AdvanceCaret(-count, roundToWord, keepSelection);

        public bool AdvanceCaretVertical(int roundToWord = 1, bool keepSelection = false)
        {
            var advanced = false;

            foreach (var selection in Selections)
            {
                var before = selection.End.Value;

                var line = GetLineAtIndex(selection.End, out var lineIndex, out _);

                lineIndex = Math.Clamp(lineIndex + roundToWord, 0, _flow.Count - 1);
                line = _flow[lineIndex];
                var indexInLine = Math.Clamp(selection.IndexInLine, 0, lineIndex == _flow.Count - 1 ? line.Length : line.Length - 1);

                selection.End.Value = line.StartIndex + indexInLine;

                if (!keepSelection)
                    selection.Start.Value = selection.End;

                advanced |= before != selection.End;
            }

            return advanced;
        }
        public bool RetreatCaretVertical(int roundToWord = 1, bool keepSelection = false) => AdvanceCaretVertical(-roundToWord, keepSelection);

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            base.OnKeyDown(e);

            switch (e.Key)
            {
                case Key.Left:
                    RetreatCaret(roundToWord: e.ControlPressed, keepSelection: e.ShiftPressed);
                    break;
                case Key.Right:
                    AdvanceCaret(roundToWord: e.ControlPressed, keepSelection: e.ShiftPressed);
                    break;
                case Key.Up:
                    if (e.ControlPressed)
                        _scroll.ScrollBy(-FontSize);
                    else
                        RetreatCaretVertical(keepSelection: e.ShiftPressed);
                    break;
                case Key.Down:
                    if (e.ControlPressed)
                        _scroll.ScrollBy(FontSize);
                    else
                        AdvanceCaretVertical(keepSelection: e.ShiftPressed);
                    break;
                case Key.BackSpace:
                    Remove(retreat: true);
                    break;
                case Key.Delete:
                    Remove();
                    break;
                case Key.Enter:
                case Key.KeypadEnter:
                    InsertNewLine(
                        advance: !e.ControlPressed,
                        passThrough: e.ShiftPressed
                    );
                    break;
                case Key.Tab:
                    if (UseSpacesAsTabs)
                    {
                        Insert(new string(' ', TabSize));
                        AdvanceCaret(TabSize);
                    }
                    else
                    {
                        Insert("\t");
                        AdvanceCaret();
                    }
                    break;
                default:
                    return false;
            }

            return true;
        }

        public void InsertNewLine(bool advance = true, bool passThrough = false)
        {
            var offset = 0;

            foreach (var selection in Selections.OrderBy(s => s.End.Value))
            {
                var index = (selection.Length > 0 ? selection.Start : selection.End) + offset;
                var text = Current.Value;

                if (selection.Length != 0)
                    text = text.Remove(index, Math.Abs(selection.Length));

                var indents = Current.Value.CountStart(' ', GetLineAtIndex(index, out _, out _).StartIndex);
                string newline;

                if (AutoIndent)
                    newline = '\n' + new string(' ', indents);
                else
                    newline = "\n";

                if (passThrough)
                {
                }
                else if (advance)
                {
                    text = text.Insert(selection.End, newline);
                    index += newline.Length;
                }
                else continue;

                GetLineAtIndex(index, out _, out selection.IndexInLine);

                Current.Value = text;
                selection.Start.Value = selection.End.Value = index;

                offset += newline.Length;
            }
        }
    }
}
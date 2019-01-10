using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace osu.Framework.Design.CodeEditor
{
    public class DrawableLine : Container<DrawableWord>
    {
        readonly FillFlowContainer<DrawableWord> _flow;

        protected override Container<DrawableWord> Content => _flow;

        public EditorLine Model { get; }

        public DrawableLine(EditorLine model)
        {
            Model = model;
            Model.Words.ItemsAdded += handleWordsAdded;
            Model.Words.ItemsRemoved += handleWordsRemoved;

            AutoSizeAxes = Axes.Both;
            InternalChildren = new Drawable[]
            {
                // Placeholder to hold the line size when it is empty
                new DrawableWord(new EditorWord(" ")),

                _flow = new FillFlowContainer<DrawableWord>
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    LayoutEasing = Easing.OutQuint,
                    LayoutDuration = 50
                }
            };

            handleWordsAdded(model.Words);
        }

        void handleWordsAdded(IEnumerable<EditorWord> models)
        {
            foreach (var model in models)
                _flow.Add(new DrawableWord(model));

            updateWordPositions();
        }

        void handleWordsRemoved(IEnumerable<EditorWord> models)
        {
            foreach (var model in models)
                _flow.Remove(_flow.First(l => l.Model == model));

            updateWordPositions();
        }

        void updateWordPositions()
        {
            foreach (var word in this)
                _flow.SetLayoutPosition(word, Model.Words.IndexOf(word.Model));
        }
    }
}
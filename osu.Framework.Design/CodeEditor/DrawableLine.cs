using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace osu.Framework.Design.CodeEditor
{
    public class DrawableLine : FillFlowContainer<DrawableWord>
    {
        public EditorLine Model { get; }

        public DrawableLine(EditorLine model)
        {
            Model = model;
            Model.Words.ItemsAdded += handleWordsAdded;
            Model.Words.ItemsRemoved += handleWordsRemoved;

            AutoSizeAxes = Axes.Both;
            Direction = FillDirection.Horizontal;
            LayoutEasing = Easing.OutQuint;
            LayoutDuration = 50;

            handleWordsAdded(model.Words);
        }

        void handleWordsAdded(IEnumerable<EditorWord> models)
        {
            foreach (var model in models)
                Add(new DrawableWord(model));

            updateWordPositions();
        }

        void handleWordsRemoved(IEnumerable<EditorWord> models)
        {
            foreach (var model in models)
                Remove(this.First(l => l.Model == model));

            updateWordPositions();
        }

        void updateWordPositions()
        {
            foreach (var word in this)
                SetLayoutPosition(word, Model.Words.IndexOf(word.Model));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

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

            handleWordsAdded(model.Words);
        }

        void handleWordsAdded(IEnumerable<EditorWord> models)
        {
            foreach (var model in models)
                Add(new DrawableWord(model));

            updateWordDepths();
        }

        void handleWordsRemoved(IEnumerable<EditorWord> models)
        {
            // For faster removal performance, convert to an array
            var modelsArray = models.ToArray();

            RemoveAll(w => Array.IndexOf(modelsArray, w.Model) != -1);

            updateWordDepths();
        }

        void updateWordDepths()
        {
            foreach (var word in this)
                ChangeChildDepth(word, Model.Words.IndexOf(word.Model));
        }
    }
}
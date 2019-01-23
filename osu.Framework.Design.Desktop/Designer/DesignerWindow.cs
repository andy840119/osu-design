using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Design.Workspaces;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace osu.Framework.Design.Designer
{
    public class DesignerWindow : ComponentWindow
    {
        DocumentTabControl _tabControl;

        public DesignerWindow() : base("Designer")
        {
        }

        [BackgroundDependencyLoader]
        void load()
        {
            AddInternal(new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = DesignerColours.Editor,
                Depth = float.MaxValue
            });

            Head.Child = _tabControl = new DocumentTabControl
            {
                RelativeSizeAxes = Axes.Both
            };

            _tabControl.Current.BindValueChanged(handleChange);
        }

        readonly Dictionary<Document, DrawableDesigner> _designerDrawables = new Dictionary<Document, DrawableDesigner>();

        public void SelectDocument(Document doc)
        {
            WorkingDocument workingDoc;

            if (_designerDrawables.TryGetValue(doc, out var drawable))
                workingDoc = drawable.WorkingDocument;
            else
            {
                workingDoc = doc.CreateWorkingDocument();

                Add(_designerDrawables[doc] = new DrawableDesigner(workingDoc)
                {
                    RelativeSizeAxes = Axes.Both
                });

                _tabControl.AddItem(workingDoc);
            }

            _tabControl.Current.Value = workingDoc;
        }

        void handleChange(WorkingDocument workingDoc)
        {
            var drawable = _designerDrawables[workingDoc.Document];

            foreach (var child in this)
                if (child != drawable)
                    child.Hide();

            drawable.ClearTransforms();
            drawable.Show();
        }
    }
}
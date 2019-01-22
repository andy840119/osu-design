using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Design.UserInterface;
using osu.Framework.Design.Workspaces;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace osu.Framework.Design.Designer
{
    public class DesignerWindow : ComponentWindow
    {
        DesignerTabControl<Document> _tabControl;

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

            Head.Child = _tabControl = new DesignerTabControl<Document>
            {
                RelativeSizeAxes = Axes.Both
            };

            _tabControl.Current.BindValueChanged(handleChange);
        }

        readonly Dictionary<Document, DrawableDesigner> _designerDrawables = new Dictionary<Document, DrawableDesigner>();

        public void SelectDocument(Document doc)
        {
            if (!_designerDrawables.ContainsKey(doc))
            {
                Add(_designerDrawables[doc] = new DrawableDesigner(doc)
                {
                    RelativeSizeAxes = Axes.Both
                });

                _tabControl.AddItem(doc);
            }

            _tabControl.Current.Value = doc;
        }

        void handleChange(Document doc)
        {
            var drawable = _designerDrawables[doc];

            foreach (var child in this)
                if (child != drawable)
                    child.Hide();

            drawable.ClearTransforms();
            drawable.Show();
        }
    }
}
using System;
using osu.Framework.Configuration;
using osu.Framework.Design.UserInterface;
using osu.Framework.Design.Workspaces;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace osu.Framework.Design.Designer
{
    public class DocumentTabControl : DesignerTabControl<WorkingDocument>
    {
        protected override TabItem<WorkingDocument> CreateTabItem(WorkingDocument value) => new DocumentTabItem(value);

        public class DocumentTabItem : DesignerTabItem
        {
            readonly Circle _syncIndicator;

            readonly Bindable<string> _content;
            readonly Bindable<DateTime> _lastWriteTime;

            public DocumentTabItem(WorkingDocument value) : base(value)
            {
                Add(_syncIndicator = new Circle
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Size = new Vector2(10),
                    Margin = new MarginPadding
                    {
                        Left = 6
                    }
                });

                _lastWriteTime = value.Document.LastWriteTime.GetBoundCopy();
                _lastWriteTime.BindValueChanged(handleWrite, runOnceImmediately: true);

                _content = value.Content.GetBoundCopy();
                _content.BindValueChanged(handleChange);
            }

            void handleWrite(DateTime writeTime)
            {
                if (Value.IsSynchronized)
                    _syncIndicator.FadeOut(duration: 200);
                else
                    _syncIndicator.FadeIn(duration: 200);
            }

            void handleChange(string content)
            {
                _syncIndicator.FadeIn(duration: 200);
            }
        }
    }
}
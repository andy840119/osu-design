using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace osu.Framework.Design.Designer
{
    public class HalvedContainer : CompositeDrawable
    {
        public Direction Direction { get; set; }

        public HalvedContainer(Drawable full)
        {
            Set(full);
        }
        public HalvedContainer(Direction dir, Drawable first, Drawable second)
        {
            Direction = dir;

            Set(first, second);
        }

        public Drawable First { get; private set; }
        public Drawable Second { get; private set; }

        public void Set(Drawable full)
        {
            ClearInternal();
            AddInternal(First = full);
            Second = null;
        }
        public void Set(Drawable first, Drawable second)
        {
            ClearInternal();
            AddInternal(First = first);
            AddInternal(Second = second);
        }

        protected override void Update()
        {
            base.Update();

            if (First == null || !First.IsPresent)
            {
                if (Second == null || !Second.IsPresent)
                    return;

                Second.Size = DrawSize;
                Second.Position = Vector2.Zero;
            }

            if (Second == null || !Second.IsPresent)
            {
                First.Size = DrawSize;
                First.Position = Vector2.Zero;
                return;
            }

            if (Direction == Direction.Horizontal)
            {
                First.Height = DrawHeight;
                First.Position = Vector2.Zero;

                Second.Size = new Vector2(DrawWidth - First.DrawWidth, DrawHeight);
                Second.Position = new Vector2(First.DrawWidth, 0);
            }
            else
            {
                First.Width = DrawWidth;
                First.Position = Vector2.Zero;

                Second.Size = new Vector2(DrawWidth, DrawHeight - First.DrawHeight);
                Second.Position = new Vector2(0, First.DrawHeight);
            }
        }
    }
}
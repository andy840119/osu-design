using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Framework.Design.Designer
{
    public class ComponentWindowContainer : ComponentWindow
    {
        public Direction Direction { get; set; }

        public ComponentWindowContainer() : base(null)
        {
        }

        protected override void Update()
        {
            base.Update();
        }
    }
}
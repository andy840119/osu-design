using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace osu.Framework.Design.CodeEditor
{
    public class DrawableCaret : CompositeDrawable
    {
        public DrawableCaret()
        {
            Masking = true;
            CornerRadius = 2;

            Size = new Vector2(4, 20);

            InternalChild = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.White.Opacity(0.8f)
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            // this.FadeIn(30)
            //     .Delay(500)
            //     .FadeOut(200)
            //     .Delay(300)
            //     .Loop();
        }
    }
}
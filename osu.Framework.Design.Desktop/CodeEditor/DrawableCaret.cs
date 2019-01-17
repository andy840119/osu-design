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
            CornerRadius = 1.5f;

            Size = new Vector2(3, 20);

            InternalChild = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.White.Opacity(0.8f)
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            this.FadeIn(30)
                .Delay(500)
                .FadeTo(0.4f, 200)
                .Delay(300)
                .Loop();
        }
    }
}
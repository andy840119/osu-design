namespace osu - design . osuML{
    public partial class BasicDrawable : osu.Framework.Graphics.Containers.Container
    {
        public osu.Framework.Graphics.Shapes.Box Background
        {
            get;
            private set;
        }

        public osu.Framework.Graphics.Containers.Container MyContainer
        {
            get;
            private set;
        }

        public BasicDrawable()
        {
            Name = "BasicDrawable";
            Size = new osuTK.Vector2(400F, 300F);
        }

        [osu.Framework.Allocation.BackgroundDependencyLoaderAttribute]
        private void load()
        {
            Children = new osu.Framework.Graphics.Drawable[]{Background = new osu.Framework.Graphics.Shapes.Box{Name = "Background", RelativeSizeAxes = osu.Framework.Graphics.Axes.Both, Colour = osu.Framework.Graphics.Colour.ColourInfo.SingleColour(new osuTK.Graphics.Color4(0.980392158F, 0.5882353F, 0.392156869F, 0.784313738F))}, MyContainer = new osu.Framework.Graphics.Containers.Container{Name = "MyContainer", Padding = new osu.Framework.Graphics.MarginPadding{Top = 60F, Right = 40F, Bottom = 60F, Left = 40F}, RelativeSizeAxes = osu.Framework.Graphics.Axes.Both, Children = new osu.Framework.Graphics.Drawable[]{new osu.Framework.Graphics.Shapes.Box{RelativeSizeAxes = osu.Framework.Graphics.Axes.Both, Colour = new osu.Framework.Graphics.Colour.ColourInfo{TopLeft = new osuTK.Graphics.Color4(0F, 1F, 0F, 1F), TopRight = new osuTK.Graphics.Color4(1F, 0F, 1F, 1F), BottomRight = new osuTK.Graphics.Color4(0F, 1F, 1F, 1F), BottomLeft = new osuTK.Graphics.Color4(1F, 0F, 0F, 1F)}}}}, new osu.Framework.Graphics.Sprites.SpriteText{Text = "My SpriteText is awesome", Font = "Nunito-BoldItalic", TextSize = 20F, Margin = new osu.Framework.Graphics.MarginPadding{Top = 10F, Right = 20F, Bottom = 10F, Left = 20F}}};
        }
    }
}
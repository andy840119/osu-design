namespace osu - design . osuML{
    public partial class ScrollingDrawable : osu.Framework.Graphics.Containers.Container
    {
        public osu.Framework.Graphics.Containers.ScrollContainer MyScroll
        {
            get;
            private set;
        }

        public ScrollingDrawable()
        {
            Name = "ScrollingDrawable";
            Size = new osuTK.Vector2(400F, 200F);
        }

        [osu.Framework.Allocation.BackgroundDependencyLoaderAttribute]
        private void load()
        {
            Children = new osu.Framework.Graphics.Drawable[]{new osu.Framework.Graphics.Shapes.Box{RelativeSizeAxes = osu.Framework.Graphics.Axes.Both, Colour = osu.Framework.Graphics.Colour.ColourInfo.SingleColour(new osuTK.Graphics.Color4(0.101960786F, 0.101960786F, 0.101960786F, 1F))}, MyScroll = new osu.Framework.Graphics.Containers.ScrollContainer{Name = "MyScroll", RelativeSizeAxes = osu.Framework.Graphics.Axes.Both, Children = new osu.Framework.Graphics.Drawable[]{new osu.Framework.Graphics.Containers.FillFlowContainer{RelativeSizeAxes = osu.Framework.Graphics.Axes.X, AutoSizeAxes = osu.Framework.Graphics.Axes.Y, Spacing = new osuTK.Vector2(0F, 10F), Padding = new osu.Framework.Graphics.MarginPadding(10F), Children = new osu.Framework.Graphics.Drawable[]{new osu.Framework.Graphics.UserInterface.TextBox{RelativeSizeAxes = osu.Framework.Graphics.Axes.X, Height = 30F, Text = "My textbox content"}, new osu.Framework.Graphics.UserInterface.TextBox{RelativeSizeAxes = osu.Framework.Graphics.Axes.X, Height = 40F, Text = "Second textbox"}, new osu.Framework.Graphics.Sprites.SpriteText{Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus sed felis leo. Donec magna risus, eleifend sed rutrum quis, dictum ut enim. Aenean auctor rutrum neque, id laoreet nisi rhoncus ac. Donec vehicula risus vel quam pharetra dapibus. Vestibulum vehicula dui eu lacus pulvinar efficitur. Quisque mollis eu erat id suscipit. Fusce placerat nibh in posuere aliquam. Donec et dui ipsum. Curabitur ante est, lacinia id fermentum eu, viverra eget ligula.", RelativeSizeAxes = osu.Framework.Graphics.Axes.X}}}}}};
        }
    }
}
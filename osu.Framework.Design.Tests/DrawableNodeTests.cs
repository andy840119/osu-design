using osu.Framework.Design.Markup;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using Xunit;

namespace osu.Framework.Design.Tests
{
    public class DrawableNodeTests
    {
        [Fact]
        public void TestLoad()
        {
            //Given
            var node = new DrawableNode();

            //When
            node.Load(@"
<Box xmlns=""osufx://osu.Framework/*""
    Name=""MyBox""/>");

            //Then
            Assert.Equal(typeof(Box).FullName, node.DrawableType.FullName);
            Assert.Equal("MyBox", node.GivenName);
        }

        [Fact]
        public void TestLoadInvalidContainer()
        {
            //Given
            var node = new DrawableNode();

            //Then
            Assert.Throws<MarkupException>(() => node.Load(@"
<Box xmlns=""osufx://osu.Framework/*"">
    <Box/>
    <SpriteText/>
</Box>"));
        }

        [Fact]
        public void TestLoadValidContainer()
        {
            //Given
            var node = new DrawableNode();

            //When
            node.Load(@"
<Container xmlns=""osufx://osu.Framework/*"">
    <Box/>
    <SpriteText/>
    <Container Name=""ContainerInContainer"">
        <Box/>
    </Container>
</Container>");

            // Then
            Assert.Equal(typeof(Container).FullName, node.DrawableType.FullName);
            Assert.Collection(node,
                box => Assert.Equal(typeof(Box).FullName, box.DrawableType.FullName),
                spriteText => Assert.Equal(typeof(SpriteText).FullName, spriteText.DrawableType.FullName),
                container =>
                {
                    Assert.Equal(typeof(Container).FullName, container.DrawableType.FullName);
                    Assert.Collection(container,
                        box => Assert.Equal(typeof(Box).FullName, box.DrawableType.FullName));
                });
        }

        [Fact]
        public void TestEmbeddedProperties()
        {
            //Given
            var node = new DrawableNode();

            //When
            node.Load(@"
<Box xmlns=""osufx://osu.Framework/*""
    Size=""100,200""
    RelativeSizeAxes=""Both""/>");

            //Then
            Assert.Equal(typeof(Box).FullName, node.DrawableType.FullName);
            Assert.Collection(node.Properties,
                size =>
                {
                    Assert.Equal("Size", size.Name);
                    Assert.IsType<EmbeddedDrawableProperty>(size);
                    Assert.Equal(new Vector2(100, 200), ((EmbeddedDrawableProperty)size).ParsedValue);
                },
                axes =>
                {
                    Assert.Equal("RelativeSizeAxes", axes.Name);
                    Assert.IsType<EmbeddedDrawableProperty>(axes);
                    Assert.Equal(Axes.Both, ((EmbeddedDrawableProperty)axes).ParsedValue);
                });
        }

        [Fact]
        public void TestNonexistentProperty()
        {
            //Given
            var node = new DrawableNode();

            //Then
            Assert.Throws<MarkupException>(() => node.Load(@"
<Box xmlns=""osufx://osu.Framework/*""
    MyNonexistentProperty=""1234""/>"));
        }

        [Fact]
        public void TestInvalidBindingName()
        {
            //Given
            var node = new DrawableNode();

            //When
            node.Load(@"
<Box xmlns=""osufx://osu.Framework/*""
    Name=""{Binding Local=BoxName}""/>");

            //Then
            Assert.Throws<MarkupException>(() => node.GivenName);
        }

        [Fact]
        public void TestInvalidChildrenProperty()
        {
            //Given
            var node = new DrawableNode();

            //Then
            Assert.Throws<MarkupException>(() => node.Load(@"
<Box xmlns=""osufx://osu.Framework/*""
    Name=""MyBox""
    Children=""Should throw exception here""/>"));
        }
    }
}
using System;
using System.IO;
using osu.Framework.Design.CodeGeneration;
using osu.Framework.Design.Markup;
using Xunit;

namespace osu.Framework.Design.Tests
{
    public class DrawableGeneratorTests
    {
        [Fact]
        public void TestGenerate()
        {
            //Given
            var doc = new DrawableNode();
            var code =
@"public partial class MyContainer : osu.Framework.Graphics.Containers.Container
{
    public osu.Framework.Graphics.Shapes.Box MyBox { get; private set; }

    public MyContainer()
    {
        Name = ""MyContainer"";
        RelativeSizeAxes = osu.Framework.Graphics.Axes.Both;
    }

    [osu.Framework.Allocation.BackgroundDependencyLoaderAttribute]
    private void load()
    {
        Children = new osu.Framework.Graphics.Drawable[]
        {
            MyBox = new osu.Framework.Graphics.Shapes.Box
            {
                Name = ""MyBox"",
                Colour = osu.Framework.Graphics.Colour.ColourInfo.SingleColour(new osuTK.Graphics.Color4(1F, 0F, 1F, 1F))
            }
        };
    }
}";

            doc.Load(@"
<Container
    xmlns=""osufx://osu.Framework/*""
    Name=""MyContainer""
    RelativeSizeAxes=""Both"">
    <Box
        Name=""MyBox""
        Colour=""#ff00ff""/>
</Container>");

            //When
            string generated;

            using (var writer = new StringWriter())
            {
                doc.GenerateClass(writer);
                generated = writer.ToString();
            }

            //Then
            Console.WriteLine(generated);
            Assert.Equal(code.RemoveAllSpaces(), generated.RemoveAllSpaces());
        }

        [Fact]
        public void TestGenerate2()
        {
            var doc = new DrawableNode();
            var code =
@"public partial class MyScreen : osu.Framework.Screens.Screen
{
    public osu.Framework.Graphics.Containers.Container MyFirstContainer { get; private set; }
    public osu.Framework.Graphics.Containers.Container MySecondContainer { get; private set; }
    public osu.Framework.Graphics.Shapes.Box MyInnermostBox { get; private set; }

    public MyScreen()
    {
        Name = ""MyScreen"";
    }

    [osu.Framework.Allocation.BackgroundDependencyLoaderAttribute]
    private void load()
    {
        Children = new osu.Framework.Graphics.Drawable[]
        {
            MyFirstContainer = new osu.Framework.Graphics.Containers.Container
            {
                Name = ""MyFirstContainer"",
                RelativeSizeAxes = osu.Framework.Graphics.Axes.Both,
                Colour = osu.Framework.Graphics.Colour.ColourInfo.SingleColour(new osuTK.Graphics.Color4(1F, 0F, 1F, 1F)),
                Children = new osu.Framework.Graphics.Drawable[]
                {
                    new osu.Framework.Graphics.Shapes.Box
                    {
                        RelativeSizeAxes = osu.Framework.Graphics.Axes.Both
                    },
                    MySecondContainer = new osu.Framework.Graphics.Containers.Container
                    {
                        Name = ""MySecondContainer"",
                        RelativeSizeAxes = osu.Framework.Graphics.Axes.Both,
                        Colour = osu.Framework.Graphics.Colour.ColourInfo.GradientVertical(new osuTK.Graphics.Color4(0F, 0F, 1F, 1F), new osuTK.Graphics.Color4(1F, 0F, 0F, 1F)),
                        Padding = new osu.Framework.Graphics.MarginPadding(100F),
                        Children = new osu.Framework.Graphics.Drawable[]
                        {
                            MyInnermostBox = new osu.Framework.Graphics.Shapes.Box
                            {
                                Name = ""MyInnermostBox"",
                                RelativeSizeAxes = osu.Framework.Graphics.Axes.Both
                            }
                        }
                    }
                }
            }
        };
    }
}";

            doc.Load(@"
<Screen
    xmlns=""osufx://osu.Framework/*""
    Name=""MyScreen"">
    <Container
        Name=""MyFirstContainer""
        RelativeSizeAxes=""Both""
        Colour=""#ff00ff"">
        <Box
            RelativeSizeAxes=""Both""/>
        <Container
            Name=""MySecondContainer""
            RelativeSizeAxes=""Both""
            Colour=""gradient(vertical, #0000ff, #ff0000)""
            Padding=""100"">
            <Box
                Name=""MyInnermostBox""
                RelativeSizeAxes=""Both""/>
        </Container>
    </Container>
</Screen>");

            //When
            string generated;

            using (var writer = new StringWriter())
            {
                doc.GenerateClass(writer);
                generated = writer.ToString();
            }

            //Then
            Console.WriteLine(generated);
            Assert.Equal(code.RemoveAllSpaces(), generated.RemoveAllSpaces());
        }
    }
}
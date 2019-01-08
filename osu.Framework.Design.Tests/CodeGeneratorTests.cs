using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using osu.Framework.Design.CodeGeneration;
using osu.Framework.Design.Markup;

namespace osu.Framework.Design.Tests
{
    [TestClass]
    public class CodeGeneratorTests
    {
        [TestMethod]
        public void TestSimpleGeneration()
        {
            var data = @"
<fx:Screen
    id=""MyScreen""
    xmlns:fx=""osu.Framework:*""
    xmlns:local = ""osu.Framework.Design:*""
    Width=""500"">
    <fx:Box
        id=""MyBox""
        Position=""10,20""
        RelativeSizeAxes=""Both""
        Colour=""#00ff00"">
    </fx:Box>
    <fx:Container
        id=""MyContainer_lv1"">
        <fx:Box
            id=""BoxInContainer1""/>
        <fx:Container
            id=""MyContainer_lv2"">
            <fx:Container
                id=""MyContainer_lv3"">
                <fx:Container
                    id=""MyContainer_lv4""/>
            </fx:Container>
            <fx:Box
                id=""BoxInContainer2""/>
        </fx:Container>
    </fx:Container>
</fx:Screen>";

            var parser = new MarkupReader();

            // Parse markup
            DrawableData d;

            using (var reader = new StringReader(data))
                d = parser.Parse(reader);

            // Generate code
            var workspace = new AdhocWorkspace();
            var project = workspace.AddProject("Test", "C#");
            var document = project.AddDocument("TestDrawable.cs", string.Empty, new[] { "folder1", "folder2" });
            var generator = new CodeGenerator(document);

            var root = generator.Generate(d);
        }
    }
}
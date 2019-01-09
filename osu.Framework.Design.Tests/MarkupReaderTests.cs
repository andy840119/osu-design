using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using osu.Framework.Design.Markup;

namespace osu.Framework.Design.Tests
{
    [TestClass]
    public class MarkupReaderTests
    {
        [TestMethod]
        public void TestSimpleMarkup()
        {
            var data = @"
<fx:Screen
    xmlns:fx=""osu.Framework:*""
    xmlns:local = ""osu.Framework.Design:*""
    Width=""500"">
    <fx:Box Position=""10,20"">
        <_RelativeSizeAxes>Both</_RelativeSizeAxes>
        <_Colour>#00ff00</_Colour>
    </fx:Box>
</fx:Screen>";

            var parser = new MarkupReader();

            DrawableData d;

            using (var reader = new StringReader(data))
                d = parser.Read(reader);

            Assert.IsTrue(parser.ImportedTypes.ContainsKey("osu.Framework:*"), "parser did not load fx lib");
            Assert.IsTrue(parser.ImportedTypes.ContainsKey("osu.Framework.Design:*"), "parser did not load local lib");
            Assert.AreEqual(typeof(Screens.Screen), d.DrawableType, "incorrectly parsed root type.");
            Assert.IsFalse(d.Attributes.ContainsKey("fx"), "fx namespace got serialized.");
            Assert.IsFalse(d.Attributes.ContainsKey("local"), "local namespace got serialized.");
            Assert.IsTrue(d.Attributes.ContainsKey("Width"), "width didn't get serialized.");
            Assert.AreEqual(500f, d.Attributes["Width"].Value, "width serialization is bad.");
            Assert.AreEqual(2, d.Children.Count, "more or less children than we fed?");
            Assert.AreEqual(1, d.Attributes.Count, "we should only have one attr");
            Assert.AreEqual(typeof(Graphics.Shapes.Box), d.Children[0].DrawableType, "first child not box");
            Assert.AreEqual(3, d.Children[0].Attributes.Count, "first child attrs not right");
            Assert.AreEqual(0, d.Children[0].Children.Count, "first child has children?");
            Assert.IsTrue(d.Children[0].Attributes.ContainsKey("Position"), "box position not serialized");
            Assert.AreEqual(new osuTK.Vector2(10, 20), d.Children[0].Attributes["Position"].Value, "box position is bad.");
            Assert.IsTrue(d.Children[0].Attributes.ContainsKey("RelativeSizeAxes"), "box axes not serialized");
            Assert.AreEqual(Graphics.Axes.Both, d.Children[0].Attributes["RelativeSizeAxes"].Value, "box axes are bad.");
            Assert.IsTrue(d.Children[0].Attributes.ContainsKey("Colour"), "box colour not serialized");
            Assert.AreEqual(Graphics.Colour.ColourInfo.SingleColour(new osuTK.Graphics.Color4(0, 255, 0, 255)), d.Children[0].Attributes["Colour"].Value, "box colour is bad.");
            Assert.AreEqual(typeof(DesignGame), d.Children[1].DrawableType, "second child not game");
            Assert.AreEqual(0, d.Children[1].Attributes.Count, "second child has attrs?");
            Assert.AreEqual(0, d.Children[1].Children.Count, "second child has children?");
        }
    }
}

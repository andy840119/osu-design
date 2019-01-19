using osu.Framework.Design.Markup;
using osu.Framework.Graphics.Shapes;
using Xunit;

namespace osu.Framework.Design.Tests
{
    public class DrawableDocumentTests
    {
        [Fact]
        public void TestLoad()
        {
            //Given
            const string content = @"
<Box
    xmlns=""osufx://osu.Framework/*.Graphics.*""
    xmlns:local=""osufx://osu.Framework.Design.Tests/*"">
</Box>";

            var doc = new DrawableDocument();

            //When
            doc.Load(content);

            //Then
            Assert.Equal(typeof(Box).FullName, doc.DrawableType.FullName);
        }
    }
}
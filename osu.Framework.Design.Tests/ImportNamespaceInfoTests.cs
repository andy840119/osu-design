using System;
using osu.Framework.Design.Markup;
using Xunit;

namespace osu.Framework.Design.Tests
{
    public class ImportNamespaceInfoTests
    {
        [Fact]
        public void TestParse()
        {
            //Given
            var import = "osufx://osu.Framework/*";

            //When
            var importInfo = ImportNamespaceInfo.Parse(import);

            //Then
            Assert.Equal("osu.Framework", importInfo.AssemblyName);
            Assert.Equal("*", importInfo.ImportPattern);
        }

        [Fact]
        public void TestParseInvalidScheme()
        {
            //Given
            var import = "http://osu.Framework/*";

            //Then
            Assert.Throws<NotSupportedException>(() => ImportNamespaceInfo.Parse(import));
        }

        [Fact]
        public void TestParseMalformedImport()
        {
            //Given
            var import = "osufx://osu./Framework/*";

            //Then
            Assert.Throws<FormatException>(() => ImportNamespaceInfo.Parse(import));
        }

        [Fact]
        public void TestEquals()
        {
            //Given
            var one = new ImportNamespaceInfo("osu.Framework", "*");
            var two = new ImportNamespaceInfo("osu.Framework.Design.Tests", "*");

            //Then
            Assert.True(one.Equals(one));
            Assert.False(one.Equals(two));
            Assert.True(two.Equals(two));
        }
    }
}
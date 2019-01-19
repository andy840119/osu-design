using System;
using osu.Framework.Design.Markup.ValueConverters;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osuTK;
using osuTK.Graphics;
using Xunit;

namespace osu.Framework.Design.Tests
{
    public class ValueConverterTests
    {
        [Fact]
        public void TestFactoryRegister()
        {
            //Give
            var conv = new StringConverter();

            //When
            ValueConverterFactory.Register(conv);

            //Then
            Assert.Equal(typeof(string), ValueConverterFactory.Get<string>().ConvertingType);
        }

        [Fact]
        public void TestFactoryRegisterNull()
        {
            //Given
            IValueConverter conv = null;

            //Then
            Assert.ThrowsAny<ArgumentNullException>(() => ValueConverterFactory.Register(conv));
        }

        [Fact]
        public void TestColor4Hex()
        {
            //Given
            var data = "#02b5fe";
            var value = new Color4(2, 181, 254, 255);

            //Then
            testAssert(data, value, false);
        }

        [Fact]
        public void TestColor4Rgb()
        {
            //Given
            var data = "rgb(2, 181, 254)";
            var value = new Color4(2, 181, 254, 255);

            //Then
            testAssert(data, value, false);
        }

        [Fact]
        public void TestColor4Rgba()
        {
            //Given
            var data = "rgba(2, 181, 254, 43)";
            var value = new Color4(2, 181, 254, 43);

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestColourInfoSingle()
        {
            //Given
            var data = "rgba(2, 181, 254, 255)";
            var value = ColourInfo.SingleColour(new Color4(2, 181, 254, 255));

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestColourInfoHorizontalGradient()
        {
            //Given
            var data = "gradient(horizontal, rgba(2, 181, 254, 255), rgba(217, 228, 51, 255))";
            var value = ColourInfo.GradientHorizontal(new Color4(2, 181, 254, 255), new Color4(217, 228, 51, 255));

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestColourInfoVerticalGradient()
        {
            //Given
            var data = "gradient(vertical, rgba(2, 181, 254, 234), rgba(217, 228, 51, 255))";
            var value = ColourInfo.GradientVertical(new Color4(2, 181, 254, 234), new Color4(217, 228, 51, 255));

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestColourInfoAllColors()
        {
            //Given
            var data = "rgba(2, 181, 254, 234), rgba(24, 25, 240, 255), rgba(217, 228, 51, 255), rgba(12, 34, 56, 78)";
            var value = new ColourInfo
            {
                TopLeft = new Color4(2, 181, 254, 234),
                TopRight = new Color4(24, 25, 240, 255),
                BottomRight = new Color4(217, 228, 51, 255),
                BottomLeft = new Color4(12, 34, 56, 78)
            };

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestEnum()
        {
            //Given
            var data = "Vertical";
            var value = Direction.Vertical;

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestBitwiseEnum()
        {
            //Given
            var data = "Both";
            var value = Axes.X | Axes.Y;

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestBool()
        {
            //Given
            var data = "true";
            var value = true;

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestByte()
        {
            //Given
            var data = "234";
            var value = (byte)234;

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestInt16()
        {
            //Given
            var data = "32234";
            var value = (short)32234;

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestInt32()
        {
            //Given
            var data = "2047483647";
            var value = 2047483647;

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestInt64()
        {
            //Given
            var data = "9221372036854875807";
            var value = 9221372036854875807;

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestSingle()
        {
            //Given
            var data = "1.401298E-45";
            var value = 1.401298E-45F;

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestDouble()
        {
            //Given
            var data = "4.94065645841247E-324";
            var value = 4.94065645841247E-324;

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestVector2()
        {
            //Given
            var data = "100, 200";
            var value = new Vector2(100, 200);

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestVector3()
        {
            //Given
            var data = "100, 200, 300";
            var value = new Vector3(100, 200, 300);

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestVector4()
        {
            //Given
            var data = "100, 200, 300, 400";
            var value = new Vector4(100, 200, 300, 400);

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestMarginPaddingSingle()
        {
            //Given
            var data = "200";
            var value = new MarginPadding(200);

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestMarginPaddingAll()
        {
            //Given
            var data = "10, 20, 30, 40";
            var value = new MarginPadding
            {
                Top = 10,
                Right = 20,
                Bottom = 30,
                Left = 40
            };

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestEdgeEffect()
        {
            //Given
            var data = "Glow, rgba(2, 181, 254, 234), 20, 0, (0, 20)";
            var value = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = new Color4(2, 181, 254, 234),
                Radius = 20,
                Roundness = 0,
                Offset = new Vector2(0, 20),
                Hollow = false
            };

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestBlendingSimple()
        {
            //Given
            var data = "Additive";
            var value = (BlendingParameters)BlendingMode.Additive;

            //Then
            testAssert(data, value);
        }

        [Fact]
        public void TestBlendingFull()
        {
            //Given
            var data = "Additive, alpha(Min), rgb(Add)";
            var value = new BlendingParameters
            {
                Mode = BlendingMode.Additive,
                AlphaEquation = BlendingEquation.Min,
                RGBEquation = BlendingEquation.Add
            };

            //Then
            testAssert(data, value);
        }

        static void testAssert<T>(string data, T value, bool givenExpectDataEqual = true)
        {
            //Given
            var conv = ValueConverterFactory.Get<T>();

            //When
            conv.Serialize(value, typeof(T), out var data2);
            conv.Deserialize(data, typeof(T), out var value2);

            //Then
            if (givenExpectDataEqual)
                Assert.Equal(data, data2);
            else
                Assert.NotEqual(data, data2);

            Assert.Equal(value, value2);
        }
    }
}
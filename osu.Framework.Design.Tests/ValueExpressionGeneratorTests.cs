using System;
using osu.Framework.Design.CodeGeneration.ValueExpressionGenerators;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osuTK;
using osuTK.Graphics;
using Xunit;

namespace osu.Framework.Design.Tests
{
    public class ValueExpressionGeneratorTests
    {
        [Fact]
        public void TestFactoryRegister()
        {
            //Give
            var conv = new StringExpressionGenerator();

            //When
            ValueExpressionGeneratorFactory.Register(conv);

            //Then
            Assert.Equal(typeof(string), ValueExpressionGeneratorFactory.Get<string>().GeneratingType);
        }

        [Fact]
        public void TestFactoryRegisterNull()
        {
            //Given
            IValueExpressionGenerator conv = null;

            //Then
            Assert.ThrowsAny<ArgumentNullException>(() => ValueExpressionGeneratorFactory.Register(conv));
        }

        [Fact]
        public void TestColor4()
        {
            //Given
            var code = "new osuTK.Graphics.Color4(0.1F, 0.2F, 0.3F, 0.4F)";
            var value = new Color4(0.1f, 0.2f, 0.3f, 0.4f);

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestColourInfoSingle()
        {
            //Given
            var code = @"osu.Framework.Graphics.Colour.ColourInfo.SingleColour(new osuTK.Graphics.Color4(0.1F, 0.2F, 0.3F, 0.4F))";
            var value = ColourInfo.SingleColour(new Color4(0.1F, 0.2F, 0.3F, 0.4F));

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestColourInfoGradientVertical()
        {
            //Given
            var code = @"osu.Framework.Graphics.Colour.ColourInfo.GradientVertical(new osuTK.Graphics.Color4(0.1F, 0.2F, 0.3F, 0.4F), new osuTK.Graphics.Color4(0.4F, 0.3F, 0.2F, 0.1F))";
            var value = ColourInfo.GradientVertical(new Color4(0.1F, 0.2F, 0.3F, 0.4F), new osuTK.Graphics.Color4(0.4F, 0.3F, 0.2F, 0.1F));

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestColourInfoGradientHorizontal()
        {
            //Given
            var code = @"osu.Framework.Graphics.Colour.ColourInfo.GradientHorizontal(new osuTK.Graphics.Color4(0.1F, 0.2F, 0.3F, 0.4F), new osuTK.Graphics.Color4(0.4F, 0.3F, 0.2F, 0.1F))";
            var value = ColourInfo.GradientHorizontal(new Color4(0.1F, 0.2F, 0.3F, 0.4F), new osuTK.Graphics.Color4(0.4F, 0.3F, 0.2F, 0.1F));

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestColourInfoFull()
        {
            //Given
            var code =
@"new osu.Framework.Graphics.Colour.ColourInfo
{
    TopLeft = new osuTK.Graphics.Color4(0.1F, 0.2F, 0.3F, 0.4F),
    TopRight = new osuTK.Graphics.Color4(0.4F, 0.3F, 0.2F, 0.1F),
    BottomRight = new osuTK.Graphics.Color4(0.1F, 0.2F, 0.3F, 0.4F),
    BottomLeft = new osuTK.Graphics.Color4(0.4F, 0.3F, 0.2F, 0.1F)
}";
            var value = new ColourInfo
            {
                TopLeft = new Color4(0.1F, 0.2F, 0.3F, 0.4F),
                TopRight = new Color4(0.4F, 0.3F, 0.2F, 0.1F),
                BottomRight = new Color4(0.1F, 0.2F, 0.3F, 0.4F),
                BottomLeft = new Color4(0.4F, 0.3F, 0.2F, 0.1F)
            };

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestEnum()
        {
            //Given
            var code = "osu.Framework.Graphics.Direction.Vertical";
            var value = Direction.Vertical;

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestBitwiseEnum()
        {
            //Given
            var code = "osu.Framework.Graphics.Axes.Both";
            var value = Axes.X | Axes.Y;

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestBool()
        {
            //Given
            var code = "true";
            var value = true;

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestByte()
        {
            //Given
            var code = "234";
            var value = (byte)234;

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestInt16()
        {
            //Given
            var code = "32234";
            var value = (short)32234;

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestInt32()
        {
            //Given
            var code = "2047483647";
            var value = 2047483647;

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestInt64()
        {
            //Given
            var code = "9221372036854875807L";
            var value = 9221372036854875807;

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestSingle()
        {
            //Given
            var code = "1.401298E-45F";
            var value = 1.401298E-45F;

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestDouble()
        {
            //Given
            var code = "4.94065645841247E-324";
            var value = 4.94065645841247E-324;

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestVector2()
        {
            //Given
            var code = "new osuTK.Vector2(100F, 200F)";
            var value = new Vector2(100, 200);

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestVector3()
        {
            //Given
            var code = "new osuTK.Vector3(100F, 200F, 300F)";
            var value = new Vector3(100, 200, 300);

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestVector4()
        {
            //Given
            var code = "new osuTK.Vector4(100F, 200F, 300F, 400F)";
            var value = new Vector4(100, 200, 300, 400);

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestMarginPaddingSingle()
        {
            //Given
            var code = "new osu.Framework.Graphics.MarginPadding(200F)";
            var value = new MarginPadding(200);

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestMarginPaddingAll()
        {
            //Given
            var code =
@"new osu.Framework.Graphics.MarginPadding
{
    Top = 10F,
    Right = 20F,
    Bottom = 30F,
    Left = 40F
}";
            var value = new MarginPadding
            {
                Top = 10,
                Right = 20,
                Bottom = 30,
                Left = 40
            };

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestEdgeEffect()
        {
            //Given
            var code =
@"new osu.Framework.Graphics.Containers.EdgeEffectParameters
{
    Type = osu.Framework.Graphics.Containers.EdgeEffectType.Glow,
    Colour = new osuTK.Graphics.Color4(0.1F, 0.2F, 0.3F, 0.4F),
    Radius = 20F,
    Roundness = 30F,
    Offset = new osuTK.Vector2(0F, 20F),
    Hollow = true
}";
            var value = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = new Color4(0.1f, 0.2f, 0.3f, 0.4f),
                Radius = 20,
                Roundness = 30,
                Offset = new Vector2(0, 20),
                Hollow = true
            };

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestBlendingSimple()
        {
            //Given
            var code =
@"new osu.Framework.Graphics.BlendingParameters
{
    Mode = osu.Framework.Graphics.BlendingMode.Additive
}";
            var value = (BlendingParameters)BlendingMode.Additive;

            //Then
            testAssert(code, value);
        }

        [Fact]
        public void TestBlendingFull()
        {
            //Given
            var data =
@"new osu.Framework.Graphics.BlendingParameters
{
    Mode = osu.Framework.Graphics.BlendingMode.Additive,
    AlphaEquation = osu.Framework.Graphics.BlendingMode.Min,
    RGBEquation = osu.Framework.Graphics.BlendingMode.Add
}";
            var value = new BlendingParameters
            {
                Mode = BlendingMode.Additive,
                AlphaEquation = BlendingEquation.Min,
                RGBEquation = BlendingEquation.Add
            };

            //Then
            testAssert(data, value);
        }

        static void testAssert<T>(string code, T value)
        {
            //Given
            var conv = ValueExpressionGeneratorFactory.Get<T>();

            //When
            var syntax = conv.GenerateSyntax(value, typeof(T));
            var generated = syntax.ToFullString();

            //Then
            Console.WriteLine(generated);
            Assert.Equal(code.RemoveAllSpaces(), generated);
        }
    }
}
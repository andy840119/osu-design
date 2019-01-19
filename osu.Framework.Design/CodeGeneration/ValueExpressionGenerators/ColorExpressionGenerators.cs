using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using osu.Framework.Graphics.Colour;
using osuTK.Graphics;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace osu.Framework.Design.CodeGeneration.ValueExpressionGenerators
{
    public class Color4ExpressionGenerator : IValueExpressionGenerator
    {
        public Type GeneratingType => typeof(Color4);

        public ExpressionSyntax GenerateSyntax(object value, Type type) => GenerateSyntax((Color4)value);
        public static ExpressionSyntax GenerateSyntax(Color4 c)
        {
            return ObjectCreationExpression(
                type: ParseTypeName(typeof(Color4).FullName),
                argumentList: ArgumentList(SeparatedList<ArgumentSyntax>(new[]
                {
                    Argument(LiteralExpression(
                        kind: SyntaxKind.NumericLiteralExpression,
                        token: Literal(c.R)
                    )),
                    Argument(LiteralExpression(
                        kind: SyntaxKind.NumericLiteralExpression,
                        token: Literal(c.G)
                    )),
                    Argument(LiteralExpression(
                        kind: SyntaxKind.NumericLiteralExpression,
                        token: Literal(c.B)
                    )),
                    Argument(LiteralExpression(
                        kind: SyntaxKind.NumericLiteralExpression,
                        token: Literal(c.A)
                    ))
                })),
                initializer: null
            );
        }
    }

    public class SRGBColourExpressionGenerator : IValueExpressionGenerator
    {
        public Type GeneratingType => typeof(SRGBColour);

        public ExpressionSyntax GenerateSyntax(object value, Type type)
        {
            var c = (SRGBColour)value;

            // SRGBColour has implicit conversion to Color4
            return Color4ExpressionGenerator.GenerateSyntax(c);
        }
    }

    public class ColourInfoExpressionGenerator : IValueExpressionGenerator
    {
        public Type GeneratingType => typeof(ColourInfo);

        public ExpressionSyntax GenerateSyntax(object value, Type type) => GenerateSyntax((ColourInfo)value);
        public static ExpressionSyntax GenerateSyntax(ColourInfo c)
        {
            // SingleColour
            if (c.TopLeft.Equals(c.TopRight) && c.TopLeft.Equals(c.BottomRight) && c.TopLeft.Equals(c.BottomLeft))
                return InvocationExpression(
                    expression: IdentifierName($"{typeof(ColourInfo).FullName}.{nameof(ColourInfo.SingleColour)}"),
                    argumentList: ArgumentList(SingletonSeparatedList<ArgumentSyntax>(
                        Argument(Color4ExpressionGenerator.GenerateSyntax(c.TopLeft))
                    ))
                );

            // GradientHorizontal
            if (c.TopLeft.Equals(c.BottomLeft) && c.TopRight.Equals(c.BottomRight) && !c.TopLeft.Equals(c.TopRight))
                return InvocationExpression(
                    expression: IdentifierName($"{typeof(ColourInfo).FullName}.{nameof(ColourInfo.GradientHorizontal)}"),
                    argumentList: ArgumentList(SeparatedList<ArgumentSyntax>(new[]
                    {
                        Argument(Color4ExpressionGenerator.GenerateSyntax(c.TopLeft)),
                        Argument(Color4ExpressionGenerator.GenerateSyntax(c.TopRight))
                    }))
                );

            // GradientVertical
            if (c.TopLeft.Equals(c.TopRight) && c.BottomLeft.Equals(c.BottomRight) && !c.TopLeft.Equals(c.BottomLeft))
                return InvocationExpression(
                    expression: IdentifierName($"{typeof(ColourInfo).FullName}.{nameof(ColourInfo.GradientVertical)}"),
                    argumentList: ArgumentList(SeparatedList<ArgumentSyntax>(new[]
                    {
                        Argument(Color4ExpressionGenerator.GenerateSyntax(c.TopLeft)),
                        Argument(Color4ExpressionGenerator.GenerateSyntax(c.BottomLeft))
                    }))
                );

            // Full initialization
            return ObjectCreationExpression(
                type: ParseTypeName(typeof(ColourInfo).FullName),
                argumentList: null,
                initializer: InitializerExpression(
                    kind: SyntaxKind.ObjectInitializerExpression,
                    expressions: SeparatedList(new ExpressionSyntax[]
                    {
                        AssignmentExpression(
                            kind: SyntaxKind.SimpleAssignmentExpression,
                            left: IdentifierName(nameof(ColourInfo.TopLeft)),
                            right: Color4ExpressionGenerator.GenerateSyntax(c.TopLeft)
                        ),
                        AssignmentExpression(
                            kind: SyntaxKind.SimpleAssignmentExpression,
                            left: IdentifierName(nameof(ColourInfo.TopRight)),
                            right: Color4ExpressionGenerator.GenerateSyntax(c.TopRight)
                        ),
                        AssignmentExpression(
                            kind: SyntaxKind.SimpleAssignmentExpression,
                            left: IdentifierName(nameof(ColourInfo.BottomRight)),
                            right: Color4ExpressionGenerator.GenerateSyntax(c.BottomRight)
                        ),
                        AssignmentExpression(
                            kind: SyntaxKind.SimpleAssignmentExpression,
                            left: IdentifierName(nameof(ColourInfo.BottomLeft)),
                            right: Color4ExpressionGenerator.GenerateSyntax(c.BottomLeft)
                        )
                    })
                )
            );
        }
    }
}
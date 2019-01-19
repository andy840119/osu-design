using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using osuTK;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace osu.Framework.Design.CodeGeneration.ValueExpressionGenerators
{
    public class Vector2ExpressionGenerator : IValueExpressionGenerator
    {
        public Type GeneratingType => typeof(Vector2);

        public ExpressionSyntax GenerateSyntax(object value, Type type) => GenerateSyntax((Vector2)value);
        public static ExpressionSyntax GenerateSyntax(Vector2 v)
        {
            return ObjectCreationExpression(
                type: ParseTypeName(typeof(Vector2).FullName),
                argumentList: ArgumentList(SeparatedList<ArgumentSyntax>(new[]
                {
                    Argument(LiteralExpression(
                        kind: SyntaxKind.NumericLiteralExpression,
                        token: Literal(v.X)
                    )),
                    Argument(LiteralExpression(
                        kind: SyntaxKind.NumericLiteralExpression,
                        token: Literal(v.Y)
                    ))
                })),
                initializer: null
            );
        }
    }

    public class Vector3ExpressionGenerator : IValueExpressionGenerator
    {
        public Type GeneratingType => typeof(Vector3);

        public ExpressionSyntax GenerateSyntax(object value, Type type) => GenerateSyntax((Vector3)value);
        public static ExpressionSyntax GenerateSyntax(Vector3 v)
        {
            return ObjectCreationExpression(
                type: ParseTypeName(typeof(Vector3).FullName),
                argumentList: ArgumentList(SeparatedList<ArgumentSyntax>(new[]
                {
                    Argument(LiteralExpression(
                        kind: SyntaxKind.NumericLiteralExpression,
                        token: Literal(v.X)
                    )),
                    Argument(LiteralExpression(
                        kind: SyntaxKind.NumericLiteralExpression,
                        token: Literal(v.Y)
                    )),
                    Argument(LiteralExpression(
                        kind: SyntaxKind.NumericLiteralExpression,
                        token: Literal(v.Z)
                    ))
                })),
                initializer: null
            );
        }
    }

    public class Vector4ExpressionGenerator : IValueExpressionGenerator
    {
        public Type GeneratingType => typeof(Vector4);

        public ExpressionSyntax GenerateSyntax(object value, Type type) => GenerateSyntax((Vector4)value);
        public static ExpressionSyntax GenerateSyntax(Vector4 v)
        {
            return ObjectCreationExpression(
                type: ParseTypeName(typeof(Vector4).FullName),
                argumentList: ArgumentList(SeparatedList<ArgumentSyntax>(new[]
                {
                    Argument(LiteralExpression(
                        kind: SyntaxKind.NumericLiteralExpression,
                        token: Literal(v.X)
                    )),
                    Argument(LiteralExpression(
                        kind: SyntaxKind.NumericLiteralExpression,
                        token: Literal(v.Y)
                    )),
                    Argument(LiteralExpression(
                        kind: SyntaxKind.NumericLiteralExpression,
                        token: Literal(v.Z)
                    )),
                    Argument(LiteralExpression(
                        kind: SyntaxKind.NumericLiteralExpression,
                        token: Literal(v.W)
                    ))
                })),
                initializer: null
            );
        }
    }
}
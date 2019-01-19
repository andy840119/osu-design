using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using osu.Framework.Graphics;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace osu.Framework.Design.CodeGeneration.ValueExpressionGenerators
{
    public class MarginPaddingExpressionGenerator : IValueExpressionGenerator
    {
        public Type GeneratingType => typeof(MarginPadding);

        public ExpressionSyntax GenerateSyntax(object value, Type type) => GenerateSyntax((MarginPadding)value);
        public static ExpressionSyntax GenerateSyntax(MarginPadding m)
        {
            if (m.Top == m.Left && m.Top == m.Right && m.Top == m.Bottom)
                return ObjectCreationExpression(
                    type: ParseTypeName(typeof(MarginPadding).FullName),
                    argumentList: ArgumentList(SingletonSeparatedList<ArgumentSyntax>(
                        Argument(LiteralExpression(
                            kind: SyntaxKind.NumericLiteralExpression,
                            token: Literal(m.Top)
                        ))
                    )),
                    initializer: null
                );

            return ObjectCreationExpression(
                type: ParseTypeName(typeof(MarginPadding).FullName),
                argumentList: null,
                initializer: InitializerExpression(
                    kind: SyntaxKind.ObjectInitializerExpression,
                    expressions: SeparatedList<ExpressionSyntax>(new[]
                    {
                        AssignmentExpression(
                            kind: SyntaxKind.SimpleAssignmentExpression,
                            left: IdentifierName(nameof(MarginPadding.Top)),
                            right: LiteralExpression(
                                kind: SyntaxKind.NumericLiteralExpression,
                                token: Literal(m.Top)
                            )
                        ),
                        AssignmentExpression(
                            kind: SyntaxKind.SimpleAssignmentExpression,
                            left: IdentifierName(nameof(MarginPadding.Right)),
                            right: LiteralExpression(
                                kind: SyntaxKind.NumericLiteralExpression,
                                token: Literal(m.Right)
                            )
                        ),
                        AssignmentExpression(
                            kind: SyntaxKind.SimpleAssignmentExpression,
                            left: IdentifierName(nameof(MarginPadding.Bottom)),
                            right: LiteralExpression(
                                kind: SyntaxKind.NumericLiteralExpression,
                                token: Literal(m.Bottom)
                            )
                        ),
                        AssignmentExpression(
                            kind: SyntaxKind.SimpleAssignmentExpression,
                            left: IdentifierName(nameof(MarginPadding.Left)),
                            right: LiteralExpression(
                                kind: SyntaxKind.NumericLiteralExpression,
                                token: Literal(m.Left)
                            )
                        )
                    })
                )
            );
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using osu.Framework.Graphics.Containers;
using osuTK;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace osu.Framework.Design.CodeGeneration.ValueExpressionGenerators
{
    public class EdgeEffectExpressionGenerator : IValueExpressionGenerator
    {
        public Type GeneratingType => typeof(EdgeEffectParameters);

        public ExpressionSyntax GenerateSyntax(object value, Type type) => GenerateSyntax((EdgeEffectParameters)value);
        public static ExpressionSyntax GenerateSyntax(EdgeEffectParameters p)
        {
            var list = new List<ExpressionSyntax>();
            list.Add(AssignmentExpression(
                kind: SyntaxKind.SimpleAssignmentExpression,
                left: IdentifierName(nameof(EdgeEffectParameters.Type)),
                right: ParseTypeName($"{typeof(EdgeEffectType).FullName}.{p.Type}")
            ));
            list.Add(AssignmentExpression(
                kind: SyntaxKind.SimpleAssignmentExpression,
                left: IdentifierName(nameof(EdgeEffectParameters.Colour)),
                right: Color4ExpressionGenerator.GenerateSyntax(p.Colour)
            ));

            if (p.Radius != default)
                list.Add(AssignmentExpression(
                    kind: SyntaxKind.SimpleAssignmentExpression,
                    left: IdentifierName(nameof(EdgeEffectParameters.Radius)),
                    right: LiteralExpression(
                        kind: SyntaxKind.NumericLiteralExpression,
                        token: Literal(p.Radius)
                    )
                ));
            if (p.Roundness != default)
                list.Add(AssignmentExpression(
                    kind: SyntaxKind.SimpleAssignmentExpression,
                    left: IdentifierName(nameof(EdgeEffectParameters.Roundness)),
                    right: LiteralExpression(
                        kind: SyntaxKind.NumericLiteralExpression,
                        token: Literal(p.Roundness)
                    )
                ));
            if (p.Offset != default)
                list.Add(AssignmentExpression(
                    kind: SyntaxKind.SimpleAssignmentExpression,
                    left: IdentifierName(nameof(EdgeEffectParameters.Offset)),
                    right: Vector2ExpressionGenerator.GenerateSyntax(p.Offset)
                ));
            if (p.Hollow)
                list.Add(AssignmentExpression(
                    kind: SyntaxKind.SimpleAssignmentExpression,
                    left: IdentifierName(nameof(EdgeEffectParameters.Hollow)),
                    right: LiteralExpression(
                        kind: SyntaxKind.TrueLiteralExpression
                    )
                ));

            return ObjectCreationExpression(
                type: ParseTypeName(typeof(EdgeEffectParameters).FullName),
                argumentList: null,
                initializer: InitializerExpression(
                    kind: SyntaxKind.ObjectInitializerExpression,
                    expressions: SeparatedList(list)
                )
            );
        }
    }
}
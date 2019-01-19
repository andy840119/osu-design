using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using osu.Framework.Graphics;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace osu.Framework.Design.CodeGeneration.ValueExpressionGenerators
{
    public class BlendingExpressionGenerator : IValueExpressionGenerator
    {
        public Type GeneratingType => typeof(BlendingParameters);

        public ExpressionSyntax GenerateSyntax(object value, Type type) => GenerateSyntax((BlendingParameters)value);
        public static ExpressionSyntax GenerateSyntax(BlendingParameters b)
        {
            var list = new List<ExpressionSyntax>();

            if (b.Mode != default)
                list.Add(AssignmentExpression(
                    kind: SyntaxKind.SimpleAssignmentExpression,
                    left: IdentifierName(nameof(BlendingParameters.Mode)),
                    right: ParseTypeName($"{typeof(BlendingMode).FullName}.{b.Mode}")
                ));
            if (b.AlphaEquation != default)
                list.Add(AssignmentExpression(
                    kind: SyntaxKind.SimpleAssignmentExpression,
                    left: IdentifierName(nameof(BlendingParameters.AlphaEquation)),
                    right: ParseTypeName($"{typeof(BlendingMode).FullName}.{b.AlphaEquation}")
                ));
            if (b.RGBEquation != default)
                list.Add(AssignmentExpression(
                    kind: SyntaxKind.SimpleAssignmentExpression,
                    left: IdentifierName(nameof(BlendingParameters.RGBEquation)),
                    right: ParseTypeName($"{typeof(BlendingMode).FullName}.{b.RGBEquation}")
                ));

            return ObjectCreationExpression(
                type: ParseTypeName(typeof(BlendingParameters).FullName),
                argumentList: null,
                initializer: InitializerExpression(
                    kind: SyntaxKind.ObjectInitializerExpression,
                    expressions: SeparatedList(list)
                )
            );
        }
    }
}
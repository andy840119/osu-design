using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace osu.Framework.Design.CodeGeneration.ValueExpressionGenerators
{
    public class StringExpressionGenerator : IValueExpressionGenerator
    {
        public Type GeneratingType => typeof(string);

        public ExpressionSyntax GenerateSyntax(object value, Type type) => LiteralExpression(
            SyntaxKind.StringLiteralExpression,
            Literal(value.ToString())
        );
    }
}
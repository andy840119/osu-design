using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using osu.Framework.Localisation;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace osu.Framework.Design.CodeGeneration.ValueExpressionGenerators
{
    public class LocalisedStringExpressionGenerator : IValueExpressionGenerator
    {
        public Type GeneratingType => typeof(LocalisedString);

        public ExpressionSyntax GenerateSyntax(object value, Type type) => LiteralExpression(
            SyntaxKind.StringLiteralExpression,
            Literal(value.ToString())
        );
    }
}
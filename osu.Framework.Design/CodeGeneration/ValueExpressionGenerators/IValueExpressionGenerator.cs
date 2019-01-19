using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace osu.Framework.Design.CodeGeneration.ValueExpressionGenerators
{
    public interface IValueExpressionGenerator
    {
        Type GeneratingType { get; }

        ExpressionSyntax GenerateSyntax(object value, Type type);
    }
}
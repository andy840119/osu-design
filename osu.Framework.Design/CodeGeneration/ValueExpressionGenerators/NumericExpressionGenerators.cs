using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace osu.Framework.Design.CodeGeneration.ValueExpressionGenerators
{
    public class BoolExpressionGenerator : IValueExpressionGenerator
    {
        public Type GeneratingType => typeof(bool);

        public ExpressionSyntax GenerateSyntax(object value, Type type) => LiteralExpression(
            kind: (bool)value ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression
        );
    }

    public class ByteExpressionGenerator : IValueExpressionGenerator
    {
        public Type GeneratingType => typeof(byte);

        public ExpressionSyntax GenerateSyntax(object value, Type type) => LiteralExpression(
            kind: SyntaxKind.NumericLiteralExpression,
            token: Literal((byte)value)
        );
    }

    public class Int16ExpressionGenerator : IValueExpressionGenerator
    {
        public Type GeneratingType => typeof(short);

        public ExpressionSyntax GenerateSyntax(object value, Type type) => LiteralExpression(
            kind: SyntaxKind.NumericLiteralExpression,
            token: Literal((short)value)
        );
    }

    public class Int32ExpressionGenerator : IValueExpressionGenerator
    {
        public Type GeneratingType => typeof(int);

        public ExpressionSyntax GenerateSyntax(object value, Type type) => LiteralExpression(
            kind: SyntaxKind.NumericLiteralExpression,
            token: Literal((int)value)
        );
    }

    public class Int64ExpressionGenerator : IValueExpressionGenerator
    {
        public Type GeneratingType => typeof(long);

        public ExpressionSyntax GenerateSyntax(object value, Type type) => LiteralExpression(
            kind: SyntaxKind.NumericLiteralExpression,
            token: Literal((long)value)
        );
    }

    public class SingleExpressionGenerator : IValueExpressionGenerator
    {
        public Type GeneratingType => typeof(float);

        public ExpressionSyntax GenerateSyntax(object value, Type type) => LiteralExpression(
            kind: SyntaxKind.NumericLiteralExpression,
            token: Literal((float)value)
        );
    }

    public class DoubleExpressionGenerator : IValueExpressionGenerator
    {
        public Type GeneratingType => typeof(double);

        public ExpressionSyntax GenerateSyntax(object value, Type type) => LiteralExpression(
            kind: SyntaxKind.NumericLiteralExpression,
            token: Literal((double)value)
        );
    }
}
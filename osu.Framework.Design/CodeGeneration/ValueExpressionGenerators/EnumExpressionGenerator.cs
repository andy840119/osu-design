using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace osu.Framework.Design.CodeGeneration.ValueExpressionGenerators
{
    public class EnumExpressionGenerator : IValueExpressionGenerator
    {
        public Type GeneratingType => typeof(Enum);

        public ExpressionSyntax GenerateSyntax(object value, Type type)
        {
            // Splitting by comma to handle bitwise enums
            var components = value
                .ToString()
                .SplitByComma()
                .ToArray();

            var expression = (ExpressionSyntax)ParseTypeName($"{type.FullName}.{components[0]}");

            expression = components.Skip(1).Aggregate(expression, (exp, flag) => BinaryExpression(
                kind: SyntaxKind.BitwiseOrExpression,
                left: exp,
                right: ParseTypeName($"{type.FullName}.{flag}")
            ));

            return expression;
        }

        static IEnumerable<Enum> getFlags(Enum input, Type type)
        {
            if (!type.IsDefined(typeof(FlagsAttribute), false))
            {
                yield return input;
                yield break;
            }

            foreach (Enum value in Enum.GetValues(type))
                if (input.HasFlag(value))
                    yield return value;
        }
    }
}
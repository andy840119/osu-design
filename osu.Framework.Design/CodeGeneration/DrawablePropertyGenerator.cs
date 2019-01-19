using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using osu.Framework.Design.Markup;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace osu.Framework.Design.CodeGeneration
{
    public static class DrawablePropertyGenerator
    {
        public static AssignmentExpressionSyntax GenerateInitializerSyntax(this DrawableProperty prop)
        {
            if (prop is EmbeddedDrawableProperty embedded)
                return embedded.GenerateInitializerSyntax();

            if (prop is LocalBoundDrawableProperty localBound)
                return localBound.GenerateInitializerSyntax();

            if (prop is ConfigBoundDrawableProperty configBound)
                return configBound.GenerateInitializerSyntax();

            return null;
        }

        public static AssignmentExpressionSyntax GenerateInitializerSyntax(this EmbeddedDrawableProperty prop)
        {
            return AssignmentExpression(
                kind: SyntaxKind.SimpleAssignmentExpression,
                left: IdentifierName(prop.Name),
                right: prop.ExpressionGenerator.GenerateSyntax(prop.ParsedValue, prop.PropertyInfo.PropertyType)
            );
        }

        public static AssignmentExpressionSyntax GenerateInitializerSyntax(this LocalBoundDrawableProperty prop)
        {
            return null;
        }

        public static AssignmentExpressionSyntax GenerateInitializerSyntax(this ConfigBoundDrawableProperty prop)
        {
            return null;
        }
    }
}
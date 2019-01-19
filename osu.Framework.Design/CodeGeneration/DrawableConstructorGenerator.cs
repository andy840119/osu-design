using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using osu.Framework.Allocation;
using osu.Framework.Design.Markup;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace osu.Framework.Design.CodeGeneration
{
    public static class DrawableConstructorGenerator
    {
        public static ConstructorDeclarationSyntax GenerateConstructorSyntax(this DrawableNode node)
        {
            return ConstructorDeclaration(
                attributeLists: List<AttributeListSyntax>(),
                modifiers: TokenList(
                    Token(SyntaxKind.PublicKeyword)
                ),
                identifier: Identifier(node.GivenName),
                parameterList: ParameterList(),
                initializer: null,
                body: GenerateBody(node)
            );
        }

        public static BlockSyntax GenerateBody(DrawableNode node)
        {
            return Block(List<StatementSyntax>(
                GenerateEmbeddedPropertyInitializers(node)
            ));
        }

        public static IEnumerable<StatementSyntax> GenerateEmbeddedPropertyInitializers(DrawableNode node)
        {
            foreach (var property in node.Properties.OfType<EmbeddedDrawableProperty>())
                yield return ExpressionStatement(property.GenerateInitializerSyntax());
        }
    }
}
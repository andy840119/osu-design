using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using osu.Framework.Allocation;
using osu.Framework.Design.Markup;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace osu.Framework.Design.CodeGeneration
{
    public static class DrawableDescendantPropertyGenerator
    {
        public static IEnumerable<PropertyDeclarationSyntax> GeneratePropertySyntaxesOfDescendants(this DrawableNode node)
        {
            foreach (var d in EnumerateDescendantsWithName(node))
                yield return PropertyDeclaration(
                    attributeLists: List<AttributeListSyntax>(),
                    modifiers: TokenList(
                        Token(SyntaxKind.PublicKeyword)
                    ),
                    type: ParseTypeName(d.DrawableType.FullName),
                    explicitInterfaceSpecifier: null,
                    identifier: Identifier(d.GivenName),
                    accessorList: AccessorList(List<AccessorDeclarationSyntax>(new[]
                    {
                        AccessorDeclaration(
                            kind: SyntaxKind.GetAccessorDeclaration,
                            attributeLists: List<AttributeListSyntax>(),
                            modifiers: TokenList(),
                            keyword: Token(SyntaxKind.GetKeyword),
                            body: null,
                            semicolonToken: Token(SyntaxKind.SemicolonToken)
                        ),
                        AccessorDeclaration(
                            kind: SyntaxKind.SetAccessorDeclaration,
                            attributeLists: List<AttributeListSyntax>(),
                            modifiers: TokenList(
                                Token(SyntaxKind.PrivateKeyword)
                            ),
                            keyword: Token(SyntaxKind.SetKeyword),
                            body: null,
                            semicolonToken: Token(SyntaxKind.SemicolonToken)
                        )
                    }))
                );
        }

        public static IEnumerable<DrawableNode> EnumerateDescendantsWithName(DrawableNode node) =>
            node.RecursiveSelect(n => n).Where(n => n.IsNameSpecified);
    }
}
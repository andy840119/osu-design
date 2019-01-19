using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using osu.Framework.Allocation;
using osu.Framework.Design.Markup;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace osu.Framework.Design.CodeGeneration
{
    public static class DrawableLoaderGenerator
    {
        public static MethodDeclarationSyntax GenerateBackgroundLoaderSyntax(this DrawableNode node)
        {
            return MethodDeclaration(
                attributeLists: SingletonList<AttributeListSyntax>(AttributeList(SingletonSeparatedList<AttributeSyntax>(
                    Attribute(ParseName(typeof(BackgroundDependencyLoaderAttribute).FullName))
                ))),
                modifiers: TokenList(
                    Token(SyntaxKind.PrivateKeyword)
                ),
                returnType: PredefinedType(
                    Token(SyntaxKind.VoidKeyword)
                ),
                explicitInterfaceSpecifier: null,
                identifier: Identifier("load"),
                typeParameterList: null,
                parameterList: ParameterList(),
                constraintClauses: List<TypeParameterConstraintClauseSyntax>(),
                body: GenerateBody(node),
                expressionBody: null
            );
        }

        public static BlockSyntax GenerateBody(DrawableNode node)
        {
            var list = new List<StatementSyntax>();
            list.AddRange(GenerateLocalBoundPropertyInitializers(node));
            list.Add(ExpressionStatement(GenerateChildrenInitializers(node)));

            return Block(List<StatementSyntax>(list));
        }

        public static IEnumerable<StatementSyntax> GenerateLocalBoundPropertyInitializers(DrawableNode node)
        {
            foreach (var property in node.Properties.OfType<LocalBoundDrawableProperty>())
                yield return ExpressionStatement(property.GenerateInitializerSyntax());
        }

        public static ExpressionSyntax GenerateChildrenInitializers(DrawableNode node)
        {
            return AssignmentExpression(
                kind: SyntaxKind.SimpleAssignmentExpression,
                left: IdentifierName(nameof(Container.Children)),
                right: ArrayCreationExpression(
                    type: ArrayType(
                        elementType: ParseTypeName(typeof(Drawable).FullName),
                        rankSpecifiers: SingletonList<ArrayRankSpecifierSyntax>(ArrayRankSpecifier())
                    ),
                    initializer: InitializerExpression(
                        kind: SyntaxKind.ArrayInitializerExpression,
                        expressions: SeparatedList<ExpressionSyntax>(
                            node.Select(GenerateChildInitializer)
                        )
                    )
                )
            );
        }

        public static ExpressionSyntax GenerateChildInitializer(DrawableNode node)
        {
            var list = new List<ExpressionSyntax>();
            list.AddRange(node.Properties.Select(DrawablePropertyGenerator.GenerateInitializerSyntax));

            if (node.IsContainer && node.Any())
                list.Add(GenerateChildrenInitializers(node));

            var expression = (ExpressionSyntax)ObjectCreationExpression(
                type: ParseTypeName(node.DrawableType.FullName),
                argumentList: null,
                initializer: InitializerExpression(
                    kind: SyntaxKind.ObjectInitializerExpression,
                    expressions: SeparatedList<ExpressionSyntax>(list)
                )
            );

            if (node.IsNameSpecified)
                expression = AssignmentExpression(
                    kind: SyntaxKind.SimpleAssignmentExpression,
                    left: IdentifierName(node.GivenName),
                    right: expression
                );

            return expression;
        }
    }
}
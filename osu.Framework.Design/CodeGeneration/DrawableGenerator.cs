using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using osu.Framework.Design.Markup;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace osu.Framework.Design.CodeGeneration
{
    public static class DrawableGenerator
    {
        public static void GenerateClass(
            this DrawableNode node,
            TextWriter writer,
            string indentation = "    ",
            string lineSeparator = null
        )
        {
            var syntax = node.GenerateClassSyntax().NormalizeWhitespace(
                indentation: indentation,
                eol: lineSeparator ?? Environment.NewLine
            );

            syntax.WriteTo(writer);
        }

        public static ClassDeclarationSyntax GenerateClassSyntax(this DrawableNode node)
        {
            return ClassDeclaration(
                attributeLists: List<AttributeListSyntax>(),
                modifiers: TokenList(
                    Token(SyntaxKind.PublicKeyword)
                ),
                identifier: Identifier(node.GivenName),
                typeParameterList: null,
                baseList: BaseList(SingletonSeparatedList<BaseTypeSyntax>(
                    SimpleBaseType(ParseTypeName(node.DrawableType.FullName))
                )),
                constraintClauses: List<TypeParameterConstraintClauseSyntax>(),
                members: GenerateMembers(node)
            );
        }

        public static SyntaxList<MemberDeclarationSyntax> GenerateMembers(DrawableNode node)
        {
            var list = new List<MemberDeclarationSyntax>();
            list.AddRange(node.GeneratePropertySyntaxesOfDescendants());
            list.Add(node.GenerateConstructorSyntax());
            list.Add(node.GenerateBackgroundLoaderSyntax());

            return List<MemberDeclarationSyntax>(list);
        }
    }
}
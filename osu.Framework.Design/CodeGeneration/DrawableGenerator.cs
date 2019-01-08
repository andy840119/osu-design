using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using osu.Framework.Design.Markup;
using osu.Framework.Design.Markup.Converters;
using osu.Framework.Graphics;

namespace osu.Framework.Design.CodeGeneration
{
    public static class DrawableGenerator
    {
        public static SyntaxNode Generate(DrawableData d, SyntaxGenerator g)
        {
            return g.ClassDeclaration(
                name: d.Id,
                accessibility: Accessibility.Public,
                modifiers: DeclarationModifiers.Partial,
                baseType: g.IdentifierName(d.DrawableType.FullName),
                members: new[]
                {
                    GenerateDescendantFields(d, g),
                    new[] { GenerateConstructor(d,g) }
                }
                .SelectMany(n => n)
            );
        }

        public static IEnumerable<SyntaxNode> GenerateDescendantFields(DrawableData d, SyntaxGenerator g)
        {
            foreach (var child in d.Children)
            {
                yield return g.FieldDeclaration(
                    name: child.Id,
                    type: g.IdentifierName(child.DrawableType.FullName),
                    accessibility: Accessibility.Private,
                    modifiers: DeclarationModifiers.ReadOnly
                );

                // Recursively add descendants
                foreach (var n in GenerateDescendantFields(child, g))
                    yield return n;
            }
        }

        public static SyntaxNode GenerateConstructor(DrawableData d, SyntaxGenerator g)
        {
            return g.ConstructorDeclaration(
                containingTypeName: d.Id,
                accessibility: Accessibility.Public,
                statements: new[]
                {
                    GenerateAttributeInitialization(d, g),
                    GenerateDescendantInitialization(d, g, isRoot: true)
                }
                .SelectMany(n => n)
            );
        }

        public static IEnumerable<SyntaxNode> GenerateAttributeInitialization(DrawableData d, SyntaxGenerator g)
        {
            foreach (var a in d.Attributes.Values)
                yield return g.AssignmentStatement(
                    left: g.IdentifierName(a.Name),
                    right: a.Converter.GenerateInstantiation(a.Value, g)
                );
        }

        public static IEnumerable<SyntaxNode> GenerateDescendantInitialization(DrawableData d, SyntaxGenerator g, bool isRoot = false)
        {
            foreach (var child in d.Children)
            {
                // Assign to field
                yield return g.AssignmentStatement(
                    left: g.IdentifierName(child.Id),
                    right: g.ObjectCreationExpression(
                        namedType: g.IdentifierName(child.DrawableType.FullName),
                        arguments: new SyntaxNode[0]
                    )
                );

                // Initialize descendants
                foreach (var n in GenerateDescendantInitialization(child, g))
                    yield return n;

                // Invoke Add
                yield return g.InvocationExpression(
                    expression: g.IdentifierName(isRoot ? "Add" : $"{d.Id}.Add"),
                    arguments: new[] { g.IdentifierName(child.Id) }
                );
            }
        }
    }
}
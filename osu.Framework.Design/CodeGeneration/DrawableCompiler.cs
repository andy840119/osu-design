using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using osu.Framework.Design.Markup;
using osu.Framework.Graphics;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace osu.Framework.Design.CodeGeneration
{
    public static class DrawableCompiler
    {
        public static Compilation CreateCompilation(DrawableDocument doc)
        {
            return CSharpCompilation.Create(
                assemblyName: "TestAssembly",
                options: new CSharpCompilationOptions(
                    outputKind: OutputKind.DynamicallyLinkedLibrary
                ),
                references: new[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(doc.DrawableType.Assembly.Location),
                    MetadataReference.CreateFromFile(doc.GetType().Assembly.Location)
                },
                syntaxTrees: new[]
                {
                    GenerateTree(doc)
                }
            );
        }

        public static SyntaxTree GenerateTree(DrawableDocument doc)
        {
            return SyntaxTree(
                root: NamespaceDeclaration(
                    name: IdentifierName(doc.Namespace),
                    externs: List<ExternAliasDirectiveSyntax>(),
                    usings: List<UsingDirectiveSyntax>(),
                    members: SingletonList<MemberDeclarationSyntax>(
                        doc.GenerateClassSyntax()
                    )
                ),
                options: new CSharpParseOptions(
                    languageVersion: LanguageVersion.Latest,
                    documentationMode: DocumentationMode.None,
                    kind: SourceCodeKind.Regular
                )
            );
        }
    }
}
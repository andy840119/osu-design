using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using osu.Framework.Design.Markup;

namespace osu.Framework.Design.CodeGeneration
{
    public class CodeGenerator
    {
        readonly Document _doc;

        public CodeGenerator(Document document)
        {
            _doc = document;
        }

        public SyntaxNode Generate(DrawableData drawable)
        {
            // Get code generator
            var generator = SyntaxGenerator.GetGenerator(_doc);

            // Generate drawable
            var drawableNode = DrawableGenerator.Generate(drawable, generator);

            // Return the root node (namespace) with drawable declaration
            return generator.NamespaceDeclaration(string.Join('.', _doc.Folders), drawableNode);
        }

        public void Generate(DrawableData drawable, TextWriter writer) => Generate(drawable).WriteTo(writer);
    }
}
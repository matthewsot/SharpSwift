using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        //var something, something_else = "123";
        [ParsesType(typeof(VariableDeclarationSyntax))]
        public static string VariableDeclaration(VariableDeclarationSyntax node)
        {
            var thisVar = node.Variables.First(); //Only one var at a time for now
            var output = "var " + thisVar.Identifier.Text;
            if (!node.Type.IsVar)
            {
                output += ": " + Type(node.Type);
            }
            if (thisVar.Initializer == null)
            {
                return output + ";\r\n";
            }

            output += " " + SyntaxNode(thisVar.Initializer);
            return output;
        }
    }
}

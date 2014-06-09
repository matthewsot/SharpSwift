using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        //var something, something_else = "123";
        [ParsesType(typeof(VariableDeclarationSyntax))]
        public static string VariableDeclaration(VariableDeclarationSyntax node)
        {
            var output = "var ";
            foreach (var currVar in node.Variables)
            {
                output += currVar.Identifier.Text;
                if (!node.Type.IsVar)
                {
                    output += ": " + Type(node.Type);
                }
                if (currVar.Initializer != null)
                {
                    output += " " + SyntaxNode(currVar.Initializer);
                }
                output += ", ";
            }

            return output.TrimEnd(',', ' ');
        }

        //var something = something_else;
        [ParsesType(typeof(LocalDeclarationStatementSyntax))]
        public static string LocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            return SyntaxNode(node.Declaration) + Semicolon(node.SemicolonToken);
        }
    }
}

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        //using(var something = new StreamReader()) { something }
        [ParsesType(typeof(UsingStatementSyntax))]
        public static string UsingStatement(UsingStatementSyntax node)
        {
            var output = SyntaxNode(node.Declaration) + ";\r\n";

            output += Block((BlockSyntax)node.Statement, false);

            output += node.Declaration.Variables.First().Identifier.Text + " = nil;\r\n";
            return output;
        }


        [ParsesType(typeof(ReturnStatementSyntax))]
        public static string ReturnStatement(ReturnStatementSyntax node)
        {
            var output = "return";

            if (node.Expression != null)
            {
                output += " " + SyntaxNode(node.Expression).Trim();
            }

            return output + Semicolon(node.SemicolonToken);
        }
    }
}

using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        //using(var something = new StreamReader()) { something }
        [ParsesType(typeof(UsingStatementSyntax))]
        public static string UsingStatement(UsingStatementSyntax node)
        {
            var output = SyntaxNode(node.Declaration) + ";" + NewLine;

            output += Block((BlockSyntax)node.Statement, false);

            //Swift calls deinit when you make a variable nil

            output += string.Join("",
                node.Declaration.Variables.Select(variable => variable.Identifier.Text + " = nil;" + NewLine));

            return output;
        }


        [ParsesType(typeof(ReturnStatementSyntax))]
        public static string ReturnStatement(ReturnStatementSyntax node)
        {
            var output = "return";

            if (node.Expression != null)
            {
                output += " " + SyntaxNode(node.Expression);
            }

            return output + Semicolon(node.SemicolonToken);
        }

        [ParsesType(typeof(IfStatementSyntax))]
        public static string IfStatement(IfStatementSyntax node)
        {
            var output = node.IfKeyword.Text + " (";
            output += SyntaxNode(node.Condition);
            output += ")" + NewLine + SyntaxNode(node.Statement);
            if (node.Else != null)
            {
                output += SyntaxNode(node.Else);
            }
            return output;
        }

        [ParsesType(typeof(ElseClauseSyntax))]
        public static string ElseClause(ElseClauseSyntax node)
        {
            var output = node.ElseKeyword.Text + " ";
            output += SyntaxNode(node.Statement);
            return output;
        }
    }
}

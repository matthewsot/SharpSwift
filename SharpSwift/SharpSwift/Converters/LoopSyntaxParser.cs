using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        [ParsesType(typeof(ForEachStatementSyntax))]
        public static string ForEachStatement(ForEachStatementSyntax node)
        {
            var output = "for ";
            output += node.Identifier.Text; //TODO: should we do anything to this?
            output += " in " + SyntaxNode(node.Expression) + " " + SyntaxNode(node.Statement);
            return output;
        }

        [ParsesType(typeof (ForStatementSyntax))]
        public static string ForStatement(ForStatementSyntax node)
        {
            var output = "for ";
            output += SyntaxNode(node.Declaration) + "; " + SyntaxNode(node.Condition) + "; " + //TODO: these semicolons should be handled in their syntaxParsers
                      SyntaxNode(node.Incrementors.First()).TrimEnd(); //TODO: handle multiple incrementors
            output += " " + SyntaxNode(node.Statement);
            return output;
        }
    }
}
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
    }
}
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        //= something_else()
        [ParsesType(typeof(EqualsValueClauseSyntax))]
        public static string EqualsValueClause(EqualsValueClauseSyntax node)
        {
            return node.EqualsToken + " " + SyntaxNode(node.Value).Trim();
        }
    }
}

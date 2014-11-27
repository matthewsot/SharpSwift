using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        /// <summary>
        /// Converts an equals value clause to Swift
        /// </summary>
        /// <example>= something_else()</example>
        /// <param name="clause">The clause to convert</param>
        /// <returns>The converted Swift clause</returns>
        [ParsesType(typeof(EqualsValueClauseSyntax))]
        public static string EqualsValueClause(EqualsValueClauseSyntax clause)
        {
            return clause.EqualsToken + " " + SyntaxNode(clause.Value).Trim();
        }
    }
}

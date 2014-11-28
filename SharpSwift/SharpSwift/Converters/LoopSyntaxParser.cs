using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        /// <summary>
        /// Converts a foreach statement to Swift
        /// </summary>
        /// <example>foreach(var y in x) { }</example>
        /// <param name="statement">The statement to convert</param>
        /// <returns>The converted Swift statement</returns>
        [ParsesType(typeof(ForEachStatementSyntax))]
        public static string ForEachStatement(ForEachStatementSyntax statement)
        {
            return "for " + statement.Identifier.Text + " in " + SyntaxNode(statement.Expression) + " " +
                   SyntaxNode(statement.Statement);
        }

        /// <summary>
        /// Converts a for statement to Swift
        /// </summary>
        /// <example>for(var i = 0;i == 2;i++) { }</example>
        /// <param name="statement"></param>
        /// <returns></returns>
        [ParsesType(typeof (ForStatementSyntax))]
        public static string ForStatement(ForStatementSyntax statement)
        {
            var output = "for ";

            output += SyntaxNode(statement.Declaration) + "; " + SyntaxNode(statement.Condition) + "; " + //TODO: these semicolons should be handled in their syntaxParsers
                      SyntaxNode(statement.Incrementors.First()).TrimEnd(); //TODO: handle multiple incrementors

            output += " " + SyntaxNode(statement.Statement);
            return output;
        }
    }
}
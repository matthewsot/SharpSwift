using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        /// <summary>
        /// Converts a using statement to Swift
        /// </summary>
        /// <example>using(var something = new StreamReader()) { something }</example>
        /// <param name="statement">The statement to convert</param>
        /// <returns>The converted Swift statement</returns>
        [ParsesType(typeof(UsingStatementSyntax))]
        public static string UsingStatement(UsingStatementSyntax statement)
        {
            var output = SyntaxNode(statement.Declaration) + ";" + NewLine;

            output += Block((BlockSyntax)statement.Statement, false);

            //Swift calls deinit when you make a variable nil

            output += string.Join("",
                statement.Declaration.Variables.Select(variable => variable.Identifier.Text + " = nil;" + NewLine));

            return output;
        }

        /// <summary>
        /// Converts a return statement to Swift
        /// </summary>
        /// <param name="statement">The statement to convert</param>
        /// <returns>The converted Swift statement</returns>
        [ParsesType(typeof(ReturnStatementSyntax))]
        public static string ReturnStatement(ReturnStatementSyntax statement)
        {
            var output = "return";

            if (statement.Expression != null)
            {
                output += " " + SyntaxNode(statement.Expression);
            }

            return output + Semicolon(statement.SemicolonToken);
        }
    }
}

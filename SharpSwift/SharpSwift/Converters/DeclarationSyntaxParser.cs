using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        /// <summary>
        /// Converts a local declaration statement to Swift.
        /// Differs from VariableDeclaration in that it includes the semicolon.
        /// </summary>
        /// <example>var something = something_else;</example>
        /// <param name="declaration">The LocalDeclarationStatementSyntax to convert</param>
        /// <returns>The Swift declaration, including the semicolon if necessary</returns>
        [ParsesType(typeof(LocalDeclarationStatementSyntax))]
        public static string LocalDeclarationStatement(LocalDeclarationStatementSyntax declaration)
        {
            return SyntaxNode(declaration.Declaration) + Semicolon(declaration.SemicolonToken);
        }

        /// <summary>
        /// Converts a variable declaration to Swift
        /// </summary>
        /// <example>var hello = 1</example>
        /// <example>string something, something_else = "123"</example>
        /// <param name="declaration">The VariableDeclarationSyntax to convert.</param>
        /// <returns>A Swift variable declaration</returns>
        [ParsesType(typeof(VariableDeclarationSyntax))]
        public static string VariableDeclaration(VariableDeclarationSyntax declaration)
        {
            var isConst = declaration.Parent is LocalDeclarationStatementSyntax
                          && ((LocalDeclarationStatementSyntax) declaration.Parent).IsConst;

            var output = isConst ? "let " :  "var ";

            foreach (var currVar in declaration.Variables)
            {
                output += currVar.Identifier.Text;

                if (!declaration.Type.IsVar)
                {
                    output += ": " + SyntaxNode(declaration.Type);
                }

                if (currVar.Initializer != null)
                {
                    output += " " + SyntaxNode(currVar.Initializer);
                }
                output += ", ";
            }

            return output.TrimEnd(',', ' ');
        }
    }
}

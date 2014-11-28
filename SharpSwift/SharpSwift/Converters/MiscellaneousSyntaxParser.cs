using Microsoft.CodeAnalysis;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        /// <summary>
        /// Adds a semicolon to the end of a line if there is one
        /// </summary>
        /// <param name="semicolonToken">The semicolon</param>
        /// <returns>The semicolon and newline</returns>
        private static string Semicolon(SyntaxToken semicolonToken)
        {
            return semicolonToken.Text == ";" ? ";" + NewLine : "";
        }
    }
}

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        /// <summary>
        /// Converts an argument to Swift
        /// </summary>
        /// <example>name: value</example>
        /// <param name="arg">The argument to convert</param>
        /// <returns>The converted Swift argument</returns>
        [ParsesType(typeof(ArgumentSyntax))]
        public static string Argument(ArgumentSyntax arg)
        {
            var output = SyntaxNode(arg.Expression);

            if (arg.NameColon != null)
            {
                output = SyntaxNode(arg.NameColon.Name) + ": " + output;
            }
            return output;
        }

        /// <summary>
        /// Converts an attribute argument to Swift
        /// </summary>
        /// <param name="arg">The attribute argument to convert</param>
        /// <returns>The converted Swift argument</returns>
        [ParsesType(typeof(AttributeArgumentSyntax))]
        public static string AttributeArgument(AttributeArgumentSyntax arg)
        {
            var output = SyntaxNode(arg.Expression);
            if (arg.NameColon != null)
            {
                output = SyntaxNode(arg.NameColon.Name) + ": " + output;
            }
            return output;
        }
    }
}

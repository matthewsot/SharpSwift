using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        /// <summary>
        /// Converts a type argument list to Swift
        /// </summary>
        /// <param name="list">The list to convert</param>
        /// <returns>The converted Swift list</returns>
        [ParsesType(typeof(TypeArgumentListSyntax))]
        public static string TypeArgumentList(TypeArgumentListSyntax list)
        {
            return "<" + string.Join(", ", list.Arguments.Select(SyntaxNode)) + ">";
        }

        /// <summary>
        /// Converts a type parameter list to Swift
        /// </summary>
        /// <param name="list">The list to convert</param>
        /// <returns>The converted Swift list</returns>
        [ParsesType(typeof(TypeParameterListSyntax))]
        public static string TypeParameterList(TypeParameterListSyntax list)
        {
            return string.Join(", ", list.Parameters.Select(SyntaxNode));
        }

        /// <summary>
        /// Converts a parameter list to Swift
        /// </summary>
        /// <example>(ParameterListSyntax list)</example>
        /// <param name="list">The parameter list to convert</param>
        /// <returns>The converted Swift list</returns>
        [ParsesType(typeof(ParameterListSyntax))]
        public static string ParameterList(ParameterListSyntax list)
        {
            return "(" + string.Join(", ", list.Parameters.Select(SyntaxNode)) + ")";
        }

        /// <summary>
        /// Converts an attribute list to Swift
        /// </summary>
        /// <param name="list">The list to convert</param>
        /// <returns>The converted Swift list</returns>
        [ParsesType(typeof(AttributeListSyntax))]
        public static string AttributeList(AttributeListSyntax list)
        {
            return string.Join(" ", list.Attributes.Select(SyntaxNode));
        }

        /// <summary>
        /// Converts an argument list to Swift
        /// </summary>
        /// <param name="list">The list to convert</param>
        /// <returns>The converted Swift list</returns>
        [ParsesType(typeof(ArgumentListSyntax))]
        public static string ArgumentList(ArgumentListSyntax list)
        {
            return "(" + string.Join(", ", list.Arguments.Select(SyntaxNode)) + ")";
        }

        /// <summary>
        /// Converts an attribute argument list to Swift
        /// </summary>
        /// <param name="list">The list to convert</param>
        /// <returns>The converted Swift list</returns>
        [ParsesType(typeof(AttributeArgumentListSyntax))]
        public static string AttributeArgumentList(AttributeArgumentListSyntax list)
        {
            return "(" + string.Join(", ", list.Arguments.Select(SyntaxNode)) + ")";
        }
    }
}
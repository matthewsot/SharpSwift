using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        /// <summary>
        /// Detects whether an attribute is a certain SharpSwift-specific Attribute (like [ExportAs])
        /// </summary>
        /// <param name="attribute">The AttributeSyntax to check</param>
        /// <param name="expectingName">The attribute name you're looking for, or null to match all SharpSwift attributes</param>
        /// <returns>A boolean value representing whether it is or isn't the SharpSwift attribute</returns>
        private static bool IsSharpSwiftAttribute(AttributeSyntax attribute, string expectingName = null)
        {
            var symbolInfo = Model.GetSymbolInfo(attribute.Name);

            if (symbolInfo.Symbol == null)
            {
                return false;
            }

            var containingNamespace = symbolInfo.Symbol.ContainingSymbol.ContainingNamespace;

            var containingContainingNamespace = containingNamespace.ContainingNamespace;
            if (containingContainingNamespace == null)
            {
                return false;
            }

            return containingContainingNamespace.Name == "SharpSwift"
                   && containingNamespace.Name == "Attributes"
                   && (expectingName == null || symbolInfo.Symbol.ContainingSymbol.Name == expectingName);
        }

        /// <summary>
        /// Converts a C# attribute, ignoring SharpSwift-specific attributes
        /// </summary>
        /// <param name="attribute">The attribute to convert</param>
        /// <returns>Swift code representing the same attribute</returns>
        [ParsesType(typeof (AttributeSyntax))]
        public static string AttributeSyntax(AttributeSyntax attribute)
        {
            var output = "@" + SyntaxNode(attribute.Name).TrimEnd('!');

            if (IsSharpSwiftAttribute(attribute))
            {
                return "";
            }

            if (attribute.ArgumentList != null)
            {
                output += SyntaxNode(attribute.ArgumentList);
            }

            return output;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        private static bool IsSharpSwiftAttribute(AttributeSyntax node, string expectingName)
        {
            var symbolInfo = model.GetSymbolInfo(node.Name);

            var containingNamespace = symbolInfo.Symbol.ContainingSymbol.ContainingNamespace;

            var containingContainingNamespace = containingNamespace.ContainingNamespace;
            if (containingContainingNamespace == null)
            {
                return false;
            }

            return containingContainingNamespace.Name == "SharpSwift"
                   && containingNamespace.Name == "Attributes"
                   && symbolInfo.Symbol.ContainingSymbol.Name == expectingName;
        }

        [ParsesType(typeof (AttributeSyntax))]
        public static string AttributeSyntax(AttributeSyntax node)
        {
            var output = "@" + SyntaxNode(node.Name);

            if (IsSharpSwiftAttribute(node, "ExportAttribute"))
            {
                return "";
            }

            if (node.ArgumentList != null)
            {
                output += SyntaxNode(node.ArgumentList);
            }

            return output;
        }
    }
}

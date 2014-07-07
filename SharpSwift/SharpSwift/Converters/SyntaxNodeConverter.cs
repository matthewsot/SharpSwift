using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        public static SemanticModel model;

        [ParsesType(typeof (BlockSyntax))]
        public static string Block(BlockSyntax node, bool includeBraces = true)
        {
            var output = (includeBraces ? "{\r\n" : "");

            output += string.Join("", node.ChildNodes().Select(SyntaxNode));

            return output + (includeBraces ? "}\r\n" : "");
        }

        private static string Semicolon(SyntaxToken semicolonToken)
        {
            return semicolonToken.Text == ";" ? ";\r\n" : "";
        }

        /// <summary>
        /// Returns the Swift equivilant for a C# type
        /// </summary>
        /// <param name="typeName">The C# type's identifier as a string</param>
        /// <returns>The Swift equivilant type as a string</returns>
        private static string Type(string typeName)
        {
            switch (typeName)
            {
                case "string":
                    return "String";
                case "char":
                    return "Character";
                case "int":
                    return "Int";
                case "void":
                    return "Void";
            }
            return typeName;
        }

        [ParsesType(typeof(TypeSyntax))]
        public static string Type(TypeSyntax node)
        {
            var typeName = ((PredefinedTypeSyntax)node).Keyword.Text;
            return Type(typeName);
        }

        /// <summary>
        /// Converts a C# Roslyn SyntaxNode to it's Swift equivilant
        /// </summary>
        /// <param name="node">Roslyn SyntaxNode representing the C# code to convert</param>
        /// <returns>A string with the converted Swift code</returns>
        public static string SyntaxNode(SyntaxNode node)
        {
            if (node == null)
            {
                return "";
            }
            if (node.HasLeadingTrivia)
            {
                foreach (var trivia in node.GetLeadingTrivia())
                {
                    if (trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
                        trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                    {
                        var triviaText = trivia.ToString().TrimStart('/', '*').TrimStart();
                        if (triviaText.ToLower().Trim() == "ignore")
                        {
                            return "";
                        }
                    }
                }
            }

            if (node is BlockSyntax)
            {
                //Block() takes two arguments, & I don't want to worry about it w/ reflection.
                return Block((BlockSyntax)node);
            }

            /*
             * We're gonna search through ConverToSwift's static methods for one
             * with the ParsesType attribute that matches the typeof node.
             * If one isn't found we'll just return the C# code
             */
            var nodeType = node.GetType();

            var methods = typeof (ConvertToSwift).GetMethods();
            var matchedMethod =
                methods.FirstOrDefault(method => //find method that parses this syntax
                        method.GetCustomAttributes(true).OfType<ParsesTypeAttribute>()
                            .Any(attr => nodeType == attr.ParsesType));

            if (matchedMethod != null)
            {
                var s = new ConvertToSwift();
                return matchedMethod.Invoke(s, new[] { node }).ToString();
            }

            return node.ToString() + "\r\n";
        }
    }
}

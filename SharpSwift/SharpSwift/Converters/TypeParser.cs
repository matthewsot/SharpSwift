using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
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

        [ParsesType(typeof (ArrayTypeSyntax))]
        public static string ArrayType(ArrayTypeSyntax node)
        {
            return "[" + SyntaxNode(node.ElementType) + "]"; //TODO: rankspecifiers
        }

        [ParsesType(typeof (PredefinedTypeSyntax))]
        public static string PredefinedType(PredefinedTypeSyntax node)
        {
            return Type(node.Keyword.Text);
        }
    }
}

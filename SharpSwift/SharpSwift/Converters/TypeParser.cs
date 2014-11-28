using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        /// <summary>
        /// Returns the Swift equivilant for a C# type
        /// </summary>
        /// <param name="typeName">The C# type's identifier as a string</param>
        /// <param name="implyUnwrapped">If true, unwraps the type with an ! at the end</param>
        /// <returns>The Swift equivilant type as a string</returns>
        //TODO: figure out the unwrapping already
        private static string Type(string typeName, bool implyUnwrapped = false)
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
                case "bool":
                    return "Bool";
            }
            return typeName + (implyUnwrapped ? "!" : "");
        }

        /// <summary>
        /// Converts an array type to Swift
        /// </summary>
        /// <param name="array">The array type to convert</param>
        /// <returns>The converted Swift type</returns>
        [ParsesType(typeof (ArrayTypeSyntax))]
        public static string ArrayType(ArrayTypeSyntax array)
        {
            return "[" + SyntaxNode(array.ElementType) + "]"; //TODO: rankspecifiers
        }

        /// <summary>
        /// Converts a PredefinedType to Swift
        /// </summary>
        /// <param name="type">The type to convert</param>
        /// <returns>The converted Swift type</returns>
        [ParsesType(typeof (PredefinedTypeSyntax))]
        public static string PredefinedType(PredefinedTypeSyntax type)
        {
            return Type(type.Keyword.Text);
        }
    }
}

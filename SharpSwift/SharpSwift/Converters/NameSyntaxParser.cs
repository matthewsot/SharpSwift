using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        /// <summary>
        /// Converts an IdentifierNameSyntax to Swift
        /// </summary>
        /// <param name="identifierName">The identifier to convert</param>
        /// <returns>The Swift identifier</returns>
        [ParsesType(typeof(IdentifierNameSyntax))]
        public static string IdentifierName(IdentifierNameSyntax identifierName)
        {
            //Looks for an ExportAttribute
            var symbol = Model.GetSymbolInfo(identifierName).Symbol;
            string nameToUse = null;
            
            if (symbol != null)
            {
                //Check for an [Export()] attribute
                var exportAttr = symbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass.Name.Contains("ExportAttribute"));
                if (exportAttr != null)
                {
                    nameToUse = exportAttr.ConstructorArguments[0].Value.ToString();
                }
            }

            return nameToUse ?? Type(identifierName.Identifier.Text);
        }

        /// <summary>
        /// Converts a generic name to Swift
        /// </summary>
        /// <param name="name">The name to convert</param>
        /// <returns>The converted Swift code</returns>
        [ParsesType(typeof (GenericNameSyntax))]
        public static string GenericName(GenericNameSyntax name)
        {
            //TODO: replace the screwed up Action<>/Func<> conversions w/ DNSwift implementations
            switch (name.Identifier.Text)
            {
                case "Action":
                    //Action<string, int> converts to (String, Int) -> Void
                    return ": (" + SyntaxNode(name.TypeArgumentList) + ") -> Void";
                case "Func":
                    //Func<string, int, string> converts to (String, Int) -> String
                    var output = ": (";

                    //The last generic argument in Func<> is used as a return type
                    var allButLastArguments = name.TypeArgumentList.Arguments.Take(name.TypeArgumentList.Arguments.Count - 1);

                    output += string.Join(", ", allButLastArguments.Select(SyntaxNode));

                    return output + ") -> " + SyntaxNode(name.TypeArgumentList.Arguments.Last());
                case "Unwrapped":
                    return SyntaxNode(name.TypeArgumentList.Arguments.First()).TrimEnd('!') + "!";
                case "Optional":
                    return SyntaxNode(name.TypeArgumentList.Arguments.First()).TrimEnd('!') + "?";
                case "AmbiguousWrapping":
                    return SyntaxNode(name.TypeArgumentList.Arguments.First()).TrimEnd('!');
            }

            //Something<another, thing> converts to Something<another, thing> :D
            return Type(name.Identifier.Text) + SyntaxNode(name.TypeArgumentList);
        }
    }
}
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        [ParsesType(typeof(IdentifierNameSyntax))]
        public static string IdentifierName(IdentifierNameSyntax node)
        {
            //Looks for an ExportAttribute
            var symbol = model.GetSymbolInfo(node).Symbol;
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

            if(nameToUse != null)
            {
                return nameToUse;
            }

            return Type(node.Identifier.Text);
        }

        [ParsesType(typeof (GenericNameSyntax))]
        public static string GenericName(GenericNameSyntax node)
        {
            //TODO: replace the screwed up Action<>/Func<> conversions w/ DNSwift implementations
            //Action<string, int> converts to (String, Int) -> Void
            if (node.Identifier.Text == "Action")
            {
                return ": (" + SyntaxNode(node.TypeArgumentList) + ") -> Void";
            }
            //Func<string, int, string> converts to (String, Int) -> String
            if (node.Identifier.Text == "Func")
            {
                var output = ": (";

                //The last generic argument in Func<> is used as a return type
                var allButLastArguments = node.TypeArgumentList.Arguments.Take(node.TypeArgumentList.Arguments.Count - 1);

                output += string.Join(", ", allButLastArguments.Select(SyntaxNode));

                return output + ") -> " + SyntaxNode(node.TypeArgumentList.Arguments.Last());
            }

            //Something<another, thing> converts to Something<another, thing> :D
            return Type(node.Identifier.Text) + SyntaxNode(node.TypeArgumentList);
        }
    }
}
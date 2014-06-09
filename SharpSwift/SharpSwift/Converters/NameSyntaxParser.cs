using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        [ParsesType(typeof(PredefinedTypeSyntax))]
        public static string PredefinedType(PredefinedTypeSyntax node)
        {
            return Type(node.Keyword.Text);
        }

        [ParsesType(typeof(IdentifierNameSyntax))]
        public static string IdentifierName(IdentifierNameSyntax node)
        {
            return Type(node.Identifier.Text);
        }

        [ParsesType(typeof (GenericNameSyntax))]
        public static string GenericName(GenericNameSyntax node)
        {
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

                output += string.Join(", ", allButLastArguments.Select(Type));

                return output + ") -> " + Type(node.TypeArgumentList.Arguments.Last());
            }

            //Something<another, thing> converts to Something<another, thing> :D
            return node.Identifier.Text + "<" + SyntaxNode(node.TypeArgumentList) + ">";
        }
    }
}
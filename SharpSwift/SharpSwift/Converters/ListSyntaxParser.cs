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
        [ParsesType(typeof (ArgumentListSyntax))]
        public static string ArgumentList(ArgumentListSyntax node)
        {
            var output = "(";
            foreach (var arg in node.Arguments)
            {
                output += SyntaxNode(arg.Expression) + ", ";
            }
            return output.TrimEnd(',', ' ') + ")";
        }

        [ParsesType(typeof(GenericNameSyntax))]
        public static string GenericName(GenericNameSyntax node)
        {
            var output = "";

            //Action<string, int> converts to (String, Int) -> Void
            if (node.Identifier.Text == "Action")
            {
                output += ": (";
                output += string.Join(", ", node.TypeArgumentList.Arguments.Select(Type));
                return output + ") -> Void";
            }
            //Func<string, int, string> converts to (String, Int) -> String
            if (node.Identifier.Text == "Func")
            {
                output += ": (";
                
                //The last generic argument in Func<> is used as a return type
                var allButLastArguments = node.TypeArgumentList.Arguments.Take(node.TypeArgumentList.Arguments.Count - 1);

                output += string.Join(", ", allButLastArguments.Select(Type));
                return output + ") -> " + Type(node.TypeArgumentList.Arguments.Last());
            }

            //Something<another, thing> converts to Something<another, thing> :D
            output = node.Identifier.Text + "<";
            output += string.Join(", ", node.TypeArgumentList.Arguments.Select(Type));
            output += ">";
            return output;
        }

        //string something
        [ParsesType(typeof(ParameterSyntax))]
        public static string Parameter(ParameterSyntax node)
        {
            var output = node.Identifier.Text;

            if (node.Type != null)
            {
                //TODO: this is ew.
                if (node.Type is GenericNameSyntax)
                {
                    output += SyntaxNode(node.Type);
                }
                else if (node.Type is IdentifierNameSyntax)
                {
                    output += ": " + Type(((IdentifierNameSyntax)node.Type).Identifier.Text);
                }
                else
                {
                    output += ": " + SyntaxNode(node.Type);
                }
            }
            return output;
        }

        [ParsesType(typeof (ParameterListSyntax))]
        public static string ParameterList(ParameterListSyntax node)
        {
            var output = "(";
            foreach (var arg in node.Parameters)
            {
                output += SyntaxNode(arg) + ", ";
            }
            return output.TrimEnd(',', ' ') + ")";
        }
    }
}
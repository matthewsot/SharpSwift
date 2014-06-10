using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        //TODO: _creatingObject is sketch. look into other ways to do this
        private static string _creatingObject = null;

        [ParsesType(typeof(ArgumentSyntax))]
        public static string Argument(ArgumentSyntax node)
        {
            var output = SyntaxNode(node.Expression);
            if (node.NameColon != null)
            {
                output = SyntaxNode(node.NameColon.Name) + ": " + output;
            }
            else if(_creatingObject != null)
            {
                //In Swift, object creation arguments must be named
                Console.WriteLine("WARNING: Non-named argument found while creating object \"" + _creatingObject + "\".");
            }
            return output;
        }

        [ParsesType(typeof (ArgumentListSyntax))]
        public static string ArgumentList(ArgumentListSyntax node)
        {
            return "(" + string.Join(", ", node.Arguments.Select(SyntaxNode)) + ")";
        }

        [ParsesType(typeof(TypeParameterSyntax))]
        public static string TypeParameter(TypeParameterSyntax node)
        {
            return node.Identifier.Text;
        }

        [ParsesType(typeof(TypeParameterListSyntax))]
        public static string TypeParameterList(TypeParameterListSyntax node)
        {
            return string.Join(", ", node.Parameters.Select(SyntaxNode));
        }

        //string something
        [ParsesType(typeof(ParameterSyntax))]
        public static string Parameter(ParameterSyntax node)
        {
            var output = node.Identifier.Text;

            if (node.Type != null)
            {
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

            //variadic parameters O:
            //params string[] strs -> strs: string...
            //kindof.
            if (node.Modifiers.Any(mod => mod.ToString() == "params"))
            {
                output = output.TrimEnd().TrimEnd('[', ']') + "...";
            }
            return output;
        }

        [ParsesType(typeof (ParameterListSyntax))]
        public static string ParameterList(ParameterListSyntax node)
        {
            return "(" + string.Join(", ", node.Parameters.Select(SyntaxNode)) + ")";
        }
    }
}
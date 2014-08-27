using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        //NOTE: you shouldn't use anything special in Argument or ArgumentList - check ObjectCreationExpression FMI
        [ParsesType(typeof(ArgumentSyntax))]
        public static string Argument(ArgumentSyntax node)
        {
            var output = SyntaxNode(node.Expression);
            if (node.NameColon != null)
            {
                output = SyntaxNode(node.NameColon.Name) + ": " + output;
            }
            return output;
        }

        [ParsesType(typeof(AttributeArgumentSyntax))]
        public static string AttributeArgument(AttributeArgumentSyntax node)
        {
            var output = SyntaxNode(node.Expression);
            if (node.NameColon != null)
            {
                output = SyntaxNode(node.NameColon.Name) + ": " + output;
            }
            return output;
        }

        [ParsesType(typeof (ArgumentListSyntax))]
        public static string ArgumentList(ArgumentListSyntax node)
        {
            return "(" + string.Join(", ", node.Arguments.Select(SyntaxNode)) + ")";
        }

        [ParsesType(typeof(AttributeListSyntax))]
        public static string AttributeList(AttributeListSyntax node)
        {
            return string.Join(" ", node.Attributes.Select(SyntaxNode));
        }

        [ParsesType(typeof(AttributeArgumentListSyntax))]
        public static string AttributeArgumentList(AttributeArgumentListSyntax node)
        {
            return "(" + string.Join(", ", node.Arguments.Select(SyntaxNode)) + ")";
        }

        [ParsesType(typeof(TypeArgumentListSyntax))]
        public static string TypeArgumentList(TypeArgumentListSyntax node)
        {
            return "<" + string.Join(", ", node.Arguments.Select(SyntaxNode)) + ">";
        }

        [ParsesType(typeof(TypeParameterSyntax))]
        public static string TypeParameter(TypeParameterSyntax node)
        {
            var constraint = "";
            if (node.Parent.Parent is MethodDeclarationSyntax)
            {
                var typeConstraints = ((MethodDeclarationSyntax)node.Parent.Parent).ConstraintClauses;
                var constraints = typeConstraints.FirstOrDefault(constr => SyntaxNode(constr.Name) == node.Identifier.Text).Constraints;
                constraint = ": " + string.Join(", ", constraints); //TODO: check if this is the right syntax for multiple constraints
            }
            return node.Identifier.Text + constraint;
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
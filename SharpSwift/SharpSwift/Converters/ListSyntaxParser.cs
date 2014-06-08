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

        [ParsesType(typeof (ParameterListSyntax))]
        public static string ParameterList(ParameterListSyntax node)
        {
            var output = "(";
            foreach (var arg in node.Parameters)
            {
                output += arg.Identifier.Text;
                if (arg.Type != null)
                {
                    output += ": " + Type(((IdentifierNameSyntax)arg.Type).Identifier.Text);
                }
                output += ", ";
            }
            return output.TrimEnd(',', ' ') + ")";
        }
    }
}
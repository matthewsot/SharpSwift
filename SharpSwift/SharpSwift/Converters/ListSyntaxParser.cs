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
                    //TODO: this is ew.
                    if (arg.Type is GenericNameSyntax)
                    {
                        var type = (GenericNameSyntax) arg.Type;
                        //Action<string, int> convets to (String, Int) -> Void
                        if (type.Identifier.Text == "Action")
                        {
                            output += ": (";
                            foreach (var innerArg in type.TypeArgumentList.Arguments)
                            {
                                output += Type(innerArg) + ", ";
                            }
                            output = output.TrimEnd(',', ' ') + ") -> Void";
                        }
                        if (type.Identifier.Text == "Func")
                        {
                            output += ": (";
                            foreach (var innerArg in type.TypeArgumentList.Arguments.Take(type.TypeArgumentList.Arguments.Count - 1))
                            {
                                output += Type(innerArg) + ", ";
                            }
                            output = output.TrimEnd(',', ' ') + ") -> " + Type(type.TypeArgumentList.Arguments.Last());
                        }
                    }
                    else if (arg.Type is IdentifierNameSyntax)
                    {
                        output += ": " + Type(((IdentifierNameSyntax) arg.Type).Identifier.Text);
                    }
                }
                output += ", ";
            }
            return output.TrimEnd(',', ' ') + ")";
        }
    }
}
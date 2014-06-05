using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {

        [ParsesType(typeof(TypeSyntax))]
        public static string Type(TypeSyntax node)
        {
            var typeName = ((PredefinedTypeSyntax)node).Keyword.Text;
            switch (typeName)
            {
                case "string":
                    return "NSString";
                    break;
            }
            return typeName;
        }

        public static string SyntaxNode(SyntaxNode node)
        {
            var nodeType = node.GetType();

            var methods = typeof (ConvertToSwift).GetMethods();
            var matchedMethod =
                methods.FirstOrDefault(method => //find method that parses this syntax
                        method.GetCustomAttributes(true).OfType<ParsesTypeAttribute>()
                            .Any(attr => nodeType == attr.ParsesType));

            if (matchedMethod != null)
            {
                var s = new ConvertToSwift();
                return matchedMethod.Invoke(s, new[] { node }).ToString();
            }

            return node.ToString();
        }
    }
}

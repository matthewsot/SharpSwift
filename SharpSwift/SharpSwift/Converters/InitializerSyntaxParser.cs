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
        //= something_else()
        [ParsesType(typeof(EqualsValueClauseSyntax))]
        public static string EqualsValueClause(EqualsValueClauseSyntax node)
        {
            return node.EqualsToken + " " + SyntaxNode(node.Value).Trim();
        }
    }
}

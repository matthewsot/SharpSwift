using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        [ParsesType(typeof(ObjectCreationExpressionSyntax))]
        public static string ObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            var output = Type(((IdentifierNameSyntax)node.Type).Identifier.Text) + "(";

            foreach (var arg in node.ArgumentList.Arguments)
            {
                output += SyntaxNode(arg.Expression).Trim() + ", ";
            }

            output = output.Trim(' ', ',') + ")";
            return output;
        }

        [ParsesType(typeof(LocalDeclarationStatementSyntax))]
        public static string LocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            return SyntaxNode(node.Declaration) + (node.SemicolonToken.Text == ";" ? ";\r\n" : "");
        }

        [ParsesType(typeof(BinaryExpressionSyntax))]
        public static string BinaryExpression(BinaryExpressionSyntax node)
        {
            var output = SyntaxNode(node.Left).Trim();
            output += " " + node.OperatorToken.Text + " ";
            output += SyntaxNode(node.Right).Trim();
            return output;
        }

        [ParsesType(typeof(ExpressionStatementSyntax))]
        public static string ExpressionStatement(ExpressionStatementSyntax node)
        {
            return SyntaxNode(node.Expression) + (node.SemicolonToken.Text == ";" ? ";\r\n" : "");
            //return node.ToString();
        }

        [ParsesType(typeof(ExpressionSyntax))]
        public static string Expression(ExpressionSyntax node)
        {
            if (node.IsKind(SyntaxKind.StringLiteralExpression))
            {
                return node.ToString();
            }
            return node.ToString();
        }

        [ParsesType(typeof(LiteralExpressionSyntax))]
        public static string LiteralExpression(LiteralExpressionSyntax node)
        {
            if (node.IsKind(SyntaxKind.CharacterLiteralExpression))
            {
                //this is sketch, probably shouldn't use char literals o.o
                return '"' + node.Token.ValueText.Replace("\\'", "'").Replace("\"", "\\\"") + '"';
            }
            return node.ToString();
        }
    }
}

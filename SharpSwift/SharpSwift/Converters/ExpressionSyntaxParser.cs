using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        //TODO: check this out
        [ParsesType(typeof(ExpressionSyntax))]
        public static string Expression(ExpressionSyntax node)
        {
            return node.ToString();
        }

        [ParsesType(typeof(MemberAccessExpressionSyntax))]
        public static string MemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            return SyntaxNode(node.Expression) + node.OperatorToken.Text + SyntaxNode(node.Name);
        }

        //something.Method("arg")
        [ParsesType(typeof(InvocationExpressionSyntax))]
        public static string InvocationExpression(InvocationExpressionSyntax node)
        {
            return SyntaxNode(node.Expression) + SyntaxNode(node.ArgumentList);
        }

        //new Something()
        [ParsesType(typeof(ObjectCreationExpressionSyntax))]
        public static string ObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            return SyntaxNode(node.Type) + SyntaxNode(node.ArgumentList);
        }

        //something (+/-/+=/etc) something_else
        [ParsesType(typeof(BinaryExpressionSyntax))]
        public static string BinaryExpression(BinaryExpressionSyntax node)
        {
            return SyntaxNode(node.Left) + " " + node.OperatorToken.Text + " " + SyntaxNode(node.Right);
        }

        [ParsesType(typeof(ExpressionStatementSyntax))]
        public static string ExpressionStatement(ExpressionStatementSyntax node)
        {
            return SyntaxNode(node.Expression) + Semicolon(node.SemicolonToken);
        }

        [ParsesType(typeof(LiteralExpressionSyntax))]
        public static string LiteralExpression(LiteralExpressionSyntax node)
        {
            switch (node.CSharpKind())
            {
                case SyntaxKind.CharacterLiteralExpression:
                    //this is sketch, probably shouldn't use char literals o.o
                    return '"' + node.Token.ValueText.Replace("\\'", "'").Replace("\"", "\\\"") + '"';
                default:
                    return node.ToString();
            }
        }

        [ParsesType(typeof(ImplicitArrayCreationExpressionSyntax))]
        public static string ImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node)
        {
            var output = "[ ";
            output += string.Join(", ", node.Initializer.Expressions.Select(SyntaxNode));
            return output + " ]";
        }

        [ParsesType(typeof(ArrayCreationExpressionSyntax))]
        public static string ArrayCreationExpression(ArrayCreationExpressionSyntax node)
        {
            var output = "[ ";
            output += string.Join(", ", node.Initializer.Expressions.Select(SyntaxNode));
            return output + " ]";
        }
    }
}

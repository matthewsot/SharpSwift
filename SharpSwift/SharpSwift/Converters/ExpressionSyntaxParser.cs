using System.Linq;
using Microsoft.CodeAnalysis;
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

        [ParsesType(typeof (PostfixUnaryExpressionSyntax))]
        public static string PostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            return SyntaxNode(node.Operand) + node.OperatorToken.Text; //TODO: double check this stuff
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
            //Name all the parameters
            //Thanks! http://stackoverflow.com/questions/24174602/get-constructor-declaration-from-objectcreationexpressionsyntax-with-roslyn/24191494#24191494
            var symbol = Model.GetSymbolInfo(node).Symbol as IMethodSymbol;

            var newArgumentListArguments = new SeparatedSyntaxList<ArgumentSyntax>();

            for (int i = 0; i < node.ArgumentList.Arguments.Count; i++)
            {
                var oldArgumentSyntax = node.ArgumentList.Arguments[i];
                var parameterName = symbol.Parameters[i].Name;

                var identifierSyntax = SyntaxFactory.IdentifierName(parameterName);
                var nameColonSyntax = SyntaxFactory
                    .NameColon(identifierSyntax)
                    .WithTrailingTrivia(SyntaxFactory.Whitespace(" "));

                var newArgumentSyntax = SyntaxFactory.Argument(
                    nameColonSyntax,
                    oldArgumentSyntax.RefOrOutKeyword,
                    oldArgumentSyntax.Expression);

                newArgumentListArguments = newArgumentListArguments.Add(newArgumentSyntax);
            }

            //NOTE: this takes out node.parent and everything, and probably screws with syntaxmodel stuff too
            var argList = SyntaxFactory.ArgumentList(newArgumentListArguments);
            var newNode = SyntaxFactory.ObjectCreationExpression(node.NewKeyword, node.Type, argList, node.Initializer);

            var output = SyntaxNode(node.Type) + SyntaxNode(newNode.ArgumentList);
            return output;
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

        [ParsesType(typeof(BaseExpressionSyntax))]
        public static string BaseExpression(BaseExpressionSyntax node)
        {
            return "super";
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

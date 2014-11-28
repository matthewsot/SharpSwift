using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    //TODO: Double check & add examples to all the expressions
    partial class ConvertToSwift
    {
        /// <summary>
        /// Converts an arbitrary expression to Swift
        /// </summary>
        /// <param name="expression">The expression to convert</param>
        /// <returns>The converted Swift expression.</returns>
        [ParsesType(typeof(ExpressionSyntax))]
        public static string Expression(ExpressionSyntax expression)
        {
            return expression.ToString();
        }

        /// <summary>
        /// Converts an expression statement (with semicolon) to Swift
        /// </summary>
        /// <param name="statement">The statement to convert</param>
        /// <returns>The converted Swift statement</returns>
        [ParsesType(typeof(ExpressionStatementSyntax))]
        public static string ExpressionStatement(ExpressionStatementSyntax statement)
        {
            return SyntaxNode(statement.Expression) + Semicolon(statement.SemicolonToken);
        }

        /// <summary>
        /// Converts a postfix unary expression to Swift
        /// </summary>
        /// <param name="expression">The expression to convert</param>
        /// <returns>The converted Swift expression</returns>
        [ParsesType(typeof (PostfixUnaryExpressionSyntax))]
        public static string PostfixUnaryExpression(PostfixUnaryExpressionSyntax expression)
        {
            return SyntaxNode(expression.Operand) + expression.OperatorToken.Text;
        }

        /// <summary>
        /// Converts a prefix unary expression to Swift
        /// </summary>
        /// <param name="expression">The expression to convert</param>
        /// <returns>The converted Swift expression</returns>
        [ParsesType(typeof(PrefixUnaryExpressionSyntax))]
        public static string PrefixUnaryExpression(PrefixUnaryExpressionSyntax expression)
        {
            return expression.OperatorToken.Text + SyntaxNode(expression.Operand);
        }

        /// <summary>
        /// Converts a member access expression to Swift
        /// </summary>
        /// <param name="expression">The expression to convert</param>
        /// <returns>The converted Swift expression</returns>
        [ParsesType(typeof(MemberAccessExpressionSyntax))]
        public static string MemberAccessExpression(MemberAccessExpressionSyntax expression)
        {
            return SyntaxNode(expression.Expression) + expression.OperatorToken.Text + SyntaxNode(expression.Name);
        }

        /// <summary>
        /// Converts an invocation expression to Swift
        /// </summary>
        /// <example>something.Method("arg")</example>
        /// <param name="invocation">The invocation expression to convert</param>
        /// <returns>The converted Swift code</returns>
        [ParsesType(typeof(InvocationExpressionSyntax))]
        public static string InvocationExpression(InvocationExpressionSyntax invocation)
        {
            return SyntaxNode(invocation.Expression) + SyntaxNode(invocation.ArgumentList);
        }

        /// <summary>
        /// Converts an object creation expression to Swift
        /// </summary>
        /// <example>new Something()</example>
        /// <param name="expression">The expression to convert</param>
        /// <returns>The converted Swift object creation expression</returns>
        [ParsesType(typeof(ObjectCreationExpressionSyntax))]
        public static string ObjectCreationExpression(ObjectCreationExpressionSyntax expression)
        {
            //Name all the arguments, since Swift usually requires named arguments when you create new objects.
            //Thanks! http://stackoverflow.com/questions/24174602/get-constructor-declaration-from-objectcreationexpressionsyntax-with-roslyn/24191494#24191494
            var symbol = Model.GetSymbolInfo(expression).Symbol as IMethodSymbol;

            var namedArgumentsList = new SeparatedSyntaxList<ArgumentSyntax>();

            for (var i = 0; i < expression.ArgumentList.Arguments.Count; i++)
            {
                var oldArgumentSyntax = expression.ArgumentList.Arguments[i];
                var parameterName = symbol.Parameters[i].Name;

                var nameColonSyntax = SyntaxFactory
                    .NameColon(SyntaxFactory.IdentifierName(parameterName))
                    .WithTrailingTrivia(SyntaxFactory.Whitespace(" "));

                var namedArgumentSyntax = SyntaxFactory.Argument(nameColonSyntax, oldArgumentSyntax.RefOrOutKeyword, oldArgumentSyntax.Expression);

                namedArgumentsList = namedArgumentsList.Add(namedArgumentSyntax);
            }

            //NOTE: this takes out expression.parent and everything, and probably screws with SyntaxModel stuff to
            return SyntaxNode(expression.Type) + SyntaxNode(SyntaxFactory.ArgumentList(namedArgumentsList));
        }

        /// <summary>
        /// Converts a binary expression to Swift
        /// </summary>
        /// <example>something (+/-/+=/etc) something_else</example>
        /// <param name="expression">The expression to convert</param>
        /// <returns>The converted Swift expression</returns>
        [ParsesType(typeof(BinaryExpressionSyntax))]
        public static string BinaryExpression(BinaryExpressionSyntax expression)
        {
            return SyntaxNode(expression.Left) + " " + expression.OperatorToken.Text + " " + SyntaxNode(expression.Right);
        }

        /// <summary>
        /// Converts a "base" expression to Swift
        /// </summary>
        /// <example>base</example>
        /// <param name="expression">The expression to convet</param>
        /// <returns>"super"</returns>
        [ParsesType(typeof(BaseExpressionSyntax))]
        public static string BaseExpression(BaseExpressionSyntax expression)
        {
            return "super";
        }

        /// <summary>
        /// Converts a C# literal expression to Swift
        /// </summary>
        /// <example>"hello!"</example>
        /// <example>123</example>
        /// <param name="expression">The literal expression to convert</param>
        /// <returns>The converted Swift literal</returns>
        [ParsesType(typeof(LiteralExpressionSyntax))]
        public static string LiteralExpression(LiteralExpressionSyntax expression)
        {
            switch (expression.CSharpKind())
            {
                //Swift doesn't use the same 'c' character literal syntax, instead you create a String and type annotate it as a Character
                case SyntaxKind.CharacterLiteralExpression:
                    //this is sketch, probably shouldn't use char literals o.o
                    return '"' + expression.Token.ValueText.Replace("\\'", "'").Replace("\"", "\\\"") + '"';
                default:
                    return expression.ToString();
            }
        }

        /// <summary>
        /// Converts an implicit array to Swift
        /// </summary>
        /// <example>new[] { 1, 2, 3 }</example>
        /// <param name="array">The implicit array to convert</param>
        /// <returns>The converted Swift array</returns>
        [ParsesType(typeof(ImplicitArrayCreationExpressionSyntax))]
        public static string ImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax array)
        {
            var output = "[ ";
            output += string.Join(", ", array.Initializer.Expressions.Select(SyntaxNode));
            return output + " ]";
        }

        /// <summary>
        /// Converts an explicit array to Swift
        /// </summary>
        /// <example>new string[] { 1, 2, 3 }</example>
        /// <param name="node">The explicit array to convert</param>
        /// <returns>The converted Swift array</returns>
        [ParsesType(typeof(ArrayCreationExpressionSyntax))]
        public static string ArrayCreationExpression(ArrayCreationExpressionSyntax node)
        {
            var output = "[ ";
            output += string.Join(", ", node.Initializer.Expressions.Select(SyntaxNode));
            return output + " ]";
        }
    }
}

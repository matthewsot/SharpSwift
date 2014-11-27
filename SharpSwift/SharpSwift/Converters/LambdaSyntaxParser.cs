using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        /// <summary>
        /// Converts a C# lambda expression to Swift
        /// </summary>
        /// <example>(a, b) => return a + b;</example>
        /// <param name="lambda">The expression to convert</param>
        /// <returns>The converted Swift expression</returns>
        //TODO: lambda != closure, detect when to use what resulting swift code
        [ParsesType(typeof(ParenthesizedLambdaExpressionSyntax))]
        public static string ParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax lambda)
        {
            var output = "{ ";
            output += SyntaxNode(lambda.ParameterList) + " in" + NewLine;
            return output + Block((BlockSyntax)lambda.Body, false) + NewLine + "}";
        }

        /// <summary>
        /// Converts a simple C# lambda expression to Swift
        /// </summary>
        /// <example>a => return a.ToString();</example>
        /// <param name="lambda">The expression to convert</param>
        /// <returns>The converted Swift expression</returns>
        [ParsesType(typeof(SimpleLambdaExpressionSyntax))]
        public static string SimpleLambdaExpression(SimpleLambdaExpressionSyntax lambda)
        {
            var output = "{ (" + lambda.Parameter.Identifier.Text;
            
            if (lambda.Parameter.Type != null)
            {
                output += ": " + SyntaxNode(lambda.Parameter.Type);
            }

            output += ") in" + NewLine;
            return output + Block((BlockSyntax)lambda.Body, false) + NewLine + "}";
        }
    }
}

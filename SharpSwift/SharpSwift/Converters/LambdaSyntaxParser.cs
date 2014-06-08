using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        [ParsesType(typeof(ParenthesizedLambdaExpressionSyntax))]
        public static string ParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            var output = "{ ";
            output += SyntaxNode(node.ParameterList);
            output += " in\r\n";
            output += Block((BlockSyntax)node.Body, false) + "\r\n";
            output += "}";
            return output;
        }

        [ParsesType(typeof(SimpleLambdaExpressionSyntax))]
        public static string SimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            return node.ToString();
        }
    }
}

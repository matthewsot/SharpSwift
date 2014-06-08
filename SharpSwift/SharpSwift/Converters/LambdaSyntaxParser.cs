using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        //TODO: lambda != closure, detect when to use what resulting swift code
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
            var output = "{ (";
            output += node.Parameter.Identifier.Text;
            if (node.Parameter.Type != null)
            {
                output += ": " + Type(node.Parameter.Type);
            }
            output += ") in\r\n";
            output += Block((BlockSyntax)node.Body, false) + "\r\n";
            output += "}";
            return output;
        }
    }
}

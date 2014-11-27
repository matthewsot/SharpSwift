using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        //(a, b) => return a + b;
        //TODO: lambda != closure, detect when to use what resulting swift code
        [ParsesType(typeof(ParenthesizedLambdaExpressionSyntax))]
        public static string ParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            var output = "{ ";
            output += SyntaxNode(node.ParameterList) + " in" + NewLine;
            return output + Block((BlockSyntax)node.Body, false) + NewLine + "}";
        }

        //a => return a.ToString();
        [ParsesType(typeof(SimpleLambdaExpressionSyntax))]
        public static string SimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            var output = "{ (" + node.Parameter.Identifier.Text;
            
            if (node.Parameter.Type != null)
            {
                output += ": " + SyntaxNode(node.Parameter.Type);
            }

            output += ") in" + NewLine;
            return output + Block((BlockSyntax)node.Body, false) + NewLine + "}";
        }
    }
}

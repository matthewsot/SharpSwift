using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        /// <summary>
        /// Converts a parameter to Swift
        /// </summary>
        /// <example>ParameterSyntax param = null</example>
        /// <param name="param">The parameter to convert</param>
        /// <returns>The converted Swift parameter</returns>
        [ParsesType(typeof(ParameterSyntax))]
        public static string Parameter(ParameterSyntax param)
        {
            if (param.Type == null) return param.Identifier.Text;

            if (param.Type is IdentifierNameSyntax)
            {
                return param.Identifier.Text + ": " + Type(((IdentifierNameSyntax)param.Type).Identifier.Text);
            }

            // TODO: Double check the variadic parameters handling
            if (param.Modifiers.Any(mod => mod.ToString() == "params"))
            {
                return param.Identifier.Text + ": " + SyntaxNode(((ArrayTypeSyntax)param.Type).ElementType) + "...";
            }

            return param.Identifier.Text + ": " + SyntaxNode(param.Type);
        }

        /// <summary>
        /// Converts a type parameter to Swift
        /// </summary>
        /// <example>Method<T: IEnumerable></typeparam></example>
        /// <param name="param">The parameter to convert</param>
        /// <returns>The converted Swift parameter</returns>
        [ParsesType(typeof(TypeParameterSyntax))]
        public static string TypeParameter(TypeParameterSyntax param)
        {
            if (!(param.Parent.Parent is MethodDeclarationSyntax)) return param.Identifier.Text;

            var typeConstraints = ((MethodDeclarationSyntax)param.Parent.Parent).ConstraintClauses;
            var constraints = typeConstraints
                .FirstOrDefault(constr => SyntaxNode(constr.Name) == param.Identifier.Text).Constraints;

            return param.Identifier.Text + ": " + string.Join(", ", constraints); //TODO: check if this is the right syntax for multiple constraints
        }
    }
}

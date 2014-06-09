using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        [ParsesType(typeof (ClassDeclarationSyntax))]
        public static string ClassDeclaration(ClassDeclarationSyntax classSyntax)
        {
            var output = "";
            output += "class " + classSyntax.Identifier.Text;

            //parse the base type, if there is one
            var baseType = classSyntax.BaseList.Types.OfType<IdentifierNameSyntax>().FirstOrDefault();
            if (baseType != null)
            {
                output += ": " + baseType;
            }
            output += " {\r\n";

            foreach (var member in classSyntax.Members)
            {
                output += SyntaxNode(member);
            }

            return output + "}\r\n";
        }

        [ParsesType(typeof (MethodDeclarationSyntax))]
        public static string MethodDeclaration(MethodDeclarationSyntax node)
        {
            var output = "func " + node.Identifier.Text + SyntaxNode(node.ParameterList) + " ";

            output += SyntaxNode(node.Body);

            return output;
        }

        [ParsesType(typeof (EnumDeclarationSyntax))]
        public static string EnumDeclaration(EnumDeclarationSyntax node)
        {
            var output = "enum " + node.Identifier.Text;
            foreach (var decl in node.ChildNodes().OfType<EnumMemberDeclarationSyntax>().Where(decl => decl.EqualsValue != null).Select(decl => decl.EqualsValue.Value))
            {
                if (decl.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    output += ": String";
                }
                else if (decl.IsKind(SyntaxKind.CharacterLiteralExpression))
                {
                    output += ": Character";
                }
                else if (decl.IsKind(SyntaxKind.NumericLiteralExpression))
                {
                    output += ": Int";
                }
                else
                {
                    continue;
                }
                break;
            }
            output += " {\r\n";
            foreach (var member in node.Members)
            {
                output += member.Identifier.Text;
                if (member.EqualsValue != null)
                {
                    output += " " + member.EqualsValue.EqualsToken.Text + " " + SyntaxNode(member.EqualsValue.Value);
                }
                output += ", \r\n";
            }
            output = output.TrimEnd().TrimEnd(',') + "\r\n";
            output += "}\r\n";
            return output;
        }

        [ParsesType(typeof(AccessorDeclarationSyntax))]
        public static string AccessorDeclaration(AccessorDeclarationSyntax node)
        {
            return SyntaxNode(node.Body);
        }

        [ParsesType(typeof(FieldDeclarationSyntax))]
        public static string FieldDeclaration(FieldDeclarationSyntax node)
        {
            return SyntaxNode(node.Declaration) + Semicolon(node.SemicolonToken);
        }

        [ParsesType(typeof (PropertyDeclarationSyntax))]
        public static string PropertyDeclaration(PropertyDeclarationSyntax node)
        {
            var output = "var " + node.Identifier.Text;
            output += ": " + Type(node.Type);
            //accessors not supported yet, basically makes a field
            return output + Semicolon(node.SemicolonToken);
        }

        [ParsesType(typeof (ConstructorDeclarationSyntax))]
        public static string ConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            var output = "init(";
            foreach (var parameter in node.ParameterList.Parameters)
            {
                output += parameter.Identifier.Text + ": " + Type(parameter.Type) + ", ";
            }

            output = output.Trim(' ', ',') + ") ";
            
            output += Block(node.Body);
            return output;
        }


        [ParsesType(typeof(DestructorDeclarationSyntax))]
        public static string DestructorDeclaration(DestructorDeclarationSyntax node)
        {
            return "deinit " + Block(node.Body);
        }
    }
}
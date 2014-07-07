using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        [ParsesType(typeof (ClassDeclarationSyntax))]
        public static string ClassDeclaration(ClassDeclarationSyntax node)
        {
            //Looks for an ExportAttribute
            string nameToUse = null;
            if (node.AttributeLists != null)
            {
                var attrLists = node.AttributeLists.FirstOrDefault(attrList => attrList.Attributes.Any(attr => attr.Name.ToString().Contains("Export")));
                if (attrLists != null)
                {
                    var exportAttr = attrLists.Attributes.First(attr => attr.Name.ToString().Contains("Export"));
                    nameToUse = SyntaxNode(exportAttr.ArgumentList.Arguments[0].Expression).Trim('"');
                }
            }

            var output = "";
            output += "class " + (nameToUse ?? node.Identifier.Text);

            //parse the base type, if there is one
            if (node.BaseList != null)
            {
                var baseType = node.BaseList.Types.OfType<IdentifierNameSyntax>().FirstOrDefault();
                output += ": " + SyntaxNode(baseType);
            }
            output += " {\r\n";

            output += string.Join("", node.Members.Select(SyntaxNode));

            return output + "}\r\n";
        }

        [ParsesType(typeof (MethodDeclarationSyntax))]
        public static string MethodDeclaration(MethodDeclarationSyntax node)
        {
            //Looks for an ExportAttribute
            string nameToUse = null;
            if(node.AttributeLists != null)
            {
                var attrLists = node.AttributeLists.FirstOrDefault(attrList => attrList.Attributes.Any(attr => attr.Name.ToString().Contains("Export")));
                if (attrLists != null)
                {
                    var exportAttr = attrLists.Attributes.First(attr => attr.Name.ToString().Contains("Export"));
                    nameToUse = SyntaxNode(exportAttr.ArgumentList.Arguments[0].Expression).Trim('"');
                }
            }

            var output = "func " + (nameToUse ?? node.Identifier.Text);

            if (node.TypeParameterList != null) //public string Something<T>
            {
                output += "<" + SyntaxNode(node.TypeParameterList) + ">";
            }

            output += SyntaxNode(node.ParameterList);

            if (node.ReturnType != null)
            {
                output += " -> " + SyntaxNode(node.ReturnType);
            }

            return output + " " + SyntaxNode(node.Body);
        }


        [ParsesType(typeof(EnumMemberDeclarationSyntax))]
        public static string EnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
            var output = node.Identifier.Text;
            if (node.EqualsValue != null)
            {
                output += " " + SyntaxNode(node.EqualsValue);
            }
            return output;
        }

        [ParsesType(typeof (EnumDeclarationSyntax))]
        public static string EnumDeclaration(EnumDeclarationSyntax node)
        {
            //TODO: parse EnumMemberDeclaration separately
            var output = "enum " + node.Identifier.Text;

            //Get the value of the enum
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

            output += string.Join(",\r\n", node.Members.Select(SyntaxNode)) + "\r\n}\r\n";
            return output;
        }

        [ParsesType(typeof(AccessorDeclarationSyntax))]
        public static string AccessorDeclaration(AccessorDeclarationSyntax node)
        {
            //TODO: implement this
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
            return "init" + SyntaxNode(node.ParameterList) + " " + Block(node.Body);
        }

        [ParsesType(typeof(DestructorDeclarationSyntax))]
        public static string DestructorDeclaration(DestructorDeclarationSyntax node)
        {
            return "deinit " + Block(node.Body);
        }
    }
}
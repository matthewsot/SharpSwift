using System;
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
        private static Tuple<string, string> ParseAttributes(SyntaxList<AttributeListSyntax> attributeLists)
        {
            var output = "";
            string exportAs = null;

            if (attributeLists.Any())
            {
                foreach (var attrList in attributeLists)
                {
                    foreach (var attribute in attrList.Attributes)
                    {
                        if (IsSharpSwiftAttribute(attribute, "ExportAttribute"))
                        {
                            exportAs = SyntaxNode(attribute.ArgumentList.Arguments[0].Expression).Trim().Trim('"');
                            continue;
                        }

                        output += SyntaxNode(attribute) + " ";
                    }
                }
            }

            return new Tuple<string, string>(output, exportAs);
        }

        //TODO: rewrite the MemberDeclarationParsers

        [ParsesType(typeof (ClassDeclarationSyntax))]
        public static string ClassDeclaration(ClassDeclarationSyntax node)
        {
            var parsedAttributes = ParseAttributes(node.AttributeLists);

            var output = parsedAttributes.Item1;
            var nameToUse = parsedAttributes.Item2;

            output += "class " + (nameToUse ?? node.Identifier.Text);

            //parse the base type, if there is one
            if (node.BaseList != null)
            {
                var baseType = node.BaseList.Types.OfType<IdentifierNameSyntax>().FirstOrDefault();
                output += ": " + SyntaxNode(baseType);
            }
            output += " {" + NewLine;

            output += string.Join("", node.Members.Select(SyntaxNode));

            return output + "}" + NewLine;
        }

        [ParsesType(typeof (MethodDeclarationSyntax))]
        public static string MethodDeclaration(MethodDeclarationSyntax node)
        {
            var parsedAttributes = ParseAttributes(node.AttributeLists);

            var output = parsedAttributes.Item1;
            var nameToUse = parsedAttributes.Item2;

            output += "func " + (nameToUse ?? node.Identifier.Text);

            if (node.TypeParameterList != null) //public string Something<T>
            {
                output += SyntaxNode(node.TypeParameterList);
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
            var parsedAttributes = ParseAttributes(node.AttributeLists);

            var output = parsedAttributes.Item1;
            var nameToUse = parsedAttributes.Item2;

            output += "enum " + (nameToUse ?? node.Identifier.Text);

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

            output += " {" + NewLine;

            output += string.Join("," + NewLine, node.Members.Select(SyntaxNode)) + NewLine + "}" + NewLine;
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
            var parsedAttributes = ParseAttributes(node.AttributeLists);

            var output = parsedAttributes.Item1;
            var nameToUse = parsedAttributes.Item2;

            //TODO: handle ExportAttribute
            return output + SyntaxNode(node.Declaration) + Semicolon(node.SemicolonToken);
        }

        [ParsesType(typeof (PropertyDeclarationSyntax))]
        public static string PropertyDeclaration(PropertyDeclarationSyntax node)
        {
            var parsedAttributes = ParseAttributes(node.AttributeLists);

            var output = parsedAttributes.Item1;
            var nameToUse = parsedAttributes.Item2;

            output += "var " + (nameToUse ?? node.Identifier.Text);
            output += ": " + SyntaxNode(node.Type);
            //accessors not supported yet, basically makes a field
            return output + Semicolon(node.SemicolonToken);
        }

        [ParsesType(typeof (ConstructorDeclarationSyntax))]
        public static string ConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            var parsedAttributes = ParseAttributes(node.AttributeLists);

            var output = parsedAttributes.Item1;

            return output + "init" + SyntaxNode(node.ParameterList) + " " + Block(node.Body);
        }

        [ParsesType(typeof(DestructorDeclarationSyntax))]
        public static string DestructorDeclaration(DestructorDeclarationSyntax node)
        {
            var parsedAttributes = ParseAttributes(node.AttributeLists);

            var output = parsedAttributes.Item1;

            return output + "deinit " + Block(node.Body);
        }
    }
}
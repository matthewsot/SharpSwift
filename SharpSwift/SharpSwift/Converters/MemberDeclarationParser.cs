using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        /// <summary>
        /// Parses an attribute
        /// </summary>
        /// <param name="attributeLists">The list of attributes to parse</param>
        /// <param name="separator">The separator to use between attributes</param>
        /// <returns>A tuple where the first value is the Swift attributes and the second contains the value of an ExportAttribute</returns>
        private static Tuple<string, string> ParseAttributes(SyntaxList<AttributeListSyntax> attributeLists, string separator = " ")
        {
            if (!attributeLists.Any()) return new Tuple<string, string>("", null);

            var output = "";
            string exportAs = null;

            foreach (var attribute in attributeLists.SelectMany(attrList => attrList.Attributes))
            {
                if (IsSharpSwiftAttribute(attribute, "ExportAttribute"))
                {
                    exportAs = SyntaxNode(attribute.ArgumentList.Arguments[0].Expression).Trim().Trim('"');
                    continue;
                }

                output += SyntaxNode(attribute) + separator;
            }

            return new Tuple<string, string>(output, exportAs);
        }

        /// <summary>
        /// Converts member declaration modifiers to Swift
        /// NOTE that internal will be converted to public, as Swift doesn't have an internal modifier
        /// </summary>
        /// <example>public readonly</example>
        /// <param name="modifiers">The modifiers to convert</param>
        /// <returns>The converted Swift modifiers</returns>
        private static string ParseModifiers(SyntaxTokenList modifiers)
        {
            return string.Join(" ", modifiers.Select(modifier => 
                modifier.Text == "internal" ? "public" : modifier.Text)) + " ";
        }

        /// <summary>
        /// Converts a class declaration to Swift
        /// </summary>
        /// <example>public class SomeClass { }</example>
        /// <param name="declaration">The class to convert</param>
        /// <returns>The converted Swift class</returns>
        [ParsesType(typeof (ClassDeclarationSyntax))]
        public static string ClassDeclaration(ClassDeclarationSyntax declaration)
        {
            var parsedAttributes = ParseAttributes(declaration.AttributeLists, NewLine);

            var output = parsedAttributes.Item1;
            var nameToUse = parsedAttributes.Item2;

            output += "class " + (nameToUse ?? declaration.Identifier.Text);

            //parse the base type, if there is one
            if (declaration.BaseList != null)
            {
                var baseType = declaration.BaseList.Types.OfType<IdentifierNameSyntax>().FirstOrDefault();
                output += ": " + SyntaxNode(baseType);
            }
            output += " {" + NewLine;

            output += string.Join("", declaration.Members.Select(SyntaxNode));

            return output + "}" + NewLine + NewLine;
        }

        /// <summary>
        /// Converts a method to Swift
        /// </summary>
        /// <example>public void Something() { }</example>
        /// <param name="method">The method to convert</param>
        /// <returns>The converted Swift method</returns>
        [ParsesType(typeof (MethodDeclarationSyntax))]
        public static string MethodDeclaration(MethodDeclarationSyntax method)
        {
            var parsedAttributes = ParseAttributes(method.AttributeLists);

            var output = parsedAttributes.Item1;
            var nameToUse = parsedAttributes.Item2;

            output += ParseModifiers(method.Modifiers);

            output += "func " + (nameToUse ?? method.Identifier.Text);

            if (method.TypeParameterList != null) //public string Something<T>
            {
                output += SyntaxNode(method.TypeParameterList);
            }

            output += SyntaxNode(method.ParameterList);

            if (method.ReturnType != null)
            {
                output += " -> " + SyntaxNode(method.ReturnType);
            }

            return output + " " + SyntaxNode(method.Body);
        }

        /// <summary>
        /// Converts an enum member to Swift
        /// </summary>
        /// <example>name = 1</example>
        /// <param name="declaration">The declaration to convert</param>
        /// <returns>The converted Swift declaration</returns>
        [ParsesType(typeof(EnumMemberDeclarationSyntax))]
        public static string EnumMemberDeclaration(EnumMemberDeclarationSyntax declaration)
        {
            var output = declaration.Identifier.Text;
            if (declaration.EqualsValue != null)
            {
                output += " " + SyntaxNode(declaration.EqualsValue);
            }
            return output;
        }

        /// <summary>
        /// Converts an enum to Swift
        /// </summary>
        /// <example>public enum thing { }</example>
        /// <param name="declaration">The enum declaration to convert</param>
        /// <returns>The converted Swift enum</returns>
        [ParsesType(typeof (EnumDeclarationSyntax))]
        public static string EnumDeclaration(EnumDeclarationSyntax declaration)
        {
            var parsedAttributes = ParseAttributes(declaration.AttributeLists);

            var output = parsedAttributes.Item1;
            var nameToUse = parsedAttributes.Item2;

            output += "enum " + (nameToUse ?? declaration.Identifier.Text);

            //Get the value of the enum
            foreach (var decl in declaration.ChildNodes().OfType<EnumMemberDeclarationSyntax>().Where(decl => decl.EqualsValue != null).Select(decl => decl.EqualsValue.Value))
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

            return output + " {" + NewLine +
                             string.Join("," + NewLine, declaration.Members.Select(SyntaxNode)) + NewLine +
                             "}" + NewLine + NewLine;
        }

        [ParsesType(typeof(AccessorDeclarationSyntax))]
        public static string AccessorDeclaration(AccessorDeclarationSyntax node)
        {
            //TODO: implement this
            return SyntaxNode(node.Body);
        }

        /// <summary>
        /// Converts a field declaration to Swift
        /// </summary>
        /// <param name="declaration">The declarationt to convert</param>
        /// <returns>The converted Swift code</returns>
        [ParsesType(typeof(FieldDeclarationSyntax))]
        public static string FieldDeclaration(FieldDeclarationSyntax declaration)
        {
            var parsedAttributes = ParseAttributes(declaration.AttributeLists);

            var output = parsedAttributes.Item1;
            var nameToUse = parsedAttributes.Item2;

            //TODO: handle ExportAttribute
            return output + SyntaxNode(declaration.Declaration) + Semicolon(declaration.SemicolonToken);
        }

        /// <summary>
        /// Converts a property declaration to Swift
        /// </summary>
        /// <param name="declaration">The declaration to convert</param>
        /// <returns>The converted Swift code</returns>
        [ParsesType(typeof (PropertyDeclarationSyntax))]
        public static string PropertyDeclaration(PropertyDeclarationSyntax declaration)
        {
            var parsedAttributes = ParseAttributes(declaration.AttributeLists);

            var output = parsedAttributes.Item1;
            var nameToUse = parsedAttributes.Item2;

            output += "var " + (nameToUse ?? declaration.Identifier.Text);
            output += ": " + SyntaxNode(declaration.Type);
            //accessors not supported yet, basically makes a field
            return output + Semicolon(declaration.SemicolonToken);
        }

        /// <summary>
        /// Converts a constructor to Swift
        /// </summary>
        /// <param name="constructor">The constructor to convert</param>
        /// <returns>The converted Swift constructor</returns>
        [ParsesType(typeof (ConstructorDeclarationSyntax))]
        public static string ConstructorDeclaration(ConstructorDeclarationSyntax constructor)
        {
            var parsedAttributes = ParseAttributes(constructor.AttributeLists);

            var output = parsedAttributes.Item1;

            return output + "init" + SyntaxNode(constructor.ParameterList) + " " + Block(constructor.Body);
        }

        /// <summary>
        /// Converts a destructor to Swift
        /// </summary>
        /// <param name="destructor">The destructor to convert</param>
        /// <returns>The converted Swift destructor</returns>
        [ParsesType(typeof(DestructorDeclarationSyntax))]
        public static string DestructorDeclaration(DestructorDeclarationSyntax destructor)
        {
            var parsedAttributes = ParseAttributes(destructor.AttributeLists);

            var output = parsedAttributes.Item1;

            return output + "deinit " + Block(destructor.Body);
        }
    }
}
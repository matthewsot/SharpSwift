using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var output = "func " + node.Identifier.Text + "(";
            foreach (var parameter in node.ParameterList.Parameters)
            {
                output += parameter.Identifier.Text + ": " + Type(parameter.Type) + ", ";
            }

            output = output.Trim(' ', ',') + ") {\r\n";

            foreach (var member in node.ChildNodes())
            {
                output += SyntaxNode(member);
            }

            output += "}\r\n";
            return output;
        }

        [ParsesType(typeof (EnumMemberDeclarationSyntax))]
        public static string EnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
            return node.ToString();
        }


        [ParsesType(typeof(AccessorDeclarationSyntax))]
        public static string AccessorDeclaration(AccessorDeclarationSyntax node)
        {
            return SyntaxNode(node.Body);
        }

        [ParsesType(typeof(VariableDeclarationSyntax))]
        public static string VariableDeclaration(VariableDeclarationSyntax node)
        {
            var thisVar = node.Variables.First(); //Only one var at a time for now
            var output = "var " + thisVar.Identifier.Text;
            if (!node.Type.IsVar)
            {
                output += ": " + Type(node.Type);
            }
            if (thisVar.Initializer == null)
            {
                return output + ";\r\n";
            }

            output += " " + SyntaxNode(thisVar.Initializer);
            return output;
        }

        [ParsesType(typeof(FieldDeclarationSyntax))]
        public static string FieldDeclaration(FieldDeclarationSyntax node)
        {
            var output = SyntaxNode(node.Declaration);
            return output.Trim().TrimEnd(';') + ";\r\n";
        }

        [ParsesType(typeof (PropertyDeclarationSyntax))]
        public static string PropertyDeclaration(PropertyDeclarationSyntax node)
        {
            var output = "var " + node.Identifier.Text;
            output += ": " + Type(node.Type);
            //accessors not supported yet, basically makes a field
            return output.Trim().TrimEnd(';') + ";\r\n";
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
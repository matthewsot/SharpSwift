using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpSwift.Converters; 

namespace SharpSwift
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = @"C:\Users\Matthew\GitHub\SharpSwift\SharpSwift\SharpSwift\test.cs";
            //file = Console.ReadLine();

            var output = "";
            var tree = CSharpSyntaxTree.ParseFile(file);
            var root = (CompilationUnitSyntax)tree.GetRoot();
            var rootNamespace = root.Members.OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
            var classes = rootNamespace.Members.OfType<ClassDeclarationSyntax>();

            foreach (var childClass in classes)
            {
                output += ConvertToSwift.SyntaxNode(childClass);
            }


            Console.ReadLine();
        }
    }
}

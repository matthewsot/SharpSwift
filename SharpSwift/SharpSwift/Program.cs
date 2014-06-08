using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpSwift.Converters; 

namespace SharpSwift
{
    class Program
    {
        static string ParseFile(string path, bool doIndent = true)
        {
            Console.WriteLine("Parsing file " + path);

            var output = "//Converted with SharpSwift - https://github.com/matthewsot/SharpSwift\r\n";
            output += "//See https://github.com/matthewsot/DNSwift FMI about these includes\r\n\r\n";
            output += "include DNSwift;\r\n";
            var tree = CSharpSyntaxTree.ParseFile(path);
            var root = (CompilationUnitSyntax)tree.GetRoot();
            var rootNamespace = root.Members.OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
            var classes = rootNamespace.Members.OfType<ClassDeclarationSyntax>();

            foreach (var usingDecl in root.Usings)
            {
                if (usingDecl.ToString().StartsWith("System"))
                {
                    output += "DNSwift." + usingDecl + ";";
                }

                if (usingDecl.HasLeadingTrivia) continue;

                foreach (var trivia in usingDecl.GetLeadingTrivia())
                {
                    if (!trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) &&
                        !trivia.IsKind(SyntaxKind.SingleLineCommentTrivia)) continue;

                    var comment = trivia.ToString().TrimStart('/', '*').Trim();
                    if (comment.StartsWith("include"))
                    {
                        output += comment + "\r\n";
                    }
                }
            }
            output += "\r\n";

            foreach (var childClass in classes)
            {
                output += ConvertToSwift.SyntaxNode(childClass);
            }

            return doIndent ? Indenter.IndentDocument(output) : output;
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Must specify an input file");
                return;
            }

            var path = args[0].Trim('"');
            var doIndent = true;
            string outPath = null;
            if (args.Length > 1)
            {
                doIndent = args[1] != "-noindent";
                outPath = doIndent ? args[1] : null;
            }
            if (args.Length > 2)
            {
                outPath = outPath ?? args[2];
            }

            if (Directory.Exists(path))
            {
                //It's a folder
                foreach (var file in Directory.GetFiles(path))
                {
                    if (!file.EndsWith(".cs"))
                        continue;

                    var parsed = ParseFile(file, doIndent);

                    var outputPath = outPath ?? file.Replace(".cs", ".swift");
                    if (!outputPath.EndsWith(".swift"))
                    {
                        outputPath = outputPath.TrimEnd('\\') + "\\" + file.Split('\\').Last().Replace(".cs", ".swift");
                    }

                    using (var writer = new StreamWriter(outputPath))
                    {
                        writer.Write(parsed);
                        writer.Flush();
                    }
                }
            }
            else if (File.Exists(path) && path.EndsWith(".cs"))
            {
                //It's a file
                var parsed = ParseFile(path, doIndent);

                var outputPath = outPath ?? path.Replace(".cs", ".swift");

                using (var writer = new StreamWriter(outputPath))
                {
                    writer.Write(parsed);
                    writer.Flush();
                }
            }
            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}

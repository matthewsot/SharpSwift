using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using SharpSwift.Converters; 

namespace SharpSwift
{
    class Program
    {
        static void Main(string[] args)
        {
            var argData = new ArgData(args);
            if (!argData.Clean())
            {
                Console.WriteLine("Sorry, there was a fatal error with your arguments");
                return;
            }

            var parsed = ParseFile(argData.InputPath, argData.SlnPath, argData.Indent);
            using (var writer = new StreamWriter(argData.OutputPath))
            {
                writer.Write(parsed);
                writer.Flush();
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        private static Document GetDocumentFromSolution(string solutionPath, string documentPath)
        {
            var workspace = MSBuildWorkspace.Create();
            var solution = workspace.OpenSolutionAsync(solutionPath).Result;

            return solution.Projects.SelectMany(project => project.Documents)
                                    .FirstOrDefault(document => document.FilePath.EndsWith(documentPath));
        }



        static string GetImportsFromTrivia(SyntaxTriviaList triviaList)
        {
            var output = "";
            foreach (var trivia in triviaList)
            {
                if (!trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) &&
                    !trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                    continue;

                var comment = trivia.ToString().TrimStart('/', '*').Trim();
                if (comment.StartsWith("import"))
                {
                    output += comment + "" + ConvertToSwift.NewLine;
                }
            }
            return output;
        }

        static string ParseFile(string path, string solutionPath, bool doIndent = true)
        {
            Console.WriteLine("Parsing file " + path);

            var output = "//Converted with SharpSwift - https://github.com/matthewsot/SharpSwift" + ConvertToSwift.NewLine;
            output += "//See https://github.com/matthewsot/DNSwift FMI about these imports" + ConvertToSwift.NewLine + ConvertToSwift.NewLine;
            output += "import DNSwift;" + ConvertToSwift.NewLine;

            Document doc = null;
            if (solutionPath != null)
            {
                doc = GetDocumentFromSolution(solutionPath, path);
            }

            var tree = CSharpSyntaxTree.ParseFile(path);

            if (doc != null)
            {
                SemanticModel model = doc.GetSemanticModelAsync().Result;
                if (model != null)
                {
                    ConvertToSwift.model = model;
                }

                tree = doc.GetSyntaxTreeAsync().Result;
                if (tree == null)
                {
                    tree = CSharpSyntaxTree.ParseFile(path);
                }
            }

            var root = (CompilationUnitSyntax)tree.GetRoot();
            var rootNamespace = root.Members.OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
            var classes = rootNamespace.Members.OfType<ClassDeclarationSyntax>();

            foreach (var usingDir in root.Usings)
            {
                if (!usingDir.Name.ToString().StartsWith("SharpSwift."))
                {
                    output += "import " + usingDir.Name.ToString().Replace(".", "") + ";\r\n";
                }
                output += GetImportsFromTrivia(usingDir.GetLeadingTrivia());
            }

            output += GetImportsFromTrivia(root.Usings.Last().GetTrailingTrivia()); //in case they added imports to the bottom
            output += GetImportsFromTrivia(rootNamespace.GetLeadingTrivia());
            output += "" + ConvertToSwift.NewLine;

            foreach (var childClass in classes)
            {
                output += ConvertToSwift.SyntaxNode(childClass);
            }

            return doIndent ? Indenter.IndentDocument(output) : output;
        }
    }
}

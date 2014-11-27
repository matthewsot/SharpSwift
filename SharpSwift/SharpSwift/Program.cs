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
            return triviaList
                        .Where(trivia => trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) || trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                        .Select(trivia => trivia.ToString().TrimStart('/', '*').Trim())
                        .Where(comment => comment.StartsWith("import"))
                        .Aggregate("", (current, comment) => current + (comment + ConvertToSwift.NewLine));
        }

        static string ParseFile(string path, string solutionPath, bool doIndent)
        {
            Console.WriteLine("Parsing file " + path);

            var output = "// Converted with SharpSwift - https://github.com/matthewsot/SharpSwift" + ConvertToSwift.NewLine;

            var doc = GetDocumentFromSolution(solutionPath, path);
            var tree = doc.GetSyntaxTreeAsync().Result;
            ConvertToSwift.Model = doc.GetSemanticModelAsync().Result;

            var root = (CompilationUnitSyntax)tree.GetRoot();
            var rootNamespace = root.Members.OfType<NamespaceDeclarationSyntax>().FirstOrDefault();

            // Search for "//import XYZ" in the usings to output in the final Swift code
            output = root.Usings.Aggregate(output, (current, usingDir) => current + GetImportsFromTrivia(usingDir.GetLeadingTrivia()));
            output += GetImportsFromTrivia(root.Usings.Last().GetTrailingTrivia());
            output += GetImportsFromTrivia(rootNamespace.GetLeadingTrivia());
            output += "" + ConvertToSwift.NewLine;

            // Parses each class in the file into output.
            output = rootNamespace.Members.OfType<ClassDeclarationSyntax>()
                                        .Aggregate(output, (current, childClass) =>
                                                           current + ConvertToSwift.SyntaxNode(childClass));

            return doIndent ? Indenter.IndentDocument(output) : output;
        }
    }
}

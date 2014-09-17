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
        private static Document GetDocumentFromSolution(string solutionPath, string documentPath)
        {
            var workspace = MSBuildWorkspace.Create();
            var solution = workspace.OpenSolutionAsync(solutionPath).Result;

            foreach (var document in solution.Projects.SelectMany(project => project.Documents))
            {
                if (document.FilePath.EndsWith(documentPath))
                {
                    return document;
                }
            }
            return null;
        }

        static string FindSolution(string path, int levels = 0)
        {
            if (File.Exists(path))
            {
                path = (new FileInfo(path)).DirectoryName;
            }

            if (levels == 5) //too much recursion is bad o:
            {
                return "";
            }

            foreach (var file in Directory.GetFiles(path))
            {
                if (file.EndsWith(".sln"))
                {
                    return file;
                }
            }

            return FindSolution((new DirectoryInfo(path)).Parent.FullName);
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

        static void Main(string[] args)
        {
            var inPath = args.FirstOrDefault(arg => !arg.StartsWith("-"));
            if(args.Contains("-input"))
            {
                inPath = args[args.ToList().IndexOf("-input") + 1];
            }

            var doIndent = !args.Contains("-noindent");

            var outPath = args.ToList().FirstOrDefault(arg => arg != inPath && !arg.StartsWith("-"));
            if (args.Contains("-output"))
            {
                outPath = args[args.ToList().IndexOf("-output") + 1];
            }

            if (inPath == null)
            {
                Console.WriteLine("You must specify an input file");
                return;
            }

            var solution = args.Contains("-solution") ? args[args.ToList().IndexOf("-solution") + 1] : FindSolution(inPath);

            inPath = inPath.Trim('"');
            outPath = (outPath == null) ? null : outPath.Trim('"');

            if (Directory.Exists(inPath))
            {
                //It's a folder
                foreach (var file in Directory.GetFiles(inPath))
                {
                    if (!file.EndsWith(".cs"))
                        continue;

                    var parsed = ParseFile(file, solution, doIndent);

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
            else if (File.Exists(inPath) && inPath.EndsWith(".cs"))
            {
                //It's a file
                var parsed = ParseFile(inPath, solution, doIndent);

                var outputPath = outPath ?? inPath.Replace(".cs", ".swift");

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

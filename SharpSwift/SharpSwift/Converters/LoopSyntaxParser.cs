using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpSwift.Converters
{
    partial class ConvertToSwift
    {
        [ParsesType(typeof(ForEachStatementSyntax))]
        public static string ForEachStatement(ForEachStatementSyntax node)
        {
            var output = "for ";
            output += node.Identifier.Text; //TODO: should we do anything to this?
            output += " in " + SyntaxNode(node.Expression) + " " + SyntaxNode(node.Statement);
            return output;
        }

        [ParsesType(typeof (ForStatementSyntax))]
        public static string ForStatement(ForStatementSyntax node)
        {
            var output = "for ";
            output += SyntaxNode(node.Declaration) + "; " + SyntaxNode(node.Condition) + "; " + //TODO: these semicolons should be handled in their syntaxParsers
                      SyntaxNode(node.Incrementors.First()).TrimEnd(); //TODO: handle multiple incrementors
            output += " " + SyntaxNode(node.Statement);
            return output;
        }

        [ParsesType(typeof (SwitchSectionSyntax))]
        public static string SwitchSection(SwitchSectionSyntax node)
        {
            var output = "case " + SyntaxNode(node.Labels.First().Value) + ":\r\n"; //todo multiple labels
            if (node.Labels.First().IsKind(SyntaxKind.DefaultSwitchLabel))
            {
                output = "default:\r\n";
            }

            foreach (var statement in node.Statements)
            {
                if (statement is BreakStatementSyntax)
                {
                    break;
                }
                output += "    " + SyntaxNode(statement); //TODO: fix the tabbing here with Indenter.cs
            }
            return output;
        }

        //TODO: this isn't in the right place
        [ParsesType(typeof(SwitchStatementSyntax))]
        public static string SwitchStatement(SwitchStatementSyntax node)
        {
            var output = "switch ";
            output += SyntaxNode(node.Expression) + " {\r\n";
            foreach (var sect in node.Sections)
            {
                output += SyntaxNode(sect);
            }
            output += "}";
            return output;
        }
    }
}
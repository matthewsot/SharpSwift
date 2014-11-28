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
        /// Converts a switch statement to Swift
        /// </summary>
        /// <example>switch(z) { case 12: ... break; }</example>
        /// <param name="statement">The statement to convert</param>
        /// <returns>The converted Swift statement</returns>
        [ParsesType(typeof(SwitchStatementSyntax))]
        public static string SwitchStatement(SwitchStatementSyntax statement)
        {
            var output = "switch ";
            output += SyntaxNode(statement.Expression) + " {" + NewLine;

            output = statement.Sections.Aggregate(output, (current, sect) => current + SyntaxNode(sect));

            return output + "}" + NewLine;
        }

        /// <summary>
        /// Converts a switch label to Swift
        /// </summary>
        /// <example>case "123":</example>
        /// <param name="label">The label to convert</param>
        /// <returns>The converted Swift label</returns>
        //TODO: Handle multiple labels
        [ParsesType(typeof(SwitchLabelSyntax))]
        public static string SwitchLabel(SwitchLabelSyntax label)
        {
            if (label.CaseOrDefaultKeyword.IsKind(SyntaxKind.CaseKeyword))
            {
                return "case " + SyntaxNode(label.Value) + ":" + NewLine;
            }
            if (label.CaseOrDefaultKeyword.IsKind(SyntaxKind.DefaultKeyword))
            {
                return "default:" + NewLine;
            }
            return "";
        }

        /// <summary>
        /// Converts a switch section to Swift
        /// </summary>
        /// <param name="section">The section to convert</param>
        /// <returns>The converted Swift section</returns>
        [ParsesType(typeof(SwitchSectionSyntax))]
        public static string SwitchSection(SwitchSectionSyntax section)
        {
            var output = section.Labels.Aggregate("", (current, label) => current + SyntaxNode(label));

            return section.Statements.TakeWhile(statement => !(statement is BreakStatementSyntax)) // Swift doesn't use break; statements.
                .Aggregate(output, (current, statement) => current + ("    " + SyntaxNode(statement))); //TODO: Handle case/switch indenting in Indenter.cs
        }

        /// <summary>
        /// Converts an if statement to Swift
        /// </summary>
        /// <example>if (x == y) { }</example>
        /// <param name="statement">The statement to convert</param>
        /// <returns>The converted Swift statement</returns>
        [ParsesType(typeof(IfStatementSyntax))]
        public static string IfStatement(IfStatementSyntax statement)
        {
            var output = statement.IfKeyword.Text + " (";
            output += SyntaxNode(statement.Condition);
            output += ")" + NewLine + SyntaxNode(statement.Statement);
            if (statement.Else != null)
            {
                output += SyntaxNode(statement.Else);
            }
            return output;
        }

        /// <summary>
        /// Converts an else clause to Swift
        /// </summary>
        /// <example>else { }</example>
        /// <param name="clause">The else clause to convert</param>
        /// <returns>The converted Swift clause</returns>
        [ParsesType(typeof(ElseClauseSyntax))]
        public static string ElseClause(ElseClauseSyntax clause)
        {
            var output = clause.ElseKeyword.Text + " ";
            output += SyntaxNode(clause.Statement);
            return output;
        }
    }
}

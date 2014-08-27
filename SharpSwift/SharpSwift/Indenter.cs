using System;

namespace SharpSwift
{
    internal class Indenter
    {
        public static string IndentDocument(string swift, string newLine = null, string indentWith = "\t")
        {
            if (newLine == null)
            {
                newLine = Environment.NewLine;
            }

            var output = "";
            var lines = swift.Split(new[] {"\n", "\r\n", "\r", Environment.NewLine, newLine}, StringSplitOptions.None);
            var currIndent = "";
            foreach (var line in lines)
            {
                if (line.Contains("}") && !line.Contains("{"))
                {
                    currIndent = currIndent.Substring(indentWith.Length);
                }

                output += currIndent + line + newLine;

                if (line.Contains("{") && !line.Contains("}"))
                {
                    currIndent += indentWith;
                }
            }

            return output.Trim() + newLine;
        }
    }
}

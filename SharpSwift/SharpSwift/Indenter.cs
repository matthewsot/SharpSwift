using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSwift
{
    internal class Indenter
    {
        public static string IndentDocument(string swift)
        {
            var output = "";
            var lines = swift.Split('\n');
            var currIndent = "";

            foreach (var line in lines)
            {
                if (line.Contains("}") && !line.Contains("{"))
                {
                    currIndent = currIndent.Substring(1);
                }

                output += currIndent + line;

                if (line.Contains("{") && !line.Contains("}"))
                {
                    currIndent += '\t';
                }
            }

            return output;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSwift
{
    class ArgData
    {
        /// <summary>
        /// Path to the .sln file
        /// </summary>
        public string SlnPath { get; set; }

        /// <summary>
        /// Path to the file to parse
        /// </summary>
        public string InputPath { get; set; }

        public bool Indent { get; set; }

        public string OutputPath { get; set; }

        public ArgData(string[] args)
        {
            Indent = true;
            string namedArgument = null;

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg.StartsWith("-"))
                {
                    namedArgument = arg.ToLower().Trim('-');
                    if (namedArgument == "no-indent")
                    {
                        Indent = false;
                    }
                    continue;
                }

                if (namedArgument != null)
                {
                    switch (namedArgument)
                    {
                        case "input":
                        case "i":
                            InputPath = arg;
                            break;
                        case "solution":
                        case "sln":
                        case "s":
                            SlnPath = arg;
                            break;
                        case "output":
                        case "o":
                            OutputPath = arg;
                            break;
                    }
                    namedArgument = null;
                    continue;
                }

                switch (i)
                {
                    case 0:
                        InputPath = arg;
                        break;
                    case 1:
                        OutputPath = arg;
                        break;
                    case 2:
                        SlnPath = arg;
                        break;
                }
            }
        }

        private string FindSolution(string path, int levels)
        {
            if (File.Exists(path))
            {
                path = (new FileInfo(path)).DirectoryName;
            }

            if (levels == 0) //too much recursion is bad o:
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

            return FindSolution((new DirectoryInfo(path)).Parent.FullName, levels - 1);
        }

        public bool Clean()
        {
            if (!File.Exists(InputPath) || !InputPath.EndsWith(".cs"))
            {
                return false;
            }

            SlnPath = SlnPath ?? FindSolution(InputPath, 5);
            if (!File.Exists(SlnPath) || !SlnPath.EndsWith(".sln"))
            {
                return false;
            }

            OutputPath = OutputPath ?? InputPath.Substring(0, (InputPath.Length - ".cs".Length)) + ".swift";

            return OutputPath.EndsWith(".swift");
        }
    }
}

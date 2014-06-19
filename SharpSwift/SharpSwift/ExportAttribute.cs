using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSwift
{
    class ExportAttribute : Attribute
    {
        public string ExportAs { get; set; }
        public ExportAttribute(string exportAs)
        {
            ExportAs = exportAs;
        }
    }
}

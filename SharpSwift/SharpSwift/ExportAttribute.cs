using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSwift.Attributes
{
    class ExportAttribute : Attribute
    {
        public string ExportAs { get; set; }
        public ExportAttribute(string exportAs)
        {
            ExportAs = exportAs;
        }
    }

    internal class TestAttribute : Attribute
    {
        public string ExportAs { get; set; }

        public TestAttribute()
        {
        }
    }
}

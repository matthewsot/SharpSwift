using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSwift.Attributes
{
    internal class TestAttribute : Attribute
    {
        public string ExportAs { get; set; }

        public TestAttribute()
        {
        }
    }
}

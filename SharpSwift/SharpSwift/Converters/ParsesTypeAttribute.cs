using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSwift.Converters
{
    class ParsesTypeAttribute : Attribute
    {
        public Type ParsesType { get; set; }

        public ParsesTypeAttribute(Type parsesType)
        {
            ParsesType = parsesType;
        }
    }
}

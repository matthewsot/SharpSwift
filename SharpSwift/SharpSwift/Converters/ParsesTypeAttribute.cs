using System;

namespace SharpSwift.Converters
{
    class ParsesTypeAttribute : Attribute
    {
        public Type ParsesType { get; private set; }

        public ParsesTypeAttribute(Type parsesType)
        {
            ParsesType = parsesType;
        }
    }
}

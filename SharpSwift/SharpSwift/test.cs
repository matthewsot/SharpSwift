using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSwift
{
    class test : ASCIIEncoding
    {
        private string somdething = "123";
        private static string _something { get; set; }

        public static string something
        {
            get { return "hi"; }
            set { _something = value; }
        }

        public test(string something)
        {
            //IGNORE
            var y = new test("hello");
            //raw: hello there!
            var str = somdething;
            str = str.Trim('1') + "hello";
            str += "hi there!";
            str = "";
        }
    }
}

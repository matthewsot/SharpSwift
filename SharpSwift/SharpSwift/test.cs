using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//include Something.Else;
using SharpSwift.Attributes;

namespace SharpSwift
{
    [Export("anotherClass")]
    class AnotherClass
    {
        [Export("something")]
        public static void SomeThing<T>() where T : IEnumerable<string>, IEnumerator<bool>
        {
            return;
        }
    }
    class test : ASCIIEncoding
    {
        /*private string somdething = "123";
        //private static string _something { get; set; }

        [Export("doSomething")]
        private static T DoSomething<T>(T input, params string[] pms)
        {
            return input;
        }

        enum Something
        {
            Some = 1,
            Another,
            Third = 3
        }*/

        [Test] private string something = "123";
        
        [Export("Test")]
        public test(string something)
        {
            var m = new AnotherClass();
            const string constant = "123";
            List<int> intArray = new int[] {1, 2, 3}.ToList();
            string[] strArray = new[] {"hi", "bye", "why"};
            foreach (var str in strArray)
            {
                
            }

            for (var i = 0; i < 10; i++)
            {
                var md = i;
            }

            switch (constant)
            {
                case "123":
                    var f = 1;
                    var mdf = 3;
                    break;
                case "456":
                case "754":
                    var d = 2;
                    break;
                case "643":
                    var c = 3;
                    break;
                default:
                    break;
            }
            /*
            var yd = DoSomething("hello", "something", "another");
            var ints = new int[] {0, 1, 2};
            var y = new test("hello");
            string x, z = "123";
            using (var reader = new StreamReader(""))
            {
                var d = "1232";
            }
            /*
            //IGNORE
            var y = new test("hello");
            //raw: hello there!
            var str = somdething;
            str = str.Trim('1') + "hello";
            str += "hi there!";
            str = "";
            //DoSomething((a, b) => { return a; });
            char one = '1';
            if (one == '1')
            {
                return;
            }
            else if (one == '2')
            {
                return;
            }
            else
            {
                return;
            }
            using (var yz = new StreamReader(""))
            {
                
            }
            */
        }
    }
}

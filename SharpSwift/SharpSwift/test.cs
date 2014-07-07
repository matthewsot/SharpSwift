using System;
using System.IO;
using System.Text;
//include Something.Else;

namespace SharpSwift
{
    [Export("anotherClass")]
    class AnotherClass
    {
        [Export("something")]
        public static void SomeThing()
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
        
        [Export("Test")]
        public test(string something)
        {
            AnotherClass.SomeThing();
            var m = new AnotherClass();
            const string constant = "123";
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

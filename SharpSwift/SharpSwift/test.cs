﻿using System;
using System.IO;
using System.Text;
//include Something.Else;

namespace SharpSwift
{
    class test : ASCIIEncoding
    {
        private string somdething = "123";
        //private static string _something { get; set; }

        private static T DoSomething<T>(Func<string, int, string> del, T input)
        {
            return input;
        }

        enum Something
        {
            Some = 1,
            Another,
            Third = 3
        }

        public test(string something)
        {
            var ints = new[] {0, 1, 2};
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

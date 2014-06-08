using System.IO;
using System.Text;
//include Something.Else;

namespace SharpSwift
{
    class test : ASCIIEncoding
    {
        private string somdething = "123";
        private static string _something { get; set; }

        private delegate string someDel(string a, int b);

        private static void DoSomething(someDel del)
        {
            
        }

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
            DoSomething((a, b) => { return a; });
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
        }
    }
}

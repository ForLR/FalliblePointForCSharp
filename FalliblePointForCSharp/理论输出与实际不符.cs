using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalliblePointForCSharp
{
    public class 理论输出与实际不符
    {
        static void Main(string[] args)
        {
            Foo foo = new Foo
            {
                name = "A",
                temp = new Temp
                {
                    id = 1
                }
            };
            //Test(foo);
            var temp = Test1(foo);
            Console.WriteLine(foo.name);//理论输出C  实际输出B
            Console.WriteLine(foo.temp.id);

            Console.WriteLine(temp.name);
            Console.WriteLine(temp.temp.id);
            Console.ReadKey();
        }
        static void Test(Foo foo)
        {
            foo.name = "B";
            foo.temp = new Temp { id = 2 };
            foo = new Foo
            {
                name = "C",
                temp = new Temp { id = 3 }
            };
        }
        static Foo Test1(Foo foo)
        {
            foo.name = "B";
            foo.temp = new Temp { id = 2 };
            foo = new Foo
            {
                name = "C",
                temp = new Temp { id = 3 }
            };
            return foo;
        }
        public class Foo
        {
            public string name { get; set; }
            public Temp temp { get; set; }
        }
        public class Temp
        {
            public int id { get; set; }
        }
    }
}

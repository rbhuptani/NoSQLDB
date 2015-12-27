using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class X
    {
        public void methodx()
        {
            Console.WriteLine("Some method X");
        }
    }
    class Program
    {
        public static bool checkMethod(Type t, MethodInfo M)
        {
            foreach(var m in t.GetMethods())
            {
                if (m == M)
                    return true;
            }
            return false;
        }
        static void Main(string[] args)
        {
            X x1 = new X();
            MethodInfo mi = typeof(X).GetMethod("methodx"); //x.methos.getInfo();
            Type tx = x1.GetType();
            Console.WriteLine(checkMethod(tx, mi));
        }
    }
}

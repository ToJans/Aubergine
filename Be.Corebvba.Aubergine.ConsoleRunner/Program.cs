using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Corebvba.Aubergine.Runner;

namespace Be.Corebvba.Aubergine.ConsoleRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowUsage();
                return;
            }

            var r = new RunnerImpl();
            var fi = new System.IO.FileInfo(args[0]);
            var types = System.Reflection.Assembly.LoadFile(fi.FullName).GetTypes();
            PrettyPrint(r.RunStories(types),"");

            Console.ReadLine();
        }

        static void PrettyPrint(IEnumerable<ITestResult> r, string spacesprepend)
        {
            if (r==null) return;
            foreach (var v in r)
            {
                if (spacesprepend == "")
                {
                    Console.WriteLine("==STORY================================================================");
                }

                Console.Write(spacesprepend);
                Console.Write(v.Description);
                Console.Write(" => ");
                switch (v.Status)
                {
                    case null:
                        Console.WriteLine("IMPLEMENTATION ERROR");
                        break;
                    case true:
                        Console.WriteLine("OK");
                        break;
                    case false:
                        Console.WriteLine("NOK");
                        break;
                }
                if (!string.IsNullOrEmpty(v.ExtraStatusInfo))
                    Console.WriteLine(spacesprepend+" INFO : " + v.ExtraStatusInfo );

                if (spacesprepend == "")
                {
                    Console.WriteLine("========================================================================");
                }

                PrettyPrint(v.Children, spacesprepend + "   ");
            }
        }

        static void ShowUsage()
        {
            Console.WriteLine(@"
Aubergine Console Runner - Core bvba - Tom Janssens 2009

Usage :
Be.Corebvba.Aubergine.ConsoleRunner xxxx.DLL
");
        }
    }
}

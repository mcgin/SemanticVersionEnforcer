using System;
using System.IO;

namespace SemanticVersionEnforcer
{
    public class Program
    {
        public static int Main(String[] args)
        {
            if (args.Length != 2)
            {
                PrintUsage();
                return 1;
            }

            string file1 = args[0];
            string file2 = args[1];
            if (!File.Exists(file1))
            {
                Console.Error.WriteLine("Error the file {0} dies not exist", file1);
                return 1;
            }
            if (!File.Exists(file2))
            {
                Console.Error.WriteLine("Error the file {0} dies not exist", file2);
                return 1;
            }
            return 0;
        }

        private static void PrintUsage()
        {
            Console.Error.WriteLine("Error you must provide 2 package locations as arguments");
            Console.Error.WriteLine("Usage: SemanticVersionEnforcer.exe oldPackage.nupkg newPackage.nupkg");
        }
    }
}
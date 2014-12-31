using System;
using System.IO;
using SemanticVersionEnforcer.Core;

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
            var checker = SemanticVersionCheckerFactory.NewInstance();

            Version version;
            try
            {
                version = checker.DetermineCorrectSemanticVersion(args[0], args[1]);
            }
            catch (FileNotFoundException e)
            {
                Console.Error.WriteLine(e.Message);
                return 1;
            }

            Console.WriteLine(version);
            return 0;
        }

        private static void PrintUsage()
        {
            Console.Error.WriteLine("Error you must provide 2 package locations as arguments");
            Console.Error.WriteLine("Usage: SemanticVersionEnforcer.exe oldPackage.nupkg newPackage.nupkg");
        }
    }
}
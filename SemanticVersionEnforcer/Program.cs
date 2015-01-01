using System;
using System.IO;
using SemanticVersionEnforcer.Core;

namespace SemanticVersionEnforcer
{
    public class Program
    {
        public static int Main(String[] args)
        {
            if ( args.Length < 1 )
            {
                PrintUsage();
                return 1;
            }
            var checker = SemanticVersionCheckerFactory.NewInstance();

            Version version;
            try
            {
                version = args.Length==1?checker.DetermineCorrectSemanticVersion(args[0]) : checker.DetermineCorrectSemanticVersion(args[1], args[0]);
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
            Console.Error.WriteLine("Error you must provide at least one package location as an argument");
            Console.Error.WriteLine("Usage: SemanticVersionEnforcer.exe newPackage.nupkg [oldPackage.nupkg]");
        }
    }
}
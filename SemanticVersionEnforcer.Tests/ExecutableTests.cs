using System;
using System.IO;
using NUnit.Framework;

namespace SemanticVersionEnforcer.Tests
{
    class ExecutableTests
    {
        [Test]
        public void GivenTheExecutableFile_WhenIRunItWithNoArguments_ThenTheUsageIsDisplayed()
        {
            StringWriter sw = new StringWriter();
            Console.SetError(sw);

            var exitCode = Program.Main(new string[0]);

            Assert.IsTrue(sw.ToString().Contains("Usage: SemanticVersionEnforcer.exe oldPackage.nupkg newPackage.nupkg"), "The usage message is not correct");
            Assert.AreEqual(1,exitCode, "Program exit code is incorrect");
        }

        [Test]
        public void GivenTheExecutableFile_WhenIRunItWithNoArguments_ThenAnAppropriateErrorMessageIsPrinted()
        {
            StringWriter sw = new StringWriter();
            Console.SetError(sw);

            var exitCode = Program.Main(new string[0]);

            Assert.IsTrue(sw.ToString().Contains("Error you must provide 2 package locations as arguments"), "The error message is not correct");
            Assert.AreEqual(1, exitCode, "Program exit code is incorrect");
        }

        [Test]
        public void GivenTheExecutableFile_WhenIRunItWithOneArgument_ThenAnAppropriateErrorMessageIsPrinted()
        {
            StringWriter sw = new StringWriter();
            Console.SetError(sw);

            var exitCode = Program.Main(new string[1] { "arg1" });

            Assert.IsTrue(sw.ToString().Contains("Error you must provide 2 package locations as arguments"), "The error message is not correct");
            Assert.AreEqual(1, exitCode, "Program exit code is incorrect");
        }

        [Test]
        public void GivenTheExecutableFile_WhenIRunItWithTwoFilesThatDoNotExist_ThenAnAppropriateErrorMessageIsPrinted()
        {
            const string filename = "fileName";
            StringWriter sw = new StringWriter();
            Console.SetError(sw);

            var exitCode = Program.Main(new string[2] { filename, filename });

            string console = sw.ToString();
            Assert.AreEqual(1, exitCode, "Program exit code is incorrect");
            //TODO Use a regex
            Assert.IsTrue(console.Contains(String.Format("Could not find file")), "The error message is not correct: {0}", console);
            Assert.IsTrue(console.Contains(String.Format(filename)), "The error message is not correct: {0}", console);
        }
    }
}

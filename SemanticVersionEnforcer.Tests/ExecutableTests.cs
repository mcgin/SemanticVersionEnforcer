using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
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
            string filename = "fileName";
            StringWriter sw = new StringWriter();
            Console.SetError(sw);

            var exitCode = Program.Main(new string[2] { filename, filename });

            Assert.AreEqual(1, exitCode, "Program exit code is incorrect");
            Assert.IsTrue(sw.ToString().Contains(String.Format("Error the file {0} dies not exist", filename)), "The error message is not correct");
        }

    }
}

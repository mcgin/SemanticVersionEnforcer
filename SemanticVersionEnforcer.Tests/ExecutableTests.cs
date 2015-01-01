using System;
using System.IO;
using NUnit.Framework;

namespace SemanticVersionEnforcer.Tests
{
    internal class ExecutableTests
    {
        [Test]
        public void GivenTheExecutableFile_WhenIRunItWithNoArguments_ThenTheUsageIsDisplayed()
        {
            var sw = new StringWriter();
            Console.SetError(sw);

            var exitCode = Program.Main(new string[0]);

            var errorMessage = sw.ToString();
            Assert.IsTrue(
                errorMessage.Contains("Usage: SemanticVersionEnforcer.exe newPackage.nupkg [oldPackage.nupkg]"),
                "The usage message is not correct {0}", errorMessage);
            Assert.AreEqual(1, exitCode, "Program exit code is incorrect");
        }

        [Test]
        public void GivenTheExecutableFile_WhenIRunItWithNoArguments_ThenAnAppropriateErrorMessageIsPrinted()
        {
            var sw = new StringWriter();
            Console.SetError(sw);

            var exitCode = Program.Main(new string[0]);

            var errorMessage = sw.ToString();
            Assert.IsTrue(errorMessage.Contains("Error you must provide at least one package location as an argument"),
                "The error message is not correct: {0}", errorMessage);
            Assert.AreEqual(1, exitCode, "Program exit code is incorrect");
        }

        [Test]
        [Category("Integration")]
        public void GivenTheExecutableFile_WhenIRunItWithAValidPackageAsAnArguemnt_ThenItReturnsTheCorrectVersion()
        {
            var sw = new StringWriter();
            Console.SetOut(sw);

            var exitCode = Program.Main(new string[1] { "TestData/SemanticVersionEnforcer.1.0.1.0.nupkg" });

            var result = Decimal.Parse(sw.ToString());
            Assert.IsTrue(result > 1.0m);
            Assert.AreEqual(0, exitCode, "Program exit code is incorrect");
        }

        [Test]
        public void GivenTheExecutableFile_WhenIRunItWithTwoValidPackagesAsAnArguemnt_ThenItReturnsTheCorrectVersion()
        {
            var sw = new StringWriter();
            Console.SetOut(sw);

            var exitCode = Program.Main(new string[2] { "TestData/SemanticVersionEnforcer.2.2.0.0.nupkg", "TestData/SemanticVersionEnforcer.1.0.1.0.nupkg" });

            var result = Decimal.Parse(sw.ToString());
            Assert.AreEqual(result, 2.0m, "Version Number is incorrect");
            Assert.AreEqual(0, exitCode, "Program exit code is incorrect");
        }

        [Test]
        public void GivenTheExecutableFile_WhenIRunItWithTwoFilesThatDoNotExist_ThenAnAppropriateErrorMessageIsPrinted()
        {
            const string filename = "fileName";
            var sw = new StringWriter();
            Console.SetError(sw);

            var exitCode = Program.Main(new string[2] {filename, filename});

            var console = sw.ToString();
            Assert.AreEqual(1, exitCode, "Program exit code is incorrect");
            //TODO Use a regex
            Assert.IsTrue(console.Contains(String.Format("Could not find file")),
                "The error message is not correct: {0}", console);
            Assert.IsTrue(console.Contains(String.Format(filename)), "The error message is not correct: {0}", console);
        }
    }
}
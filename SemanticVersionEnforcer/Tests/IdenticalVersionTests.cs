using System.IO;
using System.Net.Security;
using System.Reflection;
using System.Reflection.Emit;
using NuGet;
using NuGet.Runtime;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.CodeDom;

namespace SemanticVersionEnforcer.Tests
{
    class IdenticalVersionTests :SemanticVersionBase
    {
        #region SameVersion

        #region Abstract Classes
        [Test]
        public void GivenTwoPackages_WhenTheyBothContainTheSameAbstractClass_ItShouldHaveTheySameVersionNumbers()
        {
            String oldSource = "public abstract class abstractClass { public abstract void blah(); }";
            String newSource = "public abstract class abstractClass { public abstract void blah(); }";

            int oldMajor = 2;
            int oldMinor = 3;
            int newMajor = 2;
            int newMinor = 3;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> { oldSource }, new List<String> { newSource }, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);

            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(2, 3), checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));

        }

        #endregion

        #region Interfaces
        [Test]
        public void GivenTwoPackages_WhenTheyBothContainTheSameInterfaces_ItShouldHaveTheSameVersion()
        {
            String oldSource = "interface x { string GetSomething(); }";
            String newSource = "interface x { string GetSomething(); }";

            int oldMajor = 2;
            int oldMinor = 3;
            int newMajor = 2;
            int newMinor = 3;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> { oldSource }, new List<String> { newSource }, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);

            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(2, 3), checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }
        #endregion

        #region Private Methods
        [Test]
        public void GivenTwoPackages_WhenTheNewerOneContainsAdditionalPrivateMethods_ItShouldHaveTheSameVersion()
        {
            String oldSource = "public class B { public void hello() { int x=7; } }";
            String newSource = "public class B { public void hello() { int x=7; } private void hello2() { int x=7; } }";

            int oldMajor = 2;
            int oldMinor = 3;
            int newMajor = 2;
            int newMinor = 3;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> { oldSource }, new List<String> { newSource }, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);

            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(2, 3), checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }
        #endregion

        #region Private Classes

        [Test]
        public void GivenTwoPackages_WhenTheNewerOneContainsAdditionalInternalClasses_ItShouldHaveTheSameVersion()
        {
            String oldSource = "public class B { public void hello() { int x=7; } }";
            String newSource = "public class B { public void hello() { int x=7; } }";
            String newSource2 = "internal class C { public void hellc() { int x=7; } }";

            int oldMajor = 2;
            int oldMinor = 3;
            int newMajor = 2;
            int newMinor = 3;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> { oldSource }, new List<String> { newSource, newSource2 }, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);

            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(2, 3), checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }
        #endregion

        #region DLLs read from filesystem
        [Test]
        public void GivenTwoIdenticalPackages_WhenTheSemanticVersionIsCalculated_ItShouldBeTheSameVersionAsTheOldOne()
        {
            //IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("C:\\Users\\xxx\\AppData\\Local\\NuGet\\Cache");
            //IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");
            //IPackage oldPackage = repo.FindPackage("NUnit", new SemanticVersion("2.6.2"));
            //IPackage newPackage = repo.FindPackage("NUnit", new SemanticVersion("2.6.2"));
            //Console.WriteLine(oldPackage.Version.Version.Major);
            //Console.WriteLine(oldPackage.Version.Version.Minor);
            //Console.WriteLine(oldPackage.Version.Version.Build);
            //New public methods in package => new minor version
            //Removal of public method in package => new major version

            Mock<IPackageFile> mockDll = new Mock<IPackageFile>();
            mockDll.Setup(dll => dll.EffectivePath).Returns("test.dll");
            mockDll.Setup(dll => dll.GetStream()).Returns(File.Open("Tests/TestData/nunit.framework.2.6.2.dll", FileMode.Open));

            Mock<IPackage> mockPackage = new Mock<IPackage>();
            mockPackage.Setup(p => p.GetFiles()).Returns(new List<IPackageFile> { mockDll.Object });
            mockPackage.Setup(p => p.Version).Returns(new SemanticVersion(2, 6, 2, 0));

            Mock<IPackageFile> newMockDll = new Mock<IPackageFile>();
            newMockDll.Setup(dll => dll.EffectivePath).Returns("test.dll");
            newMockDll.Setup(dll => dll.GetStream()).Returns(File.Open("Tests/TestData/nunit.framework.copy.2.6.2.dll", FileMode.Open));//For some reason I can't get it to work using the same mock

            Mock<IPackage> newMockPackage = new Mock<IPackage>();
            newMockPackage.Setup(p => p.GetFiles()).Returns(new List<IPackageFile> { newMockDll.Object });

            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(2, 6), checker.DetermineCorrectSemanticVersion(mockPackage.Object, newMockPackage.Object));

        }
        #endregion

        [Test]
        public void GivenTwoPackages_WhenTheyAreTheSame_ItShouldHaveTheSameMajorAndMinorVersion()
        {
            String oldSource = "public class B { public void hello() { int x=7; } }";
            String newSource = "public class B { public void hello() { int x=7; } }";

            int oldMajor = 2;
            int oldMinor = 3;
            int newMajor = 2;
            int newMinor = 3;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> { oldSource }, new List<String> { newSource }, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);

            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(2, 3), checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }
        #endregion
    }
}

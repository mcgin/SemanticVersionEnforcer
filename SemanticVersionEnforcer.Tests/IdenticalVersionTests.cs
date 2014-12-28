using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using NuGet;
using NUnit.Framework;
using SemanticVersionEnforcer.Core;

namespace SemanticVersionEnforcer.Tests
{
    class IdenticalVersionTests : SemanticVersionBase
    {
        #region Abstract Classes
        [Test]
        public void GivenTwoPackages_WhenTheyBothContainTheSameAbstractClass_ItShouldHaveTheySameVersionNumbers()
        {
            const string oldSource = "public abstract class abstractClass { public abstract void blah(); }";
            const string newSource = "public abstract class abstractClass { public abstract void blah(); }";

            const int oldMajor = 2;
            const int oldMinor = 3;
            const int newMajor = 2;
            const int newMinor = 3;

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
            const String oldSource = "interface x { string GetSomething(); }";
            const String newSource = "interface x { string GetSomething(); }";

            const int oldMajor = 2;
            const int oldMinor = 3;
            const int newMajor = 2;
            const int newMinor = 3;

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
            const String oldSource = "public class B { public void hello() { int x=7; } }";
            const String newSource = "public class B { public void hello() { int x=7; } private void hello2() { int x=7; } }";

            const int oldMajor = 2;
            const int oldMinor = 3;
            const int newMajor = 2;
            const int newMinor = 3;

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
            const String oldSource = "public class B { public void hello() { int x=7; } }";
            const String newSource = "public class B { public void hello() { int x=7; } }";
            const String newSource2 = "internal class C { public void hellc() { int x=7; } }";

            const int oldMajor = 2;
            const int oldMinor = 3;
            const int newMajor = 2;
            const int newMinor = 3;

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
            mockDll.Setup(dll => dll.GetStream()).Returns(File.Open("TestData/nunit.framework.2.6.2.dll", FileMode.Open));

            Mock<IPackage> mockPackage = new Mock<IPackage>();
            mockPackage.Setup(p => p.GetFiles()).Returns(new List<IPackageFile> { mockDll.Object });
            mockPackage.Setup(p => p.Version).Returns(new SemanticVersion(2, 6, 2, 0));

            Mock<IPackageFile> newMockDll = new Mock<IPackageFile>();
            newMockDll.Setup(dll => dll.EffectivePath).Returns("test.dll");
            newMockDll.Setup(dll => dll.GetStream()).Returns(File.Open("TestData/nunit.framework.copy.2.6.2.dll", FileMode.Open));//For some reason I can't get it to work using the same mock

            Mock<IPackage> newMockPackage = new Mock<IPackage>();
            newMockPackage.Setup(p => p.GetFiles()).Returns(new List<IPackageFile> { newMockDll.Object });

            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(2, 6), checker.DetermineCorrectSemanticVersion(mockPackage.Object, newMockPackage.Object));

        }
        [Test]
        public void GivenTwoIdenticalPackages_WhenTheSemanticVersionIsCalculatedByPassingTheFilesAsStrings_ItShouldBeTheSameVersionAsTheOldOne()
        {
            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(1, 0), checker.DetermineCorrectSemanticVersion("TestData/SemanticVersionEnforcer.1.0.1.0.nupkg", "TestData/SemanticVersionEnforcer.1.0.1.0.nupkg"));

        }
        #endregion

        [Test]
        public void GivenTwoPackages_WhenTheyAreTheSame_ItShouldHaveTheSameMajorAndMinorVersion()
        {
            const String oldSource = "public class B { public void hello() { int x=7; } }";
            const String newSource = "public class B { public void hello() { int x=7; } }";

            const int oldMajor = 2;
            const int oldMinor = 3;
            const int newMajor = 2;
            const int newMinor = 3;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> { oldSource }, new List<String> { newSource }, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);

            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(2, 3), checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }
    }
}

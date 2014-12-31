using System;
using System.Collections.Generic;
using Moq;
using NuGet;
using NUnit.Framework;
using SemanticVersionEnforcer.Core;

namespace SemanticVersionEnforcer.Tests
{
    internal class MajorVersionTests : SemanticVersionBase
    {
        #region Increment Major Version

        #region Different packages

        [Test]
        public void GivenTwoPackages_WhenTheyAreCompletelyDifferent_ItShouldIncrementTheMajorVersionAndResetTheMinor()
        {
            const String oldSource = "public class B { public void hello() { int x=7; } }";
            const String newSource = "public class C { public void bye() { int x=7; } }";

            const int oldMajor = 2;
            const int oldMinor = 3;
            const int newMajor = 3;
            const int newMinor = 0;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> {oldSource}, new List<String> {newSource}, oldMajor, oldMinor, newMajor,
                newMinor, out oldPackage, out newPackage);

            var checker = SemanticVersionCheckerFactory.NewInstance();
            Assert.AreEqual(new Version(3, 0),
                checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }

        #endregion

        #region Public Methods

        [Test]
        public void
            GivenTwoPackages_WhenTheOlderOneContainsAdditionalPublicMethods_ItShouldIncrementTheMajorAndResetTheMinor()
        {
            const String oldSource =
                "public class B { public void hello() { int x=7; } public void hello2() { int x=7; } public void hello3() { int x=7; } }";
            const String newSource =
                "public class B { public void hello() { int x=7; } public void hello2() { int x=7; } }";

            const int oldMajor = 2;
            const int oldMinor = 3;
            const int newMajor = 3;
            const int newMinor = 0;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> {oldSource}, new List<String> {newSource}, oldMajor, oldMinor, newMajor,
                newMinor, out oldPackage, out newPackage);

            var checker = SemanticVersionCheckerFactory.NewInstance();
            Assert.AreEqual(new Version(3, 0),
                checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }

        #endregion

        #region Interfaces

        [Test]
        public void
            GivenTwoPackages_WhenTheOlderOneContainsAdditionalInterfaces_ItShouldIncrementTheMajorAndResetTheMinor()
        {
            const String oldSource1 = "interface x { string GetSomething(); }";
            const String oldSource2 =
                "public class B { public void hello() { int x=7; } public void hello2() { int x=7; } }";
            const String newSource =
                "public class B { public void hello() { int x=7; } public void hello2() { int x=7; } }";

            const int oldMajor = 2;
            const int oldMinor = 3;
            const int newMajor = 3;
            const int newMinor = 0;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> {oldSource1, oldSource2}, new List<String> {newSource}, oldMajor, oldMinor,
                newMajor, newMinor, out oldPackage, out newPackage);

            var checker = SemanticVersionCheckerFactory.NewInstance();
            Assert.AreEqual(new Version(3, 0),
                checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }

        [Test]
        public void
            GivenTwoPackages_WhenTheOlderOneContainsAdditionalMethodsOnACommonInterface_ItShouldIncrementTheMajorAndResetTheMinor
            ()
        {
            const String oldSource1 = "interface x { string GetSomething(); string GetSomething2(); }";
            const String newSource = "interface x { string GetSomething();  }";

            const int oldMajor = 2;
            const int oldMinor = 3;
            const int newMajor = 3;
            const int newMinor = 0;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> {oldSource1}, new List<String> {newSource}, oldMajor, oldMinor, newMajor,
                newMinor, out oldPackage, out newPackage);

            var checker = SemanticVersionCheckerFactory.NewInstance();
            Assert.AreEqual(new Version(3, 0),
                checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }

        #endregion

        #region Abstract Classes

        [Test]
        public void
            GivenTwoPackages_WhenTheOlderOneContainsAdditionalPublicAbstractClass_ItShouldIncrementTheMajorAndResetTheMinor
            ()
        {
            const String oldSource1 = "public abstract class abstractClass { public abstract void blah(); }";
            const String oldSource2 =
                "public class B { public void hello() { int x=7; } public void hello2() { int x=7; } }";
            const String newSource =
                "public class B { public void hello() { int x=7; } public void hello2() { int x=7; } }";

            const int oldMajor = 2;
            const int oldMinor = 3;
            const int newMajor = 3;
            const int newMinor = 0;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> {oldSource1, oldSource2}, new List<String> {newSource}, oldMajor, oldMinor,
                newMajor, newMinor, out oldPackage, out newPackage);

            var checker = SemanticVersionCheckerFactory.NewInstance();
            Assert.AreEqual(new Version(3, 0),
                checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }

        [Test]
        public void
            GivenTwoPackages_WhenTheOlderOneContainsAdditionalPublicMethodsInAnAbstractClass_ItShouldIncrementTheMajorAndResetTheMinor
            ()
        {
            const String oldSource1 =
                "public abstract class abstractClass { public abstract void blah(); public abstract void blahBlah(); }";
            const String oldSource2 =
                "public class B { public void hello() { int x=7; } public void hello2() { int x=7; } }";
            const String newSource1 = "public abstract class abstractClass2 { public abstract void blah(); }";
            const String newSource2 =
                "public class B { public void hello() { int x=7; } public void hello2() { int x=7; } }";

            const int oldMajor = 2;
            const int oldMinor = 3;
            const int newMajor = 3;
            const int newMinor = 0;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> {oldSource1, oldSource2}, new List<String> {newSource1, newSource2}, oldMajor,
                oldMinor, newMajor, newMinor, out oldPackage, out newPackage);

            var checker = SemanticVersionCheckerFactory.NewInstance();
            Assert.AreEqual(new Version(3, 0),
                checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }

        #endregion

        #endregion
    }
}
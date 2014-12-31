using System;
using System.Collections.Generic;
using Moq;
using NuGet;
using NUnit.Framework;
using SemanticVersionEnforcer.Core;

namespace SemanticVersionEnforcer.Tests
{
    internal class MinorVersionTests : SemanticVersionBase
    {
        #region MinorVersion Should Increment

        #region Abstract Classes

        [Test]
        public void
            GivenTwoPackages_WhenTheNewerOneContainsAdditionalAbstractClasses_ItShouldHaveItsMinorVersionIncremented()
        {
            const String oldSource = "public class B { public void hello() { int x=7; } }";
            const String newSource1 = "public class B { public void hello() { int x=7; } }";
            const String newSource2 = "public abstract class C { public void hello() { int x=7; } }";

            const int oldMajor = 2;
            const int oldMinor = 3;
            const int newMajor = 2;
            const int newMinor = 4;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> {oldSource}, new List<String> {newSource1, newSource2}, oldMajor, oldMinor,
                newMajor, newMinor, out oldPackage, out newPackage);


            var checker = SemanticVersionCheckerFactory.NewInstance();
            Assert.AreEqual(new Version(2, 4),
                checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }

        [Test]
        public void
            GivenTwoPackages_WhenTheNewerOneContainsAdditionalAbstractMethods_ItShouldHaveItsMinorVersionIncremented()
        {
            const String oldSource = "public abstract class B { public void hello() { int x=7; } }";
            const String newSource =
                "public abstract class B { public void hello() { int x=7; } public abstract void abstractHello(); }";

            const int oldMajor = 2;
            const int oldMinor = 3;
            const int newMajor = 2;
            const int newMinor = 4;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(oldSource, newSource, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);

            var checker = SemanticVersionCheckerFactory.NewInstance();
            Assert.AreEqual(new Version(2, 4),
                checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }

        #endregion

        #region Regular methods/classes

        [Test]
        public void
            GivenTwoPackages_WhenTheNewerOneContainsAdditionalPublicMethods_ItShouldHaveItsMinorVersionIncremented()
        {
            const String oldSource = "public class B { public void hello() { int x=7; } }";
            const String newSource =
                "public class B { public void hello() { int x=7; }public void hello2() { int x=7; } }";

            const int oldMajor = 2;
            const int oldMinor = 3;
            const int newMajor = 2;
            const int newMinor = 4;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(oldSource, newSource, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);

            var checker = SemanticVersionCheckerFactory.NewInstance();
            Assert.AreEqual(new Version(2, 4),
                checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }

        [Test]
        public void GivenTwoPackages_WhenTheNewerOneContainsAdditionalPublicClasses_ItShouldIncrementTheMinorVersion()
        {
            const String oldSource = "public class B { public void hello() { int x=7; } }";
            const String newSource = "public class B { public void hello() { int x=7; } }";
            const String newSource2 = "public class C { public void hellc() { int x=7; } }";

            const int oldMajor = 2;
            const int oldMinor = 3;
            const int newMajor = 2;
            const int newMinor = 4;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> {oldSource}, new List<String> {newSource, newSource2}, oldMajor, oldMinor,
                newMajor, newMinor, out oldPackage, out newPackage);


            var checker = SemanticVersionCheckerFactory.NewInstance();
            Assert.AreEqual(new Version(2, 4),
                checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }

        #endregion

        #region Interfaces

        [Test]
        public void
            GivenTwoPackages_WhenTheNewOneContainsTheSameInterfaceWithAdditonalMethods_ItShouldIncrementTheMinorVersion()
        {
            const String oldSource = "interface x { string GetSomething(); }";
            const String newSource = "interface x { string GetSomething(); int SomethingElse();}";

            const int oldMajor = 2;
            const int oldMinor = 3;
            const int newMajor = 2;
            const int newMinor = 4;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> {oldSource}, new List<String> {newSource}, oldMajor, oldMinor, newMajor,
                newMinor, out oldPackage, out newPackage);

            var checker = SemanticVersionCheckerFactory.NewInstance();
            Assert.AreEqual(new Version(newMajor, newMinor),
                checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }

        [Test]
        public void GivenTwoPackages_WhenTheNewOneContainsAnAdditionalInterface_ItShouldIncrementTheMinorVersion()
        {
            const String oldSource = "interface x { string GetSomething(); }";
            const String newSource = "interface x { string GetSomething(); }";
            const String newSource2 = "interface y { string GetSomethingElse(); }";

            const int oldMajor = 2;
            const int oldMinor = 3;
            const int newMajor = 2;
            const int newMinor = 4;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> {oldSource}, new List<String> {newSource, newSource2}, oldMajor, oldMinor,
                newMajor, newMinor, out oldPackage, out newPackage);

            var checker = SemanticVersionCheckerFactory.NewInstance();
            Assert.AreEqual(new Version(newMajor, newMinor),
                checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }

        #endregion

        #endregion
    }
}
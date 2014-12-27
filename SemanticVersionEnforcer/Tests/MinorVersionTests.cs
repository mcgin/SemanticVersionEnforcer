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
    class MinorVersionTests :SemanticVersionBaseTest
    {
        #region MinorVersion Should Increment

        #region Abstract Classes
        [Test]
        public void GivenTwoPackages_WhenTheNewerOneContainsAdditionalAbstractClasses_ItShouldHaveItsMinorVersionIncremented()
        {
            String oldSource = "public class B { public void hello() { int x=7; } }";
            String newSource1 = "public class B { public void hello() { int x=7; } }";
            String newSource2 = "public abstract class C { public void hello() { int x=7; } }";

            int oldMajor = 2;
            int oldMinor = 3;
            int newMajor = 2;
            int newMinor = 4;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> { oldSource }, new List<String> { newSource1, newSource2 }, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);


            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(2, 4), checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }
        [Test]
        public void GivenTwoPackages_WhenTheNewerOneContainsAdditionalAbstractMethods_ItShouldHaveItsMinorVersionIncremented()
        {
            String oldSource = "public abstract class B { public void hello() { int x=7; } }";
            String newSource = "public abstract class B { public void hello() { int x=7; } public abstract void abstractHello(); }";

            int oldMajor = 2;
            int oldMinor = 3;
            int newMajor = 2;
            int newMinor = 4;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(oldSource, newSource, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);

            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(2, 4), checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }
        #endregion

        #region Regular methods/classes
        [Test]
        public void GivenTwoPackages_WhenTheNewerOneContainsAdditionalPublicMethods_ItShouldHaveItsMinorVersionIncremented()
        {
            String oldSource = "public class B { public void hello() { int x=7; } }";
            String newSource = "public class B { public void hello() { int x=7; }public void hello2() { int x=7; } }";

            int oldMajor = 2;
            int oldMinor = 3;
            int newMajor = 2;
            int newMinor = 4;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(oldSource, newSource, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);

            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(2, 4), checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }
        [Test]
        public void GivenTwoPackages_WhenTheNewerOneContainsAdditionalPublicClasses_ItShouldIncrementTheMinorVersion()
        {
            String oldSource = "public class B { public void hello() { int x=7; } }";
            String newSource = "public class B { public void hello() { int x=7; } }";
            String newSource2 = "public class C { public void hellc() { int x=7; } }";

            int oldMajor = 2;
            int oldMinor = 3;
            int newMajor = 2;
            int newMinor = 4;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String>() { oldSource }, new List<String>() { newSource, newSource2 }, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);


            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(2, 4), checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }
        #endregion

        #region Interfaces

        [Test]
        public void GivenTwoPackages_WhenTheNewOneContainsTheSameInterfaceWithAdditonalMethods_ItShouldIncrementTheMinorVersion()
        {
            String oldSource = "interface x { string GetSomething(); }";
            String newSource = "interface x { string GetSomething(); int SomethingElse();}";

            int oldMajor = 2;
            int oldMinor = 3;
            int newMajor = 2;
            int newMinor = 4;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> { oldSource }, new List<String> { newSource }, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);

            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(newMajor, newMinor), checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }
        [Test]
        public void GivenTwoPackages_WhenTheNewOneContainsAnAdditionalInterface_ItShouldIncrementTheMinorVersion()
        {
            String oldSource = "interface x { string GetSomething(); }";
            String newSource = "interface x { string GetSomething(); }";
            String newSource2 = "interface y { string GetSomethingElse(); }";

            int oldMajor = 2;
            int oldMinor = 3;
            int newMajor = 2;
            int newMinor = 4;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> { oldSource }, new List<String> { newSource, newSource2 }, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);

            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(newMajor, newMinor), checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }
        #endregion

        #endregion
    }
}

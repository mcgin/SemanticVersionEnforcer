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
    class MajorVersionTests : SemanticVersionBase
    {
        #region Increment Major Version

        #region Different packages
        [Test]
        public void GivenTwoPackages_WhenTheyAreCompletelyDifferent_ItShouldIncrementTheMajorVersionAndResetTheMinor()
        {
            String oldSource = "public class B { public void hello() { int x=7; } }";
            String newSource = "public class C { public void bye() { int x=7; } }";

            int oldMajor = 2;
            int oldMinor = 3;
            int newMajor = 3;
            int newMinor = 0;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> { oldSource }, new List<String> { newSource }, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);

            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(3, 0), checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }
        #endregion

        #region Public Methods
        [Test]
        public void GivenTwoPackages_WhenTheOlderOneContainsAdditionalPublicMethods_ItShouldIncrementTheMajorAndResetTheMinor()
        {
            String oldSource = "public class B { public void hello() { int x=7; } public void hello2() { int x=7; } public void hello3() { int x=7; } }";
            String newSource = "public class B { public void hello() { int x=7; } public void hello2() { int x=7; } }";

            int oldMajor = 2;
            int oldMinor = 3;
            int newMajor = 3;
            int newMinor = 0;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> { oldSource }, new List<String> { newSource }, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);

            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(3, 0), checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));

        }
        #endregion

        #region Interfaces
        [Test]
        public void GivenTwoPackages_WhenTheOlderOneContainsAdditionalInterfaces_ItShouldIncrementTheMajorAndResetTheMinor()
        {
            String oldSource1 = "interface x { string GetSomething(); }";
            String oldSource2 = "public class B { public void hello() { int x=7; } public void hello2() { int x=7; } }";
            String newSource = "public class B { public void hello() { int x=7; } public void hello2() { int x=7; } }";

            int oldMajor = 2;
            int oldMinor = 3;
            int newMajor = 3;
            int newMinor = 0;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> { oldSource1, oldSource2 }, new List<String> { newSource }, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);

            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(3, 0), checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));

        }
        [Test]
        public void GivenTwoPackages_WhenTheOlderOneContainsAdditionalMethodsOnACommonInterface_ItShouldIncrementTheMajorAndResetTheMinor()
        {
            String oldSource1 = "interface x { string GetSomething(); string GetSomething2(); }";
            String newSource = "interface x { string GetSomething();  }";

            int oldMajor = 2;
            int oldMinor = 3;
            int newMajor = 3;
            int newMinor = 0;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> { oldSource1 }, new List<String> { newSource }, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);

            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(3, 0), checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));

        }
        #endregion

        #region Abstract Classes
        [Test]
        public void GivenTwoPackages_WhenTheOlderOneContainsAdditionalPublicAbstractClass_ItShouldIncrementTheMajorAndResetTheMinor()
        {
            String oldSource1 = "public abstract class abstractClass { public abstract void blah(); }";
            String oldSource2 = "public class B { public void hello() { int x=7; } public void hello2() { int x=7; } }";
            String newSource = "public class B { public void hello() { int x=7; } public void hello2() { int x=7; } }";

            int oldMajor = 2;
            int oldMinor = 3;
            int newMajor = 3;
            int newMinor = 0;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> { oldSource1, oldSource2 }, new List<String> { newSource }, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);

            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(3, 0), checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));

        }

        [Test]
        public void GivenTwoPackages_WhenTheOlderOneContainsAdditionalPublicMethodsInAnAbstractClass_ItShouldIncrementTheMajorAndResetTheMinor()
        {
            String oldSource1 = "public abstract class abstractClass { public abstract void blah(); public abstract void blahBlah(); }";
            String oldSource2 = "public class B { public void hello() { int x=7; } public void hello2() { int x=7; } }";
            String newSource1 = "public abstract class abstractClass2 { public abstract void blah(); }";
            String newSource2 = "public class B { public void hello() { int x=7; } public void hello2() { int x=7; } }";

            int oldMajor = 2;
            int oldMinor = 3;
            int newMajor = 3;
            int newMinor = 0;

            Mock<IPackage> oldPackage;
            Mock<IPackage> newPackage;
            SetupMocks(new List<String> { oldSource1, oldSource2 }, new List<String> { newSource1, newSource2 }, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);

            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(3, 0), checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));

        }
        #endregion

        #endregion
    }
}

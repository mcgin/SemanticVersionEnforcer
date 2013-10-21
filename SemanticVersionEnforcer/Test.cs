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

namespace SemanticVersionEnforcer
{
    public class B { public void hello() { int x = 7; } }

    [TestFixture]
    class TestClass
    {
        private String singlePublicMethod = "public class B { public void hello() { int x=7; } }";
        private String twoPublicMethods = "public class B { public void hello() { int x=7; }public void hello2() { int x=7; } }";
        
        private static Random random = new Random();

        private String GenerateAssemblySourceWithVersion(int major, int minor, int patch, int other)
        {
            return String.Format( "[assembly: System.Reflection.AssemblyVersionAttribute(\"{0}.{1}.{2}.{3}\")]", major,minor,patch,other);
        }
        public String CreateAssembly(String sourceString, int major, int minor)
        {
            String name = "TestData/AutoGen"+ random.Next(100000) +".dll";
            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.OutputAssembly = name;

            CompilerResults r = CodeDomProvider.CreateProvider("CSharp").CompileAssemblyFromSource(parameters, sourceString, GenerateAssemblySourceWithVersion(major,minor,0,0));

            Assembly result = Assembly.LoadFrom(name);
            return name;
        }


        [Test]
        public void GivenTwoPackages_WhenTheNewerOneContainsAdditionalPublicMethods_ItShouldHaveItsMinorVersionIncremented()
        {
            //string sourceCode = File.ReadAllText("TestData/PublicInterfaces.txt");
            String oldAssembley = CreateAssembly(singlePublicMethod, 2, 3);
            String newAssembley = CreateAssembly(twoPublicMethods, 2, 4);

            Mock<IPackageFile> oldMock = new Mock<IPackageFile>();
            oldMock.Setup(dll => dll.EffectivePath).Returns(oldAssembley);
            oldMock.Setup(dll => dll.GetStream()).Returns(File.Open(oldAssembley, FileMode.Open));

            Mock<IPackageFile> newMock = new Mock<IPackageFile>();
            newMock.Setup(dll => dll.EffectivePath).Returns(newAssembley);
            newMock.Setup(dll => dll.GetStream()).Returns(File.Open(newAssembley, FileMode.Open));

            Mock<IPackage> oldPackage = new Mock<IPackage>();
            oldPackage.Setup(p => p.GetFiles()).Returns(new List<IPackageFile> { oldMock.Object });
            oldPackage.Setup(p => p.Version).Returns(new SemanticVersion(2,3,0,0));

            Mock<IPackage> newPackage = new Mock<IPackage>();
            newPackage.Setup(p => p.GetFiles()).Returns(new List<IPackageFile> { newMock.Object });


            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(2, 4), checker.DetermineCorrectSemanticVersion(oldPackage.Object, newPackage.Object));
        }

        [Test]
        public void GivenTwoIdenticalPackages_WhenTheSemanticVersionIsCalculated_ItShouldBeTheSameVersionAsTheOldOne()
        {
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("C:\\Users\\Aidan\\AppData\\Local\\NuGet\\Cache");
            //IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");
            IPackage oldPackage = repo.FindPackage("NUnit", new SemanticVersion("2.6.2"));
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

            SemanticVersionChecker checker = new SemanticVersionChecker();
            Assert.AreEqual(new Version(2,6),checker.DetermineCorrectSemanticVersion(oldPackage, mockPackage.Object));

        }

        [TearDown]
        public void TearDown()
        {
            //Should delete all the temporary files"";
        }
    }
}

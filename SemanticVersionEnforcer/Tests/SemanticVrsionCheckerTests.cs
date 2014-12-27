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
    [TestFixture]
    public abstract class SemanticVersionBaseTest
    {
        
        private static Random random = new Random();
        private const string TEST_DLL_PREFIX = "TestData/AutoGen";

        #region Test Setup and TearDown
        [TearDown]
        public void TearDown()
        {
            //TODO: Delete the TestData/AutoGen* files

        }

        #endregion

		#region Helpers

        protected void SetupMocks(string oldSource, string newSource, int oldMajor, int oldMinor, int newMajor, int newMinor, out Mock<IPackage> oldPackage, out Mock<IPackage> newPackage)
        {
            this.SetupMocks(new List<String> { oldSource }, new List<String> { newSource }, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);
        }
        protected void SetupMocks(List<String> oldSource, List<String> newSource, int oldMajor, int oldMinor, int newMajor, int newMinor, out Mock<IPackage> oldPackage, out Mock<IPackage> newPackage)
        {
            String oldAssembley = CreateAssembly(oldSource, oldMajor, oldMinor);
            String newAssembley = CreateAssembly(newSource, newMajor, newMinor);

            Mock<IPackageFile> oldMock = new Mock<IPackageFile>();
            oldMock.Setup(dll => dll.EffectivePath).Returns(oldAssembley);
            oldMock.Setup(dll => dll.GetStream()).Returns(File.Open(oldAssembley, FileMode.Open));

            Mock<IPackageFile> newMock = new Mock<IPackageFile>();
            newMock.Setup(dll => dll.EffectivePath).Returns(newAssembley);
            newMock.Setup(dll => dll.GetStream()).Returns(File.Open(newAssembley, FileMode.Open));

            oldPackage = new Mock<IPackage>();
            oldPackage.Setup(p => p.GetFiles()).Returns(new List<IPackageFile> { oldMock.Object });
            oldPackage.Setup(p => p.Version).Returns(new SemanticVersion(oldMajor, oldMinor, 0, 0));

            newPackage = new Mock<IPackage>();
            newPackage.Setup(p => p.GetFiles()).Returns(new List<IPackageFile> { newMock.Object });
        }
        protected String GenerateAssemblySourceWithVersion(int major, int minor, int patch, int other)
        {
            return String.Format("[assembly: System.Reflection.AssemblyVersionAttribute(\"{0}.{1}.{2}.{3}\")]", major, minor, patch, other);
        }
        protected String CreateAssembly(String sourceString, int major, int minor)
        {
            return CreateAssembly(new List<String>() { sourceString }, major, minor);
        }
        protected String CreateAssembly(List<String> sourceStrings, int major, int minor)
        {
            String name = TEST_DLL_PREFIX + random.Next(100000) + ".dll";
            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.OutputAssembly = name;
            List<String> args = new List<String>();
            foreach (String s in sourceStrings)
            {
                args.Add(s);
            }
            args.Add(GenerateAssemblySourceWithVersion(major, minor, 0, 0));

            CompilerResults r = CodeDomProvider.CreateProvider("CSharp").CompileAssemblyFromSource(parameters, sourceStrings.ToArray());

            Assembly result = Assembly.LoadFrom(name);
            return name;
        }
        #endregion
        
    }
}

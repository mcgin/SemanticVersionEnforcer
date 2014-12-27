using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Moq;
using NuGet;
using NUnit.Framework;

namespace SemanticVersionEnforcer.Tests
{
    [SetUpFixture]
    public class SemanticVersionBase
    {
        
        private static readonly Random Random = new Random();
        private const string TestDllDir = "Tests/TestData/AutoGen";

        #region Test Setup and TearDown
            [SetUp]
	        public void RunBeforeAnyTests()
	        {
                Directory.CreateDirectory(TestDllDir);
	        }

            [TearDown]
            public void RunAfterAnyTests()
	        {
                //TODO: Fix the race condition so this gets deleted
                //Directory.Delete(TEST_DLL_DIR, true);
	        }

        #endregion

		#region Helpers

        protected void SetupMocks(string oldSource, string newSource, int oldMajor, int oldMinor, int newMajor, int newMinor, out Mock<IPackage> oldPackage, out Mock<IPackage> newPackage)
        {
            SetupMocks(new List<String> { oldSource }, new List<String> { newSource }, oldMajor, oldMinor, newMajor, newMinor, out oldPackage, out newPackage);
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
            return CreateAssembly(new List<String> { sourceString }, major, minor);
        }
        protected String CreateAssembly(List<String> sourceStrings, int major, int minor)
        {
            String name = TestDllDir + "/" + Random.Next(100000) + ".dll";
            CompilerParameters parameters = new CompilerParameters {GenerateExecutable = false, OutputAssembly = name};
            
            CodeDomProvider.CreateProvider("CSharp").CompileAssemblyFromSource(parameters, sourceStrings.ToArray());

            Assembly.LoadFrom(name);
            return name;
        }
        #endregion
        
    }
}

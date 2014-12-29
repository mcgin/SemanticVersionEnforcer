using System.IO;
using NUnit.Framework;

namespace SemanticVersionEnforcer.Tests
{
    [SetUpFixture]
    public class TestFixtureSetup
    {
        public const string TestDllDir = "TestData/AutoGen";
        
        [SetUp]
        public void RunBeforeAnyTests()
        {
            Directory.CreateDirectory(TestDllDir);
        }

        [TearDown]
        public void RunAfterAnyTests()
        {
            //TODO: Fix the race condition so this gets deleted
            Directory.Delete(TestDllDir, true);
        }
    }
}
using System.IO;
using System.Linq;
using System.Reflection;
using NuGet;
using NUnit.Framework;
using SemanticVersionEnforcer.Core;

namespace SemanticVersionEnforcer.Tests
{
    class PackagePublicationVersionTests
    {
        private const string testFileName = "tempPackage.nupkg";
        [TearDown]
        public void TearDown()
        {
            File.Delete(testFileName);
        }

        [Test]
        [Category("Integration")]
        public void TheVersionNumberOfTheAssemblyShouldBeSemanticallyCorrect()
        {
            var version = Assembly.GetAssembly(typeof(Program)).GetName().Version;
            var metadata = new ManifestMetadata()
            {
                Authors = "An Author",
                Version = version.ToString(),
                Id =  "SemanticVersionEnforcer",
                Description = "A Description"
            };
            PackageBuilder builder = new PackageBuilder();
            builder.Populate(metadata);
            builder.PopulateFiles(".", new[] { new ManifestFile() { Source = "SemanticVersionEnforcer.Core.dll" } });
            using (FileStream stream = File.Open(testFileName, FileMode.OpenOrCreate))
            {
                builder.Save(stream);
            }
            
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");
            var oldPackage = repo.FindPackagesById(metadata.Id).Single(item => item.IsLatestVersion);

            ZipPackage newPackage = new ZipPackage(testFileName);

            var expectedVersion = new SemanticVersionChecker().DetermineCorrectSemanticVersion(oldPackage, newPackage);

            Assert.AreEqual(expectedVersion.Major, version.Major, "Major version mismatch: {0} instead of {1}", version, expectedVersion);
            Assert.AreEqual(expectedVersion.Minor, version.Minor, "Minor version mismatch: {0} instead of {1}", version, expectedVersion);
        }
    }
}
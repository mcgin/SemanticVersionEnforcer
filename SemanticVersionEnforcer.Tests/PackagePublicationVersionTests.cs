using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NuGet;
using NUnit.Framework;
using SemanticVersionEnforcer.Core;

namespace SemanticVersionEnforcer.Tests
{
    internal class PackagePublicationVersionTests
    {
        private const string testFileName = "tempPackage.nupkg";
        private const string PackageId = "SemanticVersionEnforcer";
        private IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");
            
        [TearDown]
        public void TearDown()
        {
            File.Delete(testFileName);
        }

        [Test]
        [Category("Integration")]
        public void TheVersionNumberOfTheAssemblyShouldBeSemanticallyCorrect()
        {
            var newPackageVersion = DynamicallyCreateNewPackageFromSource();
            
            var oldPackage = repo.FindPackagesById(PackageId).Single(item => item.IsLatestVersion);
            var newPackage = new ZipPackage(testFileName);

            var expectedVersion = new SemanticVersionChecker().DetermineCorrectSemanticVersion(oldPackage, newPackage);
            var truncatedNewPackageVersion = new Version(newPackageVersion.Major, newPackageVersion.Minor);

            Assert.GreaterOrEqual(truncatedNewPackageVersion, expectedVersion);
        }

        public Version DynamicallyCreateNewPackageFromSource()
        {
            var version = Assembly.GetAssembly(typeof(Program)).GetName().Version;
            var metadata = new ManifestMetadata
            {
                Authors = "An Author",
                Version = version.ToString(),
                Id = PackageId,
                Description = "A Description"
            };
            var builder = new PackageBuilder();
            builder.Populate(metadata);
            builder.PopulateFiles(".", new[] { new ManifestFile { Source = "SemanticVersionEnforcer.Core.dll" } });
            builder.PopulateFiles(".", new[] { new ManifestFile { Source = "SemanticVersionEnforcer.exe" } });
            using (var stream = File.Open(testFileName, FileMode.OpenOrCreate))
            {
                builder.Save(stream);
            }
            return version;
        }
    }
}
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
        private const string TestFileName = "tempPackage.nupkg";
        private const string PackageId = "SemanticVersionEnforcer";
        private IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");
            
        [TearDown]
        public void TearDown()
        {
            File.Delete(TestFileName);
        }

        [Test]
        [Ignore("Provided as a simple example of usage, not really a useful test")]
        public void AStraightForwardExample()
        {
            const string packageToCheck = "SemanticVersionEnforcer";
            var oldVersion = new SemanticVersion(2, 1, 1, 0);
            var newVersion = new SemanticVersion(2, 2, 0, 0);

            // Here we will use the packages from nuget, however you could just as easily use 
            // local files via the DetermineCorrectSemanticVersion(String, String) method
            var oldPackage = repo.FindPackagesById(packageToCheck).Single(x => x.Version.Equals(oldVersion));
            var newPackage = repo.FindPackagesById(packageToCheck).Single(x => x.Version.Equals(newVersion));

            var expectedVersion = SemanticVersionCheckerFactory.NewInstance().DetermineCorrectSemanticVersion(oldPackage, newPackage);

            Assert.AreEqual(new Version(2, 2), expectedVersion);
        }

        [Test]
        [Category("Integration")]
        public void TheVersionNumberOfTheAssemblyShouldBeSemanticallyCorrect()
        {
            var newPackageVersion = DynamicallyCreateNewPackageFromSource();

            var oldPackage = repo.FindPackagesById(PackageId).Single(item => item.IsLatestVersion);
            var newPackage = new ZipPackage(TestFileName);

            var expectedVersion = SemanticVersionCheckerFactory.NewInstance().DetermineCorrectSemanticVersion(oldPackage, newPackage);
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
            using (var stream = File.Open(TestFileName, FileMode.OpenOrCreate))
            {
                builder.Save(stream);
            }
            return version;
        }
    }
}
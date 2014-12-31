using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NuGet;
using NUnit.Framework;
using SemanticVersionEnforcer.Core;

namespace SemanticVersionEnforcer.Tests
{
    class ExternalNugetRepoTests : SemanticVersionBase
    {
        [Test]
        public void ShouldReturnTheRightVersionWhenThereIsOnlyOnePackageInTheRemoteRepository()
        {
            //Arrange
            IPackage oldPackage = new ZipPackage("TestData/SemanticVersionEnforcer.1.0.1.0.nupkg");

            Mock<IPackageRepository> mockRepo = new Mock<IPackageRepository>();
            mockRepo.Setup(x => x.GetPackages()).Returns((new List<IPackage> { oldPackage }).AsQueryable());

            //Act
            ISemanticVersionChecker checker = new SemanticVersionChecker(mockRepo.Object);
            var version = checker.DetermineCorrectSemanticVersion("TestData/SemanticVersionEnforcer.1.0.1.0.nupkg");

            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(0, version.Minor);
        }

        [Test]
        public void ShouldReturnTheRightVersionWhenThereIsMultiplePackagesInTheRemoteRepository()
        {
            //Arrange
            var mockPackage1 = CreateMockablePackageFromFile("TestData/SemanticVersionEnforcer.1.0.1.0.nupkg");
            mockPackage1.Setup(x => x.IsLatestVersion).Returns(false);
            IPackage oldPackage1 = mockPackage1.Object;

            var mockPackage2 = CreateMockablePackageFromFile("TestData/SemanticVersionEnforcer.2.2.0.0.nupkg");
            mockPackage2.Setup(x => x.IsLatestVersion).Returns(true);
            IPackage oldPackage2 = mockPackage2.Object;

            Mock<IPackageRepository> mockRepo = new Mock<IPackageRepository>();
            mockRepo.Setup(x => x.GetPackages()).Returns((new List<IPackage> { oldPackage1, oldPackage2 }).AsQueryable());

            //Act
            ISemanticVersionChecker checker = new SemanticVersionChecker(mockRepo.Object);
            var version = checker.DetermineCorrectSemanticVersion("TestData/SemanticVersionEnforcer.2.2.0.0.nupkg");

            Assert.AreEqual(2, version.Major, "Major version number is incorrect");
            Assert.AreEqual(2, version.Minor, "Minor version number is incorrect");
        }

        [Test]
        public void ShouldReturnTheRightVersionWhenTheVersionNumberShouldChangeBecauseMethodsHaveBeenRemoved()
        {
            IPackage oldPackage = new ZipPackage("TestData/SemanticVersionEnforcer.2.2.0.0.nupkg");

            Mock<IPackageRepository> mockRepo = new Mock<IPackageRepository>();
            mockRepo.Setup(x => x.GetPackages()).Returns((new List<IPackage> { oldPackage }).AsQueryable());

            ISemanticVersionChecker checker = new SemanticVersionChecker(mockRepo.Object);
            var version = checker.DetermineCorrectSemanticVersion("TestData/SemanticVersionEnforcer.1.0.1.0.nupkg");

            Assert.AreEqual(3, version.Major);
            Assert.AreEqual(0, version.Minor);
        }

        private static Mock<IPackage> CreateMockablePackageFromFile(String packageFileName)
        {
            IPackage package = new ZipPackage(packageFileName);

            Mock<IPackage> mockPackage = new Mock<IPackage>();
            mockPackage.Setup(x => x.IsAbsoluteLatestVersion).Returns(package.IsAbsoluteLatestVersion);
            mockPackage.Setup(x => x.IsLatestVersion).Returns(package.IsLatestVersion);
            mockPackage.Setup(x => x.Listed).Returns(package.Listed);
            mockPackage.Setup(x => x.Published).Returns(package.Published);
            mockPackage.Setup(x => x.AssemblyReferences).Returns(package.AssemblyReferences);
            mockPackage.Setup(x => x.GetFiles()).Returns(package.GetFiles());
            mockPackage.Setup(x => x.GetSupportedFrameworks()).Returns(package.GetSupportedFrameworks());
            mockPackage.Setup(x => x.GetStream()).Returns(package.GetStream());
            mockPackage.Setup(x => x.Id).Returns(package.Id);
            mockPackage.Setup(x => x.Version).Returns(package.Version);
            return mockPackage;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NuGet;

namespace SemanticVersionEnforcer.Core
{
    public class SemanticVersionChecker : ISemanticVersionChecker
    {
        private IPackageRepository repo;
        [Obsolete("Use SemanticVersionCheckerFactory instead, cannot guarantee this method will exist in versions >= 3.x")]
        public SemanticVersionChecker() : this(PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2"))
        {
        }

        public SemanticVersionChecker(IPackageRepository repository)
        {
            repo = repository;
        }
        public Version DetermineCorrectSemanticVersion(string package)
        {
            var newPackage = new ZipPackage(package);
            var oldPackage = repo.FindPackagesById(newPackage.Id).Single(x=>x.IsLatestVersion);

            return DetermineCorrectSemanticVersion(oldPackage, newPackage);
        }

        public Version DetermineCorrectSemanticVersion(String oldPackage, String newPackage)
        {
            return DetermineCorrectSemanticVersion(new ZipPackage(oldPackage), new ZipPackage(newPackage));
        }

        public Version DetermineCorrectSemanticVersion(IPackage oldPackage, IPackage newPackage)
        {
            var publicMethodsInOldPackage = EnumeratePublicMethods(oldPackage);
            var publicMethodsInNewPackage = EnumeratePublicMethods(newPackage);
            var semanticVersion = new Version(oldPackage.Version.Version.Major, oldPackage.Version.Version.Minor);

            foreach (var methodInfo in publicMethodsInNewPackage)
            {
                if (!publicMethodsInOldPackage.Contains(methodInfo))
                {
                    semanticVersion = new Version(oldPackage.Version.Version.Major, oldPackage.Version.Version.Minor + 1);
                }
            }

            if (publicMethodsInOldPackage.Any(methodInfo => !publicMethodsInNewPackage.Contains(methodInfo)))
            {
                return new Version(semanticVersion.Major + 1, 0);
            }
            return semanticVersion;
        }

        private ISet<MethodDescriptor> EnumeratePublicMethods(IPackage package)
        {
            ISet<MethodDescriptor> allMethodInfo = new HashSet<MethodDescriptor>();
            var types = EnumerateTypes(package);
            foreach (var type in types)
            {
                var methods = type.Type.GetMethods();
                foreach (var methodInfo in methods)
                {
                    if (methodInfo.IsPublic)
                    {
                        var md = new MethodDescriptor
                        {
                            Name = methodInfo.Name,
                            ReturnType = methodInfo.ReturnParameter,
                            Parameters = methodInfo.GetParameters(),
                            Type = methodInfo.ReflectedType
                        };
                        allMethodInfo.Add(md);
                    }
                }
            }
            return allMethodInfo;
        }

        private SortedSet<ComparableType> EnumerateTypes(IPackage package)
        {
            var types = new SortedSet<ComparableType>();
            foreach (var file in package.GetFiles())
            {
                if (file.EffectivePath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ||
                    file.EffectivePath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    var assembly = Assembly.Load(file.GetStream().ReadAllBytes());
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsPublic || type.IsInterface)
                        {
                            types.Add(new ComparableType(type));
                        }
                    }
                }
            }

            return types;
        }
    }
}
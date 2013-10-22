using System.Reflection;
using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SemanticVersionEnforcer
{
    public class SemanticVersionChecker
    {
        public void run()
        {
            //ID of the package to be looked up
            string packageID = "NUnit";

            //Connect to the official package repository
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("C:\\Users\\Aidan\\AppData\\Local\\NuGet\\Cache");
            repo.FindPackage("NUnit", new SemanticVersion("2.6.1"));

            //Get the list of all NuGet packages with ID 'EntityFramework'       
            List<IPackage> packages = repo.FindPackagesById(packageID).ToList();

            //Filter the list of packages that are not Release (Stable) versions
            packages = packages.Where (item => (item.IsReleaseVersion() == true)).ToList();
            //C:\Users\Aidan\AppData\Local\NuGet\Cache
            //Iterate through the list and print the full name of the pre-release packages to console
            IPackage oldPackage = null;
            IPackage newPackage = null;
            foreach (IPackage p in packages)
            {
                Console.WriteLine(p.GetFullName());

                if (p.GetFullName().EndsWith("2.6.1"))
                {
                    oldPackage = p;
                }
                if (p.GetFullName().EndsWith("2.6.2"))
                {
                    newPackage = p;
                }



            }
            
        }
		
        public Version DetermineCorrectSemanticVersion(IPackage oldPackage, IPackage newPackage)
        {
            ISet<MethodDescriptor> publicMethodsInOldPackage = EnumeratePublicMethods(oldPackage);
            ISet<MethodDescriptor> publicMethodsInNewPackage = EnumeratePublicMethods(newPackage);
            Version semanticVersion = new Version(oldPackage.Version.Version.Major, oldPackage.Version.Version.Minor);

            //oldPackage.Version.Version;
            foreach (MethodDescriptor methodInfo in publicMethodsInNewPackage)
            {
                //If there is public methods in the old packge not in the new then instant swith
                
                if (!publicMethodsInOldPackage.Contains(methodInfo))
                {
                    semanticVersion = new Version(oldPackage.Version.Version.Major, oldPackage.Version.Version.Minor + 1);
                }
            }

			foreach (MethodDescriptor methodInfo in publicMethodsInOldPackage)
			{
				if (!publicMethodsInNewPackage.Contains(methodInfo))
				{
					return new Version(semanticVersion.Major+1, 0);
				}
                
			}
            return semanticVersion;
        }

        private ISet<MethodDescriptor> EnumeratePublicMethods(IPackage package)
        {
            ISet<MethodDescriptor> allMethodInfo = new HashSet<MethodDescriptor>();
            var types = EnumerateTypes(package);
            foreach (ComparableType type in types)
            {
                var methods = type.Type.GetMethods();
                foreach (MethodInfo methodInfo in methods)
                {
                    if (methodInfo.IsPublic)
                    {
                        MethodDescriptor md = new MethodDescriptor
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
            SortedSet<ComparableType> types = new SortedSet<ComparableType>();
            foreach (IPackageFile file in package.GetFiles())
            {
				var fileName = file.EffectivePath;
                if (file.EffectivePath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    Assembly assembly = Assembly.Load(file.GetStream().ReadAllBytes());
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsPublic)
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

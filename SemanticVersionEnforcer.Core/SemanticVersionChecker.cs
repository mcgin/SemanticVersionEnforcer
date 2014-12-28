﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NuGet;

namespace SemanticVersionEnforcer.Core
{
    public class SemanticVersionChecker
    {
        private const Boolean Debug = true;
        public Version DetermineCorrectSemanticVersion(IPackage oldPackage, IPackage newPackage, String[] assembliesBoundBySemanticVersioningContract)
        {
            throw new NotSupportedException("Filtering not yet implemented, not even sure I want to implement it....");
        }

        public Version DetermineCorrectSemanticVersion(String oldPackage, String newPackage)
        {
            return DetermineCorrectSemanticVersion(new ZipPackage(oldPackage), new ZipPackage(newPackage));
        }

        public Version DetermineCorrectSemanticVersion(IPackage oldPackage, IPackage newPackage)
        {
            ISet<MethodDescriptor> publicMethodsInOldPackage = EnumeratePublicMethods(oldPackage);
            ISet<MethodDescriptor> publicMethodsInNewPackage = EnumeratePublicMethods(newPackage);
            Version semanticVersion = new Version(oldPackage.Version.Version.Major, oldPackage.Version.Version.Minor);
            if (Debug)
            {
                Console.WriteLine("Old methods: " + SetToString(publicMethodsInOldPackage));
                Console.WriteLine("New methods: " + SetToString(publicMethodsInNewPackage));
            }
            foreach (MethodDescriptor methodInfo in publicMethodsInNewPackage)
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

        private string SetToString(IEnumerable cats)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var cat in cats) // Loop through all strings
            {
                builder.Append(cat).Append("\n"); // Append string to StringBuilder
            }
            return builder.ToString(); 
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
				if (file.EffectivePath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    Assembly assembly = Assembly.Load(file.GetStream().ReadAllBytes());
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsPublic  || type.IsInterface )
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
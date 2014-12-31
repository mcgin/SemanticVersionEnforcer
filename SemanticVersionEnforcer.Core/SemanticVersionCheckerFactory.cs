using NuGet;

namespace SemanticVersionEnforcer.Core
{
    public class SemanticVersionCheckerFactory
    {
        private const string DefaultRepo = "https://packages.nuget.org/api/v2";

        public static ISemanticVersionChecker NewInstance()
        {
            return new SemanticVersionChecker(PackageRepositoryFactory.Default.CreateRepository(DefaultRepo));
        }
    }
}
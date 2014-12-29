using System;
using NuGet;

namespace SemanticVersionEnforcer.Core
{
    interface ISemanticVersionChecker
    {
        /// <summary> 
        /// Compares oldPackage to newPackage and calculates the minimal Major/Minor version combination 
        /// that can satisfy semantic versioning
        /// </summary>
        /// <remarks>
        /// Major version is incremented if any publicly accessible fields or methods are removed from newPackage
        /// Minor version is incremented if any new publicly accessible fields or methods are added to newPackage
        /// The combination is only a guide, as it is always possible (through changing the implementation)
        /// to break the contract with consumers of the API without changing publicly accessible methods/fields.
        /// </remarks>
        /// <returns>The minimum version which the newPackage should have in order to conform to Semantic
        /// Versioning</returns>
        /// <param name="oldPackage"> The file containing the old nuget package you want to compare</param>
        /// <param name="newPackage"> The file for the new package which is being compared against oldPackage</param>
        /// <seealso cref="DetermineCorrectSemanticVersion(IPackage, IPackage)"/> 
        Version DetermineCorrectSemanticVersion(String oldPackage, String newPackage);

        /// <seealso cref="DetermineCorrectSemanticVersion(String, String)"></seealso> 
        Version DetermineCorrectSemanticVersion(IPackage oldPackage, IPackage newPackage);
    }
}

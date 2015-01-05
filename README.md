SemanticVersionEnforcer
=======================
Compares two NuGet packages to determine if the Major and Minor version are semantically correct based on the publicly exposed classes/methods.  
Semantic Version Enforcer is available as a [package on NuGet](https://www.nuget.org/packages/SemanticVersionEnforcer/)
Try it out
==========
If you don't wish to compile the code yourself you can obtain it using Nuget:
```
NuGet.exe install SemanticVersionEnforcer -OutputDirectory SemanticVersionEnforcer  -ExcludeVersion
#Need to copy the nuget.core dll into the executable directory
copy .\SemanticVersionEnforcer\Nuget.Core\lib\net40-Client\NuGet.Core.dll .\SemanticVersionEnforcer\SemanticVersionEnforcer\lib\net45
```
All you need to execute the tool will be in the `.\SemanticVersionEnforcer\SemanticVersionEnforcer\lib\net45` directory, simply copy all the files in that directory to a convenient location.  Usage is like so:
```
SemanticVersionEnforcer.exe newPackage.nupkg [oldPackage.nupkg]
```

If you pass a single nupkg file, it will check that package against the latest version on nuget
```
.\SemanticVersionEnforcer\SemanticVersionEnforcer\lib\net45\SemanticVersionEnforcer .\SemanticVersionEnforcer\SemanticVersionEnforcer\SemanticVersionEnforcer.nupkg
```

Alternatively if you have the two nupkg files locally can pass both of those
```
.\SemanticVersionEnforcer\SemanticVersionEnforcer\lib\net45\SemanticVersionEnforcer .\SemanticVersionEnforcer\SemanticVersionEnforcer\SemanticVersionEnforcer.nupkg .\SemanticVersionEnforcer\SemanticVersionEnforcer\SemanticVersionEnforcer.nupkg
```

Use it Programmatically
=======================
If you wish to use the Enforcer programmatically you can interact with it programmatically via the [ISemanticVersionChecker Interface](https://github.com/mcgin/SemanticVersionEnforcer/blob/master/SemanticVersionEnforcer.Core/ISemanticVersionChecker.cs).  
Most usecases for this are via some form of test which is executed before a package is published to ensure it conforms at some basic level with semantic versioning.  
An example of how this can be achieved can be seen in the [PackagePublicationVersionTests](https://github.com/mcgin/SemanticVersionEnforcer/blob/master/SemanticVersionEnforcer.Tests/PackagePublicationVersionTests.cs) test.  

[![Build Status](https://travis-ci.org/mcgin/SemanticVersionEnforcer.svg?branch=master)](https://travis-ci.org/mcgin/SemanticVersionEnforcer)
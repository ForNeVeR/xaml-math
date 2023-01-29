WPF-Math Maintainership
=======================

Publish a New Version
---------------------

1. Update the copyright year in [the license file][license], if required.
2. Update the copyright year in `src/WpfMath/WpfMath.csproj`, if required.
3. Prepare a corresponding entry in [the changelog][changelog] (usually by renaming the "Unreleased" section).
4. Copy-paste the said entry to the `<PackageReleaseNotes>` element of `src/WpfMath/WpfMath.csproj`.
5. Set `<Version>` in the `Directory.Build.props` file.
6. Push a tag in form of `v<VERSION>`, e.g. `v0.0.1`. GitHub Actions will do the rest (push a NuGet package).

Prepare NuGet Package Locally
-----------------------------

```console
$ dotnet pack WpfMath.All --configuration Release
```

Push a NuGet Package Manually
-----------------------------

```console
$ dotnet nuget push ./src/WpfMath/bin/Release/WpfMath.<VERSION>.nupkg --source https://api.nuget.org/v3/index.json --api-key <YOUR_API_KEY>
```

Runtime Support Policy
----------------------

Generally, WPF-Math supports the .NET runtime (.NET Framework or modern .NET) versions that are supported by Microsoft (both basic and extended support are considered), provided that the support cost for the very old versions is reasonable. We will stop support for the currently supported runtime version either after it gets unsupported, or when we consider support cost unreasonable.

Removing .NET runtime version from the list of compatible versions is a breaking change (as far as semantic versioning is concerned).

For reference, see [the .NET support dates list][dotnet-support-dates].

[changelog]: ./CHANGELOG.md
[dotnet-support-dates]: https://fornever.me/en/posts/2021-04-10.net-support-dates.html
[license]: ./LICENSE.md

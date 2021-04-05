WPF-Math Maintainership
=======================

Publish a New Version
---------------------

1. Update the copyright year in [the license file][license], if required.
2. Prepare a corresponding entry in [the changelog][changelog] (usually by renaming the "Unreleased" section).
3. Copy-paste the said entry to the `<PackageReleaseNotes>` element of `src/WpfMath/WpfMath.csproj`.
4. Set `<Version>` in `src/WpfMath/WpfMath.csproj`.
5. Push a tag in form of `v<VERSION>`, e.g. `v0.0.1`. GitHub Actions will do the rest (push a NuGet package).

Prepare NuGet Package Locally
-----------------------------

```console
$ dotnet pack --configuration Release
```

Push a NuGet Package Manually
-----------------------------

```console
$ dotnet nuget push ./src/WpfMath/bin/Release/WpfMath.<VERSION>.nupkg --source https://api.nuget.org/v3/index.json --api-key <YOUR_API_KEY>
```

[changelog]: ./CHANGELOG.md
[license]: ./LICENSE.md

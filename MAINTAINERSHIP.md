XAML-Math Maintainership
========================

Publish a New Version
---------------------
1. Update the copyright year in the `LICENSE.md` file, if required.
2. Prepare a corresponding entry in the `CHANGELOG.md` file (usually by renaming the "Unreleased" section).
3. Set `<Version>` in the `Directory.Build.props` file.
4. Update the copyright year in `Directory.Build.props`, if required.
5. Merge the aforementioned changes via a pull request.
6. Push a tag in form of `v<VERSION>`, e.g. `v0.0.1`. GitHub Actions will do the rest (push a NuGet package).

Prepare NuGet Package Locally
-----------------------------
```console
$ dotnet pack XamlMath.All.sln --configuration Release
```

Push a NuGet Package Manually
-----------------------------
```console
$ dotnet nuget push ./src/WpfMath/bin/Release/WpfMath.<VERSION>.nupkg --source https://api.nuget.org/v3/index.json --api-key <YOUR_API_KEY>
$ dotnet nuget push ./src/AvaloniaMath/bin/Release/AvaloniaMath.<VERSION>.nupkg --source https://api.nuget.org/v3/index.json --api-key <YOUR_API_KEY>
```

Rotate NuGet Publishing Key
---------------------------
CI relies on NuGet API key being added to the secrets. From time to time, this key requires maintenance: it will become obsolete and will have to be updated.

To update the key:

1. Sign in onto nuget.org.
2. Go to the [API keys][nuget.api-keys] section.
3. Update the existing or create a new key named `xaml-math.github` with a permission to **Push only new package versions** and only allowed to publish the following packages:
   - **AvaloniaMath**,
   - **WpfMath**,
   - **XamlMath.Shared**.
4. Paste the generated key to the `NUGET_TOKEN` variable on the [action secrets][github.secrets] section of GitHub settings.

Runtime Support Policy
----------------------
Generally, XAML-Math supports the .NET runtime (.NET Framework or modern .NET) versions that are supported by Microsoft (both basic and extended support are considered), provided that the support cost for the very old versions is reasonable. We will stop support for the currently supported runtime version either after it gets unsupported, or when we consider support cost unreasonable.

Removing .NET runtime version from the list of compatible versions is a breaking change (as far as semantic versioning is concerned).

For reference, see [the .NET support dates list][dotnet-support-dates].

[dotnet-support-dates]: https://fornever.me/en/posts/2021-04-10.net-support-dates.html
[github.secrets]: https://github.com/ForNeVeR/xaml-math/settings/secrets/actions
[nuget.api-keys]: https://www.nuget.org/account/apikeys

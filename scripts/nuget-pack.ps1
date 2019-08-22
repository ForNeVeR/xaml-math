param (
    $SourceDirectory = "$PSScriptRoot/..",
    $AvaloniaMathDirectory = "$SourceDirectory/src/AvaloniaMath",

    $dotnet = 'dotnet',
    $msbuild = 'msbuild',
    $nuget = 'nuget'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

& $msbuild /m "$SourceDirectory/WpfMath.sln" /p:Configuration=Release /p:Platform="Any CPU" '/t:Clean;Rebuild'
if (-not $?) {
    throw "msbuild returned an error code: $LASTEXITCODE"
}

& $nuget pack -BasePath $SourceDirectory "$SourceDirectory/WpfMath.nuspec"
if (-not $?) {
    throw "NuGet returned an error code: $LASTEXITCODE"
}

Push-Location $AvaloniaMathDirectory
try {
    & $dotnet pack --configuration Release
    if (-not $?) {
        throw "dotnet pack returned an error code: $LASTEXITCODE"
    }
} finally {
    Pop-Location
}

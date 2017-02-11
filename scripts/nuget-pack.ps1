param (
    $SourceDirectory = "$PSScriptRoot/..",
    $msbuild = 'msbuild',
    $nuget = 'nuget'
)

$ErrorActionPreference = 'Stop'

& $msbuild /m "$SourceDirectory/WPF-Math.sln" /p:Configuration=Release /p:Platform="Any CPU" '/t:Clean;Rebuild'
if (-not $?) {
    throw "msbuild returned error code: $LASTEXITCODE"
}

& $nuget pack -BasePath $SourceDirectory "$SourceDirectory/WpfMath.nuspec"

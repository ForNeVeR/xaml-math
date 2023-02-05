param(
    [string] $RefName,
    [string] $RepositoryRoot = "$PSScriptRoot/.."
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

Write-Host "Determining version from ref `"$RefName`"…"
if ($RefName -match '^refs/tags/v') {
    $version = $RefName -replace '^refs/tags/v', ''
    Write-Host "Pushed ref is a version tag, version: $version"
} else {
    $propsFilePath = "$RepositoryRoot/Directory.Build.props"
    [xml] $props = Get-Content $propsFilePath
    foreach ($group in $props.Project.PropertyGroup) {
        if ($group.Label -eq 'Packaging') {
            $version = $group.Version
            break
        }
    }
    Write-Host "Pushed ref is a not version tag, get version from $($propsFilePath): $version"
}

Write-Output $version

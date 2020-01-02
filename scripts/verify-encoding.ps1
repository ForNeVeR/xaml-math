<#
.SYNOPSIS
    This script will verify that there's no UTF-8 BOM in the files inside of the project.
#>
param (
    $SourceRoot = "$PSScriptRoot/..",
    $Autofix = $false
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# For PowerShell to process git ls-tree output properly we need to set up the output encoding:
[Console]::OutputEncoding = [Text.Encoding]::UTF8

$allFiles = git -c core.quotepath=off ls-tree -r HEAD --name-only
Write-Output "Total files in the repository: $($allFiles.Length)"

# https://stackoverflow.com/questions/6119956/how-to-determine-if-git-handles-a-file-as-binary-or-as-text#comment15281840_6134127
$nullHash = '4b825dc642cb6eb9a060e54bf8d69288fbee4904'
$textFiles = git -c core.quotepath=off diff --numstat $nullHash HEAD -- @allFiles |
    Where-Object { -not $_.StartsWith('-') } |
    ForEach-Object { [Regex]::Unescape($_.Split("`t", 3)[2]) }
Write-Output "Text files in the repository: $($textFiles.Length)"

$bom = @(0xEF, 0xBB, 0xBF)
$errors = @()

try {
    Push-Location $SourceRoot
    foreach ($file in $textFiles) {
        $fullPath = Resolve-Path -LiteralPath $file
        $bytes = [IO.File]::ReadAllBytes($fullPath) | Select-Object -First $bom.Length
        $bytesEqualsBom = @(Compare-Object $bytes $bom -SyncWindow 0).Length -eq 0
        if ($bytesEqualsBom) {
            $errors += @($file)
        }

        if ($Autofix) {
            $fullContent = [IO.File]::ReadAllBytes($fullPath)
            $newContent = $fullContent | Select-Object -Skip $bom.Length
            Set-Content $file $newContent
        }
    }

    if ($errors.Length -and -not $Autofix) {
        throw "The follwing $($errors.Length) files have UTF-8 BOM:`n" + ($errors -join "`n")
    }
} finally {
    Pop-Location
}

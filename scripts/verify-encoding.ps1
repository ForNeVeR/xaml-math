<#
.SYNOPSIS
    This script will verify that there's no UTF-8 BOM in the files inside of the project.
#>
param (
    # Path to the repository root. All text files under the root will be checked for UTF-8 BOM.
    $SourceRoot = "$PSScriptRoot/..",

    # Makes the script to perform file modifications to bring them to the standard.
    [switch] $Autofix
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# For PowerShell to properly process the UTF-8 output from git ls-tree we need to set up the output encoding:
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
        if ($bytesEqualsBom -and $Autofix) {
            $fullContent = [IO.File]::ReadAllBytes($fullPath)
            $newContent = $fullContent | Select-Object -Skip $bom.Length
            [IO.File]::WriteAllBytes($fullPath, $newContent)
            Write-Output "Removed UTF-8 BOM from file $file"
        } elseif  ($bytesEqualsBom) {
            $errors += @($file)
        }
    }

    if ($errors.Length) {
        throw "The following $($errors.Length) files have UTF-8 BOM:`n" + ($errors -join "`n")
    }
} finally {
    Pop-Location
}

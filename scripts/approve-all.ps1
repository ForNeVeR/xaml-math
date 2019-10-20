param (
    $SolutionRoot = "$PSScriptRoot/..",
    $TestResultDirectory = "$SolutionRoot/src/WpfMath.Tests/TestResults"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Get-ChildItem $TestResultDirectory -Filter "*.received.txt" | ForEach-Object {
    $receivedTestResult = $_.FullName
    $approvedTestResult = $receivedTestResult.Replace('.received.txt', '.approved.txt')
    Move-Item -Force -LiteralPath $receivedTestResult $approvedTestResult
}

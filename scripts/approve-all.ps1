param (
    $SolutionRoot = "$PSScriptRoot/..",
    $ApprovalTestsDirectory = "$SolutionRoot/src/WpfMath.Tests/TestResults",
    $VerifyDirectory = "$SolutionRoot/api"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Get-ChildItem $ApprovalTestsDirectory -Filter "*.received.txt" | ForEach-Object {
    $receivedTestResult = $_.FullName
    $approvedTestResult = $receivedTestResult.Replace('.received.txt', '.approved.txt')
    Move-Item -Force -LiteralPath $receivedTestResult $approvedTestResult
}

Get-ChildItem $VerifyDirectory -Filter "*.received.*" | ForEach-Object {
    $receivedTestResult = $_.FullName
    $approvedTestResult = $receivedTestResult.Replace('.received.', '.verified.')
    Move-Item -Force -LiteralPath $receivedTestResult $approvedTestResult
}

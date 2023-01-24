param (
    $SolutionRoot = "$PSScriptRoot/..",
    $ApprovalTestsDirectories = "$SolutionRoot/src/WpfMath.Tests/TestResults",
    $VerifyDirectories = "$SolutionRoot/src/WpfMath.ApiTest/api"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Get-ChildItem $ApprovalTestsDirectories -Filter "*.received.txt" | ForEach-Object {
    $receivedTestResult = $_.FullName
    $approvedTestResult = $receivedTestResult.Replace('.received.txt', '.approved.txt')
    Move-Item -Force -LiteralPath $receivedTestResult $approvedTestResult
}

Get-ChildItem $VerifyDirectories -Filter "*.received.*" | ForEach-Object {
    $receivedTestResult = $_.FullName
    $approvedTestResult = $receivedTestResult.Replace('.received.', '.verified.')
    Move-Item -Force -LiteralPath $receivedTestResult $approvedTestResult
}

Import-Module (Resolve-Path Utilities) `
    -Function `
    Get-WebProjectPath, `
    Invoke-ExpressionWithException, `
    Write-Status `
    -Force

$projectPath = Get-WebProjectPath
$repositoryPath = Join-Path $projectPath "App_Data/CIRepository"
$launchProfile = $Env:ASPNETCORE_ENVIRONMENT -eq "CI" ? "EcommerceShopify.WebCI" : "IIS Express"
$configuration = $Env:ASPNETCORE_ENVIRONMENT -eq "CI" ? "Release" : "Debug"

$command = "dotnet run " + `
    "--launch-profile $launchProfile " + `
    "-c $configuration " + `
    "--no-build " + `
    "--no-restore " + `
    "--project $projectPath " + `
    "--kxp-ci-restore"

Invoke-ExpressionWithException $command

Write-Host "`n"
Write-Status 'CI files processed'
Write-Host "`n"
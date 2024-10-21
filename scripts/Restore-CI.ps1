Import-Module (Resolve-Path Utilities) `
    -Function `
    Get-WebProjectPath, `
    Invoke-ExpressionWithException, `
    Write-Status `
    -Force

$projectPath = Get-WebProjectPath
$repositoryPath = Join-Path $projectPath "App_Data/CIRepository"
$launchProfile = $Env:ASPNETCORE_ENVIRONMENT -eq "CI" ? "EcommerceShopify.WebCI" : "DancingGoat"
$configuration = $Env:ASPNETCORE_ENVIRONMENT -eq "CI" ? "Release" : "Debug"
$dbName = $Env:DB_NAME

$turnOffCI = "sqlcmd " + `
            "-S localhost " + `
            "-d $dbName " + `
            "-U `"sa`" " + `
            "-P `"Pass@12345`" " + `
            "-Q `"UPDATE CMS_SettingsKey SET KeyValue = N'False' WHERE KeyName = N'CMSEnableCI'`""

$updateCommand = "dotnet run " + `
    "--launch-profile $launchProfile " + `
    "-c $configuration " + `
    "--no-build " + `
    "--project $projectPath " + `
    "--kxp-update " + `
    "--skip-confirmation"

$restoreCommand = "dotnet run " + `
    "--launch-profile $launchProfile " + `
    "-c $configuration " + `
    "--no-build " + `
    "--no-restore " + `
    "--project $projectPath " + `
    "--kxp-ci-restore"

Invoke-ExpressionWithException $turnOffCI
Invoke-ExpressionWithException $updateCommand
Invoke-ExpressionWithException $restoreCommand

Write-Host "`n"
Write-Status 'CI files processed'
Write-Host "`n"
Import-Module (Resolve-Path Utilities) `
    -Function `
    Get-WebProjectPath, `
    Invoke-ExpressionWithException, `
    Write-Status `
    -Force

function SetCIValue {
    param(
        [string]$ciValue
    )

    $toggleCICommand = "sqlcmd " + `
            "-S localhost " + `
            "-d $Env:DB_NAME " + `
            "-U `"sa`" " + `
            "-P `"Pass@12345`" " + `
            "-C " + `
            "-Q `"UPDATE CMS_SettingsKey SET KeyValue = N'$ciValue' WHERE KeyName = N'CMSEnableCI'`""
    Invoke-ExpressionWithException $toggleCICommand
}

$projectPath = Get-WebProjectPath
$repositoryPath = Join-Path $projectPath "App_Data/CIRepository"
$launchProfile = $Env:ASPNETCORE_ENVIRONMENT -eq "CI" ? "EcommerceShopify.WebCI" : "DancingGoat"
$configuration = $Env:ASPNETCORE_ENVIRONMENT -eq "CI" ? "Release" : "Debug"

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

SetCIValue 'False'
Invoke-ExpressionWithException $updateCommand
Invoke-ExpressionWithException $restoreCommand
SetCIValue 'True'

Write-Host "`n"
Write-Status 'CI files processed'
Write-Host "`n"
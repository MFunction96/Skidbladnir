# https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_functions_advanced_parameters?view=powershell-7.2

param(
    [Parameter(Mandatory)]
    [string] $DesUri,
    [Parameter(Mandatory)]
    [string] $Branch,
    [bool] $Force
)

Write-Host $DesUri

Write-Host $Branch

Write-Host $env:SRC_PAT

Write-Host $env:DES_PAT

Write-Host $Force
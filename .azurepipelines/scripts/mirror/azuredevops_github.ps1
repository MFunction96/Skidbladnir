# Mirror Azure DevOps Repo to Github
# Reference
# https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_functions_advanced_parameters?view=powershell-7.2
# https://github.com/xware-gmbh/azure-devops-to-github-mirror/blob/main/git_mirror.sh

param(
    [Parameter(Mandatory)]
    [string] $Organization,
    [Parameter(Mandatory)]
    [string] $Project,
    [Parameter(Mandatory)]
    [string] $Repository,
    [Parameter(Mandatory)]
    [string] $DesRepo,
    [Parameter(Mandatory)]
    [string] $Branch,
    [Parameter()]
    [switch] $Force
)


$SrcUri = "https://$env:SRC_PAT@dev.azure.com/$Organization/$Project/_git/$Repository"
$DesUri = "https://$env:DES_PAT@github.com/$DesRepo"
$TmpDir = "TMP_$Repository"

try {
    git clone $SrcUri $TmpDir --single-branch --branch $Branch
    Set-Location $TmpDir
    git push $DesUri --force
    Set-Location ..
}
catch {
    
}
finally {
    Remove-Item -Path $TmpDir -Recurse -Force
}
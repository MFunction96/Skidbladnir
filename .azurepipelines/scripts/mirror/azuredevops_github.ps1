# Mirror Azure DevOps Repo to Github
# Reference
# https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_functions_advanced_parameters?view=powershell-7.2
# https://github.com/xware-gmbh/azure-devops-to-github-mirror/blob/main/git_mirror.sh

param(
    [Parameter(Mandatory)]
    [string] $AzureUrl,
    [Parameter(Mandatory)]
    [string] $GithubUrl,
    [Parameter()]
    [string] $Branch
)

$feedsUrl = "https://pkgs.dev.azure.com/XanaCN/Lyoko/_packaging/Subdigitals/nuget/v3/index.json"
Register-PSRepository -Name "Subdigitals" -SourceLocation $feedsUrl -PublishLocation $feedsUrl -InstallationPolicy Trusted
Install-Module -Name Skidbladnir.Net.DevOps -Repository Subdigitals
Sync-Code -AzureUrl $AzureUrl -AzurePAT $env:AzurePAT -GithubUrl $GithubUrl -GithubPAT $env:GithubPAT -Branch $Branch
param(
	[Parameter(Mandatory = $true)]
	[string] $Site,
	[Parameter(Mandatory = $true)]
	[string] $Username,
	[Parameter(Mandatory = $true)]
	[string] $Repository,
	[Parameter(Mandatory = $false)]
	[string] $Branch = 'main'
)

git checkout $Branch origin/$Branch
$uri = "git@$Site.com:$Username/$Repository.git"
git push $uri --all
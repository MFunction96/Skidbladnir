param(
	[Parameter(Mandatory = $true)]
	[string] $Site,
	[Parameter(Mandatory = $true)]
	[string] $Username,
	[Parameter(Mandatory = $true)]
	[string] $Repository
)

git checkout -b $Branch origin/$Branch
$uri = "git@$Site.com:$Username/$Repository.git"
git push $uri --follow-tags
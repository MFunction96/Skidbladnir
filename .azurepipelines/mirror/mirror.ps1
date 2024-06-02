param(
	[Parameter(Mandatory = $true)]
	[string] $Site,
	[Parameter(Mandatory = $true)]
	[string] $Username,
	[Parameter(Mandatory = $true)]
	[string] $Repository
)

git checkout -b main origin/main
$uri = "git@$Site.com:$Username/$Repository.git"
git push $uri --follow-tags
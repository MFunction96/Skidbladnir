dotnet user-secrets set "Azure:PAT" "${env:AzurePAT}" --project test/Net/DevOpsTest/DevOpsTest
dotnet user-secrets set "Github:PAT" "${env:GithubPAT}" --project test/Net/DevOpsTest/DevOpsTest
# Build repository on Windows
param(
    [Parameter(Mandatory)]
    [string] $Msys2Root,
    [switch] $Clean,
    [switch] $NoThirdParty,
    [switch] $NoSelf
)

$command = "./build.sh --mingw"
if ($Clean) {
    $command = "$command --clean"
}

if ($NoThirdParty) {
    $command = "$command --nothirdparty"
}

if ($NoSelf) {
    $command = "$command --noself"
}

$env:CHERE_INVOKING = 'yes'  # Preserve the current working directory
$env:MSYSTEM = 'MINGW64'
Start-Process -FilePath "$Msys2Root/usr/bin/bash.exe" -ArgumentList "-lc '$command'" -Wait -NoNewWindow
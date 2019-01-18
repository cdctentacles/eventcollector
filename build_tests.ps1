Set-StrictMode -Version latest
$ErrorActionPreference = "Stop"

function Exec
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ("Error executing command {0}" -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

Write-Host "Building Code"
pushd src
Exec { dotnet build }
popd

Write-Host "Running public apis tests"
pushd tests\public
Exec { dotnet test --logger:"console;verbosity=detailed" }
popd

Write-Host "Running private apis tests"
pushd tests\private
Exec { dotnet test --logger:"console;verbosity=detailed" }
popd

Write-Host "Success!!!"
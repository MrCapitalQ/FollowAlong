$currentDir = Get-Location

try {
    Set-Location $PSScriptRoot
    Set-Location ..

    # EnableMsixTooling MSBuild property is needed to build tests from dotnet CLI
    dotnet test /p:EnableMsixTooling=true
}
finally {
    # Return to original directory
    Set-Location $currentDir
}

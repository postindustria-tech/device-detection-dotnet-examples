param(
    [Parameter(Mandatory=$true)]
    [string]$RepoName = "device-detection-dotnet-examples",
    [string]$ProjectDir = ".",
    [string]$Name = "Release_x64",
    [string]$Configuration = "Release",
    [string]$Arch = "x64",
    [string]$BuildMethod = "msbuild"
)

if ($BuildMethod -eq "dotnet"){
    Write-Output "$Configuration"
    
    # Define the base directories to search in
    $BaseDirectories = @(".\Examples", ".\Tests")

    # Retrieve all .csproj files recursively from the base directories
    $Projects = Get-ChildItem -Path $BaseDirectories -Filter *.csproj -Recurse | 
                Select-Object -ExpandProperty FullName

    # Iterate over each project file
    foreach ($Project in $Projects) {
        ./dotnet/build-project-core.ps1 -RepoName $RepoName -ProjectDir $Project -Name $Name -Configuration $Configuration -Arch $Arch
    }


}
else{

    ./dotnet/build-project-framework.ps1 -RepoName $RepoName -ProjectDir $ProjectDir -Name $Name -Configuration $Configuration -Arch $Arch
}

exit $LASTEXITCODE

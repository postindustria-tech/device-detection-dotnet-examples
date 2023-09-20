
param (
    [string]$RepoName,
    [Parameter(Mandatory=$true)]
    [string]$DeviceDetection,
    [string]$DeviceDetectionUrl
)

# Fetch the TAC data file for testing with
# ./steps/fetch-hash-assets.ps1 -RepoName $RepoName -LicenseKey $DeviceDetection -Url $DeviceDetectionUrl

# Move the data file to the correct location
# $DataFileName = "TAC-HashV41.hash"
# $DataFileSource = [IO.Path]::Combine($pwd, $RepoName, $DataFileName)
$DataFileDir = [IO.Path]::Combine($pwd, $RepoName,"device-detection-data")
# $DataFileDestination = [IO.Path]::Combine($DataFileDir, $DataFileName)
# Move-Item $DataFileSource $DataFileDestination

$ErrorActionPreference = 'Stop'

$assets = New-Item -ItemType Directory -Path assets -Force
$deviceDetectionData = $DataFileDir

$downloads = @{
    "TAC-HashV41.hash" = {
        ./steps/fetch-hash-assets.ps1 -RepoName $RepoName -LicenseKey $DeviceDetection -Url $DeviceDetectionUrl
        Move-Item -Path $RepoName/$file -Destination $assets
    }
    "51Degrees-LiteV4.1.hash" = {Invoke-WebRequest -Uri "https://storage.googleapis.com/51degrees-assets/$DeviceDetection/51Degrees-LiteV4.1.hash" -OutFile $assets/$file}
    "20000 Evidence Records.yml" = {Invoke-WebRequest -Uri "https://storage.googleapis.com/51degrees-assets/$DeviceDetection/20000%20Evidence%20Records.yml" -OutFile $assets/$file}
    "20000 User Agents.csv" = {Invoke-WebRequest -Uri "https://storage.googleapis.com/51degrees-assets/$DeviceDetection/20000%20User%20Agents.csv" -OutFile $assets/$file}
    "51Degrees.csv" = {
        Invoke-WebRequest -Uri "https://storage.googleapis.com/51degrees-assets/$DeviceDetection/51Degrees-Tac.zip" -OutFile 51Degrees-Tac.zip
        Expand-Archive -Path 51Degrees-Tac.zip
        Get-Content -TotalCount 1 51Degrees-Tac/51Degrees-Tac-All.csv | Out-File $assets/$file # We only need a header
        Remove-Item -Path 51Degrees-Tac.zip, 51Degrees-Tac/51Degrees-Tac-All.csv
    }
}

foreach ($file in $downloads.Keys) {
    if (!(Test-Path $assets/$file)) {
        Write-Output "Downloading $file"
        Invoke-Command -ScriptBlock $downloads[$file]
    } else {
        Write-Output "'$file' exists, skipping download"
    }
}

# Tests mutate this file, so we copy it
# Write-Output "Copying 'TAC-HashV41.hash' to '$deviceDetectionData/Enterprise-HashV41.hash'"
# Copy-Item -Path $assets/TAC-HashV41.hash -Destination $deviceDetectionData/Enterprise-HashV41.hash

# We can just symlink these
New-Item -ItemType SymbolicLink -Force -Target "$assets/TAC-HashV41.hash.csv" -Path "$deviceDetectionData/TAC-HashV41.hash"
New-Item -ItemType SymbolicLink -Force -Target "$assets/51Degrees-LiteV4.1.hash" -Path "$deviceDetectionData/51Degrees-LiteV4.1.hash"
New-Item -ItemType SymbolicLink -Force -Target "$assets/20000 Evidence Records.yml" -Path "$deviceDetectionData/20000 Evidence Records.yml"
New-Item -ItemType SymbolicLink -Force -Target "$assets/20000 User Agents.csv" -Path "$deviceDetectionData/20000 User Agents.csv"
# New-Item -ItemType SymbolicLink -Force -Target "$assets/51Degrees.csv" -Path "$RepoName/fiftyone_devicedetection_cloud/tests/51Degrees.csv"

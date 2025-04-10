#!/usr/bin/env pwsh
$DotNetInstallerUri = 'https://dot.net/v1/dotnet-install.ps1';
$DotNetUnixInstallerUri = 'https://dot.net/v1/dotnet-install.sh'
$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent

# Make sure tools folder exists
$ToolPath = Join-Path $PSScriptRoot "tools"
if (!(Test-Path $ToolPath)) {
    Write-Verbose "Creating tools directory..."
    New-Item -Path $ToolPath -Type Directory -Force | out-null
}


if ($PSVersionTable.PSEdition -ne 'Core') {
    # Attempt to set highest encryption available for SecurityProtocol.
    # PowerShell will not set this by default (until maybe .NET 4.6.x). This
    # will typically produce a message for PowerShell v2 (just an info
    # message though)
    try {
        # Set TLS 1.3 (12288), then TLS 1.2 (3072)
        # Use integers because the enumeration values for TLS 1.3 and TLS 1.2 won't
        # exist in .NET 4.0, even though they are addressable if .NET 4.6.2+ is
        # installed (.NET 4.6.2 is an in-place upgrade).
        [System.Net.ServicePointManager]::SecurityProtocol = 12288 -bor 3072
      } catch {
        Write-Output 'Unable to set PowerShell to use TLS 1.3 and TLS 1.2 due to old .NET Framework installed. If you see underlying connection closed or trust errors, you may need to upgrade to .NET Framework 4.6.2+ and PowerShell v3'
      }
}

###########################################################################
# INSTALL .NET CORE CLI
###########################################################################

$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
$env:DOTNET_CLI_TELEMETRY_OPTOUT=1
$env:DOTNET_ROLL_FORWARD_ON_NO_CANDIDATE_FX=2


Function Remove-PathVariable([string]$VariableToRemove)
{
    $SplitChar = ';'
    if ($IsMacOS -or $IsLinux) {
        $SplitChar = ':'
    }

    $path = [Environment]::GetEnvironmentVariable("PATH", "User")
    if ($path -ne $null)
    {
        $newItems = $path.Split($SplitChar, [StringSplitOptions]::RemoveEmptyEntries) | Where-Object { "$($_)" -inotlike $VariableToRemove }
        [Environment]::SetEnvironmentVariable("PATH", [System.String]::Join($SplitChar, $newItems), "User")
    }

    $path = [Environment]::GetEnvironmentVariable("PATH", "Process")
    if ($path -ne $null)
    {
        $newItems = $path.Split($SplitChar, [StringSplitOptions]::RemoveEmptyEntries) | Where-Object { "$($_)" -inotlike $VariableToRemove }
        [Environment]::SetEnvironmentVariable("PATH", [System.String]::Join($SplitChar, $newItems), "Process")
    }
}

$InstallPath = Join-Path $PSScriptRoot ".dotnet"
$GlobalJsonPath = Join-Path $PSScriptRoot "global.json"
if (!(Test-Path $InstallPath)) {
    New-Item -Path $InstallPath -ItemType Directory -Force | Out-Null;
}

if ($IsMacOS -or $IsLinux) {
    $ScriptPath = Join-Path $InstallPath 'dotnet-install.sh'
    (New-Object System.Net.WebClient).DownloadFile($DotNetUnixInstallerUri, $ScriptPath);
    & bash $ScriptPath --jsonfile "$GlobalJsonPath" --install-dir "$InstallPath" --no-path

    Remove-PathVariable "$InstallPath"
    $env:PATH = "$($InstallPath):$env:PATH"
}
else {
    $ScriptPath = Join-Path $InstallPath 'dotnet-install.ps1'
    (New-Object System.Net.WebClient).DownloadFile($DotNetInstallerUri, $ScriptPath);
    & $ScriptPath -JSonFile $GlobalJsonPath -InstallDir $InstallPath;

    Remove-PathVariable "$InstallPath"
    $env:PATH = "$InstallPath;$env:PATH"
}
$env:DOTNET_ROOT=$InstallPath

###########################################################################
# INSTALL CAKE
###########################################################################

& dotnet tool restore

###########################################################################
# RUN BUILD SCRIPT
###########################################################################
& dotnet cake ./build.cake $args

exit $LASTEXITCODE
param
(
    [string] $EndPoints,
    [string] $ImageStoreConnectionString,
    [string] $DemoFolder,
    [switch] $Clean,
    [switch] $Upgrade,
    [switch] $Downgrade,
    [switch] $Deploy,
    [switch] $GetUpgradeProgress,
    [switch] $GetStatus
)

function Get-ScriptDirectory
{
    $Invocation = (Get-Variable MyInvocation -Scope 1).Value
    Split-Path $Invocation.MyCommand.Path
}

if (!$DemoFolder.Length)
{
    $DemoFolder = (Get-ScriptDirectory)
}

if (!$ImageStoreConnectionString.Length)
{
    $ImageStoreConnectionString = "fabric:ImageStore";
}

if (!$Endpoints.Length)
{
    Connect-ServiceFabricCluster
}
else
{
    Connect-ServiceFabricCluster $EndPoints
}

. $DemoFolder\V1\_ActorApplicationVariables.ps1

$AppParameters = @{}

if ($Deploy)
{
    Copy-ServiceFabricApplicationPackage -ApplicationPackagePath $DemoFolder\V1\PackageRoot\$AppPackageName -ImageStoreConnectionString $ImageStoreConnectionString
    Register-ServiceFabricApplicationType $AppPackageName

    Copy-ServiceFabricApplicationPackage -ApplicationPackagePath $DemoFolder\V2\PackageRoot\$AppPackageName -ImageStoreConnectionString $ImageStoreConnectionString
    Register-ServiceFabricApplicationType $AppPackageName

    Get-ServiceFabricApplicationType $AppTypeName
    New-ServiceFabricApplication $AppName $AppTypeName 1.0.0.0 -ApplicationParameter $AppParameters
}

if ($Clean)
{
    Remove-ServiceFabricApplication $AppName -force
    Get-ServiceFabricApplicationType $AppTypeName | Unregister-ServiceFabricApplicationType -force
    Write-Output "Demo Application and Application Types cleaned from the cluster."
}

if ($Upgrade)
{
    Start-ServiceFabricApplicationUpgrade -ApplicationName $AppName -ApplicationTypeVersion 2.0.0.0 -UnmonitoredAuto -ApplicationParameter $AppParameters
}

if ($GetUpgradeProgress)
{
    Get-ServiceFabricApplicationUpgrade $AppName
}

if ($Downgrade)
{
   Start-ServiceFabricApplicationUpgrade -ApplicationName $AppName -ApplicationTypeVersion 1.0.0.0 -UnmonitoredAuto -ApplicationParameter $AppParameters
}

if ($GetStatus)
{
    Get-ServiceFabricApplication | Get-ServiceFabricService | Get-ServiceFabricPartition | Get-ServiceFabricReplica
}

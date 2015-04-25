#
# AUTO-GENERATED - DO NOT MODIFY
#

param
(
    [Parameter(Mandatory=$false)]
    [string] $EndPoints,

    [Parameter(Mandatory=$true, ParameterSetName = "Clean")]
    [switch] $Clean,

    [Parameter(Mandatory=$true, ParameterSetName = "Deploy")]
    [switch] $Deploy,

    [Parameter(Mandatory=$true, ParameterSetName = "GetReplicas")]
    [switch] $GetReplicas,

    [Parameter(Mandatory=$true, ParameterSetName = "RestartHost")]
    [switch] $RestartHost,

    [Parameter(Mandatory=$true, ParameterSetName = "ResolveHost")]
    [switch] $ResolveHost,

    [Parameter(Mandatory=$false, ParameterSetName = "Deploy")]
    [string] $ImageStoreConnectionString,

    [Parameter(Mandatory=$false, ParameterSetName = "Deploy")]
    [string] $AppPackageFolder,

    [Parameter(Mandatory=$true, ParameterSetName = "RestartHost")]
    [Parameter(Mandatory=$true, ParameterSetName = "ResolveHost")]
    [string] $ActorType,

    [Parameter(Mandatory=$true, ParameterSetName = "RestartHost")]
    [Parameter(Mandatory=$true, ParameterSetName = "ResolveHost")]
    [long] $ActorId
)

function Get-ScriptDirectory 
{ 
    $Invocation = (Get-Variable MyInvocation -Scope 1).Value 
    Split-Path $Invocation.MyCommand.Path 
}

if (!$AppPackageFolder.Length)
{
    $AppPackageFolder = (Get-ScriptDirectory)
    $AppPackageFolder = $AppPackageFolder + "\PackageRoot";
}

$AppVariablesScriptPath = (Get-ScriptDirectory)
. $AppVariablesScriptPath\_ActorApplicationVariables.ps1

if (!$ImageStoreConnectionString.Length)
{
    $ImageStoreConnectionString = "fabric:ImageStore";
}

if (!$EndPoints.Length)
{
    [void](Connect-ServiceFabricCluster -WarningAction Ignore)
}
else
{
    [void](Connect-ServiceFabricCluster $EndPoints -WarningAction Ignore)
}

if ($Clean)
{
    Write-Output "Removing Application $AppName"
    Remove-ServiceFabricApplication $AppName -force

    Write-Output "Unregistering ApplicationType $AppTypeName $AppTypeVersion"
    Unregister-ServiceFabricApplicationType $ApptypeName $AppTypeVersion -force
}

if ($Deploy)
{
    Write-Output "Copying Application Package $AppPackageFolder\$AppPackageName to ImageStore $ImageStoreConnectionString"
    Copy-ServiceFabricApplicationPackage -ApplicationPackagePath $AppPackageFolder\$AppPackageName -ImageStoreConnectionString $ImageStoreConnectionString

    Write-Output "Registering ApplicationType $AppTypeName $AppTypeVersion"
    Register-ServiceFabricApplicationType $AppPackageName

    Get-ServiceFabricApplicationType $AppTypeName

    Write-Output "Creating Application $AppName"
    New-ServiceFabricApplication $AppName $AppTypeName $AppTypeVersion
}

if ($GetReplicas)
{
    Get-ServiceFabricApplication $AppName | Get-ServiceFabricService | Get-ServiceFabricPartition | Get-ServiceFabricReplica 
}

if ($RestartHost)
{
    $ActorServiceUri = $ActorServiceUriMap[$ActorType]

    if (!$ActorServiceUri)
    {
        Write-Error "Actor Service $ActorType not found"
        return
    }

    $IsStateful = $ActorServiceIsStatefulMap[$ActorType]
    $HasPersistedState = $false

    Write-Output "Resolving Actor Service ${ActorServiceUri} for $ActorType Actor $ActorId"
    $rsp = Resolve-ServiceFabricService -PartitionKindUniformInt64 -ServiceName $ActorServiceUri -PartitionKey $ActorId

    if (!$rsp)
    {
        return
    }

    $replicas = Get-ServiceFabricReplica $rsp.PartitionId

    if ($IsStateful)
    {
        $HasPersistedState = $ActorServiceHasPersistedStateMap[$ActorType]
        $replicas = $replicas | where {($_.ReplicaRole -eq "Primary")}
    }

    if ($replicas.Count -gt 0)
    {
        $partitionId = $rsp.PartitionId.ToString();

        Foreach ($replica in $replicas)
        {
            $replicaId = $replica.ReplicaOrInstanceId;
            $nodeName = $replica.NodeName;

            Write-Output ""

            if ($IsStateful -and $HasPersistedState)
            {
                Write-Output "Restarting Actor Host ${ActorServiceUri}:$partitionId on $nodeName"
    
                Restart-ServiceFabricReplica -NodeName $nodeName -PartitionId $partitionId -ReplicaOrInstanceId $replicaId
            }
            else
            {
                Write-Output "Removing Actor Host ${ActorServiceUri}:$partitionId on $nodeName"

                Remove-ServiceFabricReplica -NodeName $nodeName -PartitionId $partitionId -ReplicaOrInstanceId $replicaId
            }

            Write-Output ""
        }
    }
}


if ($ResolveHost)
{
    $ActorServiceUri = $ActorServiceUriMap[$ActorType]

    if (!$ActorServiceUri)
    {
        Write-Error "Actor Service $ActorType not found"
        return
    }

    $IsStateful = $ActorServiceIsStatefulMap[$ActorType]

    Write-Output "Resolving Actor Service ${ActorServiceUri} for $ActorType Actor $ActorId"
    $rsp = Resolve-ServiceFabricService -PartitionKindUniformInt64 -ServiceName $ActorServiceUri -PartitionKey $ActorId

    if (!$rsp)
    {
        return
    }

    $replicas = Get-ServiceFabricReplica $rsp.PartitionId
    
    if ($IsStateful)
    {
        $replicas = $replicas | where {($_.ReplicaRole -eq "Primary")}
    }

    if ($replicas.Count -gt 0)
    {
        $partitionId = $rsp.PartitionId.ToString();
        
        Write-Output ""
        if ($IsStateful)
        {
            $replica = $replicas[0]
            $replicaId = $replica.ReplicaOrInstanceId;
            $nodeName = $replica.NodeName;
            $replicaAddress = $replica.ReplicaAddress;

            Write-Output "Actor $ActorId is hosted by replica $replicaId of partition $partitionId of Service ${ActorServiceUri} on $nodeName"
            Write-Output ""
            Write-Output "Address of the ActorHost is $replicaAddress"
        }
        else
        {
            Write-Output "Actor $ActorId is hosted by partition $partitionId of Service ${ActorServiceUri} on the following instances:"

            Foreach ($instance in $replicas)
            {
                $instanceId = $instance.ReplicaOrInstanceId;
                $nodeName = $instance.NodeName;
                $instanceAddress = $instance.ReplicaAddress;

                Write-Output ""
                Write-Output "  Instance $instanceId on $nodeName"
                Write-Output "  Address of the ActorHost is $instanceAddress"
            }
        }

        Write-Output ""
    }
}

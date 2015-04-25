$dp0 = Split-Path -parent $PSCommandPath
$SolutionDir = (Get-Item "$dp0\..\").FullName
$applicationPath = "$SolutionDir\StatelessPiService\bin\Debug\StatelessPiServiceApp"

Connect-ServiceFabricCluster

Copy-ServiceFabricApplicationPackage -ApplicationPackagePath $applicationPath -ImageStoreConnectionString fabric:ImageStore

Register-ServiceFabricApplicationType -ApplicationPathInImageStore StatelessPiServiceApp

New-ServiceFabricApplication -ApplicationName fabric:/StatelessPiServiceApp -ApplicationTypeName StatelessPiServiceApp -ApplicationTypeVersion 1.0

New-ServiceFabricService -ApplicationName fabric:/StatelessPiServiceApp -ServiceName fabric:/StatelessPiServiceApp/StatelessPiService -ServiceTypeName StatelessPiService -Stateless -PartitionSchemeSingleton -InstanceCount 1


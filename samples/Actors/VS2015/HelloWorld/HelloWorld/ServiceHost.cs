// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace HelloWorld
{
    using System.Fabric;
    using System.Threading;
    using Microsoft.ServiceFabric.Actors;

    public class ServiceHost
    {
        public static void Main(string[] args)
        {
            using (FabricRuntime fabricRuntime = FabricRuntime.Create())
            {
                ActorRegistration.RegisterActor(fabricRuntime, typeof(HelloWorld));
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Samples
{
    using System;
    using System.Fabric;
    using System.Threading;

    internal class Program
    {
        private static void Main()
        {
            using (FabricRuntime runtime = FabricRuntime.Create())
            {
                runtime.RegisterActor(typeof(HelloActor));

                Thread.Sleep(Timeout.Infinite);
                GC.KeepAlive(runtime);
            }
        }
    }
}
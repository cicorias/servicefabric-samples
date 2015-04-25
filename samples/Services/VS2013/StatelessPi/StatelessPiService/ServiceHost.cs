// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace StatelessPiService
{
    using System;
    using System.Diagnostics;
    using System.Fabric;
    using System.IO;
    using System.Threading;

    internal sealed class ServiceHost
    {
        public static void Main(string[] args)
        {
            // Create a Service Fabric Runtime
            using (FabricRuntime fabricRuntime = FabricRuntime.Create())
            using (TextWriterTraceListener trace = new TextWriterTraceListener(Path.Combine(FabricRuntime.GetActivationContext().LogDirectory, "out.log")))
            {
                Trace.AutoFlush = true;
                Trace.Listeners.Add(trace);

                try
                {
                    Trace.WriteLine("Starting Service Host for Pi Service.");

                    fabricRuntime.RegisterServiceType(Service.ServiceTypeName, typeof(Service));

                    Thread.Sleep(Timeout.Infinite);
                    GC.KeepAlive(fabricRuntime);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                }
            }
        }
    }
}
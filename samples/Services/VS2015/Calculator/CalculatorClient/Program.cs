// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace CalculatorClient
{
    using System;
    using System.Fabric;
    using System.ServiceModel;
    using CalculatorInterfaces;
    using Microsoft.ServiceFabric.Services;
    using Microsoft.ServiceFabric.Services.Wcf;

    internal class Program
    {
        //
        // Name of the calculator service instance created on the cluster.
        //
        private static readonly Uri ServiceName = new Uri("fabric:/CalculatorServiceApplication/CalculatorService");

        private static void Main(string[] args)
        {
            //
            // Create a service resolver for resolving the endpoints of the calculator service.
            //
            ServicePartitionResolver serviceResolver = new ServicePartitionResolver(() => new FabricClient());

            //
            // Create the binding.
            //
            NetTcpBinding binding = CreateClientConnectionBinding();

            //
            // Create a client for communicating with the calc service which has been created with
            // Singleton partition scheme.
            //
            Client calcClient = new Client(
                new WcfCommunicationClientFactory<ICalculator>(serviceResolver, binding, null),
                ServiceName);

            //
            // Register for connection events
            //
            calcClient.Factory.ClientConnected += ClientConnected;
            calcClient.Factory.ClientDisconnected += ClientDisconnected;

            long count = 0;
            while (true)
            {
                if (calcClient.AddAsync(2, 3).Result.Equals(5))
                {
                    count++;
                }
                if ((count%500) == 0)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(@"                    ");
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(count);
                }
            }
        }

        private static void ClientDisconnected(object sender, CommunicationClientEventArgs<WcfCommunicationClient<ICalculator>> e)
        {
            Console.WriteLine();
            Console.WriteLine(@"Client disconnected from endpoint {0}", e.Client.ResolvedServicePartition.GetEndpoint().Address);
            Console.WriteLine();
        }

        private static void ClientConnected(object sender, CommunicationClientEventArgs<WcfCommunicationClient<ICalculator>> e)
        {
            Console.WriteLine();
            Console.WriteLine(@"Client connected to endpoint {0}", e.Client.ResolvedServicePartition.GetEndpoint().Address);
            Console.WriteLine();
        }

        private static NetTcpBinding CreateClientConnectionBinding()
        {
            //
            // Pick these values from client's config.
            //
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None)
            {
                SendTimeout = TimeSpan.MaxValue,
                ReceiveTimeout = TimeSpan.MaxValue,
                OpenTimeout = TimeSpan.FromSeconds(5),
                CloseTimeout = TimeSpan.FromSeconds(5),
                MaxReceivedMessageSize = 1024*1024
            };
            binding.MaxBufferSize = (int) binding.MaxReceivedMessageSize;
            binding.MaxBufferPoolSize = Environment.ProcessorCount*binding.MaxReceivedMessageSize;

            return binding;
        }
    }
}
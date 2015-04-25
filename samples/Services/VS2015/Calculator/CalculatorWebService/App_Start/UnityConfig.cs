// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace CalculatorWebService
{
    using System;
    using System.Fabric;
    using System.ServiceModel;
    using System.Web.Http;
    using CalculatorClient;
    using CalculatorInterfaces;
    using global::CalculatorWebService.Controllers;
    using Microsoft.Practices.Unity;
    using Microsoft.ServiceFabric.Services;
    using Microsoft.ServiceFabric.Services.Wcf;
    using Unity.WebApi;

    public static class UnityConfig
    {
        public static void RegisterComponents(HttpConfiguration config)
        {
            UnityContainer container = new UnityContainer();

            // Create a client for the Calculator service.
            // The application should only have one instance of the client. 
            // Unity will manage the lifetime of the client for us.
            Uri serviceNameUri = new Uri("fabric:/CalculatorServiceApplication/CalculatorService");

            ICalculator client = new Client(
                new WcfCommunicationClientFactory<ICalculator>(
                    new ServicePartitionResolver(() => new FabricClient()),
                    CreateClientConnectionBinding(),
                    null),
                serviceNameUri);

            // Inject the client as a dependency for the DefaultController.
            container.RegisterType<DefaultController>(
                new InjectionConstructor(client));

            config.DependencyResolver = new UnityDependencyResolver(container);
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
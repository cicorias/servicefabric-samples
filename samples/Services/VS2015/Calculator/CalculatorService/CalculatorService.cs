// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace CalculatorService
{
    using System;
    using System.ServiceModel;
    using System.Threading.Tasks;
    using CalculatorInterfaces;
    using Microsoft.ServiceFabric.Services;
    using Microsoft.ServiceFabric.Services.Wcf;

    public class CalculatorService : StatelessService, ICalculator
    {
        public Task<double> AddAsync(double valueOne, double valueTwo)
        {
            return Task.FromResult(valueOne + valueTwo);
        }

        public Task<double> SubtractAsync(double valueOne, double valueTwo)
        {
            return Task.FromResult(valueOne - valueTwo);
        }

        protected override ICommunicationListener CreateCommunicationListener()
        {
            WcfCommunicationListener communicationListener = new WcfCommunicationListener(typeof(ICalculator), this)
            {
                //
                // The name of the endpoint configured in the ServiceManifest under the Endpoints section
                // which identifies the endpoint that the wcf servicehost should listen on.
                //
                EndpointResourceName = "ServiceEndpoint",

                // 
                // Populate the binding information that you want the service to use.
                //
                Binding = this.CreateListenBinding()
            };

            return communicationListener;
        }

        private NetTcpBinding CreateListenBinding()
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None)
            {
                //
                // Pick these values from service config
                //
                SendTimeout = TimeSpan.MaxValue,
                ReceiveTimeout = TimeSpan.MaxValue,
                OpenTimeout = TimeSpan.FromSeconds(5),
                CloseTimeout = TimeSpan.FromSeconds(5),
                MaxConnections = int.MaxValue,
                MaxReceivedMessageSize = 1024*1024
            };

            binding.MaxBufferSize = (int) binding.MaxReceivedMessageSize;
            binding.MaxBufferPoolSize = Environment.ProcessorCount*binding.MaxReceivedMessageSize;

            return binding;
        }
    }
}
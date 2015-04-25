// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace CalculatorClient
{
    using System;
    using System.Threading.Tasks;
    using CalculatorInterfaces;
    using Microsoft.ServiceFabric.Services;
    using Microsoft.ServiceFabric.Services.Wcf;

    public class Client : ServicePartitionClient<WcfCommunicationClient<ICalculator>>, ICalculator
    {
        public Client(
            WcfCommunicationClientFactory<ICalculator> clientFactory,
            Uri serviceName)
            : base(clientFactory, serviceName)
        {
        }

        public Task<double> AddAsync(double valueOne, double valueTwo)
        {
            return this.InvokeWithRetryAsync(
                client => client.Channel.AddAsync(valueOne, valueTwo));
        }

        public Task<double> SubtractAsync(double valueOne, double valueTwo)
        {
            return this.InvokeWithRetryAsync(
                client => client.Channel.SubtractAsync(valueOne, valueTwo));
        }
    }
}
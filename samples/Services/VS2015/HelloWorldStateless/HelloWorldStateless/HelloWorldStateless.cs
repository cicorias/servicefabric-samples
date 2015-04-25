// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace HelloWorldStateless
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services;

    /// <summary>
    /// A stateless service is a simple service that doesn't maintain any internal state that needs to be made highly-available or persisted.
    /// </summary>
    public class HelloWorldStateless : StatelessService
    {
        /// <summary>
        /// Creates a listener to handle user requests.
        /// </summary>
        /// <returns></returns>
        protected override ICommunicationListener CreateCommunicationListener()
        {
            // To handle user requests, return a listener that implements ICommunicationListener.
            // This service does not handle user requests, so we can either do nothing or remove the override entirely.
            return base.CreateCommunicationListener();
        }

        /// <summary>
        /// The platform calls this method when an instance of your service is placed and ready to execute.
        /// </summary>
        /// <param name="cancellationToken">
        /// The system uses a cancellation token to signal your service when it's time to stop running.
        /// </param>
        /// <returns></returns>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            ConcurrentDictionary<int, CustomObject> dictionary = new ConcurrentDictionary<int, CustomObject>();

            int i = 1;
            while (!cancellationToken.IsCancellationRequested)
            {
                dictionary.AddOrUpdate(i, new CustomObject() {Data = i}, (k, v) => new CustomObject() {Data = v.Data + 1});

                ServiceEventSource.Current.ServiceMessage(
                    this,
                    "Total Custom Objects: {0}. Data Average: {1}",
                    dictionary.Count,
                    dictionary.Average(item => item.Value.Data));

                i = i%10 == 0 ? 1 : i + 1;

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
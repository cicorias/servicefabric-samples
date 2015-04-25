// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace HelloWorldStateful
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services;

    /// <summary>
    /// A stateful service maintains state reliably within the service itself, co-located with the code that's using it.
    /// </summary>
    public class HelloWorldStateful : StatefulService
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
            IReliableDictionary<int, CustomObject> dictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, CustomObject>>("dictionary1");

            int i = 1;
            while (!cancellationToken.IsCancellationRequested)
            {
                using (ITransaction tx = this.StateManager.CreateTransaction())
                {
                    await dictionary.AddOrUpdateAsync(tx, i, new CustomObject() {Data = i}, (k, v) => new CustomObject() {Data = v.Data + 1});
                    await tx.CommitAsync();
                }

                ServiceEventSource.Current.ServiceMessage(
                    this,
                    "Total Custom Objects: {0}. Data Average: {1}",
                    await dictionary.GetCountAsync(),
                    dictionary.Average(item => item.Value.Data));

                i = i%10 == 0 ? 1 : i + 1;

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
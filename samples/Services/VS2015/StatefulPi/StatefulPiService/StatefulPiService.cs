// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace StatefulPiService
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services;

    public class StatefulPiService : StatefulService
    {
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            Trace.WriteLine("Starting Pi estimation.");

            try
            {
                IReliableDictionary<int, Estimate> estimateDictionary =
                    await this.StateManager.GetOrAddAsync<IReliableDictionary<int, Estimate>>("estimateDictionary");

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    using (ITransaction tx = this.StateManager.CreateTransaction())
                    {
                        await estimateDictionary.AddOrUpdateAsync(
                            tx,
                            1,
                            Estimate.PI(null),
                            (key, value) =>
                            {
                                Estimate next = Estimate.PI(value);

                                ServiceEventSource.Current.ServiceMessage(this, next.ToString());

                                return next;
                            });

                        await tx.CommitAsync();
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);

                throw;
            }
        }
    }
}
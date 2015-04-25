// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Samples
{
    using System.Threading.Tasks;

    public interface IHello : IActor
    {
        Task<string> SayHello(string greeting);
    }
}
// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.Service.Fabric.Samples.VoicemailBox.Interfaces
{
    using Microsoft.ServiceFabric.Actors;

    public interface IVoicemailBoxActorEvents : IActorEvents
    {
        void NewMessagesAvailable();
    }
}
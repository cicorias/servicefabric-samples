//-----------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
// 
//      THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//      EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//      OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
//      The example companies, organizations, products, domain names,
//      e-mail addresses, logos, people, places, and events depicted
//      herein are fictitious.  No association with any real company,
//      organization, product, domain name, email address, logo, person,
//      places, or events is intended or should be inferred.
//-----------------------------------------------------------------------


namespace Microsoft.ServiceFabric.Actors.Samples
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors;

    public interface IVoicemailBoxActor : IActor
    {
        [Readonly]
        Task<List<Voicemail>> GetMessagesAsync();

        [Readonly]
        Task<string> GetGreetingAsync();

        Task LeaveMessageAsync(string message);

        Task SetGreetingAsync(string greeting);

        Task DeleteMessageAsync(Guid messageId);

        Task DeleteAllMessagesAsync();
    }
}

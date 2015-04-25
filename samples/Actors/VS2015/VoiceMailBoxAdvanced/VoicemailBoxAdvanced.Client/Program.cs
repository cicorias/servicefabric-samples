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

namespace Microsoft.Azure.Service.Fabric.Samples.VoicemailBox.Client
{
    using Microsoft.Azure.Service.Fabric.Samples.VoicemailBox.Interfaces;
    using Microsoft.ServiceFabric.Actors;
    using System;
    using System.Fabric;
    using System.Globalization;
    using System.Threading;

    public class Program
    {
        class NewMessagesEventHandler : IVoicemailBoxActorEvents
        {
            public void NewMessagesAvailable()
            {
                Console.WriteLine(@"New messages are available.");
            }
        }

        public static void Main(string[] args)
        {
            var r = new Random();
            var actorId = (args.Length > 0) ? new ActorId(long.Parse(args[0])) : ActorId.NewId();

            var count = 0;
            var deleteMessageAfter = r.Next(1, 20);
            var deleteAllMessagesAfter = r.Next(1, 20);

            const string appName = "fabric:/VoicemailBoxAdvancedApplication";
            var voicemailBoxActor = ActorProxy.Create<IVoicemailBoxActor>(actorId, appName);
            voicemailBoxActor.SubscribeAsync<IVoicemailBoxActorEvents>(new NewMessagesEventHandler()).Wait();

            PrintConnectionInfo(voicemailBoxActor);
            voicemailBoxActor.GetMessagesAsync().Wait();

            while (true)
            {
                count++;
                PrintConnectionInfo(voicemailBoxActor);

                Console.WriteLine();
                Console.WriteLine(@"Greeting: {0}", voicemailBoxActor.GetGreetingAsync().Result);

                var message = string.Format(CultureInfo.InvariantCulture, "Hello WinFab Actor {0}, Greetings # {1}", actorId, count);
                Console.WriteLine(@"Leaving message: " + message);
                voicemailBoxActor.LeaveMessageAsync(message).Wait();

                Console.WriteLine();
                Console.WriteLine(@"Playing all messages:");
                var messageList = voicemailBoxActor.GetMessagesAsync().Result;
                messageList.Sort((x, y) => x.ReceivedAt.CompareTo(y.ReceivedAt));
                foreach (var m in messageList)
                {
                    Console.WriteLine(@"	 Received: {0}, Message: {1}", m.ReceivedAt, m.Message);
                }

                if (messageList.Count >= deleteAllMessagesAfter)
                {
                    Console.WriteLine();
                    Console.WriteLine(@"Deleting all messages");
                    voicemailBoxActor.DeleteAllMessagesAsync().Wait();
                    deleteAllMessagesAfter = r.Next(1, 20);
                }
                else if (messageList.Count >= deleteMessageAfter)
                {
                    var toBeDeleted = messageList[r.Next(messageList.Count)];

                    Console.WriteLine();
                    Console.WriteLine(@"Deleting message {0}", toBeDeleted.Message);
                    voicemailBoxActor.DeleteMessageAsync(toBeDeleted.Id).Wait();
                    deleteMessageAfter = r.Next(1, 20);
                }

                Thread.Sleep(500);
            }
        }

        private static void PrintConnectionInfo(IVoicemailBoxActor voicemailBoxActor)
        {
            var actorProxy = voicemailBoxActor as IActorProxy;

            if (actorProxy != null)
            {
                ResolvedServicePartition rsp;
                if (actorProxy.ActorServicePartitionClient.TryGetLastResolvedServicePartition(out rsp))
                {
                    var endpoint = rsp.GetEndpoint();
                    Console.WriteLine();
                    Console.WriteLine(
                        @"Connected to a VoicemailBox of an actor {0} hosted by the replica of a {3} Service {1} listening at Address {2}",
                        actorProxy.ActorId,
                        rsp.ServiceName,
                        endpoint.Address,
                        endpoint.Role);
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine(
                        @"Connecting to Voicemail box of an actor {0} hosted by the replica of Service {1}",
                        actorProxy.ActorId,
                        actorProxy.ActorServicePartitionClient.ServiceUri);
                }
            }
        }
    }
}

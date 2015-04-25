// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Fabric.Actor.Samples
{
    using System;
    using System.Threading;
    using HelloWorld.Interfaces;
    using Microsoft.ServiceFabric.Actors;

    internal class Program
    {
        private const string ApplicationName = "fabric:/HelloWorldApplication";

        private static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                for (;;)
                {
                    IHelloWorld friend = ActorProxy.Create<IHelloWorld>(ActorId.NewId(), ApplicationName);
                    Console.WriteLine("\n\nFrom Actor {1}: {0}\n\n", friend.SayHello("Good morning!").Result, friend.GetActorId());
                    Thread.Sleep(50);
                }
            }
            else
            {
                IHelloWorld friend = ActorProxy.Create<IHelloWorld>(ActorId.NewId(), ApplicationName);
                Console.WriteLine("\n\nFrom Actor {1}: {0}\n\n", friend.SayHello("Good morning!").Result, friend.GetActorId());
            }

            Console.WriteLine("Press enter to exit ...");
            Console.ReadLine();
        }
    }
}
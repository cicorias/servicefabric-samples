using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace HelloWorld
{
    [EventSource(Name = "MyCompany-HelloWorldApplication-HelloWorld")]
    internal sealed class ServiceEventSource : EventSource
    {
        public static ServiceEventSource Current = new ServiceEventSource();

        [NonEvent]
        public void Message(string message, params object[] args)
        {
            string finalMessage = string.Format(message, args);
            Message(finalMessage);
        }

        [Event(1, Level = EventLevel.Verbose)]
        public void Message(string message)
        {
            if (this.IsEnabled())
            {
                WriteEvent(1, message);
            }
        }

        [Event(2, Level = EventLevel.Informational, Message = "Service host {0} registering actor type {1}")]
        public void ActorTypeRegistered(int hostProcessId, string actorType)
        {
            WriteEvent(2, hostProcessId, actorType);
        }

        [NonEvent]
        public void ActorHostInitializationFailed(Exception e)
        {
            ActorHostInitializationFailed(e.ToString());
        }

        [Event(3, Level = EventLevel.Error, Message = "Actor host initialization failed")]
        private void ActorHostInitializationFailed(string exception)
        {
            WriteEvent(3, exception);
        }

        [NonEvent]
        public void ActorActivatedStart(Actor a)
        {
            ActorActivatedStart(a.GetType().ToString(), a.Id.ToString(), a.Host.Partition.PartitionInfo.Id);
        }

        [NonEvent]
        public void ActorActivatedStart<T>(Actor<T> a) where T : class
        {
            ActorActivatedStart(a.GetType().ToString(), a.Id.ToString(), a.Host.Partition.PartitionInfo.Id);
        }

        [Event(4, Level = EventLevel.Informational, Message = "Actor {1} ({0}) activation start")]
        private void ActorActivatedStart(string actorType, string actorId, Guid partitionId)
        {
            WriteEvent(4, actorType, actorId, partitionId);
        }

        [NonEvent]
        public void ActorActivatedStop(Actor a)
        {
            ActorActivatedStop(a.GetType().ToString(), a.Id.ToString(), a.Host.Partition.PartitionInfo.Id);
        }

        [NonEvent]
        public void ActorActivatedStop<T>(Actor<T> a) where T : class
        {
            ActorActivatedStop(a.GetType().ToString(), a.Id.ToString(), a.Host.Partition.PartitionInfo.Id);
        }

        [Event(5, Level = EventLevel.Informational, Message = "Actor {1} ({0}) activated")]
        private void ActorActivatedStop(string actorType, string actorId, Guid partitionId)
        {
            WriteEvent(5, actorType, actorId, partitionId);
        }

        [NonEvent]
        public void ActorDeactivatedStart(Actor a)
        {
            ActorDeactivatedStart(a.GetType().ToString(), a.Id.ToString(), a.Host.Partition.PartitionInfo.Id);
        }

        [NonEvent]
        public void ActorDeactivatedStart<T>(Actor<T> a) where T : class
        {
            ActorDeactivatedStart(a.GetType().ToString(), a.Id.ToString(), a.Host.Partition.PartitionInfo.Id);
        }

        [Event(6, Level = EventLevel.Informational, Message = "Actor {1} ({0}) deactivation start")]
        private void ActorDeactivatedStart(string actorType, string actorId, Guid partitionId)
        {
            WriteEvent(6, actorType, actorId, partitionId);
        }

        [NonEvent]
        public void ActorDeactivatedStop(Actor a)
        {
            ActorActivatedStop(a.GetType().ToString(), a.Id.ToString(), a.Host.Partition.PartitionInfo.Id);
        }

        [NonEvent]
        public void ActorDeactivatedStop<T>(Actor<T> a) where T : class
        {
            ActorActivatedStop(a.GetType().ToString(), a.Id.ToString(), a.Host.Partition.PartitionInfo.Id);
        }

        [Event(7, Level = EventLevel.Informational, Message = "Actor {1} ({0}) deactivated")]
        private void ActorDeactivatedStop(string actorType, string actorId, Guid partitionId)
        {
            WriteEvent(7, actorType, actorId, partitionId);
        }

        [NonEvent]
        public void ActorRequestStart(ActorBase a, string requestName)
        {
            ActorRequestStart(a.GetType().ToString(), a.Id.ToString(), requestName);
        }

        [Event(8, Level = EventLevel.Informational, Message = "Actor {1} handling request {2}")]
        private void ActorRequestStart(string actorType, string actorId, string requestName)
        {
            WriteEvent(8, actorType, actorId, requestName);
        }

        [NonEvent]
        public void ActorRequestStop(ActorBase a, string requestName)
        {
            ActorRequestStop(a.GetType().ToString(), a.Id.ToString(), requestName);
        }

        [Event(9, Level = EventLevel.Informational)]
        private void ActorRequestStop(string actorType, string actorId, string requestName)
        {
            WriteEvent(9, actorType, actorId, requestName);
        }
    }
}

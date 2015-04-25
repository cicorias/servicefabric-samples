// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.Service.Fabric.Samples.VoicemailBox
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Description;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Service.Fabric.Samples.VoicemailBox.Interfaces;
    using Microsoft.ServiceFabric.Actors;

    public class VoiceMailBoxActor : Actor<VoicemailBox>, IVoicemailBoxActor, IRemindable
    {
        private const int MessageRetentionTimeInMinutes = 72*60;
        private const int OldMessageDeletionTimerFrequencyInMinutes = 60;
        private const string UnreadMessageReminderName = "UnreadMessageReminder";
        private const int UnreadMessageReminderFrequencyInMinutes = 6*60;
        private IActorTimer oldMessageDeletionTimer;
        // Implements the ReceiveReminderAsync method of the IRemindable interface
        public Task ReceiveReminderAsync(string reminderName, byte[] context, TimeSpan dueTime, TimeSpan period)
        {
            // It is possible that we no longer have unread messages. This can happen if the messages were read 
            // or deleted just before this callback was invoked, but it was already too late to prevent this 
            // reminder callback from running. So before sending the notification, check if we still have unread
            // messages
            if (this.State.HasUnreadMessages)
            {
                IVoicemailBoxActorEvents events = this.GetEvent<IVoicemailBoxActorEvents>();
                events.NewMessagesAvailable();
            }
            return Task.FromResult(true);
        }

        public Task<List<Voicemail>> GetMessagesAsync()
        {
            this.State.HasUnreadMessages = false;

            // We don't have any more unread messages, so unregister the unread messages reminder
            // and return all the messages to the caller.
            return this.UnregisterUnreadMessagesReminderAsync().ContinueWith(t => this.State.MessageList);
        }

        public Task<string> GetGreetingAsync()
        {
            if (string.IsNullOrEmpty(this.State.Greeting))
            {
                ConfigurationSettings configSettings = this.Host.ActivationContext.GetConfigurationPackageObject("Config").Settings;
                ConfigurationSection configSection = configSettings.Sections.FirstOrDefault(s => (s.Name == "GreetingConfig"));
                if (configSection != null)
                {
                    ConfigurationProperty defaultGreeting = configSection.Parameters.FirstOrDefault(p => (p.Name == "DefaultGreeting"));
                    if (defaultGreeting != null)
                    {
                        return Task.FromResult(defaultGreeting.Value);
                    }
                }

                return Task.FromResult("No one is available, please leave a message after the beep.");
            }

            return Task.FromResult(this.State.Greeting);
        }

        public Task LeaveMessageAsync(string message)
        {
            this.State.MessageList.Add(
                new Voicemail
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    ReceivedAt = DateTime.Now
                });

            if (this.State.MessageList.Count == 1)
            {
                // This is the first voicemail, so start the timer that enforces
                // the time-based retention policy for messages.
                this.RegisterOldMessageDeletionTimer(false);
            }

            if (!this.State.HasUnreadMessages)
            {
                // Send a notification that new voicemail message(s) are available. Repeat the
                // notification periodically until the new voicemail messages have been read.
                //
                // We'd like to send these notifications even if the actor is deactivated for any
                // reason, so a reminder is an appropriate choice for this. Note that a timer
                // would not be suitable in this situation because it would be disarmed if the
                // actor is deactivated for any reason.
                this.State.HasUnreadMessages = true;
                return this.RegisterReminder(
                    UnreadMessageReminderName,
                    null,
                    TimeSpan.FromMinutes(0),
                    TimeSpan.FromMinutes(UnreadMessageReminderFrequencyInMinutes),
                    ActorReminderAttributes.Readonly);
            }

            return Task.FromResult(true);
        }

        public Task SetGreetingAsync(string greeting)
        {
            this.State.Greeting = greeting;

            return Task.FromResult(true);
        }

        public Task DeleteMessageAsync(Guid messageId)
        {
            for (int i = 0; i < this.State.MessageList.Count; i++)
            {
                if (this.State.MessageList[i].Id.Equals(messageId))
                {
                    this.State.MessageList.RemoveAt(i);
                    break;
                }
            }

            return (this.State.MessageList.Count == 0) ? this.OnAllMessagesDeletedAsync() : Task.FromResult(true);
        }

        public Task DeleteAllMessagesAsync()
        {
            this.State.MessageList.Clear();

            return this.OnAllMessagesDeletedAsync();
        }

        public override Task OnActivateAsync()
        {
            if (this.State.MessageList.Count > 0)
            {
                // If we have at least one message, then start the timer that enforces
                // the time-based retention policy for messages.
                this.RegisterOldMessageDeletionTimer(true);
            }
            return Task.FromResult(true);
        }

        private void RegisterOldMessageDeletionTimer(bool invokeFirstCallbackImmediately)
        {
            // Deletion of old messages is lazy. We scan for and delete old messages only when
            // the actor is active. For this scenario, a timer is more suitable than a reminder.
            // A presence of an active timer does not prevent the actor from being deactivated 
            // if there is no other activity, i.e. client requests or active reminders, going on. 
            // Once the actor is deactivated, we timer is stopped and we no longer delete old
            // messages. When the actor becomes active again, for example due to the arrival of
            // a new client request, we resume the old message deletion activity.
            this.oldMessageDeletionTimer = this.RegisterTimer(
                this.DeleteOldMessages,
                null,
                invokeFirstCallbackImmediately
                    ? TimeSpan.FromMinutes(0)
                    : TimeSpan.FromMinutes(OldMessageDeletionTimerFrequencyInMinutes),
                TimeSpan.FromMinutes(OldMessageDeletionTimerFrequencyInMinutes),
                false);
        }

        private Task DeleteOldMessages(object context)
        {
            DateTime threshold = DateTime.Now.AddMinutes(-1*MessageRetentionTimeInMinutes);
            for (int i = 0; i < this.State.MessageList.Count; i++)
            {
                if (this.State.MessageList[i].ReceivedAt.CompareTo(threshold) < 0)
                {
                    this.State.MessageList.RemoveAt(i);
                    break;
                }
            }

            return (this.State.MessageList.Count == 0) ? this.OnAllMessagesDeletedAsync() : Task.FromResult(true);
        }

        private Task OnAllMessagesDeletedAsync()
        {
            // We don't have any messages, so we don't need the timer that enforces
            // time-based retention policy for messages.
            if (this.oldMessageDeletionTimer != null)
            {
                this.UnregisterTimer(this.oldMessageDeletionTimer);
                this.oldMessageDeletionTimer = null;
            }

            if (this.State.HasUnreadMessages)
            {
                // We don't have any messages at the moment, so we obviously don't
                // have any unread messages.
                this.State.HasUnreadMessages = false;

                // Disable the unread messages notification
                return this.UnregisterUnreadMessagesReminderAsync();
            }

            return Task.FromResult(true);
        }

        private Task UnregisterUnreadMessagesReminderAsync()
        {
            IActorReminder reminder;
            try
            {
                reminder = this.GetReminder(UnreadMessageReminderName);
            }
            catch (FabricException)
            {
                reminder = null;
            }

            return (reminder == null) ? Task.FromResult(true) : this.UnregisterReminder(reminder);
        }
    }
}
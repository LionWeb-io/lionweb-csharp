// Copyright 2025 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// SPDX-FileCopyrightText: 2024 TRUMPF Laser SE and other contributors
// SPDX-License-Identifier: Apache-2.0

namespace LionWeb.Core.Notification.Handler;

using System.Reflection;

/// A member in a directed graph that sends messages.
/// Each member is <see cref="Connect">connected</see>
/// <i>from</i> one or more <i>preceding</i> notification handlers, and
/// <i>to</i> one or more <i>following</i> notification handlers.
///
/// <para>
/// <i>Inbound</i> notification handlers have no <i>preceding</i> part.
/// They receive their messages from outside the notification handler graph via <see cref="INotificationHandler.Receive"/>. 
/// </para>
///
/// <para>
/// Upon <see cref="INotificationHandler.Receive">receiving</see> a message,
/// a notification handler can choose to <see cref="INotificationHandler.Send"/> the unmodified, modified, or a new message
/// to its <i>following</i> notification handlers.
/// A notification handler can also suppress an incoming message, i.e. not send the message to its <i>following</i> notification handlers. 
/// </para>
public interface INotificationHandler : IDisposable
{
    /// Whether anybody would receive any of the <paramref name="messageTypes"/> notifications.
    /// Useful for returning eagerly from complex logic to calculate the notification contents.
    /// <value>
    ///     <c>true</c> if someone would receive any of the <paramref name="messageTypes"/> notifications; <c>false</c> otherwise.
    /// </value>
    public bool CanReceive(params Type[] messageTypes);

    /// This notification handler receives <paramref name="message"/>.
    /// Call this on <i>inbound</i> notification handlers (i.e. notification handlers that get messages from outside the notification handler chain).
    public void Receive(INotification message);
    
    public void Receive(INotificationHandler correspondingHandler, INotification notification) =>
        Receive(notification);

    /// This notification handler wants to send <paramref name="message"/>.
    /// Only this notification handler should use this method.
    protected void Send(INotification message);

    /// Subscribes <paramref name="receiver"/> to this, <paramref name="receiver"/>
    /// <see cref="Receive">receives</see> all messages <see cref="Send">sent</see> by this notification handler.
    /// For internal use only, use <see cref="Connect"/>.
    internal void Subscribe(INotificationHandler receiver);

    protected string NotificationHandlerId { get; }

    /// All messages <see cref="INotificationHandler.Send">sent</see> by <paramref name="from"/>
    /// will be <see cref="INotificationHandler.Receive">received</see> by <paramref name="to"/>. 
    public static void Connect(
        INotificationHandler from,
        INotificationHandler to) =>
        from.Subscribe(to);

    /// Unsubscribes <paramref name="receiver"/> from this.
    /// For internal use only -- each notification handler should unsubscribe itself from all <i>preceding</i> notification handlers on disposal.
    protected internal void Unsubscribe(INotificationHandler receiver);

    protected internal void PrintAllReceivers(List<INotificationHandler> alreadyPrinted, string indent = "");

    protected static bool RecursionDetected(INotificationHandler self, List<INotificationHandler> alreadyPrinted,
        string indent)
    {
        try
        {
            if (!alreadyPrinted.Contains(self))
                return false;

            Console.WriteLine($"{indent}Recursion ^^");
            return true;
        } finally
        {
            alreadyPrinted.Add(self);
        }
    }
}
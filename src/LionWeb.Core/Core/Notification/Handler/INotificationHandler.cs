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

/// A member in a directed graph that sends notifications.
/// Each member is <see cref="INotificationHandlerConnector.Connect">connected</see>
/// <i>from</i> one or more <i>preceding</i> notification handlers, and
/// <i>to</i> one or more <i>following</i> notification handlers.
///
/// <para>
/// <i>Inbound</i> notification handlers have no <i>preceding</i> part.
/// They receive their notifications from outside the notification handler graph via <see cref="IInboundNotificationHandler.InitiateNotification"/>. 
/// </para>
///
/// <para>
/// Upon <see cref="IReceivingNotificationHandler.Receive">receiving</see> a notifications,
/// a notification handler can choose to <see cref="ISendingNotificationHandler.Send"/> the unmodified, modified, or a new notifications
/// to its <i>following</i> notification handlers.
/// A notification handler can also suppress an incoming notifications, i.e. not send the notifications to its <i>following</i> notification handlers. 
/// </para>
public interface INotificationHandler : IDisposable;

public interface INotificationHandlerConnector
{
    /// All notifications <see cref="ISendingNotificationHandler.Send">sent</see> by <paramref name="from"/>
    /// will be <see cref="IReceivingNotificationHandler.Receive">received</see> by <paramref name="to"/>. 
    public void Connect(
        ISendingNotificationHandler from,
        IReceivingNotificationHandler to) => from.Subscribe(to);
    
    public void ConnectTo(IReceivingNotificationHandler to);
}

/// A <see cref="INotificationHandler">notification handler</see> that can <see cref="Send"/> notifications
/// to <i>following</i> handlers.
public interface ISendingNotificationHandler :INotificationHandler, INotificationHandlerConnector
{
    /// This notification handler wants to send <paramref name="notification"/>.
    /// Only this notification handler should use this method.
    protected void Send(INotification notification);

    /// Subscribes <paramref name="receiver"/> to this, <paramref name="receiver"/>
    /// <see cref="IReceivingNotificationHandler.Receive">receives</see> all messages
    /// <see cref="ISendingNotificationHandler.Send">sent</see> by this notification handler.
    /// For internal use only, use <see cref="IReceivingNotificationHandler"/>.
    internal void Subscribe(IReceivingNotificationHandler receiver);

    /// Unsubscribes <paramref name="receiver"/> from this.
    /// For internal use only -- each notification handler should unsubscribe itself from all <i>preceding</i> notification handlers on disposal.
    protected internal void Unsubscribe(IReceivingNotificationHandler receiver);
}

/// A <see cref="INotificationHandler">notification handler</see> that can determine whether it
/// <see cref="Handles">can handle</see> specific notification types.
public interface IFilterReceivingNotificationHandler : INotificationHandler
{
    /// Whether anybody would receive any of the <paramref name="notificationTypes"/> notifications.
    /// Useful for returning eagerly from complex logic to calculate the notification contents.
    /// <value>
    ///     <c>true</c> if someone would receive any of the <paramref name="notificationTypes"/> notifications; <c>false</c> otherwise.
    /// </value>
    bool Handles(params Type[] notificationTypes);
}

/// An <i>Inbound</i> <see cref="INotificationHandler">notification handler</see> has no <i>preceding</i> part.
/// It <see cref="InitiateNotification">initiates</see> notifications from outside the notification handler graph,
/// and <see cref="ISendingNotificationHandler.Send">sends</see> them to <i>following</i> handlers. 
public interface IInboundNotificationHandler : IFilterReceivingNotificationHandler, ISendingNotificationHandler
{
    /// Receiving a notification from outside the notification handler graph.
    public void InitiateNotification(INotification notification);
}

/// A <see cref="INotificationHandler">notification handler</see> that can <see cref="Receive"/> notfications
/// from <i>preceding</i> handlers. 
public interface IReceivingNotificationHandler : IFilterReceivingNotificationHandler
{
    /// This notification handler receives <paramref name="notification"/>.
    void Receive(ISendingNotificationHandler correspondingHandler, INotification notification);
}

/// A <see cref="INotificationHandler">notification handler</see> that can both
/// <see cref="IReceivingNotificationHandler.Receive"/> notifications from <i>preceding</i> handlers and
/// <see cref="ISendingNotificationHandler.Send"/> notifications to <i>following</i> handlers.
public interface IConnectingNotificationHandler : IReceivingNotificationHandler, ISendingNotificationHandler;
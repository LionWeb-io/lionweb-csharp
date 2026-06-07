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

namespace LionWeb.Core.Notification.Pipe;

/// A member in notification <i>pipe</i>, as in <a href="https://en.wikipedia.org/wiki/Pipes_and_Filters">Pipes and Filters</a>.
/// A member can be <see cref="INotificationSender.ConnectTo">connected</see>
/// <i>from</i> one or more <i>preceding</i> notification pipe members, and
/// <i>to</i> one or more <i>following</i> notification pipe members.
///
/// <para>
/// <see cref="INotificationProducer">Producing</see> notification pipe members have no <i>preceding</i> part.
/// They act upon external stimuli, and <see cref="INotificationProducer.ProduceNotification">produce notifications</see> to enter the notification pipe system. 
/// </para>
///
/// <para>
/// Upon <see cref="INotificationReceiver.Receive">receiving</see> a notification,
/// a notification pipe member can choose to <see cref="NotificationPipeBase.Send"/> the unmodified, modified, or a new notification
/// to its <i>following</i> notification pipe members.
/// A notification pipe member can also suppress an incoming notifications, i.e. not send the notifications to its <i>following</i> notification pipe members. 
/// </para>
public interface INotificationPipe : IDisposable;

/// A <see cref="INotificationPipe">notification pipe member</see> that can <see cref="NotificationPipeBase.Send"/> notifications
/// to <i>following</i> pipe members.
public interface INotificationSender : INotificationPipe
{
    /// All notifications <see cref="NotificationPipeBase.Send">sent</see> by <c>this</c>
    /// will be <see cref="INotificationReceiver.Receive">received</see> by <paramref name="to"/>.
    /// <remarks>Equivalent to C# <c>event += to</c>.</remarks>
    void ConnectTo(INotificationReceiver to);

    /// Notifications <see cref="NotificationPipeBase.Send">sent</see> by <c>this</c>
    /// will <b>NOT</b> be <see cref="INotificationReceiver.Receive">received</see> by <paramref name="to"/> anymore.
    /// <remarks>Equivalent to C# <c>event -= to</c>.</remarks>
    void Disconnect(INotificationReceiver to);

    /// Subscribes <paramref name="receiver"/> to this.
    /// <para>
    /// <b>For internal use only, use <see cref="ConnectTo"/></b>.
    /// </para>
    /// <paramref name="receiver"/> <see cref="INotificationReceiver.Receive">receives</see> all messages
    /// <see cref="NotificationPipeBase.Send">sent</see> by this notification pipe.
    [Obsolete("Use ConnectTo() instead.")]
    protected internal void Subscribe(INotificationReceiver receiver) => ConnectTo(receiver);

    /// Unsubscribes <paramref name="receiver"/> from this.
    /// <para>
    /// <b>For internal use only</b> -- each notification pipe member should unsubscribe itself from all <i>preceding</i> notification pipe members on disposal.
    /// </para>
    [Obsolete("Use Disconnect() instead.")]
    protected internal void Unsubscribe(INotificationReceiver receiver) => Disconnect(receiver);
}

/// A <see cref="INotificationPipe">notification pipe member</see> that can determine whether it
/// <see cref="Handles()">can handle</see> a notification.
public interface INotificationFilter : INotificationPipe
{
    /// Whether anybody would receive any of the <paramref name="notificationTypes"/> notifications.
    /// Useful for returning eagerly from complex logic to calculate the notification contents.
    /// <returns>
    ///     <c>true</c> if someone would receive any of the <paramref name="notificationTypes"/> notifications; <c>false</c> otherwise.
    /// </returns>
    [Obsolete("Filtering by notification time is deprecated, use Handles() instead")]
    bool Handles(params Type[] notificationTypes) => Handles();

    /// Whether anybody would receive a notification.
    /// Useful for returning eagerly from complex logic to calculate the notification contents.
    /// <returns>
    ///     <c>true</c> if someone would receive a notification; <c>false</c> otherwise.
    /// </returns>
    bool Handles() => true;
}

/// A <i>producing</i> <see cref="INotificationPipe">notification pipe member</see> has no <i>preceding</i> member,
/// i.e. a "starting member" in a pipe.
/// It <see cref="ProduceNotification">produces</see> notifications from outside the notification pipe system,
/// and <see cref="NotificationPipeBase.Send">sends</see> them to <i>following</i> pipes. 
public interface INotificationProducer : INotificationFilter, INotificationSender, INotificationPipe
{
    /// Receiving a notification from outside the notification pipe system.
    void ProduceNotification(INotification notification);
}

/// A <see cref="INotificationPipe">notification pipe member</see> that can <see cref="Receive"/> notifications
/// from <i>preceding</i> notification pipe members.
public interface INotificationReceiver : INotificationPipe, INotificationFilter
{
    void IDisposable.Dispose() { }

    /// This notification pipe member receives <paramref name="notification"/>.
    void Receive(INotificationSender correspondingSender, INotification notification);
}

/// A <see cref="INotificationPipe">notification pipe member</see> that can both
/// <see cref="INotificationReceiver.Receive"/> notifications from <i>preceding</i> pipe members and
/// <see cref="NotificationPipeBase.Send"/> notifications to <i>following</i> pipe members,
/// i.e. a "middle member" in a pipe.
public interface INotificationHandler : INotificationReceiver, INotificationSender;
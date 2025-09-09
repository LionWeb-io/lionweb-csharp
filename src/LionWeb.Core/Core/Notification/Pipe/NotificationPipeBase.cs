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

using Forest;
using Partition;
using System.Reflection;

/// Base for all <see cref="INotificationPipe">notification pipes</see> that process <see cref="INotification">notifications</see>.
public abstract class NotificationPipeBase : INotificationFilter, INotificationSender
{
    protected readonly object Sender;
    private readonly Dictionary<Type, int> _subscribedNotifications = [];
    private readonly Dictionary<INotificationReceiver, EventHandler<INotification>> _handlers = [];

    /// <inheritdoc cref="NotificationPipeBase"/>
    /// <param name="sender">Optional sender of the notifications.</param>
    protected NotificationPipeBase(object? sender)
    {
        Sender = sender ?? this;
    }

    /// <inheritdoc />
    public void ConnectTo(INotificationReceiver to) =>
        ((INotificationSender)this).Subscribe(to);
    
    /// <inheritdoc />
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        foreach (var (receiver, eventHandler) in _handlers)
        {
            UnsubscribeHandler(receiver, eventHandler);
        }

        return;

        void UnsubscribeHandler<T>(INotificationReceiver receiver, EventHandler<T> _) =>
            Unsubscribe(receiver);
    }

    private event EventHandler<INotification>? InternalEvent;


    /// <inheritdoc />
    public bool Handles(params Type[] notificationTypes) =>
        InternalEvent != null &&
        notificationTypes.Any(notificationType =>
            _subscribedNotifications.TryGetValue(notificationType, out var count) && count > 0);


    /// <inheritdoc />
    void INotificationSender.Send(INotification notification) =>
        Send(notification);

    /// <inheritdoc cref="INotificationSender.Send"/>
    protected virtual void Send(INotification notification) =>
        SendWithSender(this, notification);

    /// This notification pipe wants to send <paramref name="message"/> with <paramref name="sender"/>.
    /// Only this notification pipe should use this method.
    protected void SendWithSender(object? sender, INotification message) =>
        InternalEvent?.Invoke(sender, message);

    /// <inheritdoc />
    void INotificationSender.Subscribe(INotificationReceiver receiver) =>
        Subscribe<INotification>(receiver);

    /// <inheritdoc cref="INotificationSender.Subscribe"/>
    private void Subscribe<TSubscribedNotification>(INotificationReceiver receiver)
        where TSubscribedNotification : INotification
    {
        RegisterSubscribedNotifications<TSubscribedNotification>();

        var handler = CreateHandler<INotification>(receiver);

        if (_handlers.TryAdd(receiver, handler))
            InternalEvent += handler;
    }

    private EventHandler<INotification> CreateHandler<TSubscribedNotification>(
        INotificationReceiver receiver) where TSubscribedNotification : INotification =>
        (sender, notification) =>
        {
            if (notification is not TSubscribedNotification r)
                return;

            if (sender is INotificationSender handler)
            {
                receiver.Receive(handler, r);
                return;
            }

            receiver.Receive(null, r);
        };

    private void RegisterSubscribedNotifications<TSubscribedNotification>()
    {
        var notificationType = typeof(TSubscribedNotification);
        var allSubtype = _allSubtypes[notificationType];
        foreach (var subtype in allSubtype)
        {
            if (_subscribedNotifications.TryGetValue(subtype, out var count))
            {
                _subscribedNotifications[subtype] = count + 1;
            } else
            {
                _subscribedNotifications[subtype] = 1;
            }
        }
    }

    private void UnregisterSubscribedNotifications()
    {
        var notificationType = typeof(INotification);
        var allSubtypes = _allSubtypes[notificationType];
        foreach (var subtype in allSubtypes)
        {
            _subscribedNotifications[subtype]--;
        }
    }

    /// <inheritdoc />
    public void Unsubscribe(INotificationReceiver receiver)
    {
        if (!_handlers.Remove(receiver, out var handler))
            return;

        InternalEvent -= handler;

        UnregisterSubscribedNotifications();
    }

    private static readonly ILookup<Type, Type> _allSubtypes =
        InitAllSubtypes();

    private static ILookup<Type, Type> InitAllSubtypes()
    {
        Type[] allTypes = Assembly
            .GetExecutingAssembly()
            .GetTypes();

        List<Type> baseTypes = [typeof(INotification), typeof(IForestNotification), typeof(IPartitionNotification)];

        return baseTypes
            .SelectMany(baseType => allTypes
                .Where(subType => subType.IsAssignableTo(baseType))
                .SelectMany(subType => new List<(Type, Type)> { (baseType, subType), (subType, subType) })
            )
            .ToLookup(k => k.Item1, e => e.Item2);
    }
}
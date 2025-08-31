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

using Forest;
using Partition;
using System.Reflection;

/// Base for all <see cref="INotificationHandler">notification handlers</see> that process <see cref="INotification">notifications</see>.
public abstract class NotificationHandlerBase : IFilterReceivingNotificationHandler, ISendingNotificationHandler
{
    protected readonly object Sender;
    private readonly Dictionary<Type, int> _subscribedNotifications = [];
    private readonly Dictionary<IReceivingNotificationHandler, EventHandler<INotification>> _handlers = [];

    /// <inheritdoc cref="NotificationHandlerBase"/>
    /// <param name="sender">Optional sender of the notifications.</param>
    protected NotificationHandlerBase(object? sender)
    {
        Sender = sender ?? this;
    }

    /// <inheritdoc cref="INotificationHandlerConnector.ConnectTo"/>
    public void ConnectTo(IReceivingNotificationHandler to) => INotificationHandlerConnector.Connect(this, to);
    
    /// <inheritdoc />
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        foreach (var (notificationHandler, eventHandler) in _handlers)
        {
            UnsubscribeHandler(notificationHandler, eventHandler);
        }

        return;

        void UnsubscribeHandler<T>(IReceivingNotificationHandler notificationHandler, EventHandler<T> _) =>
            Unsubscribe(notificationHandler);
    }

    private event EventHandler<INotification>? InternalEvent;


    /// <inheritdoc />
    public bool Handles(params Type[] notificationTypes) =>
        InternalEvent != null &&
        notificationTypes.Any(notificationType =>
            _subscribedNotifications.TryGetValue(notificationType, out var count) && count > 0);


    /// <inheritdoc />
    void ISendingNotificationHandler.Send(INotification notification) =>
        Send(notification);

    /// <inheritdoc cref="ISendingNotificationHandler.Send"/>
    protected virtual void Send(INotification notification) =>
        SendWithSender(this, notification);

    /// This notification handler wants to send <paramref name="message"/> with <paramref name="sender"/>.
    /// Only this notification handler should use this method.
    protected void SendWithSender(object? sender, INotification message) =>
        InternalEvent?.Invoke(sender, message);

    /// <inheritdoc />
    void ISendingNotificationHandler.Subscribe(IReceivingNotificationHandler receiver) =>
        Subscribe<INotification>(receiver);

    /// <inheritdoc cref="ISendingNotificationHandler.Subscribe"/>
    private void Subscribe<TSubscribedNotification>(IReceivingNotificationHandler receiver)
        where TSubscribedNotification : INotification
    {
        RegisterSubscribedNotifications<TSubscribedNotification>();

        var handler = CreateHandler<INotification>(receiver);

        if (_handlers.TryAdd(receiver, handler))
            InternalEvent += handler;
    }

    private EventHandler<INotification> CreateHandler<TSubscribedNotification>(
        IReceivingNotificationHandler receiver) where TSubscribedNotification : INotification =>
        (sender, notification) =>
        {
            if (notification is not TSubscribedNotification r)
                return;

            if (sender is ISendingNotificationHandler handler)
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
    public void Unsubscribe(IReceivingNotificationHandler receiver)
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
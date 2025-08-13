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

/// Base for all <see cref="IHandler">notification handlers</see> that process <see cref="INotification">notifications</see>.
public abstract class NotificationHandlerBase
{
    protected static readonly ILookup<Type, Type> AllSubtypes = InitAllSubtypes();

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

/// <inheritdoc cref="NotificationHandlerBase"/>
public abstract class NotificationHandlerBase<TNotification> : NotificationHandlerBase,
    INotificationHandler<TNotification> where TNotification : INotification
{
    protected readonly object Sender;
    private readonly Dictionary<Type, int> _subscribedNotifications = [];
    private readonly Dictionary<IHandler, EventHandler<TNotification>> _handlers = [];

    /// <inheritdoc cref="NotificationHandlerBase"/>
    /// <param name="sender">Optional sender of the notifications.</param>
    protected NotificationHandlerBase(object? sender)
    {
        Sender = sender ?? this;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        foreach (var (notificationHandler, eventHandler) in _handlers)
        {
            UnsubscribeHandler(notificationHandler, eventHandler);
        }

        return;

        void UnsubscribeHandler<T>(IHandler notificationHandler, EventHandler<T> _) =>
            Unsubscribe<T>(notificationHandler);
    }

    /// <inheritdoc />
    public string NotificationHandlerId =>
        Sender.ToString() ?? GetType().Name;

    /// <inheritdoc />
    public abstract void Receive(TNotification message);

    private event EventHandler<TNotification>? InternalEvent;


    /// <inheritdoc />
    public bool CanReceive(params Type[] messageTypes) =>
        InternalEvent != null &&
        messageTypes.Any(notificationType =>
            _subscribedNotifications.TryGetValue(notificationType, out var count) && count > 0);


    /// <inheritdoc />
    void IHandler<TNotification, TNotification>.Send(TNotification message) =>
        Send(message);

    /// <inheritdoc cref="IHandler{TReceive,TSend}.Send"/>
    protected void Send(TNotification message) =>
        InternalEvent?.Invoke(Sender, message);

    /// <inheritdoc />
    void IHandler<TNotification, TNotification>.Subscribe<TReceiveTo, TSendTo>(
        IHandler<TReceiveTo, TSendTo> receiver) =>
        Subscribe(receiver);

    /// <inheritdoc cref="IHandler{TReceive,TSend}.Subscribe{TReceiveTo,TSendTo}"/>
    protected void Subscribe<TReceiveTo, TSendTo>(IHandler<TReceiveTo, TSendTo> receiver)
    {
        RegisterSubscribedNotifications<TReceiveTo>();

        var handler = CreateHandler(receiver);

        _handlers[receiver] = handler;

        InternalEvent += handler;
    }

    private EventHandler<TNotification> CreateHandler<TReceiveTo, TSendTo>(IHandler<TReceiveTo, TSendTo> receiver)
        => (_, notification) =>
        {
            if (notification is TReceiveTo r)
                receiver.Receive(r);
        };

    private void RegisterSubscribedNotifications<TSubscribedNotification>()
    {
        var notificationType = typeof(TSubscribedNotification);
        var allSubtype = AllSubtypes[notificationType];
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

    private void UnregisterSubscribedNotifications<TSubscribedNotification>()
    {
        var notificationType = typeof(TSubscribedNotification);
        var allSubtypes = AllSubtypes[notificationType];
        foreach (var subtype in allSubtypes)
        {
            _subscribedNotifications[subtype]--;
        }
    }

    /// <inheritdoc />
    void IHandler.Unsubscribe<T>(IHandler receiver) =>
        Unsubscribe<T>(receiver);

    /// <inheritdoc cref="IHandler.Unsubscribe{T}"/>
    private void Unsubscribe<T>(IHandler receiver)
    {
        if (!_handlers.Remove(receiver, out var handler))
            return;

        InternalEvent -= handler;

        UnregisterSubscribedNotifications<T>();
    }

    /// <inheritdoc />
    public void PrintAllReceivers(List<IHandler> alreadyPrinted, string indent = "")
    {
        Console.WriteLine($"{indent}{this.GetType().Name}({Sender})");
        if (IHandler.RecursionDetected(this, alreadyPrinted, indent))
            return;

        Console.WriteLine($"{indent}Handlers:");
        foreach (var handler in this._handlers.Keys)
        {
            handler.PrintAllReceivers(alreadyPrinted, indent + "  ");
        }
    }
}
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

namespace LionWeb.Core.Notification.Processor;

using Forest;
using Partition;
using System.Reflection;

/// Base for all <see cref="IProcessor">processors</see> that process <see cref="INotification">notifications</see>.
public abstract class NotificationProcessorBase
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

/// <inheritdoc cref="NotificationProcessorBase"/>
public abstract class NotificationProcessorBase<TNotification> : NotificationProcessorBase,
    INotificationProcessor<TNotification> where TNotification : INotification
{
    protected readonly object Sender;
    private readonly Dictionary<Type, int> _subscribedNotifications = [];
    private readonly Dictionary<IProcessor, EventHandler<TNotification>> _handlers = [];

    /// <inheritdoc cref="NotificationProcessorBase"/>
    /// <param name="sender">Optional sender of the notifications.</param>
    protected NotificationProcessorBase(object? sender)
    {
        Sender = sender ?? this;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        foreach (var (processor, handler) in _handlers)
        {
            UnsubscribeProcessor(processor, handler);
        }

        return;

        void UnsubscribeProcessor<T>(IProcessor processor, EventHandler<T> _) =>
            Unsubscribe<T>(processor);
    }

    /// <inheritdoc />
    public string ProcessorId =>
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
    void IProcessor<TNotification, TNotification>.Send(TNotification message) =>
        Send(message);

    /// <inheritdoc cref="IProcessor{TReceive,TSend}.Send"/>
    protected void Send(TNotification message) =>
        InternalEvent?.Invoke(Sender, message);

    /// <inheritdoc />
    void IProcessor<TNotification, TNotification>.Subscribe<TReceiveTo, TSendTo>(
        IProcessor<TReceiveTo, TSendTo> receiver) =>
        Subscribe(receiver);

    /// <inheritdoc cref="IProcessor{TReceive,TSend}.Subscribe{TReceiveTo,TSendTo}"/>
    protected void Subscribe<TReceiveTo, TSendTo>(IProcessor<TReceiveTo, TSendTo> receiver)
    {
        RegisterSubscribedNotifications<TReceiveTo>();

        var handler = CreateHandler(receiver);

        _handlers[receiver] = handler;

        InternalEvent += handler;
    }

    private EventHandler<TNotification> CreateHandler<TReceiveTo, TSendTo>(IProcessor<TReceiveTo, TSendTo> receiver)
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
    void IProcessor.Unsubscribe<T>(IProcessor receiver) =>
        Unsubscribe<T>(receiver);

    /// <inheritdoc cref="IProcessor.Unsubscribe{T}"/>
    private void Unsubscribe<T>(IProcessor receiver)
    {
        if (!_handlers.Remove(receiver, out var handler))
            return;

        InternalEvent -= handler;

        UnregisterSubscribedNotifications<T>();
    }

    /// <inheritdoc />
    public void PrintAllReceivers(List<IProcessor> alreadyPrinted, string indent = "")
    {
        Console.WriteLine($"{indent}{this.GetType().Name}({Sender})");
        if (IProcessor.RecursionDetected(this, alreadyPrinted, indent))
            return;

        Console.WriteLine($"{indent}Handlers:");
        foreach (var processor in this._handlers.Keys)
        {
            processor.PrintAllReceivers(alreadyPrinted, indent + "  ");
        }
    }
}
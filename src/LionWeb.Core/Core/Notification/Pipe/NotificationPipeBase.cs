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

/// Base for all <see cref="INotificationPipe">notification pipes</see> that process <see cref="INotification">notifications</see>.
public abstract class NotificationPipeBase : INotificationFilter, INotificationSender
{
    protected readonly object Sender;
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
    public void Disconnect(INotificationReceiver to) => 
        ((INotificationSender)this).Unsubscribe(to);

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
        InternalEvent != null;


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
    void INotificationSender.Subscribe(INotificationReceiver receiver)
    {
        var handler = CreateHandler(receiver);

        if (_handlers.TryAdd(receiver, handler))
            InternalEvent += handler;
    }

    private EventHandler<INotification> CreateHandler(INotificationReceiver receiver) =>
        (sender, notification) => receiver.Receive(sender as INotificationSender, notification);

    /// <inheritdoc />
    public void Unsubscribe(INotificationReceiver receiver)
    {
        if (!_handlers.Remove(receiver, out var handler))
            return;

        InternalEvent -= handler;
    }
}
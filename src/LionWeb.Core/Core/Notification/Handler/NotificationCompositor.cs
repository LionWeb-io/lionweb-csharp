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

/// Compose several <see cref="INotification">notifications</see> into one <see cref="CompositeNotification"/>.
///
/// We keep a stack of composites.
/// Every time a new composite is <see cref="Push">pushed</see>, that composite is added to the previous one (if any).
public class NotificationCompositor : NotificationHandlerBase, IConnectingNotificationHandler
{
    private readonly INotificationIdProvider _idProvider;
    private readonly Stack<CompositeNotification> _composites = [];

    /// <inheritdoc cref="NotificationCompositor"/>
    public NotificationCompositor(object? notificationHandlerId) : base(notificationHandlerId)
    {
        _idProvider = new NotificationIdProvider(notificationHandlerId);
    }

    /// Pushes a new composite, and adds it to the previous one (if any).
    public CompositeNotification Push() =>
        Push(new CompositeNotification(_idProvider.CreateNotificationId()));

    /// Pushes <paramref name="composite"/>, and adds it to the previous one (if any).
    /// <returns><paramref name="composite"/></returns>
    public T Push<T>(T composite) where T : CompositeNotification
    {
        if (_composites.TryPeek(out CompositeNotification? previous))
            previous.AddPart(composite);

        _composites.Push(composite);
        return composite;
    }

    /// Pops the previous composite.
    /// <param name="send">If <c>true</c>, sends the popped composite to <i>following</i> handlers.</param>
    /// <returns>The previous composite</returns>
    /// <exception cref="InvalidOperationException">If there's no previous composite.</exception>
    public CompositeNotification Pop(bool send)
    {
        var result = _composites.Pop();
        if (send)
            Send(result);
        return result;
    }

    /// <inheritdoc />
    public void Receive(ISendingNotificationHandler correspondingHandler, INotification notification)
    {
        switch (notification)
        {
            case PartitionAddedNotification:
                RegisterPartition(correspondingHandler);
                break;
            case PartitionDeletedNotification:
                UnregisterPartition(correspondingHandler);
                break;
        }

        if (!TryAdd(notification))
            Send(notification);
    }

    private bool TryAdd(INotification notification)
    {
        if (!_composites.TryPeek(out var composite))
            return false;

        composite.AddPart(notification);
        return true;
    }

    private void RegisterPartition(ISendingNotificationHandler correspondingHandler) =>
        INotificationHandler.Connect(correspondingHandler, this);

    private void UnregisterPartition(ISendingNotificationHandler correspondingHandler) =>
        correspondingHandler.Unsubscribe(this);
}
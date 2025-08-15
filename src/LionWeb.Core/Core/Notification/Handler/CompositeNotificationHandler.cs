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

/// Composes two or more <see cref="INotificationHandler"/>s.
///
/// Every message this notification handler <see cref="Receive">receives</see>
/// is forwarded to the first <see cref="_notificationHandlers">component</see>.
/// Each component is connected to the next component.
/// The last component <see cref="IINotificationHandlerSend">sends</see> to
/// this notification handler's <i>following</i> notification handlers.
public class CompositeNotificationHandler : INotificationHandler
{
    private readonly INotificationHandler _firstHandler;
    private readonly INotificationHandler _lastHandler;
    private readonly IEnumerable<INotificationHandler> _notificationHandlers;
    private readonly object? _sender;

    public CompositeNotificationHandler(List<INotificationHandler> notificationHandlers,
        object? sender)
    {
        _notificationHandlers = notificationHandlers;
        _sender = sender;
        if (notificationHandlers.Count < 2)
            throw new ArgumentException(
                $"{nameof(CompositeNotificationHandler)} must get at least 2 notification handlers");

        _firstHandler = notificationHandlers[0];
        _lastHandler = notificationHandlers[^1];

        var enumerator = notificationHandlers.GetEnumerator();
        enumerator.MoveNext();
        var previous = enumerator.Current;
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            INotificationHandler.Connect(previous, current);

            previous = current;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        foreach (INotificationHandler handler in _notificationHandlers.Reverse())
        {
            handler.Dispose();
        }
    }

    /// <inheritdoc />
    public string NotificationHandlerId =>
        _sender?.ToString() ?? GetType().Name;

    /// <inheritdoc />
    public void Receive(INotification message) =>
        _firstHandler.Receive(message);

    /// <inheritdoc />
    public bool CanReceive(params Type[] messageTypes) =>
        _firstHandler.CanReceive(messageTypes);

    void INotificationHandler.Send(INotification message) =>
        throw new ArgumentException("Should never be called");

    void INotificationHandler.Subscribe(INotificationHandler receiver) =>
        _lastHandler.Subscribe(receiver);

    void INotificationHandler.Unsubscribe(INotificationHandler receiver) =>
        _lastHandler.Unsubscribe(receiver);

    /// <inheritdoc />
    public void PrintAllReceivers(List<INotificationHandler> alreadyPrinted, string indent = "")
    {
        Console.WriteLine($"{indent}{this.GetType().Name}({_sender})");
        if (INotificationHandler.RecursionDetected(this, alreadyPrinted, indent))
            return;

        Console.WriteLine($"{indent}Members:");
        foreach (var handler in this._notificationHandlers)
        {
            handler.PrintAllReceivers(alreadyPrinted, indent + "  ");
        }
    }
}
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

/// Composes two or more <see cref="INotificationHandler"/>s.
///
/// Every message this notification handler <see cref="Receive">receives</see>
/// is forwarded to the first <see cref="_notificationHandlers">part</see>.
/// Each part is connected to the next part.
/// The last part <see cref="INotificationSender.Send">sends</see> to
/// this notification handler's <i>following</i> notification pipes.
public class MultipartNotificationHandler : INotificationHandler
{
    private readonly INotificationHandler _firstHandler;
    private readonly INotificationHandler _lastHandler;
    private readonly IEnumerable<INotificationHandler> _notificationHandlers;
    private readonly object? _sender;

    public MultipartNotificationHandler(List<INotificationHandler> notificationHandlers, object? sender)
    {
        _notificationHandlers = notificationHandlers;
        _sender = sender;
        if (notificationHandlers.Count < 2)
            throw new ArgumentException(
                $"{nameof(MultipartNotificationHandler)} must get at least 2 notification handlers");

        _firstHandler = notificationHandlers[0];
        _lastHandler = notificationHandlers[^1];

        var enumerator = notificationHandlers.GetEnumerator();
        enumerator.MoveNext();
        var previous = enumerator.Current;
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            previous.ConnectTo(current);

            previous = current;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        foreach (var handler in _notificationHandlers.Reverse())
        {
            handler.Dispose();
        }
    }

    /// <inheritdoc />
    public void Receive(INotificationSender correspondingSender, INotification notification) =>
        _firstHandler.Receive(correspondingSender, notification);

    /// <inheritdoc />
    public bool Handles(params Type[] notificationTypes) =>
        _firstHandler.Handles(notificationTypes);

    void INotificationSender.Send(INotification notification) =>
        throw new ArgumentException("Should never be called");

    void INotificationSender.Subscribe(INotificationReceiver receiver) =>
        _lastHandler.Subscribe(receiver);

    void INotificationSender.Unsubscribe(INotificationReceiver receiver) =>
        _lastHandler.Unsubscribe(receiver);

    /// <inheritdoc />
    public void ConnectTo(INotificationReceiver to) =>
        ((INotificationSender)this).Subscribe(to);
}
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

namespace LionWeb.Core.M1.Event.Processor;

using Notification;

/// TODO: Do we really need this class?
/// Replaces all <see cref="RegisterReplacementEventId">registered event ids</see>.
public class EventIdReplacingEventProcessor<TEvent>(object? sender)
    : FilteringEventProcessor<TEvent>(sender) where TEvent : class, INotification
{
    private readonly Dictionary<INotificationId, INotificationId> _originalEventIds = [];

    /// Replaces id <paramref name="eventId"/> of future events with <paramref name="replacement"/>.
    public void RegisterReplacementEventId(INotificationId eventId, INotificationId replacement) =>
        _originalEventIds[eventId] = replacement;

    /// Stops replacing id <paramref name="eventId"/> of future events.
    public void UnregisterReplacementEventId(INotificationId eventId) =>
        _originalEventIds.Remove(eventId);

    /// <inheritdoc />
    protected override TEvent Filter(TEvent @event)
    {
        TEvent result = @event;
        if (_originalEventIds.TryGetValue(@event.NotificationId, out var replacement))
        {
            result.NotificationId = replacement;
        }

        return result;
    }
}
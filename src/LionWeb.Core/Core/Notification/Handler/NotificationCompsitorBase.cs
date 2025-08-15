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

public abstract class NotificationCompsitorBase<TNotification> : NotificationHandlerBase<TNotification>
    where TNotification : INotification
{
    private readonly INotificationIdProvider _idProvider;

    protected readonly Stack<CompositeNotification> _composites = [];

    protected NotificationCompsitorBase(object? notificationHandlerId) : base(notificationHandlerId)
    {
        _idProvider = new NotificationIdProvider(notificationHandlerId);
    }

    public CompositeNotification Push() =>
        Push(new CompositeNotification(_idProvider.CreateNotificationId()));

    public T Push<T>(T composite) where T : CompositeNotification
    {
        _composites.Push(composite);
        return composite;
    }

    public CompositeNotification Pop(bool send)
    {
        var result = _composites.Pop();
        if (send)
            Send((TNotification)(object)result);
        return result;
    }
}
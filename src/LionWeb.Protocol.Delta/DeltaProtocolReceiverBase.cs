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

namespace LionWeb.Protocol.Delta;

using Core.Notification;
using Core.Notification.Pipe;
using Message;

public abstract class DeltaProtocolReceiverBase<TContent>(object? sender) : NotificationPipeBase(sender), INotificationProducer
    where TContent : IDeltaContent
{
    /// <inheritdoc />
    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    #region Remote

    public void Receive(TContent deltaContent)
    {
        var notification = Map(deltaContent);
        ProduceNotification(notification);
    }

    public abstract INotification Map(TContent content);

    #endregion

    /// <inheritdoc />
    public void ProduceNotification(INotification notification) =>
        Send(notification);
}
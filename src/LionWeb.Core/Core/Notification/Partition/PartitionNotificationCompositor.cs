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

namespace LionWeb.Core.Notification.Partition;

using Forest;
using Handler;

public class PartitionNotificationCompositor : NotificationCompsitorBase<IPartitionNotification>
{
    private readonly ForestNotificationCompositor? _forestCompositor;

    public PartitionNotificationCompositor(object? notificationHandlerId, ForestNotificationCompositor? forestCompositor) : base(notificationHandlerId)
    {
        _forestCompositor = forestCompositor;
    }

    public override void Receive(IPartitionNotification message)
    {
        if (_forestCompositor != null && _forestCompositor.TryAdd(message))
            return;
                
        if (_composites.TryPeek(out var composite))
            composite.AddPart(message);
        else
            Send(message);
    }
}
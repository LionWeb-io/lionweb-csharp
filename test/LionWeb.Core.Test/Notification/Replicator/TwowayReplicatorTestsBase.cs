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

namespace LionWeb.Core.Test.Notification.Replicator;

using LionWeb.Core.Notification;
using LionWeb.Core.Notification.Partition;
using LionWeb.Core.Notification.Pipe;

public abstract class TwowayReplicatorTestsBase : ReplicatorTestsBase
{
    protected Tuple<INotificationHandler, INotificationHandler>
        CreateReplicators(IPartitionInstance node, IPartitionInstance clone)
    {
        var replicatorMap = new SharedNodeMap();
        var cloneMap = new SharedNodeMap();
        var replicator = CreateReplicator(node, replicatorMap, "nodeReplicator");
        var cloneReplicator = CreateReplicator(clone, cloneMap, "cloneReplicator");

        var cloneMapper = new NotificationMapper(cloneMap);
        replicator.ConnectTo(cloneMapper);
        cloneMapper.ConnectTo(cloneReplicator);

        var mapper = new NotificationMapper(replicatorMap);
        cloneReplicator.ConnectTo(mapper);
        mapper.ConnectTo(replicator);

        return Tuple.Create(replicator, cloneReplicator);
    }

    private static INotificationHandler CreateReplicator(IPartitionInstance clone, SharedNodeMap sharedNodeMap, object? sender) =>
        PartitionReplicator.Create(clone, sharedNodeMap, sender);
}
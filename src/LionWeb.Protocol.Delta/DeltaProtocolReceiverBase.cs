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
using Core.Notification.Forest;
using Core.Notification.Handler;
using Core.Notification.Partition;
using Message;

public abstract class DeltaProtocolReceiverBase<TContent, TPartition, TForest> : IDisposable
    where TContent : IDeltaContent
    where TPartition : TContent, IDeltaContent
    where TForest : TContent, IDeltaContent
{
    private readonly PartitionSharedNodeMap _sharedNodeMap;
    private readonly SharedPartitionReplicatorMap _sharedPartitionReplicatorMap;
    private readonly IConnectingNotificationHandler _forestNotificationReplicator;

    public DeltaProtocolReceiverBase(PartitionSharedNodeMap sharedNodeMap,
        SharedPartitionReplicatorMap sharedPartitionReplicatorMap,
        IConnectingNotificationHandler forestNotificationReplicator)
    {
        _sharedNodeMap = sharedNodeMap;
        _sharedPartitionReplicatorMap = sharedPartitionReplicatorMap;
        _forestNotificationReplicator = forestNotificationReplicator;
    }

    public void Init()
    {
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    #region Remote

    public void Receive(TContent deltaContent)
    {
        switch (deltaContent)
        {
            case TPartition partitionContent:
                var partitionNotification = MapPartition(partitionContent);
                if (_sharedNodeMap.TryGetPartition(partitionNotification.ContextNodeId, out var partition))
                {
                    var partitionReplicator = _sharedPartitionReplicatorMap.Lookup(partition.GetId());
                    partitionReplicator.Receive(null, partitionNotification);
                } else
                {
                    throw new InvalidOperationException();
                }

                break;

            case TForest forestContent:
                var forestNotification = MapForest(forestContent);
                _forestNotificationReplicator.Receive(null, forestNotification);
                break;

            default:
                throw new InvalidOperationException(deltaContent.ToString());
        }
    }

    protected abstract IPartitionNotification MapPartition(TPartition partitionContent);
    protected abstract IForestNotification MapForest(TForest forestContent);

    #endregion
}
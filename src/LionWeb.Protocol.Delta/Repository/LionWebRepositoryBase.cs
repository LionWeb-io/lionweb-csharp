// Copyright 2024 TRUMPF Laser GmbH
//
// Licensed under the Apache License, Version 2.0 (the "License");
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
// SPDX-FileCopyrightText: 2024 TRUMPF Laser GmbH
// SPDX-License-Identifier: Apache-2.0

namespace LionWeb.Protocol.Delta.Repository;

using Core;
using Core.M1;
using Core.M3;
using Core.Notification;
using Core.Notification.Forest;
using Core.Notification.Handler;
using Core.Notification.Partition;
using Forest;

public abstract class LionWebRepositoryBase<T> : IDisposable
{
    private readonly string _name;
    protected readonly IRepositoryConnector<T> _connector;
    protected readonly PartitionSharedNodeMap SharedNodeMap;

    protected readonly SharedPartitionReplicatorMap SharedPartitionReplicatorMap;

    protected readonly INotificationHandler<IForestNotification> _replicator;

    private long nextFreeNodeId = 0;

    public LionWebRepositoryBase(LionWebVersions lionWebVersion, List<Language> languages, string name,
        IForest forest, IRepositoryConnector<T> connector)
    {
        _name = name;
        _connector = connector;

        SharedNodeMap = new();
        SharedPartitionReplicatorMap = new SharedPartitionReplicatorMap();
        _replicator = RewriteForestNotificationReplicator.Create(forest, SharedPartitionReplicatorMap, SharedNodeMap, _name);

        INotificationHandler.Connect(_replicator, new LocalForestReceiver(name, this));

        _connector.ReceiveFromClient += OnReceiveFromClient;
    }

    /// <inheritdoc />
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        _connector.ReceiveFromClient -= OnReceiveFromClient;
        // _replicator.Dispose();
        SharedNodeMap.Dispose();
    }

    #region Local

    private class LocalForestReceiver(object? sender, LionWebRepositoryBase<T> repository) : NotificationHandlerBase<IForestNotification>(sender)
    {
        public override void Receive(IForestNotification message)
        {
            switch (message)
            {
                case PartitionAddedNotification partitionAddedEvent:
                    repository.OnLocalPartitionAdded(partitionAddedEvent);
                    break;
                case PartitionDeletedNotification partitionDeletedEvent:
                    repository.OnLocalPartitionDeleted(partitionDeletedEvent);
                    break;
            }
            
            repository.SendEventToAllClients(sender, message);
        }
    }
    
    private class LocalPartitionReceiver(object? sender, LionWebRepositoryBase<T> repository) : NotificationHandlerBase<IPartitionNotification>(sender)
    {
        public override void Receive(IPartitionNotification message) =>
            repository.SendEventToAllClients(sender, message);
    }

    private void OnLocalPartitionAdded(PartitionAddedNotification partitionAddedEvent)
    {
        var notificationHandler = SharedPartitionReplicatorMap.Lookup(partitionAddedEvent.NewPartition.GetId());
        INotificationHandler.Connect(notificationHandler, new LocalPartitionReceiver(_name, this));
    }

    private void OnLocalPartitionDeleted(PartitionDeletedNotification partitionDeletedEvent)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Remote

    protected abstract Task Receive(IMessageContext<T> content);

    private void OnReceiveFromClient(object? _, IMessageContext<T> content) =>
        Receive(content);

    #endregion

    #region Send

    protected abstract Task Send(IClientInfo clientInfo, T deltaContent);
    protected abstract Task SendAll(T deltaContent);

    private void SendEventToAllClients(object? sender, INotification? internalEvent)
    {
        if (internalEvent == null)
            return;

        var converted = _connector.Convert(internalEvent);
        SendAll(converted);
    }

    protected IEnumerable<FreeId> GetFreeNodeIds(int count)
    {
        int returnedCount = 0;
        while (returnedCount < count)
        {
            NodeId nextId = "repoProvidedId-" + ++nextFreeNodeId;
            if (!SharedNodeMap.ContainsKey(nextId))
            {
                returnedCount++;
                yield return nextId;
            }
        }
    }

    #endregion

    protected virtual void Log(string message, bool header = false)
    {
        var prependedMessage = $"{_name}: {message}";
        Console.WriteLine(header
            ? $"{ILionWebRepository.HeaderColor_Start}{prependedMessage}{ILionWebRepository.HeaderColor_End}"
            : prependedMessage);
    }
}
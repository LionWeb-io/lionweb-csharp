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
using Core.Notification.Handler;

public abstract class LionWebRepositoryBase<T> : IDisposable
{
    private readonly string _name;
    protected readonly IRepositoryConnector<T> _connector;
    protected readonly PartitionSharedNodeMap SharedNodeMap;

    protected readonly IConnectingNotificationHandler _replicator;

    private long _nextFreeNodeId = 0;

    public LionWebRepositoryBase(
        LionWebVersions lionWebVersion,
        List<Language> languages,
        string name,
        IForest forest,
        IRepositoryConnector<T> connector
    )
    {
        _name = name;
        _connector = connector;

        SharedNodeMap = new();
        _replicator = RewriteForestReplicator.Create(forest, SharedNodeMap, _name);

        INotificationHandler.Connect(_replicator, new LocalForestNotificationHandler(name, this));

        _connector.ReceiveFromClient += OnReceiveFromClient;
    }

    /// <inheritdoc />
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        _connector.ReceiveFromClient -= OnReceiveFromClient;
        _replicator.Dispose();
        SharedNodeMap.Dispose();
    }

    public event EventHandler<Exception>? CommunicationError;

    protected void OnCommunicationError(Exception ex) =>
        CommunicationError?.Invoke(this, ex);

    #region Local

    private class LocalForestNotificationHandler(object? sender, LionWebRepositoryBase<T> repository)
        : NotificationHandlerBase(sender), IConnectingNotificationHandler
    {
        public void Receive(ISendingNotificationHandler correspondingHandler, INotification notification) =>
            repository.SendNotificationToAllClients(sender, notification);
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

    private void SendNotificationToAllClients(object? sender, INotification? notification)
    {
        if (notification == null)
            return;

        var converted = _connector.Convert(notification);
        SendAll(converted);
    }

    protected IEnumerable<FreeId> GetFreeNodeIds(int count)
    {
        int returnedCount = 0;
        while (returnedCount < count)
        {
            NodeId nextId = "repoProvidedId-" + ++_nextFreeNodeId;
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
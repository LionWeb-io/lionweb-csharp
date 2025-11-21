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

namespace LionWeb.Core;

using M3;
using Notification.Partition;
using Notification.Pipe;

/// Instance of an <see cref="Concept.Partition"/>.
/// <inheritdoc />
public interface IPartitionInstance : IConceptInstance
{
    /// Optional hook to listen to partition notifications.
    /// Not supported by every implementation. 
    INotificationSender? GetNotificationSender() => GetNotificationProducer();

    /// Optional hook to raise partition notifications.
    /// Not supported by every implementation.
    protected internal IPartitionNotificationProducer? GetNotificationProducer();
}

/// <inheritdoc cref="IPartitionInstance" />
public interface IPartitionInstance<out T> : IConceptInstance<T>, IPartitionInstance where T : IReadableNode
{
}

/// Base implementation of <see cref="IPartitionInstance{T}"/>.
public abstract class PartitionInstanceBase : ConceptInstanceBase, IPartitionInstance<INode>
{
    /// <inheritdoc />
    protected PartitionInstanceBase(NodeId id) : base(id) { }

    /// <inheritdoc />
    public INotificationSender? GetNotificationSender() => GetNotificationProducer();

    /// <inheritdoc />
    IPartitionNotificationProducer? IPartitionInstance.GetNotificationProducer() => GetNotificationProducer();

    /// <inheritdoc cref="IPartitionInstance.GetNotificationProducer"/>
    protected internal abstract IPartitionNotificationProducer? GetNotificationProducer();
}
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

using System.Runtime.CompilerServices;

public interface IClientInfo
{
    ParticipationId? ParticipationId { get; set; }
    ClientId? ClientId { get; set; }
    
    bool SignedOn { get; set; }

    HashSet<NodeId> SubscribedPartitions { get; }

    PartitionChanges PartitionChanges { get; set; }

    bool NotifyAboutPartitionCreation { get; set; }
    
    bool NotifyAboutPartitionDeletion { get; set; }

    EventSequenceNumber IncrementAndGetSequenceNumber();
    EventSequenceNumber SequenceNumber { get; }

    private sealed class IdentityEqualityComparer : IEqualityComparer<IClientInfo>
    {
        public bool Equals(IClientInfo? x, IClientInfo? y) =>
            ReferenceEquals(x, y);


        public int GetHashCode(IClientInfo obj) =>
            RuntimeHelpers.GetHashCode(obj);
    }

    public static IEqualityComparer<IClientInfo> IdentityComparer { get; } =
        new IdentityEqualityComparer();
}

public enum PartitionChanges
{
    None,
    SubscribeToChangingPartitions,
    InformAboutChangingPartitions
}

public record ClientInfo : IClientInfo
{
    private EventSequenceNumber _nextSequenceNumber = 0;

    /// <inheritdoc />
    public ClientId? ClientId { get; set; }

    /// <inheritdoc />
    public ParticipationId? ParticipationId { get; set; }

    /// <inheritdoc />
    public bool SignedOn { get; set; }

    /// <inheritdoc />
    public EventSequenceNumber IncrementAndGetSequenceNumber() => 
        Interlocked.Increment(ref _nextSequenceNumber) - 1;

    /// <inheritdoc />
    public EventSequenceNumber SequenceNumber => 
        Interlocked.Read(ref _nextSequenceNumber);

    /// <inheritdoc />
    public HashSet<NodeId> SubscribedPartitions { get; } = [];

    /// <inheritdoc />
    public PartitionChanges PartitionChanges { get; set; } = PartitionChanges.None;

    /// <inheritdoc />
    public bool NotifyAboutPartitionCreation { get; set; } = false;

    /// <inheritdoc />
    public bool NotifyAboutPartitionDeletion { get; set; } = false;
}
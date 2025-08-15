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

namespace LionWeb.Core.Notification;

using Forest;
using Partition;
using System.Text;

/// Any notification in the LionWeb notification system.
public interface INotification
{
    /// Globally unique id of this notification.
    INotificationId NotificationId { get; set; }
}

/// ID of a notification in the LionWeb notification system.
public interface INotificationId;

public record CompositeNotification : IForestNotification, IPartitionNotification
{
    private readonly List<INotification> _parts;

    public CompositeNotification(IEnumerable<INotification> Parts,
        INotificationId NotificationId)
    {
        _parts = Parts.ToList();
        this.NotificationId = NotificationId;
    }

    public CompositeNotification(INotificationId NotificationId) : this([], NotificationId)
    {
    }

    public IReadOnlyList<INotification> Parts => _parts.AsReadOnly();

    public void AddPart(INotification part) =>
        _parts.Add(part);

    /// <inheritdoc />
    public INotificationId NotificationId { get; set; }

    /// <inheritdoc />
    public NodeId ContextNodeId => throw new NotImplementedException();

    /// <inheritdoc />
    public virtual bool Equals(CompositeNotification? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return NotificationId.Equals(other.NotificationId) &&
               Parts.SequenceEqual(other.Parts);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(NotificationId.GetHashCode());
        foreach (var part in Parts)
        {
            hashCode.Add(part);
        }

        return hashCode.ToHashCode();
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append(nameof(NotificationId));
        builder.Append(" = ");
        builder.Append(NotificationId);
        builder.Append(", ");
        builder.Append(nameof(Parts));
        builder.Append(" = ");

        builder.Append('[');
        bool first = true;
        foreach (var part in Parts)
        {
            if (first)
            {
                first = false;
            } else
            {
                builder.Append(", ");
            }

            builder.Append(part);
        }
        builder.Append(']');

        return true;
    }

    public void Deconstruct(out IReadOnlyList<INotification> Parts, out INotificationId NotificationId)
    {
        Parts = this.Parts;
        NotificationId = this.NotificationId;
    }
}
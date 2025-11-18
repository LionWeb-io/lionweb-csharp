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

using M1;
using Notification.Partition;

public record ReferenceTarget(ResolveInfo? ResolveInfo, NodeId? TargetId, IReadableNode? Target) : IReferenceTarget
{
    public IReadableNode? Target { get; set; } = Target;

    public static ReferenceTarget FromNode(IReadableNode? node) =>
        node is not null
            ? new ReferenceTarget(node?.GetNodeName(), node?.GetId(), node)
            : throw new InvalidValueException(null, node);

    public static ReferenceTarget? FromNodeOptional(IReadableNode? node) =>
        node is null ? null : new ReferenceTarget(node.GetNodeName(), node.GetId(), node);
}

public class ReferenceTargetIdComparer : IEqualityComparer<ReferenceTarget>
{
    /// <inheritdoc />
    public bool Equals(ReferenceTarget x, ReferenceTarget y)
    {
        if (x.Target is not null && y.Target is not null)
            return x.Target.GetId() == y.Target.GetId();

        if (x.TargetId is not null && y.TargetId is not null)
            return x.TargetId == y.TargetId;

        return x.ResolveInfo == y.ResolveInfo;
    }

    /// <inheritdoc />
    public int GetHashCode(ReferenceTarget obj)
    {
        if (obj.Target is not null)
            return obj.Target.GetId().GetHashCode();

        if (obj.TargetId is not null)
            return obj.TargetId.GetHashCode();

        if (obj.ResolveInfo is not null)
            return obj.ResolveInfo.GetHashCode();

        return 0;
    }
}
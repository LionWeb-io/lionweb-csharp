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
using TargetNode = IReadableNode;

/// <summary>
/// Describes a reference target.
///
/// <para>
/// At least one of <see cref="ResolveInfo"/> and <see cref="Target"/> MUST be non-null.
/// </para>
/// </summary>
/// <seealso cref="LionWeb.Core.Serialization.SerializedReferenceTarget"/>
public interface IReferenceTarget
{
    /// Textual hint that might be used to find the target node of this reference.
    ResolveInfo? ResolveInfo { get; }

    /// Target node id of this reference.
    NodeId? TargetId { get; }
    
    /// Target node of this reference.
    TargetNode? Target { get; set; }
}

/// <inheritdoc />
public record ReferenceTarget(ResolveInfo? ResolveInfo, NodeId? TargetId, IReadableNode? Target) : IReferenceTarget
{
    /// <inheritdoc />
    public IReadableNode? Target { get; set; } = Target;

    /// <summary>
    /// Creates a <see cref="ReferenceTarget"/> from <paramref name="node"/>.
    /// </summary>
    /// <exception cref="InvalidValueException">If <paramref name="node"/> is <c>null</c></exception>
    public static ReferenceTarget FromNode(IReadableNode node) =>
        node is not null
            ? new ReferenceTarget(node.GetNodeName(), node.GetId(), node)
            : throw new InvalidValueException(null, node);

    /// <summary>
    /// Creates a <see cref="ReferenceTarget"/> from <paramref name="node"/> if <paramref name="node"/> is not <c>null</c>.
    /// </summary>
    /// <returns><c>null</c> if <paramref name="node"/> is <c>null</c>, <see cref="ReferenceTarget"/> targeting <paramref name="node"/> otherwise.</returns>
    public static ReferenceTarget? FromNodeOptional(IReadableNode? node) =>
        node is null ? null : new ReferenceTarget(node.GetNodeName(), node.GetId(), node);
}

/// Compares two <see cref="IReferenceTarget"/>s.
public class ReferenceTargetIdComparer : IEqualityComparer<IReferenceTarget>
{
    /// <inheritdoc />
    public bool Equals(IReferenceTarget x, IReferenceTarget y)
    {
        if (x.Target is not null && y.Target is not null)
            return x.Target.GetId() == y.Target.GetId();

        if (x.TargetId is not null && y.TargetId is not null)
            return x.TargetId == y.TargetId;

        return x.ResolveInfo == y.ResolveInfo;
    }

    /// <inheritdoc />
    public int GetHashCode(IReferenceTarget obj)
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
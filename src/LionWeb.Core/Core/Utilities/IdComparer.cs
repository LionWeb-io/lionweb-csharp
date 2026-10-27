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

namespace LionWeb.Core.Utilities;

using M3;

/// <summary>
/// Variant of <see cref="Comparer"/> that flags different node ids with <see cref="NodeIdDifference"/>.
/// </summary>
public class IdComparer : Comparer
{
    /// <inheritdoc />
    public IdComparer(IList<INode?> left, IList<INode?> right) : base(left, right)
    {
    }

    /// <inheritdoc />
    public IdComparer(IList<IReadableNode?> left, IList<IReadableNode?> right) : base(left, right)
    {
    }

    /// <inheritdoc />
    protected override List<IDifference> CompareNode(IReadableNode? leftOwner, IReadableNode? left, Link? containment, IReadableNode? rightOwner,
        IReadableNode? right)
    {
        var differences = base.CompareNode(leftOwner, left, containment, rightOwner, right);
        if (left is not null && right is not null && left.GetId() != right.GetId())
        {
            differences.Add(new NodeIdDifference(left, right));
        }

        return differences;
    }
}

/// <paramref name="Left"/> and <paramref name="Right"/> have a different <see cref="IReadableNode.GetId"/>.
public record NodeIdDifference(IReadableNode Left, IReadableNode Right) : DifferenceBase
{
    /// <inheritdoc />
    protected override string Describe() =>
        $"Node: {LeftDescription()}: {Left.GetId()} vs. {RightDescription()}: {Right.GetId()}";
}
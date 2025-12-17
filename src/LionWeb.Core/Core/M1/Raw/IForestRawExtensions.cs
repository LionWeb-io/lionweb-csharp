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

namespace LionWeb.Core.M1.Raw;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Provides low-level access to the contents of a <see cref="IForest"/>.
///
/// <para/>
/// Does <i>neither</i> trigger any notifications <i>nor</i> validates constraints,
/// but <i>does</i> maintain tree shape, manages subscriptions,
/// and <i>may</i> check type compatibility
/// (e.g. maybe we cannot add a node of type <c>A</c> to a containment of type <c>B</c>).
/// 
/// <para/> 
/// Should be time and memory efficient.
/// </summary>
public static class 
    IForestRawExtensions
{
    /// <inheritdoc cref="IForest.TryGetPartitionRaw"/>
    public static bool TryGetPartitionRaw(this IForest self, NodeId nodeId,
        [NotNullWhen(true)] out IPartitionInstance? partition) =>
        self.TryGetPartitionRaw(nodeId, out partition);

    /// <inheritdoc cref="IForest.AddPartitionRaw"/>
    public static bool AddPartitionRaw(this IForest self, IPartitionInstance partition) =>
        self.AddPartitionRaw(partition);

    /// <inheritdoc cref="IForest.RemovePartitionRaw"/>
    public static bool RemovePartitionRaw(this IForest self, IPartitionInstance partition) =>
        self.RemovePartitionRaw(partition);
}
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

using M3;

/// <summary>
/// Provides low-level read access to the contents of a <see cref="IReadableNode"/>.
///
/// <para/> 
/// Does <i>neither</i> trigger any notifications <i>nor</i> validates constraints.
/// 
/// <para/> 
/// Should be time and memory efficient.
/// </summary>
public static class IReadableNodeRawExtensions
{
    /// <inheritdoc cref="IReadableNode.GetAnnotationsRaw"/>
    public static IReadOnlyList<IReadableNode> GetAnnotationsRaw(this IReadableNode self) =>
        self.GetAnnotationsRaw();

    /// <inheritdoc cref="IReadableNode.TryGetRaw"/>
    public static bool TryGetRaw(this IReadableNode self, Feature feature, out object? value) =>
        self.TryGetRaw(feature, out value);

    /// <inheritdoc cref="IReadableNode.TryGetPropertyRaw"/>
    public static bool TryGetPropertyRaw(this IReadableNode self, Property property, out object? value) =>
        self.TryGetPropertyRaw(property, out value);

    /// <inheritdoc cref="IReadableNode.TryGetContainmentRaw"/>
    public static bool TryGetContainmentRaw(this IReadableNode self, Containment containment, out IReadableNode? node) =>
        self.TryGetContainmentRaw(containment, out node);

    /// <inheritdoc cref="IReadableNode.TryGetContainmentsRaw"/>
    public static bool TryGetContainmentsRaw(this IReadableNode self, Containment containment,
        out IReadOnlyList<IReadableNode> nodes) =>
        self.TryGetContainmentsRaw(containment, out nodes);

    /// <inheritdoc cref="IReadableNode.TryGetReferenceRaw"/>
    public static bool TryGetReferenceRaw(this IReadableNode self, Reference reference, out IReferenceTarget? target) =>
        self.TryGetReferenceRaw(reference, out target);

    /// <inheritdoc cref="IReadableNode.TryGetReferencesRaw"/>
    public static bool TryGetReferencesRaw(this IReadableNode self, Reference reference,
        out IReadOnlyList<IReferenceTarget> targets) => self.TryGetReferencesRaw(reference, out targets);
}
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

public static class IReadableNodeRawExtensions
{
    /// <inheritdoc cref="IReadableNodeRaw.GetAnnotationsRaw"/>
    public static IReadOnlyList<IAnnotationInstance> GetAnnotationsRaw(this IReadableNodeRaw self) =>
        self.GetAnnotationsRaw();

    /// <inheritdoc cref="IReadableNodeRaw.TryGetPropertyRaw"/>
    public static bool TryGetPropertyRaw(this IReadableNodeRaw self, Feature property, out object? value) =>
        self.TryGetPropertyRaw(property, out value);

    /// <inheritdoc cref="IReadableNodeRaw.TryGetContainmentRaw"/>
    public static bool TryGetContainmentRaw(this IReadableNodeRaw self, Feature containment, out IWritableNode? node) =>
        self.TryGetContainmentRaw(containment, out node);

    /// <inheritdoc cref="IReadableNodeRaw.TryGetContainmentsRaw"/>
    public static bool TryGetContainmentsRaw(this IReadableNodeRaw self, Feature containment,
        out IReadOnlyList<IWritableNode> nodes) => self.TryGetContainmentsRaw(containment, out nodes);

    /// <inheritdoc cref="IReadableNodeRaw.TryGetReferenceRaw"/>
    public static bool TryGetReferenceRaw(this IReadableNodeRaw self, Feature reference, out IReferenceTarget? target) =>
        self.TryGetReferenceRaw(reference, out target);

    /// <inheritdoc cref="IReadableNodeRaw.TryGetReferencesRaw"/>
    public static bool TryGetReferencesRaw(this IReadableNodeRaw self, Feature reference,
        out IReadOnlyList<IReferenceTarget> targets) => self.TryGetReferencesRaw(reference, out targets);
}
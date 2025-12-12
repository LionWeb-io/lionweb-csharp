// Copyright 2025 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (this IWritableNodeRaw self, the "License")
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
/// Provides low-level write access to the contents of a <see cref="IWritableNode"/>.
///
/// <para/>
/// Does <i>neither</i> trigger any notifications <i>nor</i> validates constraints,
/// but <i>does</i> maintain tree shape and <i>may</i> check type compatibility
/// (e.g. maybe we cannot add a node of type <c>A</c> to a containment of type <c>B</c>).
/// 
/// <para/>
/// Should be time and memory efficient.
/// </summary>
public static class IWritableNodeRawExtensions
{
    #region Annotation

    /// <inheritdoc cref="IWritableNode.AddReferencesRaw"/>
    public static bool AddAnnotationsRaw(this IWritableNode self, IWritableNode annotation) =>
        self.AddAnnotationsRaw(annotation);

    /// <inheritdoc cref="IWritableNode.InsertAnnotationsRaw"/>
    public static bool InsertAnnotationsRaw(this IWritableNode self, Index index, IWritableNode annotation) =>
        self.InsertAnnotationsRaw(index, annotation);

    /// <inheritdoc cref="IWritableNode.RemoveAnnotationsRaw"/>
    public static bool RemoveAnnotationsRaw(this IWritableNode self, IWritableNode annotation) =>
        self.RemoveAnnotationsRaw(annotation);

    #endregion

    /// <inheritdoc cref="IWritableNode.SetRaw"/>
    public static bool SetRaw(this IWritableNode self, Feature feature, object? value) =>
        self.SetRaw(feature, value);

    #region Property

    /// <inheritdoc cref="IWritableNode.SetPropertyRaw"/>
    public static bool SetPropertyRaw(this IWritableNode self, Property property, object? value) =>
        self.SetPropertyRaw(property, value);

    #endregion

    #region Containment

    /// <inheritdoc cref="IWritableNode.SetContainmentRaw"/>
    public static bool SetContainmentRaw(this IWritableNode self, Containment containment, IWritableNode? node) =>
        self.SetContainmentRaw(containment, node);

    /// <inheritdoc cref="IWritableNode.AddContainmentsRaw"/>
    public static bool AddContainmentsRaw(this IWritableNode self, Containment containment, IWritableNode node) =>
        self.AddContainmentsRaw(containment, node);

    /// <inheritdoc cref="IWritableNode.InsertContainmentsRaw"/>
    public static bool InsertContainmentsRaw(this IWritableNode self, Containment containment, Index index,
        IWritableNode node) =>
        self.InsertContainmentsRaw(containment, index, node);

    /// <inheritdoc cref="IWritableNode.RemoveContainmentsRaw"/>
    public static bool RemoveContainmentsRaw(this IWritableNode self, Containment containment,
        IWritableNode node) =>
        self.RemoveContainmentsRaw(containment, node);

    #endregion

    #region Reference

    /// <inheritdoc cref="IWritableNode.SetReferenceRaw"/>
    public static bool SetReferenceRaw(this IWritableNode self, Reference reference, ReferenceTarget? target) =>
        self.SetReferenceRaw(reference, target);

    /// <inheritdoc cref="IWritableNode.AddReferencesRaw"/>
    public static bool AddReferencesRaw(this IWritableNode self, Reference reference, ReferenceTarget target) =>
        self.AddReferencesRaw(reference, target);

    /// <inheritdoc cref="IWritableNode.InsertReferencesRaw"/>
    public static bool InsertReferencesRaw(this IWritableNode self, Reference reference, Index index,
        ReferenceTarget target) =>
        self.InsertReferencesRaw(reference, index, target);

    /// <inheritdoc cref="IWritableNode.RemoveReferencesRaw"/>
    public static bool RemoveReferencesRaw(this IWritableNode self, Reference reference, ReferenceTarget target) =>
        self.RemoveReferencesRaw(reference, target);

    #endregion
}
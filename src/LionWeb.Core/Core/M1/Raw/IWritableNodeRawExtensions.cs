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

/// <inheritdoc cref="IWritableNodeRaw"/>
public static class IWritableNodeRawExtensions
{
    #region Annotation

    /// <inheritdoc cref="IWritableNodeRaw.AddReferencesRaw"/>
    public static bool AddAnnotationsRaw(this IWritableNodeRaw self, IWritableNode annotation) =>
        self.AddAnnotationsRaw(annotation);

    /// <inheritdoc cref="IWritableNodeRaw.InsertAnnotationsRaw"/>
    public static bool InsertAnnotationsRaw(this IWritableNodeRaw self, Index index, IWritableNode annotation) =>
        self.InsertAnnotationsRaw(index, annotation);

    /// <inheritdoc cref="IWritableNodeRaw.RemoveAnnotationsRaw"/>
    public static bool RemoveAnnotationsRaw(this IWritableNodeRaw self, IWritableNode annotation) =>
        self.RemoveAnnotationsRaw(annotation);

    #endregion

    /// <inheritdoc cref="IWritableNodeRaw.SetRaw"/>
    public static bool SetRaw(this IWritableNodeRaw self, Feature feature, object? value) =>
        self.SetRaw(feature, value);

    #region Property

    /// <inheritdoc cref="IWritableNodeRaw.SetPropertyRaw"/>
    public static bool SetPropertyRaw(this IWritableNodeRaw self, Property property, object? value) =>
        self.SetPropertyRaw(property, value);

    #endregion

    #region Containment

    /// <inheritdoc cref="IWritableNodeRaw.SetContainmentRaw"/>
    public static bool SetContainmentRaw(this IWritableNodeRaw self, Containment containment, IWritableNode? node) =>
        self.SetContainmentRaw(containment, node);

    /// <inheritdoc cref="IWritableNodeRaw.AddContainmentsRaw"/>
    public static bool AddContainmentsRaw(this IWritableNodeRaw self, Containment containment, IWritableNode node) =>
        self.AddContainmentsRaw(containment, node);

    /// <inheritdoc cref="IWritableNodeRaw.InsertContainmentsRaw"/>
    public static bool InsertContainmentsRaw(this IWritableNodeRaw self, Containment containment, Index index,
        IWritableNode node) =>
        self.InsertContainmentsRaw(containment, index, node);

    /// <inheritdoc cref="IWritableNodeRaw.RemoveContainmentsRaw"/>
    public static bool RemoveContainmentsRaw(this IWritableNodeRaw self, Containment containment,
        IWritableNode node) =>
        self.RemoveContainmentsRaw(containment, node);

    #endregion

    #region Reference

    /// <inheritdoc cref="IWritableNodeRaw.SetReferenceRaw"/>
    public static bool SetReferenceRaw(this IWritableNodeRaw self, Reference reference, ReferenceTarget? target) =>
        self.SetReferenceRaw(reference, target);

    /// <inheritdoc cref="IWritableNodeRaw.AddReferencesRaw"/>
    public static bool AddReferencesRaw(this IWritableNodeRaw self, Reference reference, ReferenceTarget target) =>
        self.AddReferencesRaw(reference, target);

    /// <inheritdoc cref="IWritableNodeRaw.InsertReferencesRaw"/>
    public static bool InsertReferencesRaw(this IWritableNodeRaw self, Reference reference, Index index,
        ReferenceTarget target) =>
        self.InsertReferencesRaw(reference, index, target);

    /// <inheritdoc cref="IWritableNodeRaw.RemoveReferencesRaw"/>
    public static bool RemoveReferencesRaw(this IWritableNodeRaw self, Reference reference, ReferenceTarget target) =>
        self.RemoveReferencesRaw(reference, target);

    #endregion
}
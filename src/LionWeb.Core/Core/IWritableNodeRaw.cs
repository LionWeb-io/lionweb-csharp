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

using M3;

public interface IWritableNodeRaw : IReadableNodeRaw, IWritableNode
{
    #region Annotation

    protected internal bool AddAnnotationsRaw(IAnnotationInstance annotation);
    
    protected internal bool InsertAnnotationsRaw(Index index, IAnnotationInstance annotation);

    protected internal bool RemoveAnnotationsRaw(IAnnotationInstance annotation);

    #endregion

    protected internal bool SetRaw(Feature feature, object? value);
    
    #region Property

    protected internal bool SetPropertyRaw(Property property, object? value);

    #endregion


    #region Containment

    protected internal bool SetContainmentRaw(Containment containment, IWritableNode? node);

    protected internal bool AddContainmentsRaw(Containment containment, IWritableNode node);

    protected internal bool InsertContainmentsRaw(Containment containment, Index index, IWritableNode node);

    protected internal bool RemoveContainmentsRaw(Containment containment, IWritableNode node);

    #endregion

    #region Reference

    protected internal bool SetReferenceRaw(Reference reference, ReferenceTarget? target);

    protected internal bool AddReferencesRaw(Reference reference, ReferenceTarget target);

    protected internal bool InsertReferencesRaw(Reference reference, Index index, ReferenceTarget target);

    protected internal bool RemoveReferencesRaw(Reference reference, ReferenceTarget target);

    #endregion
}

public interface IWritableNodeRaw<T> : IWritableNodeRaw, IWritableNode<T> where T : class, IWritableNode;
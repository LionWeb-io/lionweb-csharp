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

    protected internal bool AddAnnotationsRaw(List<IAnnotationInstance> annotations);
    
    protected internal bool InsertAnnotationsRaw(Index index, List<IAnnotationInstance> annotations);

    protected internal bool RemoveAnnotationsRaw(HashSet<IAnnotationInstance> annotations);

    #endregion

    #region Property

    protected internal bool SetPropertyRaw(Feature property, object? value);

    #endregion


    #region Containment

    protected internal bool SetContainmentRaw(Feature containment, IWritableNode? node);

    protected internal bool AddContainmentsRaw(Feature containment, List<IWritableNode> nodes);

    protected internal bool InsertContainmentsRaw(Feature containment, Index index, List<IWritableNode> nodes);

    protected internal bool RemoveContainmentsRaw(Feature containment, List<IWritableNode> nodes);

    #endregion

    #region Reference

    protected internal bool SetReferenceRaw(Feature reference, ReferenceTarget? target);

    protected internal bool AddReferencesRaw(Feature reference, List<ReferenceTarget> targets);

    protected internal bool InsertReferencesRaw(Feature reference, Index index, List<ReferenceTarget> targets);

    protected internal bool RemoveReferencesRaw(Feature reference, List<ReferenceTarget> targets);

    #endregion
}

public interface IWritableNodeRaw<T> : IWritableNodeRaw, IWritableNode<T> where T : class, IWritableNode;
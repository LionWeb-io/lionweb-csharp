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

/// Instance of an <see cref="Concept"/>.
/// <inheritdoc />
public interface IConceptInstance : IReadableNode
{
    Classifier IReadableNode.GetClassifier() => GetClassifier();

    /// <inheritdoc cref="IReadableNode.GetClassifier()"/>
    public Concept GetConcept();
}

/// <inheritdoc cref="IConceptInstance" />
public interface IConceptInstance<out T> : IReadableNode<T>, IConceptInstance where T : IReadableNode
{
}

/// Base implementation of <see cref="IConceptInstance{T}"/>.
public abstract class ConceptInstanceBase : NodeBase, IConceptInstance<INode>
{
    /// <inheritdoc />
    protected ConceptInstanceBase(NodeId id) : base(id) { }

    /// <inheritdoc cref="IConceptInstance.GetClassifier()" />
    public override Classifier GetClassifier() => GetConcept();

    /// <inheritdoc />
    public abstract Concept GetConcept();
}
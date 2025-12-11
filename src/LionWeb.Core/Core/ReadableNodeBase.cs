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

using M2;
using M3;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Utilities;

/// Base implementation of <see cref="IReadableNode{T}"/>.
[DebuggerDisplay("{GetType().Name}[{GetId()}]")]
public abstract class ReadableNodeBase<T> : IReadableNode<T> where T : IReadableNode
{
    /// The <see cref="IBuiltInsLanguage"/> variant used for this node.
    protected virtual IBuiltInsLanguage _builtIns =>
        new Lazy<IBuiltInsLanguage>(() => GetClassifier().GetLanguage().LionWebVersion.BuiltIns).Value;

    /// The <see cref="ILionCoreLanguage"/> variant used for this node.
    protected virtual ILionCoreLanguage _m3 =>
        new Lazy<ILionCoreLanguage>(() => GetClassifier().GetLanguage().LionWebVersion.LionCore).Value;


    /// <summary>
    /// Initializes <c>this</c> node's <see cref="IReadableNode.GetId">id</see> and optionally <see cref="IReadableNode.GetParent">parent</see>.
    /// </summary>
    /// <param name="id"><c>this</c> node's <see cref="IReadableNode.GetId">id</see>.</param>
    /// <param name="parent"><c>this</c> node's <see cref="IReadableNode.GetParent">parent</see></param>
    /// <exception cref="InvalidIdException">If <paramref name="id"/> is not a <see cref="IReadableNode.GetId">valid identifier</see>.</exception>
    protected ReadableNodeBase(NodeId id, T? parent)
    {
        if (!IdUtils.IsValid(id))
            throw new InvalidIdException(id);

        _id = id;
        _parent = parent;
    }

    private readonly NodeId _id;

    /// <inheritdoc />
    public NodeId GetId() => _id;

    /// <inheritdoc cref="IReadableNode.GetParent()"/>
    /// <c>protected</c> so it can be changed by <see cref="NodeBase"/>.
    // ReSharper disable once InconsistentNaming
    protected T? _parent;

    /// <inheritdoc />
    public T? GetParent() => _parent;

    /// <inheritdoc cref="IReadableNode.GetAnnotations()"/>
    // ReSharper disable once InconsistentNaming
    protected readonly List<T> _annotations = [];

    /// <inheritdoc />
    public IReadOnlyList<T> GetAnnotations() => _annotations.AsReadOnly();

    /// <inheritdoc />
    public abstract Classifier GetClassifier();

    /// <inheritdoc />
    public abstract IEnumerable<Feature> CollectAllSetFeatures();

    /// <inheritdoc />
    public abstract object? Get(Feature feature);

    /// <inheritdoc />
    public abstract bool TryGet(Feature feature, [NotNullWhen(true)] out object? value);

    #region ReadableRaw

    /// <inheritdoc/>
    public IReadOnlyList<IReadableNode> GetAnnotationsRaw() =>
        _annotations.Cast<IReadableNode>().ToImmutableList();

    /// <inheritdoc/>
    bool IReadableNode.TryGetPropertyRaw(Property property, out object? value) =>
        TryGetPropertyRaw(property, out value);

    /// <inheritdoc cref="IReadableNode.TryGetPropertyRaw"/>
    protected internal virtual bool TryGetPropertyRaw(Property property, out object? value)
    {
        value = null;
        return false;
    }

    /// <inheritdoc/>
    bool IReadableNode.TryGetContainmentRaw(Containment containment, out IReadableNode? node) =>
        TryGetContainmentRaw(containment, out node);

    /// <inheritdoc cref="IReadableNode.TryGetContainmentRaw"/>
    protected internal virtual bool TryGetContainmentRaw(Containment containment, out IReadableNode? node)
    {
        node = null;
        return false;
    }

    /// <inheritdoc />
    bool IReadableNode.TryGetContainmentsRaw(Containment containment, out IReadOnlyList<IReadableNode> nodes) =>
        TryGetContainmentsRaw(containment, out nodes);

    /// <inheritdoc cref="IReadableNode.TryGetContainmentsRaw"/>
    protected internal virtual bool TryGetContainmentsRaw(Containment containment,
        out IReadOnlyList<IReadableNode> nodes)
    {
        nodes = [];
        return false;
    }

    /// <inheritdoc/>
    bool IReadableNode.TryGetReferenceRaw(Reference reference, out IReferenceTarget? target) =>
        TryGetReferenceRaw(reference, out target);

    /// <inheritdoc cref="IReadableNode.TryGetReferenceRaw"/>
    protected internal virtual bool TryGetReferenceRaw(Reference reference, out IReferenceTarget? target)
    {
        target = null;
        return false;
    }

    /// <inheritdoc />
    bool IReadableNode.TryGetReferencesRaw(Reference reference, out IReadOnlyList<IReferenceTarget> targets) =>
        TryGetReferencesRaw(reference, out targets);

    /// <inheritdoc cref="IReadableNode.TryGetReferencesRaw"/>
    protected internal virtual bool TryGetReferencesRaw(Reference reference,
        out IReadOnlyList<IReferenceTarget> targets)
    {
        targets = [];
        return false;
    }

    #endregion

    #region References

    /// <summary>
    /// Extracts all target nodes from <paramref name="storage"/> targets.
    /// Represents unresolvable targets as <c>null</c>. 
    /// </summary>
    protected IImmutableList<R?> ReferenceTargetNullableTargets<R>(List<ReferenceTarget> storage, Reference reference)
        where R : IReadableNode =>
        storage
            .Select(r => ReferenceTargetNullableTarget<R>(r, reference))
            .ToImmutableList();

    /// <summary>
    /// Extracts target node from <paramref name="storage"/> target.
    /// Represents unresolvable target as <c>null</c>. 
    /// </summary>
    protected R? ReferenceTargetNullableTarget<R>(ReferenceTarget? storage, Reference reference) where R : IReadableNode
    {
        if (storage?.Target is null)
            return default;
        
        if (storage.Target is not R result)
            throw new InvalidValueException(reference, storage.Target);

        return result;
    }

    /// <summary>
    /// Extracts all target nodes from <paramref name="storage"/> targets.
    /// </summary>
    protected IImmutableList<R> ReferenceTargetNonNullTargets<R>(List<ReferenceTarget> storage, Reference reference)
        where R : IReadableNode =>
        storage
            .Select(r =>
            {
                var result = ReferenceTargetNonNullTarget<R>(r, reference);
                if (result is null)
                    throw new InvalidValueException(reference, r);
                return result;
            })
            .ToImmutableList();

    /// <summary>
    /// Extracts target node from <paramref name="storage"/> target.
    /// </summary>
    /// <exception cref="UnresolvedReferenceException">If <paramref name="storage"/> is unresolved.</exception>
    /// <exception cref="InvalidValueException">If <paramref name="storage"/> does not resolve to <typeparamref name="R"/>.</exception>
    protected R? ReferenceTargetNonNullTarget<R>(ReferenceTarget? storage, Reference reference) where R : IReadableNode
    {
        if (storage is null)
            return default;
        
        if (storage.Target is null)
            throw new UnresolvedReferenceException(GetId(), reference, storage);
        
        if (storage.Target is not R result)
            throw new InvalidValueException(reference, storage.Target);

        return result;
    }
    
    protected R? GetRequiredReference<R>(ReferenceTarget? storage, Reference reference) where R : IReadableNode
    {
        if (storage is null)
            throw new UnsetFeatureException(reference);
        
        return ReferenceTargetNullableTarget<R>(storage, reference);
    }
    
    /// <inheritdoc cref="AsNonEmptyReadOnly{T}(List{T},Link)"/>
    protected IReadOnlyList<R?> GetRequiredNullableReferences<R>(List<ReferenceTarget> storage, Reference reference) where R : IReadableNode =>
        storage.Count != 0
            ? ReferenceTargetNullableTargets<R>(storage, reference)
            : throw new UnsetFeatureException(reference);

    /// <inheritdoc cref="AsNonEmptyReadOnly{T}(List{T},Link)"/>
    protected IReadOnlyList<R> GetRequiredNonNullReferences<R>(List<ReferenceTarget> storage, Reference reference) where R : IReadableNode =>
        storage.Count != 0
            ? ReferenceTargetNonNullTargets<R>(storage, reference)
            : throw new UnsetFeatureException(reference);

    /// <summary>
    /// Tries to retrieve all <see cref="ReferenceTarget.Target"/>s from <paramref name="storage"/>.
    /// </summary>
    /// <returns><c>true</c> if all <paramref name="storage"/>.<see cref="ReferenceTarget.Target"/>s are non-null and of type <typeparamref name="R"/>; <c>false</c> otherwise.</returns>
    protected bool TryGetReference<R>(List<ReferenceTarget> storage, out IReadOnlyList<R> targets)
    {
        var result = storage.Count != 0;
        var nodes = new List<R>(storage.Count);
        foreach (var r in storage)
        {
            if (r.Target is not R target)
            {
                result = false;
                break;
            }
                
            nodes.Add(target);
        }
            
        targets = result ? nodes.AsReadOnly() : [];
        return result;
    }

    #endregion
}
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

public abstract partial class NodeBase
{
    bool IWritableNode.SetRaw(Feature feature, object? value) =>
        SetRaw(feature, value);

    /// <inheritdoc cref="IWritableNode.SetRaw"/>
    protected internal virtual bool SetRaw(Feature feature, object? value)
    {
        switch (feature, value)
        {
            case (Property f, _):
                return SetPropertyRaw(f, value);

            case (Containment { Multiple: false } f, IWritableNode v):
                return SetContainmentRaw(f, v);
            
            case (Reference { Multiple: false } f, ReferenceTarget v):
                return SetReferenceRaw(f, v);
            
            case (Containment { Multiple: true } f, IEnumerable<IWritableNode> v):
                return TryGetContainmentsRaw(f, out var deletedChildren)
                       && ((IReadOnlyList<IWritableNode>)deletedChildren).ToList().All(d => RemoveContainmentsRaw(f, d))
                       && v.ToList().All(a => AddContainmentsRaw(f, a));
            
            case (Reference { Multiple: true } f, IEnumerable<IReferenceTarget> v):
                return TryGetReferencesRaw(f, out var deletedTargets)
                       && deletedTargets.ToList().All(d => RemoveReferencesRaw(f, (ReferenceTarget)d))
                       && v.ToList().All(a => AddReferencesRaw(f, (ReferenceTarget)a));
            
            default:
                return false;
        }
    }

    bool IWritableNode.SetPropertyRaw(Property property, object? value) =>
        SetPropertyRaw(property, value);

    /// <inheritdoc cref="IWritableNode.SetPropertyRaw"/>
    protected internal virtual bool SetPropertyRaw(Property property, object? value) =>
        false;

    bool IWritableNode.SetContainmentRaw(Containment containment, IWritableNode? node) =>
        SetContainmentRaw(containment, node);

    /// <inheritdoc cref="IWritableNode.SetContainmentRaw"/>
    protected internal virtual bool SetContainmentRaw(Containment containment, IWritableNode? node) =>
        false;

    bool IWritableNode.SetReferenceRaw(Reference reference, ReferenceTarget? targets) =>
        SetReferenceRaw(reference, targets);

    /// <inheritdoc cref="IWritableNode.SetReferenceRaw"/>
    protected internal virtual bool SetReferenceRaw(Reference reference, ReferenceTarget? target) =>
        false;

    bool IWritableNode.AddContainmentsRaw(Containment containment, IWritableNode node) =>
        AddContainmentsRaw(containment, node);

    /// <inheritdoc cref="IWritableNode.AddContainmentsRaw"/>
    protected internal virtual bool AddContainmentsRaw(Containment containment, IWritableNode node) =>
        false;

    bool IWritableNode.AddReferencesRaw(Reference reference, ReferenceTarget target) =>
        AddReferencesRaw(reference, target);

    /// <inheritdoc cref="IWritableNode.AddReferencesRaw"/>
    protected internal virtual bool AddReferencesRaw(Reference reference, ReferenceTarget target) =>
        false;

    bool IWritableNode.InsertContainmentsRaw(Containment containment, Index index, IWritableNode node) =>
        InsertContainmentsRaw(containment, index, node);

    /// <inheritdoc cref="IWritableNode.InsertContainmentsRaw"/>
    protected internal virtual bool InsertContainmentsRaw(Containment containment, Index index, IWritableNode node) =>
        false;

    bool IWritableNode.InsertReferencesRaw(Reference reference, Index index, ReferenceTarget target) =>
        InsertReferencesRaw(reference, index, target);

    /// <inheritdoc cref="IWritableNode.InsertReferencesRaw"/>
    protected internal virtual bool InsertReferencesRaw(Reference reference, Index index, ReferenceTarget target) =>
        false;

    bool IWritableNode.RemoveContainmentsRaw(Containment containment, IWritableNode node) =>
        RemoveContainmentsRaw(containment, node);

    /// <inheritdoc cref="IWritableNode.RemoveContainmentsRaw"/>
    protected internal virtual bool RemoveContainmentsRaw(Containment containment, IWritableNode node) =>
        false;

    bool IWritableNode.RemoveReferencesRaw(Reference reference, ReferenceTarget target) =>
        RemoveReferencesRaw(reference, target);

    /// <inheritdoc cref="IWritableNode.RemoveReferencesRaw"/>
    protected internal virtual bool RemoveReferencesRaw(Reference reference, ReferenceTarget target) =>
        false;
    
    
    #region child raw api

    protected bool ExchangeChildRaw(IReadableNode? newValue, IReadableNode? storage)
    {
        if (newValue == storage)
            return false;

        SetParentNull((INode?)storage);
        AttachChild(newValue);
        return true;
    }

    protected bool ExchangeChildrenRaw<T>(List<T> newValue, List<T> storage) where T : IWritableNode
    {
        if (storage.SequenceEqual(newValue))
            return false;

        RemoveSelfParentRaw(storage);
        storage.Clear();
        storage.AddRange(SetSelfParentRaw(newValue));
        return true;
    }

    private void RemoveSelfParentRaw<T>(List<T> storage) where T : IReadableNode
    {
        foreach (T node in storage)
        {
            SetParentInternal((INode)node, null);
        }
    }

    private List<T> SetSelfParentRaw<T>(List<T> list) where T : IReadableNode
    {
        foreach (T n in list)
        {
            DetachChildInternal((INode)n);
            SetParentInternal((INode)n, this);
        }

        return list;
    }

    protected bool AddChildRaw<T>(T? newValue, List<T> storage) where T : class, IWritableNode
    {
        if (newValue is null || storage.Count != 0 && storage[^1] == newValue)
            return false;

        AttachChild(newValue);
        storage.Add(newValue);
        return true;
    }

    protected bool InsertChildRaw<T>(Index index, T? newValue, List<T> storage) where T : class, IWritableNode
    {
        if (newValue is null || !IsInRange(index, storage) || storage.Count > index && storage[index] == newValue)
            return false;

        AttachChild(newValue);
        storage.Insert(index, newValue);
        return true;
    }

    protected bool RemoveChildRaw<T>(T? valueToRemove, List<T> current) where T : class, IWritableNode
    {
        if (valueToRemove is null)
            return false;

        if (current.Remove(valueToRemove))
        {
            SetParentNull((INode)valueToRemove);
            return true;
        }

        return false;
    }

    #endregion

    #region reference raw

    protected bool SetReferencesRaw(List<ReferenceTarget> targets, List<ReferenceTarget> storage)
    {
        if (storage.SequenceEqual(targets))
            return false;

        storage.Clear();
        storage.AddRange(targets);
        return true;
    }

    protected bool AddReferencesRaw(ReferenceTarget target, List<ReferenceTarget> storage)
    {
        if (target is null)
            return false;

        storage.Add(target);
        return true;
    }

    protected bool InsertReferencesRaw(Index index, ReferenceTarget target, List<ReferenceTarget> storage)
    {
        if (target is null || !IsInRange(index, storage))
            return false;

        storage.Insert(index, target);
        return true;
    }

    protected bool RemoveReferencesRaw(ReferenceTarget target, List<ReferenceTarget> storage)
    {
        if (target is null)
            return false;

        var index = storage.FindIndex(r =>
        {
            if (target.Target is not null)
                return Equals(target.Target, r.Target);

            if (target.TargetId is not null)
                return Equals(target.TargetId, r.TargetId);

            return Equals(target.ResolveInfo, r.ResolveInfo);
        });
        if (index < 0)
            return false;

        storage.RemoveAt(index);
        return true;
    }

    #endregion
}
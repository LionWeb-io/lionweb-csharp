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
using Notification.Partition.Emitter;

public abstract partial class NodeBase
{
    #region Property

    #region Optional

    protected void SetOptionalReferenceTypeProperty<T>(T? value, Property property, T? storage, Func<T?, bool> setter)
        where T : class
    {
        PropertyNotificationEmitter emitter = new(property, this, value, storage);
        emitter.CollectOldData();
        if (setter(value))
            emitter.Notify();
    }

    protected void SetOptionalValueTypeProperty<T>(T? value, Property property, T? storage, Func<T?, bool> setter)
        where T : struct
    {
        PropertyNotificationEmitter emitter = new(property, this, value, storage);
        emitter.CollectOldData();
        if (setter(value))
            emitter.Notify();
    }

    #endregion

    #region Required

    protected void SetRequiredReferenceTypeProperty<T>(T value, Property property, T? storage, Func<T, bool> setter)
        where T : class
    {
        AssureNotNull(value, property);
        PropertyNotificationEmitter emitter = new(property, this, value, storage);
        emitter.CollectOldData();
        if (setter(value))
            emitter.Notify();
    }

    protected void SetRequiredValueTypeProperty<T>(T value, Property property, T? storage, Func<T?, bool> setter)
        where T : struct
    {
        PropertyNotificationEmitter emitter = new(property, this, value, storage);
        emitter.CollectOldData();
        if (setter(value))
            emitter.Notify();
    }

    #endregion

    #endregion

    #region Containment

    #region Optional

    #region Single

    protected void SetOptionalSingleContainment<T>(T? value, Containment containment, T? storage,
        Func<T?, bool> setter) where T : INode
    {
        ContainmentSingleNotificationEmitter<T> emitter = new(containment, this, value, storage);
        emitter.CollectOldData();
        if (setter(value))
            emitter.Notify();
    }

    #endregion

    #region Multiple

    protected void SetOptionalMultipleContainment<T>(object? value, Containment containment, List<T> storage,
        Func<List<T>, bool> setter) where T : INode
    {
        var safeNodes = containment.AsNodes<T>(value).ToList();
        ContainmentSetNotificationEmitter<T> emitter = new(containment, this, safeNodes, storage);
        emitter.CollectOldData();
        if (setter(safeNodes))
            emitter.Notify();
    }

    protected void AddOptionalMultipleContainment<T>(IEnumerable<T> nodes, Containment containment, List<T> storage,
        Func<T, bool> adder) where T : INode
    {
        var safeNodes = nodes?.ToList();
        AssureNotNull(safeNodes, containment);
        AssureNotNullMembers(safeNodes, containment);
        if (storage.SequenceEqual(safeNodes))
            return;
        foreach (var value in safeNodes)
        {
            ContainmentAddMultipleNotificationEmitter<T> emitter = new(containment, this, value, storage, null);
            emitter.CollectOldData();
            if (adder(value))
                emitter.Notify();
        }
    }

    protected void InsertOptionalMultipleContainment<T>(int index, IEnumerable<T> nodes, Containment containment,
        List<T> storage,
        Func<int, T?, bool> inserter) where T : INode
    {
        AssureInRange(index, storage);
        var safeNodes = nodes?.ToList();
        AssureNotNull(safeNodes, containment);
        AssureNoSelfMove(index, safeNodes, storage);
        AssureNotNullMembers(safeNodes, containment);
        foreach (var value in safeNodes)
        {
            ContainmentAddMultipleNotificationEmitter<T> emitter = new(containment, this, value, storage, index);
            emitter.CollectOldData();
            if (inserter(index++, value))
                emitter.Notify();
        }
    }

    protected void RemoveOptionalMultipleContainment<T>(IEnumerable<T>? nodes, Containment containment,
        List<T> storage, Func<T?, bool> remover) where T : INode
    {
        RemoveSelfParent(nodes?.ToList(), storage, containment, ContainmentRemover<T>(containment));
    }

    #endregion

    #endregion

    #region Required

    #region Single

    protected void SetRequiredSingleContainment<T>(T value, Containment containment, T? storage,
        Func<T?, bool> setter) where T : INode
    {
        AssureNotNull(value, containment);
        ContainmentSingleNotificationEmitter<T> emitter = new(containment, this, value, storage);
        emitter.CollectOldData();
        if (setter(value))
            emitter.Notify();
    }

    #endregion

    #region Multiple

    protected void SetRequiredMultipleContainment<T>(object? value, Containment containment, List<T> storage,
        Func<List<T>, bool> setter) where T : INode
    {
        var safeNodes = containment.AsNodes<T>(value).ToList();
        AssureNonEmpty(safeNodes, containment);
        ContainmentSetNotificationEmitter<T> emitter = new(containment, this, safeNodes, storage);
        emitter.CollectOldData();
        if (setter(safeNodes))
            emitter.Notify();
    }

    protected void AddRequiredMultipleContainment<T>(IEnumerable<T>? nodes, Containment containment, List<T> storage,
        Func<T, bool> adder) where T : INode
    {
        var safeNodes = nodes?.ToList();
        AssureNonEmpty(safeNodes, storage, containment);
        if (storage.SequenceEqual(safeNodes))
            return;
        foreach (var value in safeNodes)
        {
            ContainmentAddMultipleNotificationEmitter<T> emitter = new(containment, this, value, storage, null);
            emitter.CollectOldData();
            if (adder(value))
                emitter.Notify();
        }
    }

    protected void InsertRequiredMultipleContainment<T>(int index, IEnumerable<T>? nodes, Containment containment,
        List<T> storage,
        Func<int, T, bool> inserter) where T : INode
    {
        AssureInRange(index, storage);
        var safeNodes = nodes?.ToList();
        AssureNonEmpty(safeNodes, storage, containment);
        AssureNoSelfMove(index, safeNodes, storage);
        foreach (var value in safeNodes)
        {
            ContainmentAddMultipleNotificationEmitter<T> emitter = new(containment, this, value, storage, index);
            emitter.CollectOldData();
            if (inserter(index++, value))
                emitter.Notify();
        }
    }

    protected void RemoveRequiredMultipleContainment<T>(IEnumerable<T>? nodes, Containment containment,
        List<T> storage, Func<T?, bool> remover) where T : INode
    {
        var safeNodes = nodes?.ToList();
        AssureNotNull(safeNodes, containment);
        AssureNotClearing(safeNodes, storage, containment);
        RemoveSelfParent(safeNodes, storage, containment, ContainmentRemover<T>(containment));
    }

    #endregion

    #endregion

    #endregion

    #region Reference

    #region Optional

    #region Single

    protected void SetOptionalSingleReference<T>(ReferenceTarget? value, Reference reference, ReferenceTarget? storage,
        Func<ReferenceTarget?, bool> setter) where T : IReadableNode
    {
        AssureNullableInstance<T>(value, reference);
        ReferenceSingleNotificationEmitter<T> emitter = new(reference, this, value, storage);
        emitter.CollectOldData();
        if (setter(value))
            emitter.Notify();
    }

    #endregion

    #region Multiple

    protected void SetOptionalMultipleReference<T>(object? value, Reference reference, List<ReferenceTarget> storage,
        Func<List<ReferenceTarget>, bool> setter) where T : IReadableNode
    {
        var safeNodes = reference.AsReferenceTargets<T>(value).ToList();
        AssureNotNull(safeNodes, reference);
        AssureNotNullMembers(safeNodes, reference);
        ReferenceSetNotificationEmitter<T> emitter = new(reference, this, safeNodes, storage);
        emitter.CollectOldData();
        if (setter(safeNodes))
            emitter.Notify();
    }

    protected void AddOptionalMultipleReference<T>(IEnumerable<T>? nodes, Reference reference,
        List<ReferenceTarget> storage, Func<ReferenceTarget, bool> adder) where T : IReadableNode
    {
        var safeNodes = nodes?.Select(node => ReferenceTarget.FromNode(node)).ToList();
        AssureNotNull(safeNodes, reference);
        AssureNotNullMembers(safeNodes, reference);
        foreach (var value in safeNodes)
        {
            ReferenceAddMultipleNotificationEmitter<T> emitter = new(reference, this, value, storage.Count);
            emitter.CollectOldData();
            if (adder(value))
                emitter.Notify();
        }
    }

    protected void InsertOptionalMultipleReference<T>(int index, IEnumerable<T>? nodes, Reference reference,
        List<ReferenceTarget> storage,
        Func<int, ReferenceTarget, bool> inserter) where T : IReadableNode
    {
        AssureInRange(index, storage);
        var safeNodes = nodes?.Select(node => ReferenceTarget.FromNode(node)).ToList();
        AssureNotNull(safeNodes, reference);
        AssureNotNullMembers(safeNodes, reference);
        foreach (var value in safeNodes)
        {
            ReferenceAddMultipleNotificationEmitter<T> emitter = new(reference, this, value, index);
            emitter.CollectOldData();
            if (inserter(index++, value))
                emitter.Notify();
        }
    }

    protected void RemoveOptionalMultipleReference<T>(IEnumerable<T>? nodes, Reference reference,
        List<ReferenceTarget> storage, Func<ReferenceTarget, bool> remover) where T : IReadableNode
    {
        var safeNodes = nodes?.ToList();
        AssureNotNull(safeNodes, reference);
        AssureNotNullMembers(safeNodes, reference);
        RemoveAll(safeNodes, storage, ReferenceRemover<T>(reference));
    }

    #endregion

    #endregion

    #region Required

    #region Single

    protected void SetRequiredSingleReference<T>(ReferenceTarget? value, Reference reference, ReferenceTarget? storage,
        Func<ReferenceTarget?, bool> setter) where T : IReadableNode
    {
        AssureNotNullInstance<T>(value, reference);
        ReferenceSingleNotificationEmitter<T> emitter = new(reference, this, value, storage);
        emitter.CollectOldData();
        if (setter(value))
            emitter.Notify();
    }

    #endregion

    #region Multiple

    protected void SetRequiredMultipleReference<T>(object? value, Reference reference, List<ReferenceTarget> storage,
        Func<List<ReferenceTarget>, bool> setter) where T : IReadableNode
    {
        var safeNodes = reference.AsReferenceTargets<T>(value).ToList();
        AssureNonEmpty(safeNodes, reference);
        ReferenceSetNotificationEmitter<T> emitter = new(reference, this, safeNodes, storage);
        emitter.CollectOldData();
        if (setter(safeNodes))
            emitter.Notify();
    }

    protected void AddRequiredMultipleReference<T>(IEnumerable<T> nodes, Reference reference,
        List<ReferenceTarget> storage, Func<ReferenceTarget, bool> adder) where T : IReadableNode
    {
        var safeNodes = nodes?.Select(node => ReferenceTarget.FromNode(node)).ToList();
        AssureNotNull(safeNodes, reference);
        AssureNonEmpty(safeNodes, storage, reference);
        foreach (var value in safeNodes)
        {
            ReferenceAddMultipleNotificationEmitter<T> emitter = new(reference, this, value, storage.Count);
            emitter.CollectOldData();
            if (adder(value))
                emitter.Notify();
        }
    }

    protected void InsertRequiredMultipleReference<T>(int index, IEnumerable<T> nodes, Reference reference,
        List<ReferenceTarget> storage,
        Func<int, ReferenceTarget, bool> inserter) where T : IReadableNode
    {
        AssureInRange(index, storage);
        var safeNodes = nodes?.Select(node => ReferenceTarget.FromNode(node)).ToList();
        AssureNotNull(safeNodes, reference);
        AssureNonEmpty(safeNodes, storage, reference);
        foreach (var value in safeNodes)
        {
            ReferenceAddMultipleNotificationEmitter<T> emitter = new(reference, this, value, index);
            emitter.CollectOldData();
            if (inserter(index++, value))
                emitter.Notify();
        }
    }

    protected void RemoveRequiredMultipleReference<T>(IEnumerable<T>? nodes, Reference reference,
        List<ReferenceTarget> storage, Func<ReferenceTarget, bool> remover) where T : IReadableNode
    {
        var safeNodes = nodes?.ToList();
        AssureNotNull(safeNodes, reference);
        AssureNonEmpty(safeNodes, storage, reference);
        AssureNotClearing(safeNodes, ReferenceTargetNullableTargets<T>(storage, reference), reference);
        RemoveAll(safeNodes, storage, ReferenceRemover<T>(reference));
    }

    #endregion

    #endregion

    #endregion
}
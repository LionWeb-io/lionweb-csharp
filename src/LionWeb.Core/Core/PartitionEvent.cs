﻿// Copyright 2025 TRUMPF Laser SE and other contributors
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

using M1;
using M2;
using M3;
using System.Diagnostics.CodeAnalysis;
using Utilities;

public abstract class PartitionEventBase<T> where T : IReadableNode
{
    protected readonly IPartitionCommander? _partitionCommander;
    protected readonly NodeBase _newParent;

    protected PartitionEventBase(NodeBase newParent)
    {
        _newParent = newParent;
        _partitionCommander = newParent.GetPartitionCommander();
    }

    public abstract void CollectOldData();
    public abstract void RaiseEvent();

    [MemberNotNullWhen(true, nameof(_partitionCommander))]
    protected abstract bool IsActive();
}

public abstract class PartitionContainmentEventBase<T> : PartitionEventBase<T> where T : INode
{
    protected readonly Containment _containment;

    protected PartitionContainmentEventBase(Containment containment, NodeBase newParent) : base(newParent)
    {
        _containment = containment;
    }

    protected OldContainmentInfo? Collect(T value)
    {
        var oldParent = value.GetParent();
        if (oldParent == null)
            return null;

        var oldContainment = oldParent.GetContainmentOf(value);
        if (oldContainment == null)
            return null;

        var oldIndex = oldContainment.Multiple
            ? M2Extensions.AsNodes<INode>(oldParent.Get(oldContainment)).ToList().IndexOf(value)
            : 0;
        
        return new OldContainmentInfo(oldParent, oldContainment, oldIndex);
    }

    protected record OldContainmentInfo(INode Parent, Containment Containment, int Index);
}

public abstract class PartitionMultipleContainmentEventBase<T> : PartitionContainmentEventBase<T> where T : INode
{
    protected Dictionary<T, OldContainmentInfo?> _newValues;

    protected PartitionMultipleContainmentEventBase(Containment containment, NodeBase newParent, List<T>? newValues) :
        base(containment, newParent)
    {
        _newValues = newValues?.ToDictionary<T, T, OldContainmentInfo?>(k => k, _ => null) ?? [];
    }

    /// <inheritdoc />
    public override void CollectOldData()
    {
        if (!IsActive())
            return;

        foreach (T setValue in _newValues.Keys.ToList())
        {
            _newValues[setValue] = Collect(setValue);
        }
    }
}

public class SetContainmentEvent<T> : PartitionMultipleContainmentEventBase<T> where T : INode
{
    private readonly List<IListComparer<T>.Change> _changes = [];

    public SetContainmentEvent(
        Containment containment,
        NodeBase newParent,
        List<T>? setValues,
        List<T> existingValues
    ) : base(containment, newParent, setValues)
    {
        if (!IsActive() || setValues == null)
            return;

        var listComparer = new StepwiseListComparer<T>(existingValues, setValues);
        _changes = listComparer.Compare();
    }

    /// <inheritdoc />
    public override void RaiseEvent()
    {
        if (!IsActive())
            return;

        foreach (var change in _changes)
        {
            switch (change)
            {
                case IListComparer<T>.Added added:
                    switch (added, _newValues[added.Element])
                    {
                        case ({ }, null):
                            _partitionCommander.AddChild(_newParent, added.Element, _containment, added.RightIndex);
                            break;

                        case ({ }, { } o) when o.Parent != _newParent:
                            _partitionCommander.MoveChildFromOtherContainment(
                                _newParent,
                                _containment,
                                added.RightIndex,
                                added.Element,
                                o.Parent,
                                o.Containment,
                                o.Index
                            );
                            break;


                        case ({ }, { } o) when o.Parent == _newParent && o.Containment != _containment:
                            _partitionCommander.MoveChildFromOtherContainmentInSameParent(
                                _containment,
                                added.RightIndex,
                                added.Element,
                                _newParent,
                                o.Containment,
                                o.Index
                            );
                            break;

                        default:
                            throw new ArgumentException("Unknown state");
                    }

                    break;

                case IListComparer<T>.Moved moved:
                    _partitionCommander.MoveChildInSameContainment(moved.RightIndex, moved.LeftElement, _newParent,
                        _containment, moved.LeftIndex);
                    break;
                case IListComparer<T>.Deleted deleted:
                    _partitionCommander.DeleteChild(deleted.Element, _newParent, _containment, deleted.LeftIndex);
                    break;
            }
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(_partitionCommander))]
    protected override bool IsActive() =>
        _partitionCommander != null && (_partitionCommander.CanRaiseAddChild() ||
                                        _partitionCommander.CanRaiseDeleteChild() ||
                                        _partitionCommander
                                            .CanRaiseMoveChildFromOtherContainment() ||
                                        _partitionCommander
                                            .CanRaiseMoveChildFromOtherContainmentInSameParent() ||
                                        _partitionCommander
                                            .CanRaiseMoveChildInSameContainment());
}

public class AddMultipleContainmentsEvent<T> : PartitionMultipleContainmentEventBase<T> where T : INode
{
    private int _newIndex;

    public AddMultipleContainmentsEvent(
        Containment containment,
        NodeBase newParent,
        List<T>? addedValues,
        List<T> existingValues,
        int? newIndex = null
    ) : base(containment, newParent, addedValues)
    {
        _newIndex = newIndex ?? Math.Max(existingValues.Count - 1, 0);
    }

    /// <inheritdoc />
    public override void RaiseEvent()
    {
        if (!IsActive())
            return;

        foreach ((T? added, OldContainmentInfo? old) in _newValues)
        {
            switch (added, old)
            {
                case ({ }, null):
                    _partitionCommander.AddChild(_newParent, added, _containment, _newIndex);
                    break;

                case ({ }, { } o) when o.Parent != _newParent:
                    _partitionCommander.MoveChildFromOtherContainment(
                        _newParent,
                        _containment,
                        _newIndex,
                        added,
                        o.Parent,
                        o.Containment,
                        o.Index
                    );
                    break;


                case ({ }, { } o) when o.Parent == _newParent && o.Containment == _containment && o.Index == _newIndex:
                    // no-op
                    break;

                case ({ }, { } o) when o.Parent == _newParent && o.Containment == _containment:
                    _partitionCommander.MoveChildInSameContainment(
                        _newIndex,
                        added,
                        _newParent,
                        o.Containment,
                        o.Index
                    );
                    break;

                case ({ }, { } o) when o.Parent == _newParent && o.Containment != _containment:
                    _partitionCommander.MoveChildFromOtherContainmentInSameParent(
                        _containment,
                        _newIndex,
                        added,
                        _newParent,
                        o.Containment,
                        o.Index
                    );
                    break;

                default:
                    throw new ArgumentException("Unknown state");
            }

            _newIndex++;
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(_partitionCommander))]
    protected override bool IsActive() =>
        _partitionCommander != null && (_partitionCommander.CanRaiseAddChild() ||
                                        _partitionCommander
                                            .CanRaiseMoveChildFromOtherContainment() ||
                                        _partitionCommander
                                            .CanRaiseMoveChildFromOtherContainmentInSameParent() ||
                                        _partitionCommander
                                            .CanRaiseMoveChildInSameContainment());
}

public class SingleContainmentEvent<T> : PartitionContainmentEventBase<T> where T : INode
{
    private readonly T? _newValue;
    private readonly T? _oldValue;

    private INode? _oldParent;
    private Containment? _oldContainment;
    private int _oldIndex;

    public SingleContainmentEvent(Containment containment, NodeBase newParent, T? newValue, T? oldValue)
        : base(containment, newParent)
    {
        _oldValue = oldValue;
        _newValue = newValue;
    }

    /// <inheritdoc />
    public override void CollectOldData()
    {
        if (!IsActive() || _newValue == null)
            return;

        OldContainmentInfo? oldInfo = Collect(_newValue);
        if(oldInfo == null)
            return;

        _oldParent = oldInfo.Parent;
        _oldContainment = oldInfo.Containment;
        _oldIndex = oldInfo.Index;
    }

    /// <inheritdoc />
    public override void RaiseEvent()
    {
        if (!IsActive())
            return;

        switch (_oldValue, _newValue, _oldParent)
        {
            case (null, null, _):
                // fall-through
            case ({ }, { }, _) when Equals(_oldValue, _newValue):
                // no-op
                break;

            case ({ }, null, _):
                _partitionCommander.DeleteChild(_oldValue, _newParent, _containment, 0);
                break;

            case (null, { }, null):
                _partitionCommander.AddChild(_newParent, _newValue, _containment, 0);
                break;

            case ({ }, { }, null):
                _partitionCommander.ReplaceChild(_newValue, _oldValue, _newParent, _containment, 0);
                break;

            case (null, { }, { })
                when _oldParent == _newParent && _oldContainment != _containment:
                _partitionCommander.MoveChildFromOtherContainmentInSameParent(_containment, 0, _newValue,
                    _newParent, _oldContainment, _oldIndex);
                break;

            case ({ }, { }, { })
                when _oldParent == _newParent && _oldContainment != _containment:
                _partitionCommander.DeleteChild(_oldValue, _newParent, _containment, 0);
                _partitionCommander.MoveChildFromOtherContainmentInSameParent(_containment, 0, _newValue,
                    _newParent, _oldContainment, _oldIndex);
                break;

            case ({ }, { }, { })
                when _oldParent != _newParent:
                _partitionCommander.DeleteChild(_oldValue, _newParent, _containment, 0);
                _partitionCommander.MoveChildFromOtherContainment(_newParent, _containment, 0, _newValue,
                    _oldParent, _oldContainment, _oldIndex);
                break;

            case (null, { }, { })
                when _oldParent != _newParent:
                _partitionCommander.MoveChildFromOtherContainment(_newParent, _containment, 0, _newValue,
                    _oldParent, _oldContainment, _oldIndex);
                break;

            default:
                throw new ArgumentException("Unknown state");
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(_partitionCommander))]
    protected override bool IsActive() =>
        _partitionCommander != null && (_partitionCommander.CanRaiseAddChild() ||
                                        _partitionCommander.CanRaiseDeleteChild() ||
                                        _partitionCommander.CanRaiseReplaceChild() ||
                                        _partitionCommander
                                            .CanRaiseMoveChildFromOtherContainment() ||
                                        _partitionCommander
                                            .CanRaiseMoveChildFromOtherContainmentInSameParent() ||
                                        _partitionCommander
                                            .CanRaiseMoveChildInSameContainment());
}

public class SetReferenceEvent<T> : PartitionEventBase<T> where T : IReadableNode
{
    private readonly Reference _reference;
    private List<IListComparer<T>.Change> _changes = [];

    public SetReferenceEvent(Reference reference, NodeBase newParent, List<T> safeNodes, List<T> storage) : base(newParent)
    {
        _reference = reference;

        if (!IsActive())
            return;

        var listComparer = new StepwiseListComparer<T>(storage, safeNodes);
        _changes = listComparer.Compare();
    }

    /// <inheritdoc />
    public override void CollectOldData() { }

    /// <inheritdoc />
    public override void RaiseEvent()
    {
        if (!IsActive())
            return;
        
        foreach (var change in _changes)
        {
            switch (change)
            {
                case IListComparer<T>.Added added:
                    _partitionCommander.AddReference(_newParent, _reference, added.RightIndex,
                        new ReferenceTarget(null, added.Element));
                    break;
                case IListComparer<T>.Moved moved:
                    _partitionCommander.MoveEntryInSameReference(_newParent, _reference, moved.LeftIndex, moved.RightIndex,
                        new ReferenceTarget(null, moved.LeftElement));
                    break;
                case IListComparer<T>.Deleted deleted:
                    _partitionCommander.DeleteReference(_newParent, _reference, deleted.LeftIndex,
                        new ReferenceTarget(null, deleted.Element));
                    break;
            }
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(_partitionCommander))]
    protected override bool IsActive() =>
        _partitionCommander != null && (_partitionCommander.CanRaiseAddReference() ||
                                       _partitionCommander.CanRaiseMoveEntryInSameReference() ||
                                       _partitionCommander.CanRaiseDeleteReference());

}
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

using M1;
using M1.Event.Partition;
using M2;
using M3;
using System.Diagnostics.CodeAnalysis;
using Utilities;

/// Encapsulates event-related logic and data to execute
/// <see cref="CollectOldData">before</see> and <see cref="RaiseEvent">after</see>
/// the actual manipulation of the underlying nodes for one specific <see cref="Feature"/>.
/// <typeparam name="T">Type of nodes of the represented <see cref="Feature"/>.</typeparam>
public abstract class PartitionEventBase<T> where T : IReadableNode
{
    /// <see cref="IPartitionCommander"/> to use for our events, if any.
    protected readonly IPartitionCommander? PartitionCommander;

    /// Owner of the represented <see cref="Feature"/>.
    protected readonly NodeBase NewParent;

    /// <param name="newParent"> Owner of the represented <see cref="Feature"/>.</param>
    protected PartitionEventBase(NodeBase newParent)
    {
        NewParent = newParent;
        PartitionCommander = newParent.GetPartitionCommander();
    }

    /// Logic to execute <i>before</i> any changes to the underlying nodes.
    public abstract void CollectOldData();

    /// Logic to execute <i>after</i> any changes to the underlying nodes.
    public abstract void RaiseEvent();

    /// <summary>
    /// Whether this event should execute at all.
    /// </summary>
    [MemberNotNullWhen(true, nameof(PartitionCommander))]
    protected abstract bool IsActive();
}

#region Containment

/// Encapsulates event-related logic and data for <see cref="Containment"/>s.
/// <typeparam name="T">Type of nodes of the represented <see cref="Containment"/>.</typeparam>
public abstract class PartitionContainmentEventBase<T> : PartitionEventBase<T> where T : INode
{
    /// Represented <see cref="Containment"/>.
    protected readonly Containment Containment;

    /// <param name="containment">Represented <see cref="Containment"/>.</param>
    /// <param name="newParent"> Owner of the represented <paramref name="containment"/>.</param>
    protected PartitionContainmentEventBase(Containment containment, NodeBase newParent) : base(newParent)
    {
        Containment = containment;
    }

    /// Collects <see cref="OldContainmentInfo"/> from <paramref name="value"/>, to be used in <see cref="PartitionEventBase{T}.CollectOldData"/>
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

    /// Context of a node before it has been removed from its previous <paramref name="Parent"/>.
    /// <param name="Parent"></param>
    /// <param name="Containment"></param>
    /// <param name="Index"></param>
    protected record OldContainmentInfo(INode Parent, Containment Containment, Index Index);
}

/// Encapsulates event-related logic and data for <i>multiple</i> <see cref="Containment"/>s.
/// <typeparam name="T">Type of nodes of the represented <see cref="Containment"/>.</typeparam>
public abstract class PartitionMultipleContainmentEventBase<T> : PartitionContainmentEventBase<T> where T : INode
{
    /// Newly set values and their previous context.
    protected readonly Dictionary<T, OldContainmentInfo?> NewValues;

    /// <param name="containment">Represented <see cref="Containment"/>.</param>
    /// <param name="newParent"> Owner of the represented <paramref name="containment"/>.</param>
    /// <param name="newValues">Newly set values.</param>
    protected PartitionMultipleContainmentEventBase(Containment containment, NodeBase newParent, List<T>? newValues) :
        base(containment, newParent)
    {
        NewValues = newValues?.ToDictionary<T, T, OldContainmentInfo?>(k => k, _ => null) ?? [];
    }

    /// <inheritdoc />
    public override void CollectOldData()
    {
        if (!IsActive())
            return;

        foreach (T newValue in NewValues.Keys.ToList())
        {
            NewValues[newValue] = Collect(newValue);
        }
    }
}

/// Encapsulates event-related logic and data for <see cref="IWritableNode.Set">reflective</see> change of <see cref="Containment"/>s.
/// <typeparam name="T">Type of nodes of the represented <see cref="Containment"/>.</typeparam>
public class SetContainmentEvent<T> : PartitionMultipleContainmentEventBase<T> where T : INode
{
    private readonly List<IListComparer<T>.IChange> _changes = [];

    /// <param name="containment">Represented <see cref="Containment"/>.</param>
    /// <param name="newParent"> Owner of the represented <paramref name="containment"/>.</param>
    /// <param name="setValues">Newly set values.</param>
    /// <param name="existingValues">Values previously present in <paramref name="containment"/>.</param>
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
                    switch (NewValues[added.Element])
                    {
                        case null:
                            PartitionCommander.AddChild(
                                NewParent,
                                added.Element,
                                Containment,
                                added.RightIndex
                                );
                            break;

                        case { } old when old.Parent != NewParent:
                            PartitionCommander.MoveChildFromOtherContainment(
                                NewParent,
                                Containment,
                                added.RightIndex,
                                added.Element,
                                old.Parent,
                                old.Containment,
                                old.Index
                            );
                            break;


                        case { } old when old.Parent == NewParent && old.Containment != Containment:
                            PartitionCommander.MoveChildFromOtherContainmentInSameParent(
                                Containment,
                                added.RightIndex,
                                added.Element,
                                NewParent,
                                old.Containment,
                                old.Index
                            );
                            break;

                        default:
                            throw new ArgumentException("Unknown state");
                    }

                    break;

                case IListComparer<T>.Moved moved:
                    PartitionCommander.MoveChildInSameContainment(
                        moved.RightIndex,
                        moved.LeftElement,
                        NewParent,
                        Containment,
                        moved.LeftIndex
                        );
                    break;
                case IListComparer<T>.Deleted deleted:
                    PartitionCommander.DeleteChild(
                        deleted.Element,
                        NewParent,
                        Containment,
                        deleted.LeftIndex
                        );
                    break;
            }
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(PartitionCommander))]
    protected override bool IsActive() =>
        PartitionCommander != null && (PartitionCommander.CanRaiseAddChild ||
                                       PartitionCommander.CanRaiseDeleteChild ||
                                       PartitionCommander.CanRaiseMoveChildFromOtherContainment ||
                                       PartitionCommander.CanRaiseMoveChildFromOtherContainmentInSameParent ||
                                       PartitionCommander.CanRaiseMoveChildInSameContainment);
}

/// Encapsulates event-related logic and data for <i>adding</i> or <i>inserting</i> of <see cref="Containment"/>s.
/// <typeparam name="T">Type of nodes of the represented <see cref="Containment"/>.</typeparam>
public class AddMultipleContainmentsEvent<T> : PartitionMultipleContainmentEventBase<T> where T : INode
{
    private readonly List<T> _existingValues;
    private Index _newIndex;

    /// <param name="containment">Represented <see cref="Containment"/>.</param>
    /// <param name="newParent"> Owner of the represented <paramref name="containment"/>.</param>
    /// <param name="addedValues">Newly added values.</param>
    /// <param name="existingValues">Values already present in <paramref name="containment"/>.</param>
    /// <param name="startIndex">Optional index where we add <paramref name="addedValues"/> to <paramref name="containment"/>.</param>
    public AddMultipleContainmentsEvent(
        Containment containment,
        NodeBase newParent,
        List<T>? addedValues,
        List<T> existingValues,
        Index? startIndex = null
    ) : base(containment, newParent, addedValues)
    {
        _existingValues = existingValues;
        _newIndex = startIndex ?? Math.Max(existingValues.Count, 0);
    }

    /// <inheritdoc />
    public override void RaiseEvent()
    {
        if (!IsActive())
            return;

        foreach ((T? added, OldContainmentInfo? old) in NewValues)
        {
            switch (old)
            {
                case null:
                    PartitionCommander.AddChild(NewParent, added, Containment, _newIndex);
                    break;

                case not null when old.Parent != NewParent:
                    PartitionCommander.MoveChildFromOtherContainment(
                        NewParent,
                        Containment,
                        _newIndex,
                        added,
                        old.Parent,
                        old.Containment,
                        old.Index
                    );
                    break;

                case not null when old.Parent == NewParent && old.Containment == Containment && old.Index == _newIndex:
                    // no-op
                    break;

                case not null when old.Parent == NewParent && old.Containment == Containment:
                    if (old.Index < _newIndex)
                        _newIndex--;
                    PartitionCommander.MoveChildInSameContainment(
                        _newIndex,
                        added,
                        NewParent,
                        old.Containment,
                        old.Index
                    );
                    break;

                case not null when old.Parent == NewParent && old.Containment != Containment:
                    PartitionCommander.MoveChildFromOtherContainmentInSameParent(
                        Containment,
                        _newIndex,
                        added,
                        NewParent,
                        old.Containment,
                        old.Index
                    );
                    break;

                default:
                    throw new ArgumentException("Unknown state");
            }

            _newIndex++;
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(PartitionCommander))]
    protected override bool IsActive() =>
        PartitionCommander != null && (PartitionCommander.CanRaiseAddChild ||
                                       PartitionCommander.CanRaiseMoveChildFromOtherContainment ||
                                       PartitionCommander.CanRaiseMoveChildFromOtherContainmentInSameParent ||
                                       PartitionCommander.CanRaiseMoveChildInSameContainment);
}

/// Encapsulates event-related logic and data for changing <i>single</i> <see cref="Containment"/>s.
/// <typeparam name="T">Type of node of the represented <see cref="Containment"/>.</typeparam>
public class SingleContainmentEvent<T> : PartitionContainmentEventBase<T> where T : INode
{
    private readonly T? _newValue;
    private readonly T? _oldValue;

    private OldContainmentInfo? _oldContainmentInfo;

    /// <param name="containment">Represented <see cref="Containment"/>.</param>
    /// <param name="newParent"> Owner of the represented <paramref name="containment"/>.</param>
    /// <param name="newValue">Newly set value.</param>
    /// <param name="oldValue">Previous value of <paramref name="containment"/>.</param>
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
        if (oldInfo == null)
            return;

        _oldContainmentInfo = oldInfo;
    }

    /// <inheritdoc />
    public override void RaiseEvent()
    {
        if (!IsActive())
            return;

        switch (_oldValue, _newValue, _oldContainmentInfo)
        {
            case (null, null, _):
                // fall-through
            case (not null, not null, _) when Equals(_oldValue, _newValue):
                // no-op
                break;

            case (not null, null, _):
                PartitionCommander.DeleteChild(
                    _oldValue,
                    NewParent,
                    Containment,
                    0
                    );
                break;

            case (null, not null, null):
                PartitionCommander.AddChild(
                    NewParent,
                    _newValue,
                    Containment,
                    0
                    );
                break;

            case (not null, not null, null):
                PartitionCommander.ReplaceChild(
                    _newValue,
                    _oldValue,
                    NewParent,
                    Containment,
                    0
                    );
                break;

            case (null, not null, not null)
                when _oldContainmentInfo.Parent == NewParent && _oldContainmentInfo.Containment != Containment:
                PartitionCommander.MoveChildFromOtherContainmentInSameParent(
                    Containment,
                    0,
                    _newValue,
                    NewParent,
                    _oldContainmentInfo.Containment,
                    _oldContainmentInfo.Index
                    );
                break;

            case (not null, not null, not null)
                when _oldContainmentInfo.Parent == NewParent && _oldContainmentInfo.Containment != Containment:
                PartitionCommander.DeleteChild(
                    _oldValue,
                    NewParent,
                    Containment,
                    0
                    );
                PartitionCommander.MoveChildFromOtherContainmentInSameParent(
                    Containment,
                    0,
                    _newValue,
                    NewParent,
                    _oldContainmentInfo.Containment,
                    _oldContainmentInfo.Index
                    );
                break;

            case (not null, not null, not null)
                when _oldContainmentInfo.Parent != NewParent:
                PartitionCommander.DeleteChild(
                    _oldValue,
                    NewParent,
                    Containment,
                    0
                    );
                PartitionCommander.MoveChildFromOtherContainment(
                    NewParent,
                    Containment,
                    0,
                    _newValue,
                    _oldContainmentInfo.Parent,
                    _oldContainmentInfo.Containment,
                    _oldContainmentInfo.Index
                    );
                break;

            case (null, not null, not null)
                when _oldContainmentInfo.Parent != NewParent:
                PartitionCommander.MoveChildFromOtherContainment(
                    NewParent,
                    Containment,
                    0,
                    _newValue,
                    _oldContainmentInfo.Parent,
                    _oldContainmentInfo.Containment,
                    _oldContainmentInfo.Index
                    );
                break;

            default:
                throw new ArgumentException("Unknown state");
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(PartitionCommander))]
    [MemberNotNullWhen(true, nameof(_oldContainmentInfo))]
    protected override bool IsActive() =>
        PartitionCommander != null && (PartitionCommander.CanRaiseAddChild ||
                                       PartitionCommander.CanRaiseDeleteChild ||
                                       PartitionCommander.CanRaiseReplaceChild ||
                                       PartitionCommander.CanRaiseMoveChildFromOtherContainment ||
                                       PartitionCommander.CanRaiseMoveChildFromOtherContainmentInSameParent ||
                                       PartitionCommander.CanRaiseMoveChildInSameContainment);
}

#endregion

#region Reference

/// Encapsulates event-related logic and data for <see cref="IWritableNode.Set">reflective</see> change of <see cref="Reference"/>s.
/// <typeparam name="T">Type of nodes of the represented <see cref="Reference"/>.</typeparam>
public class SetReferenceEvent<T> : PartitionEventBase<T> where T : IReadableNode
{
    private readonly Reference _reference;
    private readonly List<IListComparer<T>.IChange> _changes = [];

    /// <param name="reference">Represented <see cref="Reference"/>.</param>
    /// <param name="newParent"> Owner of the represented <paramref name="reference"/>.</param>
    /// <param name="safeNodes">Newly added values.</param>
    /// <param name="storage">Values already present in <paramref name="reference"/>.</param>
    public SetReferenceEvent(Reference reference, NodeBase newParent, List<T> safeNodes, List<T> storage) :
        base(newParent)
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
                    PartitionCommander.AddReference(NewParent, _reference, added.RightIndex,
                        new ReferenceTarget(null, added.Element));
                    break;
                case IListComparer<T>.Moved moved:
                    PartitionCommander.MoveEntryInSameReference(NewParent, _reference, moved.LeftIndex,
                        moved.RightIndex,
                        new ReferenceTarget(null, moved.LeftElement));
                    break;
                case IListComparer<T>.Deleted deleted:
                    PartitionCommander.DeleteReference(NewParent, _reference, deleted.LeftIndex,
                        new ReferenceTarget(null, deleted.Element));
                    break;
            }
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(PartitionCommander))]
    protected override bool IsActive() =>
        PartitionCommander != null && (PartitionCommander.CanRaiseAddReference ||
                                       PartitionCommander.CanRaiseMoveEntryInSameReference ||
                                       PartitionCommander.CanRaiseDeleteReference);
}

#endregion

#region Annotation

/// Encapsulates event-related logic and data for <i>multiple</i> <see cref="Annotation"/>s.
public abstract class PartitionAnnotationEventBase : PartitionEventBase<INode>
{
    /// Newly set values and their previous context.
    protected readonly Dictionary<INode, OldAnnotationInfo?> NewValues;

    /// <param name="newParent"> Owner of the represented <see cref="Annotation"/>s.</param>
    /// <param name="newValues">Newly set values.</param>
    protected PartitionAnnotationEventBase(NodeBase newParent, List<INode>? newValues) : base(newParent)
    {
        NewValues = newValues?.ToDictionary<INode, INode, OldAnnotationInfo?>(k => k, _ => null) ?? [];
    }

    /// <inheritdoc />
    public override void CollectOldData()
    {
        if (!IsActive())
            return;

        foreach (var newValue in NewValues.Keys.ToList())
        {
            var oldParent = newValue.GetParent();
            if (oldParent == null)
                continue;

            var oldIndex = oldParent.GetAnnotations().ToList().IndexOf(newValue);

            NewValues[newValue] = new(oldParent, oldIndex);
        }
    }

    /// Context of an annotation instance before it has been removed from its previous <paramref name="Parent"/>.
    /// <param name="Parent"></param>
    /// <param name="Index"></param>
    protected record OldAnnotationInfo(INode Parent, Index Index);
}

/// Encapsulates event-related logic and data for <see cref="IWritableNode.Set">reflective</see> change of <see cref="Annotation"/>s.
public class SetAnnotationEvent : PartitionAnnotationEventBase
{
    private readonly List<IListComparer<INode>.IChange> _changes = [];

    /// <param name="newParent"> Owner of the represented <see cref="Annotation"/>s.</param>
    /// <param name="setValues">Newly set values.</param>
    /// <param name="existingValues">Values previously present in <see cref="IReadableNode.GetAnnotations"/>.</param>
    public SetAnnotationEvent(
        NodeBase newParent,
        List<INode>? setValues,
        List<INode> existingValues
    ) : base(newParent, setValues)
    {
        if (!IsActive() || setValues == null)
            return;

        var listComparer = new StepwiseListComparer<INode>(existingValues, setValues);
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
                case IListComparer<INode>.Added added:
                    switch (NewValues[added.Element])
                    {
                        case null:
                            PartitionCommander.AddAnnotation(NewParent, added.Element, added.RightIndex);
                            break;

                        case { } old when old.Parent != NewParent:
                            PartitionCommander.MoveAnnotationFromOtherParent(
                                NewParent,
                                added.RightIndex,
                                added.Element,
                                old.Parent,
                                old.Index
                            );
                            break;


                        default:
                            throw new ArgumentException("Unknown state");
                    }

                    break;

                case IListComparer<INode>.Moved moved:
                    PartitionCommander.MoveAnnotationInSameParent(
                        moved.RightIndex,
                        moved.LeftElement,
                        NewParent,
                        moved.LeftIndex
                        );
                    break;

                case IListComparer<INode>.Deleted deleted:
                    PartitionCommander.DeleteAnnotation(deleted.Element, NewParent, deleted.LeftIndex);
                    break;
            }
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(PartitionCommander))]
    protected override bool IsActive() =>
        PartitionCommander != null && (PartitionCommander.CanRaiseAddAnnotation ||
                                       PartitionCommander.CanRaiseDeleteAnnotation ||
                                       PartitionCommander.CanRaiseMoveAnnotationFromOtherParent ||
                                       PartitionCommander.CanRaiseMoveAnnotationInSameParent);
}

/// Encapsulates event-related logic and data for <i>adding</i> or <i>inserting</i> of <see cref="Annotation"/>s.
public class AddMultipleAnnotationsEvent : PartitionAnnotationEventBase
{
    private Index _newIndex;

    /// <param name="newParent"> Owner of the represented <see cref="Annotation"/>.</param>
    /// <param name="addedValues">Newly added values.</param>
    /// <param name="existingValues">Values already present in <see cref="IReadableNode.GetAnnotations"/>.</param>
    /// <param name="startIndex">Optional index where we add <paramref name="addedValues"/> to <see cref="Annotation"/>s.</param>
    public AddMultipleAnnotationsEvent(
        NodeBase newParent,
        List<INode>? addedValues,
        List<INode> existingValues,
        Index? startIndex = null
    ) : base(newParent, addedValues)
    {
        _newIndex = startIndex ?? Math.Max(existingValues.Count - 1, 0);
    }

    /// <inheritdoc />
    public override void RaiseEvent()
    {
        if (!IsActive())
            return;

        foreach ((INode? added, OldAnnotationInfo? old) in NewValues)
        {
            switch (old)
            {
                case null:
                    PartitionCommander.AddAnnotation(NewParent, added, _newIndex);
                    break;

                case not null when old.Parent != NewParent:
                    PartitionCommander.MoveAnnotationFromOtherParent(
                        NewParent,
                        _newIndex,
                        added,
                        old.Parent,
                        old.Index
                    );
                    break;


                case not null when old.Parent == NewParent && old.Index == _newIndex:
                    // no-op
                    break;

                case not null when old.Parent == NewParent:
                    PartitionCommander.MoveAnnotationInSameParent(
                        _newIndex,
                        added,
                        NewParent,
                        old.Index
                    );
                    break;

                default:
                    throw new ArgumentException("Unknown state");
            }

            _newIndex++;
        }
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(PartitionCommander))]
    protected override bool IsActive() =>
        PartitionCommander != null && (PartitionCommander.CanRaiseAddAnnotation ||
                                       PartitionCommander.CanRaiseMoveAnnotationFromOtherParent ||
                                       PartitionCommander.CanRaiseMoveAnnotationInSameParent);
}

#endregion
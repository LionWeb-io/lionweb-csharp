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

namespace LionWeb.Core.M1;

using M3;
using Notification;
using Notification.Partition;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Replaces <paramref name="self"/> in its parent with <paramref name="replacement"/>.
///
/// Does <i>not</i> change references to <paramref name="self"/>.
/// </summary>
/// <param name="self">Base node, must have a parent.</param>
/// <param name="replacement">Node that will replace <paramref name="self"/> in <paramref name="self"/>'s parent.</param>
/// <typeparam name="T">Type of <paramref name="replacement"/>.</typeparam>
/// <returns><paramref name="replacement"/></returns>
/// <exception cref="TreeShapeException">If <paramref name="self"/> has no parent.</exception>
internal class NodeReplacer<T>(INode self, T replacement) where T : INode
{
    private INode _parent = null!;
    private Containment _containment = null!;
    private IPartitionInstance? _newPartition;
    private IPartitionNotificationProducer? _newProducer;
    private INode? _oldParent;
    private IPartitionInstance? _oldPartition;
    private IPartitionNotificationProducer? _oldProducer;
    private INotificationId? _notificationId;

    private Containment? _oldContainment;

    private Index _oldIndex;

    public T Replace()
    {
        if (!CheckParameters())
            return replacement;

        InitFields();

        var notification = _containment == null
            ? ReplaceAnnotation()
            : ReplaceContainment();

        if (notification is null)
            return replacement;

        ProduceNotifications(notification);

        return replacement;
    }

    private void InitFields()
    {
        _containment = _parent.GetContainmentOf(self)!;

        _newPartition = _parent.GetPartition();
        _newProducer = _newPartition?.GetNotificationProducer();
        _oldParent = replacement.GetParent();
        if (_oldParent is not null)
        {
            _oldPartition = _oldParent.GetPartition();
            _oldProducer = _oldPartition?.GetNotificationProducer();
        }

        _notificationId = _newProducer?.CreateNotificationId() ?? _oldProducer?.CreateNotificationId();

        if (_oldParent is not null && NeedNotification)
        {
            _oldContainment = _oldParent.GetContainmentOf(replacement);
            _oldIndex = RetrieveOldIndex();
        }
    }

    private Index RetrieveOldIndex()
    {
        Index oldIndex;
        switch (_oldContainment)
        {
            case null:
                oldIndex = -1;
                break;

            case { Multiple: true }:
                Debug.Assert(_oldParent is not null);
                if (!_oldParent.TryGetContainmentsRaw(_oldContainment, out var oldNodes))
                    // should not happen
                    throw new TreeShapeException(self, "Node not contained in its parent");

                oldIndex = oldNodes.IndexOf(replacement);
                if (oldIndex < 0)
                    // should not happen
                    throw new TreeShapeException(self, "Node not contained in its parent");
                break;

            case { Multiple: false }:
                oldIndex = 0;
                break;
        }

        return oldIndex;
    }

    private bool CheckParameters()
    {
        if (ReferenceEquals(self, replacement))
            return false;

        _parent = self.GetParent()!;
        if (_parent == null)
            throw new TreeShapeException(self, "Cannot replace a node with no parent");

        if (replacement is null)
            throw new UnsupportedNodeTypeException(replacement, nameof(replacement));

        return true;
    }

    #region Annotation

    private INotification? ReplaceAnnotation()
    {
        var index = CheckAnnotation(out IAnnotationInstance selfAnnotation, out IAnnotationInstance replaceAnnotation);

        ReplaceAnnotation(index, replaceAnnotation, selfAnnotation);

        return CreateAnnotationNotification(index, replaceAnnotation);
    }

    private Index CheckAnnotation(out IAnnotationInstance selfAnnotation, out IAnnotationInstance replaceAnnotation)
    {
        if (self is not IAnnotationInstance ann || replacement is not IAnnotationInstance replAnn)
            throw new InvalidValueException(null, replacement);

        var index = _parent.GetAnnotationsRaw().IndexOf(ann);
        if (index < 0)
            // should not happen
            throw new TreeShapeException(self, "Node not contained in its parent");

        selfAnnotation = ann;
        replaceAnnotation = replAnn;
        return index;
    }

    private void ReplaceAnnotation(Index index, IAnnotationInstance replaceAnnotation,
        IAnnotationInstance selfAnnotation)
    {
        if (!_parent.InsertAnnotationsRaw(index, replaceAnnotation)
            || !_parent.RemoveAnnotationsRaw(selfAnnotation))
        {
            throw new InvalidValueException(null, replacement);
        }
    }

    private INotification? CreateAnnotationNotification(Index index, IAnnotationInstance replaceAnnotation)
    {
        if (!NeedNotification)
            return null;

        if (_oldParent is null)
            return new AnnotationReplacedNotification(replacement, self, _parent, index, _notificationId);

        var oldIndex = _oldParent.GetAnnotationsRaw().IndexOf(replaceAnnotation);
        if (oldIndex < 0)
            // should not happen
            throw new TreeShapeException(self, "Node not contained in its parent");

        return _oldParent switch
        {
            not null when _oldParent == _parent =>
                new AnnotationMovedAndReplacedInSameParentNotification(index, replacement, _parent, oldIndex, self,
                    _notificationId),
            not null when _oldParent != _parent =>
                new AnnotationMovedAndReplacedFromOtherParentNotification(_parent, index, replacement, _oldParent,
                    oldIndex, self, _notificationId),
            _ =>
                throw new ArgumentException()
        };
    }

    #endregion

    #region Containment

    private INotification? ReplaceContainment()
    {
        Index index = _containment switch
        {
            { Multiple: true } => ReplaceMultipleContainment(),
            { Multiple: false } => ReplaceSingleContainment()
        };

        return CreateContainmentNotification(index);
    }

    private Index ReplaceMultipleContainment()
    {
        if (!_parent.TryGetContainmentsRaw(_containment, out var nodes))
            // should not happen
            throw new TreeShapeException(self, "Node not contained in its parent");

        var index = nodes.IndexOf(self);
        if (index < 0)
            // should not happen
            throw new TreeShapeException(self, "Node not contained in its parent");

        if (!_parent.InsertContainmentsRaw(_containment, index, [replacement])
            || !_parent.RemoveContainmentsRaw(_containment, [self]))
        {
            throw new InvalidValueException(_containment, replacement);
        }

        return index;
    }

    private Index ReplaceSingleContainment()
    {
        if (!_parent.SetContainmentRaw(_containment, replacement))
            throw new InvalidValueException(_containment, replacement);

        return 0;
    }

    private INotification? CreateContainmentNotification(Index index)
    {
        if (!NeedNotification)
            return null;

        if (_oldParent is null)
            return new ChildReplacedNotification(replacement, self, _parent, _containment, index, _notificationId);

        Debug.Assert(_oldContainment is not null);

        return _oldParent switch
        {
            not null when _oldParent == _parent && _oldContainment == _containment =>
                new ChildMovedAndReplacedInSameContainmentNotification(index, replacement, _parent, _containment, self,
                    _oldIndex, _notificationId),
            not null when _oldParent == _parent && _oldContainment != _containment =>
                new ChildMovedAndReplacedFromOtherContainmentInSameParentNotification(_containment, index, replacement,
                    _parent, _oldContainment, _oldIndex, self, _notificationId),
            not null when _oldParent != _parent =>
                new ChildMovedAndReplacedFromOtherContainmentNotification(_parent, _containment, index, replacement,
                    _oldParent, _oldContainment, _oldIndex, self, _notificationId),
            _ =>
                throw new ArgumentException()
        };
    }

    #endregion

    private void ProduceNotifications(INotification notification)
    {
        _newProducer?.ProduceNotification(notification);
        if (_oldPartition != _newPartition)
            _oldProducer?.ProduceNotification(notification);
    }

    [MemberNotNullWhen(true, nameof(_notificationId))]
    private bool NeedNotification => _notificationId is not null;
}
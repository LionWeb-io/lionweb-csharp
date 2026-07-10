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

internal class NotifyingNodeReplacer<T>(INode self, T replacement) : NodeReplacer<T>(self, replacement) where T : INode
{
    private IPartitionInstance? _newPartition;
    private IPartitionNotificationProducer? _newProducer;
    
    private INode? _oldParent;
    private Containment? _oldContainment;
    private Index _replacementIndex;
    private IPartitionInstance? _oldPartition;
    private IPartitionNotificationProducer? _oldProducer;

    private INotificationId _notificationId;

    public override T Replace()
    {
        var result = base.Replace();

        if (!NeedNotification)
            return result;

        var notification = _containment is null
            ? CreateAnnotationNotification()
            : CreateContainmentNotification();

        ProduceNotifications(notification);

        return result;
    }

    protected override void InitFields()
    {
        base.InitFields();

        _newPartition = _parent.GetPartition();
        _newProducer = _newPartition?.GetNotificationProducer();

        _oldParent = replacement.GetParent();
        if (_oldParent is not null)
        {
            _oldPartition = _oldParent.GetPartition();
            _oldProducer = _oldPartition?.GetNotificationProducer();
            _oldContainment = _oldParent.GetContainmentOf(replacement);
            _replacementIndex = RetrieveReplacementIndex();
        }

        _notificationId = _newProducer?.CreateNotificationId() ?? _oldProducer?.CreateNotificationId();
    }

    private Index RetrieveReplacementIndex()
    {
        if (_oldParent == null)
        {
            return -1;
        }

        Index oldIndex;

        switch (_oldContainment)
        {
            case null when replacement is IAnnotationInstance replacementAnnotation:
                oldIndex = _oldParent.GetAnnotationsRaw().IndexOf(replacementAnnotation);
                if (oldIndex < 0)
                    // should not happen
                    throw new TreeShapeException(self, "Node not contained in its parent");
                break;

            case { Multiple: true }:
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

            default:
                throw new ArgumentException();
        }

        return oldIndex;
    }

    private INotification CreateAnnotationNotification()
    {
        switch (_oldParent)
        {
            case null:
            case not null when _oldParent.GetPartition() is null:
                return new AnnotationReplacedNotification(replacement, self, _parent, _replacedIndex, _notificationId);
            case not null when _parent.GetPartition() is null:
                return new AnnotationDeletedNotification(replacement, _oldParent, _replacementIndex, _notificationId);
            case not null when _oldParent == _parent:
                var (newIndex, indexOffset) = MoveAndReplaceInSameList();
                return new AnnotationMovedAndReplacedInSameParentNotification(newIndex, replacement, _parent, _replacementIndex, indexOffset, self, _notificationId);
            case not null when _oldParent != _parent:
                return new AnnotationMovedAndReplacedFromOtherParentNotification(_parent, _replacedIndex, replacement, _oldParent, _replacementIndex, self, _notificationId);
            default:
                throw new ArgumentException();
        }
    }

    private INotification CreateContainmentNotification()
    {
        if (_oldParent is null || _oldParent.GetPartition() is null)
            return new ChildReplacedNotification(replacement, self, _parent, _containment, _replacedIndex, _notificationId);

        Debug.Assert(_oldContainment is not null);

        if (_parent.GetPartition() is null)
            return new ChildDeletedNotification(replacement, _oldParent, _oldContainment, _replacementIndex, _notificationId);
        
        switch (_oldParent)
        {
            case not null when _oldParent == _parent && _oldContainment == _containment:
                var (newIndex, indexOffset) = MoveAndReplaceInSameList();
                return new ChildMovedAndReplacedInSameContainmentNotification(newIndex, replacement, _parent, _containment, self, _replacementIndex, indexOffset, _notificationId);
            case not null when _oldParent == _parent && _oldContainment != _containment:
                return new ChildMovedAndReplacedFromOtherContainmentInSameParentNotification(_containment, _replacedIndex, replacement, _parent, _oldContainment, _replacementIndex, self,
                    _notificationId);
            case not null when _oldParent != _parent:
                return new ChildMovedAndReplacedFromOtherContainmentNotification(_parent, _containment, _replacedIndex, replacement, _oldParent, _oldContainment, _replacementIndex, self,
                    _notificationId);
            default:
                throw new ArgumentException();
        }
    }

    private (Index newIndex, IndexOffset indexOffset) MoveAndReplaceInSameList()
    {
        // move left
        if (_replacedIndex < _replacementIndex)
            return (_replacedIndex, _replacedIndex - _replacementIndex);
        
        // move right
        return (_replacedIndex - 1, _replacedIndex - _replacementIndex);
    }

    private void ProduceNotifications(INotification notification)
    {
        _newProducer?.ProduceNotification(notification);
        if (_oldPartition != _newPartition)
            _oldProducer?.ProduceNotification(notification);
    }

    [MemberNotNullWhen(true, nameof(_notificationId))]
    private bool NeedNotification => _notificationId is not null;
}
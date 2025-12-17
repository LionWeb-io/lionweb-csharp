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
    private Index _oldIndex;
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
            _oldIndex = RetrieveOldIndex();
        }

        _notificationId = _newProducer?.CreateNotificationId() ?? _oldProducer?.CreateNotificationId();
    }

    private Index RetrieveOldIndex()
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

    private INotification CreateAnnotationNotification() =>
        _oldParent switch
        {
            null =>
                new AnnotationReplacedNotification(replacement, self, _parent, _index, _notificationId),

            not null when _oldParent == _parent =>
                new AnnotationMovedAndReplacedInSameParentNotification(_index, replacement, _parent, _oldIndex, self,
                    _notificationId),
            not null when _oldParent != _parent =>
                new AnnotationMovedAndReplacedFromOtherParentNotification(_parent, _index, replacement, _oldParent,
                    _oldIndex, self, _notificationId),
            _ =>
                throw new ArgumentException()
        };

    private INotification CreateContainmentNotification()
    {
        if (_oldParent is null)
            return new ChildReplacedNotification(replacement, self, _parent, _containment, _index, _notificationId);

        Debug.Assert(_oldContainment is not null);

        return _oldParent switch
        {
            not null when _oldParent == _parent && _oldContainment == _containment =>
                new ChildMovedAndReplacedInSameContainmentNotification(_index, replacement, _parent, _containment, self,
                    _oldIndex, _notificationId),
            not null when _oldParent == _parent && _oldContainment != _containment =>
                new ChildMovedAndReplacedFromOtherContainmentInSameParentNotification(_containment, _index, replacement,
                    _parent, _oldContainment, _oldIndex, self, _notificationId),
            not null when _oldParent != _parent =>
                new ChildMovedAndReplacedFromOtherContainmentNotification(_parent, _containment, _index, replacement,
                    _oldParent, _oldContainment, _oldIndex, self, _notificationId),
            _ =>
                throw new ArgumentException()
        };
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
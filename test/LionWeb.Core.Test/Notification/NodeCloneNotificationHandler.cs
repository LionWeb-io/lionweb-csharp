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

namespace LionWeb.Core.Test.Listener;

using Notification;
using Notification.Forest;
using Notification.Handler;
using Notification.Partition;

internal class NodeCloneNotificationHandler<TNotification>(object? sender) : NotificationHandlerBase<TNotification>(sender)
    where TNotification : INotification
{
    public override void Receive(TNotification message)
    {
        INotification result = message switch
        {
            PartitionAddedNotification e => e with { NewPartition = Clone(e.NewPartition) },
            PartitionDeletedNotification e => e with { DeletedPartition = Clone(e.DeletedPartition) },
            AnnotationAddedNotification e => e with { NewAnnotation = Clone(e.NewAnnotation), Parent = Clone(e.Parent) },
            AnnotationDeletedNotification e => e with
            {
                DeletedAnnotation = Clone(e.DeletedAnnotation), Parent = Clone(e.Parent)
            },
            AnnotationMovedAndReplacedFromOtherParentNotification e => e with
            {
                MovedAnnotation = Clone(e.MovedAnnotation),
                ReplacedAnnotation = Clone(e.ReplacedAnnotation),
                NewParent = Clone(e.NewParent),
                OldParent = Clone(e.OldParent)
            },
            AnnotationMovedAndReplacedInSameParentNotification e => e with
            {
                MovedAnnotation = Clone(e.MovedAnnotation),
                Parent = Clone(e.Parent),
                ReplacedAnnotation = Clone(e.ReplacedAnnotation)
            },
            AnnotationMovedFromOtherParentNotification e => e with
            {
                MovedAnnotation = Clone(e.MovedAnnotation),
                NewParent = Clone(e.NewParent),
                OldParent = Clone(e.OldParent)
            },
            AnnotationMovedInSameParentNotification e => e with
            {
                MovedAnnotation = Clone(e.MovedAnnotation), Parent = Clone(e.Parent)
            },
            AnnotationReplacedNotification e => e with
            {
                NewAnnotation = Clone(e.NewAnnotation),
                Parent = Clone(e.Parent),
                ReplacedAnnotation = Clone(e.ReplacedAnnotation)
            },
            ChildAddedNotification e => e with { NewChild = Clone(e.NewChild), Parent = Clone(e.Parent) },
            ChildDeletedNotification e => e with { DeletedChild = Clone(e.DeletedChild), Parent = Clone(e.Parent) },
            ChildMovedAndReplacedFromOtherContainmentNotification e => e with
            {
                NewParent = Clone(e.NewParent),
                MovedChild = Clone(e.MovedChild),
                OldParent = Clone(e.OldParent),
                ReplacedChild = Clone(e.ReplacedChild)
            },
            ChildMovedAndReplacedFromOtherContainmentInSameParentNotification e => e with
            {
                MovedChild = Clone(e.MovedChild), Parent = Clone(e.Parent), ReplacedChild = Clone(e.ReplacedChild)
            },
            ChildMovedAndReplacedInSameContainmentNotification e => e with
            {
                MovedChild = Clone(e.MovedChild), Parent = Clone(e.Parent), ReplacedChild = Clone(e.ReplacedChild)
            },
            ChildMovedFromOtherContainmentNotification e => e with
            {
                MovedChild = Clone(e.MovedChild), NewParent = Clone(e.NewParent), OldParent = Clone(e.OldParent)
            },
            ChildMovedFromOtherContainmentInSameParentNotification e => e with
            {
                MovedChild = Clone(e.MovedChild), Parent = Clone(e.Parent)
            },
            ChildMovedInSameContainmentNotification e => e with { MovedChild = Clone(e.MovedChild), Parent = Clone(e.Parent) },
            ChildReplacedNotification e => e with { NewChild = Clone(e.NewChild) },
            ClassifierChangedNotification e => e with { Node = Clone(e.Node) },
            EntryMovedAndReplacedFromOtherReferenceNotification
            {
                MovedTarget: ReferenceTarget m, ReplacedTarget: ReferenceTarget r
            } e => e with
            {
                MovedTarget = m with { Reference = Clone(m.Reference), },
                NewParent = Clone(e.NewParent),
                OldParent = Clone(e.OldParent),
                ReplacedTarget = r with { Reference = Clone(r.Reference) }
            },
            EntryMovedAndReplacedFromOtherReferenceInSameParentNotification
            {
                MovedTarget: ReferenceTarget m, ReplacedTarget: ReferenceTarget r
            } e => e with
            {
                MovedTarget = m with { Reference = Clone(m.Reference), },
                Parent = Clone(e.Parent),
                ReplacedTarget = r with { Reference = Clone(r.Reference) }
            },
            EntryMovedAndReplacedInSameReferenceNotification
            {
                MovedTarget: ReferenceTarget m, ReplacedTarget: ReferenceTarget r
            } e => e with
            {
                MovedTarget = m with { Reference = Clone(m.Reference), },
                Parent = Clone(e.Parent),
                ReplacedTarget = r with { Reference = Clone(r.Reference) }
            },
            EntryMovedFromOtherReferenceNotification { MovedTarget: ReferenceTarget m } e => e with
            {
                MovedTarget = m with { Reference = Clone(m.Reference), },
                NewParent = Clone(e.NewParent),
                OldParent = Clone(e.OldParent)
            },
            EntryMovedFromOtherReferenceInSameParentNotification { MovedTarget: ReferenceTarget m } e => e with
            {
                MovedTarget = m with { Reference = Clone(m.Reference), }, Parent = Clone(e.Parent)
            },
            EntryMovedInSameReferenceNotification { Target: ReferenceTarget t } e => e with
            {
                Parent = Clone(e.Parent), Target = t with { Reference = Clone(t.Reference) }
            },
            PropertyAddedNotification e => e with { Node = Clone(e.Node), },
            PropertyChangedNotification e => e with { Node = Clone(e.Node) },
            PropertyDeletedNotification e => e with { Node = Clone(e.Node) },
            ReferenceAddedNotification { NewTarget: ReferenceTarget t } e => e with
            {
                NewTarget = t with { Reference = Clone(t.Reference) }, Parent = Clone(e.Parent)
            },
            ReferenceChangedNotification { NewTarget: ReferenceTarget n, OldTarget: ReferenceTarget o } e => e with
            {
                NewTarget = n with { Reference = Clone(n.Reference) },
                OldTarget = o with { Reference = Clone(n.Reference) },
                Parent = Clone(e.Parent),
            },
            ReferenceDeletedNotification { DeletedTarget: ReferenceTarget t } e => e with
            {
                DeletedTarget = t with { Reference = Clone(t.Reference) }, Parent = Clone(e.Parent)
            },
            ReferenceResolveInfoAddedNotification e => e with { Parent = Clone(e.Parent), Target = Clone(e.Target) },
            ReferenceResolveInfoChangedNotification e => e with { Parent = Clone(e.Parent), Target = Clone(e.Target) },
            ReferenceResolveInfoDeletedNotification e => e with { Parent = Clone(e.Parent), Target = Clone(e.Target) },
            ReferenceTargetAddedNotification e => e with { NewTarget = Clone(e.NewTarget), Parent = Clone(e.Parent) },
            ReferenceTargetChangedNotification e => e with
            {
                NewTarget = Clone(e.NewTarget), OldTarget = Clone(e.OldTarget), Parent = Clone(e.Parent)
            },
            ReferenceTargetDeletedNotification e => e with { DeletedTarget = Clone(e.DeletedTarget), Parent = Clone(e.Parent) }
        };
        Send((TNotification)result);
    }


    private static T Clone<T>(T node) where T : class?, IReadableNode? =>
        (T)SameIdCloner.Clone((INode)node);
}
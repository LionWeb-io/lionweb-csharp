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

using M1.Event;
using M1.Event.Forest;
using M1.Event.Partition;
using M1.Event.Processor;

internal class NodeCloneProcessor<TEvent>(object? sender) : EventProcessorBase<TEvent>(sender)
    where TEvent : IEvent
{
    public override void Receive(TEvent message)
    {
        IEvent result = message switch
        {
            PartitionAddedEvent e => e with { NewPartition = Clone(e.NewPartition) },
            PartitionDeletedEvent e => e with { DeletedPartition = Clone(e.DeletedPartition) },
            AnnotationAddedEvent e => e with { NewAnnotation = Clone(e.NewAnnotation), Parent = Clone(e.Parent) },
            AnnotationDeletedEvent e => e with
            {
                DeletedAnnotation = Clone(e.DeletedAnnotation), Parent = Clone(e.Parent)
            },
            AnnotationMovedAndReplacedFromOtherParentEvent e => e with
            {
                MovedAnnotation = Clone(e.MovedAnnotation),
                ReplacedAnnotation = Clone(e.ReplacedAnnotation),
                NewParent = Clone(e.NewParent),
                OldParent = Clone(e.OldParent)
            },
            AnnotationMovedAndReplacedInSameParentEvent e => e with
            {
                MovedAnnotation = Clone(e.MovedAnnotation),
                Parent = Clone(e.Parent),
                ReplacedAnnotation = Clone(e.ReplacedAnnotation)
            },
            AnnotationMovedFromOtherParentEvent e => e with
            {
                MovedAnnotation = Clone(e.MovedAnnotation),
                NewParent = Clone(e.NewParent),
                OldParent = Clone(e.OldParent)
            },
            AnnotationMovedInSameParentEvent e => e with
            {
                MovedAnnotation = Clone(e.MovedAnnotation), Parent = Clone(e.Parent)
            },
            AnnotationReplacedEvent e => e with
            {
                NewAnnotation = Clone(e.NewAnnotation),
                Parent = Clone(e.Parent),
                ReplacedAnnotation = Clone(e.ReplacedAnnotation)
            },
            ChildAddedEvent e => e with { NewChild = Clone(e.NewChild), Parent = Clone(e.Parent) },
            ChildDeletedEvent e => e with { DeletedChild = Clone(e.DeletedChild), Parent = Clone(e.Parent) },
            ChildMovedAndReplacedFromOtherContainmentEvent e => e with
            {
                NewParent = Clone(e.NewParent),
                MovedChild = Clone(e.MovedChild),
                OldParent = Clone(e.OldParent),
                ReplacedChild = Clone(e.ReplacedChild)
            },
            ChildMovedAndReplacedFromOtherContainmentInSameParentEvent e => e with
            {
                MovedChild = Clone(e.MovedChild), Parent = Clone(e.Parent), ReplacedChild = Clone(e.ReplacedChild)
            },
            ChildMovedAndReplacedInSameContainmentEvent e => e with
            {
                MovedChild = Clone(e.MovedChild), Parent = Clone(e.Parent), ReplacedChild = Clone(e.ReplacedChild)
            },
            ChildMovedFromOtherContainmentEvent e => e with
            {
                MovedChild = Clone(e.MovedChild), NewParent = Clone(e.NewParent), OldParent = Clone(e.OldParent)
            },
            ChildMovedFromOtherContainmentInSameParentEvent e => e with
            {
                MovedChild = Clone(e.MovedChild), Parent = Clone(e.Parent)
            },
            ChildMovedInSameContainmentEvent e => e with { MovedChild = Clone(e.MovedChild), Parent = Clone(e.Parent) },
            ChildReplacedEvent e => e with { NewChild = Clone(e.NewChild) },
            ClassifierChangedEvent e => e with { Node = Clone(e.Node) },
            EntryMovedAndReplacedFromOtherReferenceEvent
            {
                MovedTarget: ReferenceTarget m, ReplacedTarget: ReferenceTarget r
            } e => e with
            {
                MovedTarget = m with { Reference = Clone(m.Reference), },
                NewParent = Clone(e.NewParent),
                OldParent = Clone(e.OldParent),
                ReplacedTarget = r with { Reference = Clone(r.Reference) }
            },
            EntryMovedAndReplacedFromOtherReferenceInSameParentEvent
            {
                MovedTarget: ReferenceTarget m, ReplacedTarget: ReferenceTarget r
            } e => e with
            {
                MovedTarget = m with { Reference = Clone(m.Reference), },
                Parent = Clone(e.Parent),
                ReplacedTarget = r with { Reference = Clone(r.Reference) }
            },
            EntryMovedAndReplacedInSameReferenceEvent
            {
                MovedTarget: ReferenceTarget m, ReplacedTarget: ReferenceTarget r
            } e => e with
            {
                MovedTarget = m with { Reference = Clone(m.Reference), },
                Parent = Clone(e.Parent),
                ReplacedTarget = r with { Reference = Clone(r.Reference) }
            },
            EntryMovedFromOtherReferenceEvent { MovedTarget: ReferenceTarget m } e => e with
            {
                MovedTarget = m with { Reference = Clone(m.Reference), },
                NewParent = Clone(e.NewParent),
                OldParent = Clone(e.OldParent)
            },
            EntryMovedFromOtherReferenceInSameParentEvent { MovedTarget: ReferenceTarget m } e => e with
            {
                MovedTarget = m with { Reference = Clone(m.Reference), }, Parent = Clone(e.Parent)
            },
            EntryMovedInSameReferenceEvent { Target: ReferenceTarget t } e => e with
            {
                Parent = Clone(e.Parent), Target = t with { Reference = Clone(t.Reference) }
            },
            PropertyAddedEvent e => e with { Node = Clone(e.Node), },
            PropertyChangedEvent e => e with { Node = Clone(e.Node) },
            PropertyDeletedEvent e => e with { Node = Clone(e.Node) },
            ReferenceAddedEvent { NewTarget: ReferenceTarget t } e => e with
            {
                NewTarget = t with { Reference = Clone(t.Reference) }, Parent = Clone(e.Parent)
            },
            ReferenceChangedEvent { NewTarget: ReferenceTarget n, OldTarget: ReferenceTarget o } e => e with
            {
                NewTarget = n with { Reference = Clone(n.Reference) },
                OldTarget = o with { Reference = Clone(n.Reference) },
                Parent = Clone(e.Parent),
            },
            ReferenceDeletedEvent { DeletedTarget: ReferenceTarget t } e => e with
            {
                DeletedTarget = t with { Reference = Clone(t.Reference) }, Parent = Clone(e.Parent)
            },
            ReferenceResolveInfoAddedEvent e => e with { Parent = Clone(e.Parent), Target = Clone(e.Target) },
            ReferenceResolveInfoChangedEvent e => e with { Parent = Clone(e.Parent), Target = Clone(e.Target) },
            ReferenceResolveInfoDeletedEvent e => e with { Parent = Clone(e.Parent), Target = Clone(e.Target) },
            ReferenceTargetAddedEvent e => e with { NewTarget = Clone(e.NewTarget), Parent = Clone(e.Parent) },
            ReferenceTargetChangedEvent e => e with
            {
                NewTarget = Clone(e.NewTarget), OldTarget = Clone(e.OldTarget), Parent = Clone(e.Parent)
            },
            ReferenceTargetDeletedEvent e => e with { DeletedTarget = Clone(e.DeletedTarget), Parent = Clone(e.Parent) }
        };
        Send((TEvent)result);
    }


    private static T Clone<T>(T node) where T : class?, IReadableNode? =>
        (T)SameIdCloner.Clone((INode)node);
}
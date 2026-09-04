// Copyright 2026 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Test.Notification;

using Core.Notification;
using Core.Notification.Forest;
using Core.Notification.Partition;
using Languages.Generated.V2024_1.TestLanguage;
using M3;

public class AllNotificationTestsBase : NotificationTestsBase
{
    private static int _nextNodeId;
    private static int _nextNotificationId;

    protected AllNotificationTestsBase()
    {
        _nextNodeId = 0;
        _nextNotificationId = 0;
    }

    protected static IEnumerable<object[]> CollectAllNotifications() =>
        CollectForestNotifications()
            .Concat(CollectPartitionNotifications())
            .Concat(CollectMiscellaneousNotifications());

    #region Forest

    protected static IEnumerable<object[]> CollectForestNotifications() =>
    [
        [CreatePartitionAddedNotification()],
        [CreatePartitionDeletedNotification()],
    ];

    protected static PartitionAddedNotification CreatePartitionAddedNotification() =>
        new(Partition(), NotificationId());

    protected static PartitionDeletedNotification CreatePartitionDeletedNotification() =>
        new(Partition(), NotificationId());

    protected static IPartitionInstance Partition() =>
        new TestPartition(NodeId())
        {
            Name = "MyPartition",
            Data = new DataTypeTestConcept(NodeId()) { IntegerValue_1 = 42 },
            Links =
            [
                LinkNode()
            ]
        };

    #endregion

    #region Partition

    protected static IEnumerable<object[]> CollectPartitionNotifications() =>
        CollectAnnotationNotifications()
            .Concat(CollectChildrenNotifications())
            .Concat(CollectPropertyNotifications())
            .Concat(CollectReferenceNotifications());

    #region Annotation

    protected static IEnumerable<object[]> CollectAnnotationNotifications() =>
    [
        [CreateAnnotationAddedNotification()],
        [CreateAnnotationDeletedNotification()],
        [CreateAnnotationMovedAndReplacedFromOtherParentNotification()],
        [CreateAnnotationMovedAndReplacedInSameParentNotification()],
        [CreateAnnotationMovedFromOtherParentNotification()],
        [CreateAnnotationMovedInSameParentNotification()],
        [CreateAnnotationReplacedNotification()],
    ];

    protected static AnnotationAddedNotification CreateAnnotationAddedNotification() =>
        new(LinkNode(), Annotation(), 0, NotificationId());

    protected static AnnotationDeletedNotification CreateAnnotationDeletedNotification() =>
        new(Annotation(), LinkNode(), 0, NotificationId());

    protected static AnnotationMovedAndReplacedFromOtherParentNotification
        CreateAnnotationMovedAndReplacedFromOtherParentNotification() =>
        new(LinkNode(), 0, Annotation(), LinkNode(), 1, Annotation(), NotificationId());

    protected static AnnotationMovedAndReplacedInSameParentNotification
        CreateAnnotationMovedAndReplacedInSameParentNotification() =>
        new(1, Annotation(), LinkNode(), 0, 1, Annotation(), NotificationId());

    protected static AnnotationMovedFromOtherParentNotification CreateAnnotationMovedFromOtherParentNotification() =>
        new(LinkNode(), 1, Annotation(), LinkNode(), 0, NotificationId());

    protected static AnnotationMovedInSameParentNotification CreateAnnotationMovedInSameParentNotification() =>
        new(1, Annotation(), LinkNode(), 0, 1, NotificationId());

    protected static AnnotationReplacedNotification CreateAnnotationReplacedNotification() =>
        new(Annotation(), Annotation(), LinkNode(), 0, NotificationId());

    protected static IWritableAnnotationInstance Annotation()
    {
        var linkNode = LinkNode();
        return new TestAnnotation(NodeId()) { Name = "MyAnnotation", Containment = linkNode, Ref = linkNode };
    }

    #endregion

    #region Children

    protected static IEnumerable<object[]> CollectChildrenNotifications() =>
    [
        [CreateChildAddedNotification()],
        [CreateChildDeletedNotification()],
        [CreateChildMovedAndReplacedFromOtherContainmentInSameParentNotification()],
        [CreateChildMovedAndReplacedFromOtherContainmentNotification()],
        [CreateChildMovedAndReplacedInSameContainmentNotification()],
        [CreateChildMovedFromOtherContainmentInSameParentNotification()],
        [CreateChildMovedFromContainmentInOtherParentNotification()],
        [CreateChildMovedInSameContainmentNotification()],
        [CreateChildReplacedNotification()],
    ];

    protected static ChildAddedNotification CreateChildAddedNotification() =>
        new(LinkNode(), LinkNode(), Containment(), 0, NotificationId());

    protected static ChildDeletedNotification CreateChildDeletedNotification() =>
        new(LinkNode(), LinkNode(), Containment(), 0, NotificationId());

    protected static ChildMovedAndReplacedFromOtherContainmentInSameParentNotification
        CreateChildMovedAndReplacedFromOtherContainmentInSameParentNotification() =>
        new(Containment(), 1, LinkNode(), LinkNode(), OtherContainment(), 0, LinkNode(), NotificationId());

    protected static ChildMovedAndReplacedFromOtherContainmentNotification
        CreateChildMovedAndReplacedFromOtherContainmentNotification() =>
        new(LinkNode(), Containment(), 1, LinkNode(), LinkNode(), OtherContainment(), 0, LinkNode(), NotificationId());

    protected static ChildMovedAndReplacedInSameContainmentNotification
        CreateChildMovedAndReplacedInSameContainmentNotification() =>
        new(1, LinkNode(), LinkNode(), Containment(), LinkNode(), 0, 1, NotificationId());

    protected static ChildMovedFromOtherContainmentInSameParentNotification
        CreateChildMovedFromOtherContainmentInSameParentNotification() =>
        new(Containment(), 1, LinkNode(), LinkNode(), OtherContainment(), 0, NotificationId());

    protected static ChildMovedFromContainmentInOtherParentNotification CreateChildMovedFromContainmentInOtherParentNotification() =>
        new(LinkNode(), Containment(), 1, LinkNode(), LinkNode(), OtherContainment(), 0, NotificationId());

    protected static ChildMovedInSameContainmentNotification CreateChildMovedInSameContainmentNotification() =>
        new(1, LinkNode(), LinkNode(), Containment(), 0, 1, NotificationId());

    protected static ChildReplacedNotification CreateChildReplacedNotification() =>
        new(LinkNode(), LinkNode(), LinkNode(), Containment(), 0, NotificationId());

    protected static Containment Containment() =>
        TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n;

    protected static Containment OtherContainment() =>
        TestLanguageLanguage.Instance.LinkTestConcept_containment_0_1;

    #endregion

    #region Property

    protected static IEnumerable<object[]> CollectPropertyNotifications() =>
    [
        [CreatePropertyAddedNotification()],
        [CreatePropertyChangedNotification()],
        [CreatePropertyDeletedNotification()],
    ];

    protected static PropertyAddedNotification CreatePropertyAddedNotification() =>
        new(DataNode(), Property(), "newValue", NotificationId());

    protected static PropertyChangedNotification CreatePropertyChangedNotification() =>
        new(DataNode(), Property(), "newValue", "oldValue", NotificationId());

    protected static PropertyDeletedNotification CreatePropertyDeletedNotification() =>
        new(DataNode(), Property(), "oldValue", NotificationId());

    protected static IWritableNode DataNode() =>
        new DataTypeTestConcept(NodeId());

    protected static Property Property() =>
        TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1;

    #endregion

    #region Reference

    protected static IEnumerable<object[]> CollectReferenceNotifications() =>
    [
        [CreateReferenceAddedNotification()],
        [CreateReferenceChangedNotification()],
        [CreateReferenceDeletedNotification()],
    ];

    protected static ReferenceAddedNotification CreateReferenceAddedNotification() =>
        new(LinkNode(), Reference(), 0, ReferenceTarget(), NotificationId());

    protected static ReferenceChangedNotification CreateReferenceChangedNotification() =>
        new(LinkNode(), Reference(), 0, ReferenceTarget(), ReferenceTarget(), NotificationId());

    protected static ReferenceDeletedNotification CreateReferenceDeletedNotification() =>
        new(LinkNode(), Reference(), 0, ReferenceTarget(), NotificationId());

    protected static Reference Reference() =>
        TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n;

    protected static IReferenceTarget ReferenceTarget() =>
        new ReferenceTarget("myResolveInfo", NodeId(), LinkNode());

    #endregion

    #region Miscellaneous

    protected static IEnumerable<object[]> CollectMiscellaneousNotifications() =>
    [
        [CreateCompositeNotification()],
    ];

    protected static CompositeNotification CreateCompositeNotification(bool includeComposite = true)
    {
        List<INotification> parts =
        [
            CreatePartitionAddedNotification(),
            CreateAnnotationMovedFromOtherParentNotification(),
            CreateChildReplacedNotification(),
            CreatePropertyChangedNotification(),
            CreateReferenceDeletedNotification()
        ];
        if (includeComposite)
            parts.Add(CreateCompositeNotification(false));

        return new(parts, NotificationId());
    }

    #endregion

    #endregion

    protected static LinkTestConcept LinkNode()
    {
        var contained = new LinkTestConcept(NodeId()) { Name = "contained" };
        var link = new LinkTestConcept(NodeId()) { Containment_0_1 = contained, Reference_0_n = [contained] };
        return link;
    }

    protected static NodeId NodeId() =>
        $"nodeId_{++_nextNodeId}";

    protected static INotificationId NotificationId() =>
        new NumericNotificationId("base", ++_nextNotificationId);
}
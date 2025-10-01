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

namespace LionWeb.Core.Test.Notification;

using Core.Notification;
using Core.Notification.Partition;
using Languages.Generated.V2025_1.Shapes.M2;
using Languages.Generated.V2025_1.TestLanguage;

[TestClass]
public class ReplicatorTests_Reference : ReplicatorTestsBase
{
    #region References

    #region ReferenceAdded

    [TestMethod]
    public void ReferenceAdded_Multiple_Only()
    {
        var bof = new BillOfMaterials("bof");
        var line = new Line("line");
        var originalPartition = new Geometry("a") { Shapes = [line] };
        originalPartition.AddAnnotations([bof]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        bof.AddMaterials([line]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ReferenceAdded_Multiple_First()
    {
        var circle = new Circle("circle");
        var bof = new BillOfMaterials("bof") { Materials = [circle] };
        var line = new Line("line");
        var originalPartition = new Geometry("a") { Shapes = [line, circle] };
        originalPartition.AddAnnotations([bof]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        bof.InsertMaterials(0, [line]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ReferenceAdded_Multiple_Last()
    {
        var circle = new Circle("circle");
        var bof = new BillOfMaterials("bof") { Materials = [circle] };
        var line = new Line("line");
        var originalPartition = new Geometry("a") { Shapes = [line, circle] };
        originalPartition.AddAnnotations([bof]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        bof.InsertMaterials(1, [line]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ReferenceAdded_Single()
    {
        var circle = new Circle("circle");
        var od = new OffsetDuplicate("od");
        var originalPartition = new Geometry("a") { Shapes = [od, circle] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        od.Source = circle;

        AssertEquals([originalPartition], [clonedPartition]);

        var clonedOffsetDuplicate = (OffsetDuplicate)clonedPartition.Shapes[0];
        var clonedCircle = (Circle)clonedPartition.Shapes[1];
        Assert.AreSame(clonedCircle, clonedOffsetDuplicate.Source);
    }

    #endregion

    #region ReferenceDeleted

    [TestMethod]
    public void ReferenceDeleted_Multiple_Only()
    {
        var line = new Line("line");
        var bof = new BillOfMaterials("bof") { Materials = [line] };
        var originalPartition = new Geometry("a") { Shapes = [line] };
        originalPartition.AddAnnotations([bof]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        bof.RemoveMaterials([line]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ReferenceDeleted_Multiple_First()
    {
        var circle = new Circle("circle");
        var line = new Line("line");
        var bof = new BillOfMaterials("bof") { Materials = [line, circle] };
        var originalPartition = new Geometry("a") { Shapes = [line, circle] };
        originalPartition.AddAnnotations([bof]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        bof.RemoveMaterials([line]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ReferenceDeleted_Multiple_Last()
    {
        var circle = new Circle("circle");
        var line = new Line("line");
        var bof = new BillOfMaterials("bof") { Materials = [circle, line] };
        var originalPartition = new Geometry("a") { Shapes = [line, circle] };
        originalPartition.AddAnnotations([bof]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        bof.RemoveMaterials([line]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void ReferenceDeleted_Single()
    {
        var circle = new Circle("circle");
        var od = new OffsetDuplicate("od") { AltSource = circle };
        var originalPartition = new Geometry("a") { Shapes = [od, circle] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        od.AltSource = null;

        AssertEquals([originalPartition], [clonedPartition]);
    }

    #endregion

    #region ReferenceChanged

    [TestMethod]
    public void ReferenceChanged_Single()
    {
        var circle = new Circle("circle");
        var line = new Line("line");
        var od = new OffsetDuplicate("od") { AltSource = circle };
        var originalPartition = new Geometry("a") { Shapes = [od, circle, line] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        od.AltSource = line;

        AssertEquals([originalPartition], [clonedPartition]);
    }
    
    #endregion

    #region ReferenceTarget

    [TestMethod]
    public void ReferenceAdded_referencetarget_refers_to_cloned_node()
    {
        var circle = new Circle("circle");
        var od = new OffsetDuplicate("od");
        var originalPartition = new Geometry("a") { Shapes = [od, circle] };

        var clonedPartition = ClonePartition(originalPartition);
        var sharedNodeMap = new SharedNodeMap();
        sharedNodeMap.RegisterNode(clonedPartition);

        var notificationMapper = new NotificationToNotificationMapper(sharedNodeMap);

        var referenceAddedNotification = new ReferenceAddedNotification(originalPartition,
            ShapesLanguage.Instance.OffsetDuplicate_source, 0,
            new ReferenceTarget(null, circle), new NumericNotificationId("refAddedNotification", 0));

        var notification = notificationMapper.Map(referenceAddedNotification);

        Assert.AreNotSame(circle, ((ReferenceAddedNotification)notification).NewTarget.Reference);
    }

    [TestMethod]
    public void ReferenceChanged_referencetarget_refers_to_cloned_node()
    {
        var circle = new Circle("circle");
        var line = new Line("line");
        var od = new OffsetDuplicate("od") { AltSource = circle };
        var originalPartition = new Geometry("a") { Shapes = [od, circle, line] };

        var clonedPartition = ClonePartition(originalPartition);
        var sharedNodeMap = new SharedNodeMap();
        sharedNodeMap.RegisterNode(clonedPartition);

        var notificationMapper = new NotificationToNotificationMapper(sharedNodeMap);
        var referenceChangedNotification = new ReferenceChangedNotification(originalPartition, ShapesLanguage.Instance.OffsetDuplicate_altSource, 0,
            new ReferenceTarget(null, line), new ReferenceTarget(null, circle), new NumericNotificationId("refChangedNotification", 0));

        var notification = notificationMapper.Map(referenceChangedNotification);

        Assert.AreNotSame(line, ((ReferenceChangedNotification)notification).NewTarget.Reference);
        Assert.AreNotSame(circle, ((ReferenceChangedNotification)notification).OldTarget.Reference);
    }

    [TestMethod]
    public void ReferenceDeleted_referencetarget_refers_to_cloned_node()
    {
        var circle = new Circle("circle");
        var od = new OffsetDuplicate("od") { AltSource = circle };
        var originalPartition = new Geometry("a") { Shapes = [od, circle] };

        var clonedPartition = ClonePartition(originalPartition);
        var sharedNodeMap = new SharedNodeMap();
        sharedNodeMap.RegisterNode(clonedPartition);

        var notificationMapper = new NotificationToNotificationMapper(sharedNodeMap);
        var referenceDeletedNotification = new ReferenceDeletedNotification(originalPartition, ShapesLanguage.Instance.OffsetDuplicate_altSource, 0,
            new ReferenceTarget(null, circle),
            new NumericNotificationId("refChangedNotification", 0));

        var notification = notificationMapper.Map(referenceDeletedNotification);

        Assert.AreNotSame(circle, ((ReferenceDeletedNotification)notification).DeletedTarget.Reference);
    }

    #endregion

    #region MoveEntryInSameReference

    [TestMethod]
    public void MoveEntryInSameReference_Backward()
    {
        var ref1 = new LinkTestConcept("ref1");
        var ref2 = new LinkTestConcept("ref2");
        var moved = new LinkTestConcept("moved");
        
        var originalPartition = new LinkTestConcept("concept")
        {
            Reference_0_n = [ref1, ref2, moved]
        };
        
        var clonedPartition = ClonePartition(originalPartition);

        var oldIndex = 2;
        var newIndex = 0;
        var notification = new EntryMovedInSameReferenceNotification(originalPartition, TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n,
            oldIndex, newIndex, new ReferenceTarget {Reference = moved}, new NumericNotificationId("moveEntryInSameReference", 0));
        
        var sharedNodeMap = new SharedNodeMap();
        RegisterReferenceTargets(originalPartition.Reference_0_n, sharedNodeMap);
        
        CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);
        
        Assert.AreEqual(3, clonedPartition.Reference_0_n.Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Reference_0_n[0].GetId());
    }

    [TestMethod]
    public void MoveEntryInSameReference_Forward()
    {
        var ref1 = new LinkTestConcept("ref1");
        var ref2 = new LinkTestConcept("ref2");
        var moved = new LinkTestConcept("moved");
        
        var originalPartition = new LinkTestConcept("concept")
        {
            Reference_0_n = [moved, ref1, ref2]
        };
        
        var clonedPartition = ClonePartition(originalPartition);

        var oldIndex = 0;
        var newIndex = 2;
        var notification = new EntryMovedInSameReferenceNotification(originalPartition, TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n,
            oldIndex, newIndex, new ReferenceTarget {Reference = moved}, new NumericNotificationId("moveEntryInSameReference", 0));
        
        var sharedNodeMap = new SharedNodeMap();
        
        RegisterReferenceTargets(originalPartition.Reference_0_n, sharedNodeMap);
        
        CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);
        
        Assert.AreEqual(3, clonedPartition.Reference_0_n.Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Reference_0_n[^1].GetId());
    }

    [TestMethod]
    public void MoveEntryInSameReference_Forward_MoreThanThreeNodes()
    {
        var ref1 = new LinkTestConcept("ref1");
        var ref2 = new LinkTestConcept("ref2");
        var ref3 = new LinkTestConcept("ref3");
        var ref4 = new LinkTestConcept("ref4");
        var moved = new LinkTestConcept("moved");
        
        var originalPartition = new LinkTestConcept("concept")
        {
            Reference_0_n = [ref1, moved, ref2, ref3, ref4]
        };
        
        var clonedPartition = ClonePartition(originalPartition);

        var oldIndex = 1;
        var newIndex = 3;
        var notification = new EntryMovedInSameReferenceNotification(originalPartition, TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n,
            oldIndex, newIndex, new ReferenceTarget {Reference = moved}, new NumericNotificationId("moveEntryInSameReference", 0));
        
        var sharedNodeMap = new SharedNodeMap();
        
        RegisterReferenceTargets(originalPartition.Reference_0_n, sharedNodeMap);
        
        CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);
        
        Assert.AreEqual(5, clonedPartition.Reference_0_n.Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Reference_0_n[3].GetId());
    }

    [TestMethod]
    public void MoveEntryInSameReference_Backward_MoreThanThreeNodes()
    {
        var ref1 = new LinkTestConcept("ref1");
        var ref2 = new LinkTestConcept("ref2");
        var ref3 = new LinkTestConcept("ref3");
        var ref4 = new LinkTestConcept("ref4");
        var moved = new LinkTestConcept("moved");
        
        var originalPartition = new LinkTestConcept("concept")
        {
            Reference_0_n = [ref1, ref2, ref3, moved, ref4]
        };
        
        var clonedPartition = ClonePartition(originalPartition);

        var oldIndex = 3;
        var newIndex = 1;
        var notification = new EntryMovedInSameReferenceNotification(originalPartition, TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n,
            oldIndex, newIndex, new ReferenceTarget {Reference = moved}, new NumericNotificationId("moveEntryInSameReference", 0));
        
        var sharedNodeMap = new SharedNodeMap();
        
        RegisterReferenceTargets(originalPartition.Reference_0_n, sharedNodeMap);
        
        CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);
        
        Assert.AreEqual(5, clonedPartition.Reference_0_n.Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Reference_0_n[1].GetId());
    }

    #endregion

    #region MoveEntryFromOtherReferenceInSameParent

    [TestMethod]
    public void MoveEntryFromOtherReferenceInSameParent()
    {
        var ref1 = new LinkTestConcept("ref1");
        var ref2 = new LinkTestConcept("ref2");
        var ref3 = new LinkTestConcept("ref3");
        var ref4 = new LinkTestConcept("ref4");
        var moved = new LinkTestConcept("moved");

        var originalPartition = new LinkTestConcept("concept")
        {
            Reference_0_n = [ref2, ref3, ref4], 
            Reference_1_n = [moved, ref1]
        };

        var clonedPartition = ClonePartition(originalPartition);

        var oldIndex = 0;
        var newIndex = 0;
        var notification = new EntryMovedFromOtherReferenceInSameParentNotification(originalPartition,
            TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, newIndex, new ReferenceTarget { Reference = moved },
            TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, oldIndex,
            new NumericNotificationId("EntryMovedFromOtherReferenceInSameParent", 0));

        var sharedNodeMap = new SharedNodeMap();

        RegisterReferenceTargets(originalPartition.Reference_0_n, sharedNodeMap);
        RegisterReferenceTargets(originalPartition.Reference_1_n, sharedNodeMap);

        CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);

        Assert.AreEqual(4, clonedPartition.Reference_0_n.Count);
        Assert.AreEqual(1, clonedPartition.Reference_1_n.Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.Reference_0_n[0].GetId());
    }

    
    [TestMethod]
    public void MoveEntryFromOtherReferenceInSameParent_singular_to_multiple_reference_first()
    {
        var ref1 = new LinkTestConcept("ref1");
        var ref2 = new LinkTestConcept("ref2");
        var ref3 = new LinkTestConcept("ref3");
        var ref4 = new LinkTestConcept("ref4");
        var moved = new LinkTestConcept("moved");

        var originalPartition = new LinkTestConcept("concept")
        {
            Reference_0_n = [ref1, ref2, ref3, ref4], 
            Reference_0_1 = moved
        };

        var clonedPartition = ClonePartition(originalPartition);

        var oldIndex = 0;
        var newIndex = 0;
        var notification = new EntryMovedFromOtherReferenceInSameParentNotification(originalPartition,
            TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, newIndex, new ReferenceTarget { Reference = moved },
            TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1, oldIndex,
            new NumericNotificationId("EntryMovedFromOtherReferenceInSameParent", 0));

        var sharedNodeMap = new SharedNodeMap();

        RegisterReferenceTargets(originalPartition.Reference_0_n, sharedNodeMap);
        RegisterReferenceTargets([originalPartition.Reference_0_1], sharedNodeMap);

        CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);

        Assert.AreEqual(5, clonedPartition.Reference_0_n.Count);
        Assert.IsNull(clonedPartition.Reference_0_1);
        Assert.AreEqual(moved.GetId(), clonedPartition.Reference_0_n[0].GetId());
    }

    [TestMethod]
    public void MoveEntryFromOtherReferenceInSameParent_singular_to_multiple_reference_last()
    {
        var ref1 = new LinkTestConcept("ref1");
        var ref2 = new LinkTestConcept("ref2");
        var ref3 = new LinkTestConcept("ref3");
        var ref4 = new LinkTestConcept("ref4");
        var moved = new LinkTestConcept("moved");

        var originalPartition = new LinkTestConcept("concept")
        {
            Reference_0_n = [ref1, ref2, ref3, ref4], 
            Reference_0_1 = moved
        };

        var clonedPartition = ClonePartition(originalPartition);

        var oldIndex = 0;
        var newIndex = 4;
        var notification = new EntryMovedFromOtherReferenceInSameParentNotification(originalPartition,
            TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, newIndex, new ReferenceTarget { Reference = moved },
            TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1, oldIndex,
            new NumericNotificationId("EntryMovedFromOtherReferenceInSameParent", 0));

        var sharedNodeMap = new SharedNodeMap();

        RegisterReferenceTargets(originalPartition.Reference_0_n, sharedNodeMap);
        RegisterReferenceTargets([originalPartition.Reference_0_1], sharedNodeMap);

        CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);

        Assert.AreEqual(5, clonedPartition.Reference_0_n.Count);
        Assert.IsNull(clonedPartition.Reference_0_1);
        Assert.AreEqual(moved.GetId(), clonedPartition.Reference_0_n[^1].GetId());
    }

    [TestMethod]
    public void MoveEntryFromOtherReferenceInSameParent_singular_to_multiple_reference_middle()
    {
        var ref1 = new LinkTestConcept("ref1");
        var ref2 = new LinkTestConcept("ref2");
        var ref3 = new LinkTestConcept("ref3");
        var ref4 = new LinkTestConcept("ref4");
        var moved = new LinkTestConcept("moved");

        var originalPartition = new LinkTestConcept("concept")
        {
            Reference_0_n = [ref1, ref2, ref3, ref4], 
            Reference_0_1 = moved
        };

        var clonedPartition = ClonePartition(originalPartition);

        var oldIndex = 0;
        var newIndex = 2;
        var notification = new EntryMovedFromOtherReferenceInSameParentNotification(originalPartition,
            TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, newIndex, new ReferenceTarget { Reference = moved },
            TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1, oldIndex,
            new NumericNotificationId("EntryMovedFromOtherReferenceInSameParent", 0));

        var sharedNodeMap = new SharedNodeMap();

        RegisterReferenceTargets(originalPartition.Reference_0_n, sharedNodeMap);
        RegisterReferenceTargets([originalPartition.Reference_0_1], sharedNodeMap);

        CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);

        Assert.AreEqual(5, clonedPartition.Reference_0_n.Count);
        Assert.IsNull(clonedPartition.Reference_0_1);
        Assert.AreEqual(moved.GetId(), clonedPartition.Reference_0_n[2].GetId());
    }

    
    [TestMethod]
    public void MoveEntryFromOtherReferenceInSameParent_multiple_to_singular_reference_first()
    {
        var ref1 = new LinkTestConcept("ref1");
        var ref2 = new LinkTestConcept("ref2");
        var ref3 = new LinkTestConcept("ref3");
        var ref4 = new LinkTestConcept("ref4");
        var moved = new LinkTestConcept("moved");

        var originalPartition = new LinkTestConcept("concept")
        {
            Reference_0_n = [moved, ref1, ref2, ref3, ref4], 
        };

        var clonedPartition = ClonePartition(originalPartition);

        var oldIndex = 0;
        var newIndex = 0;
        var notification = new EntryMovedFromOtherReferenceInSameParentNotification(originalPartition,
            TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1, newIndex, new ReferenceTarget { Reference = moved },
            TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, oldIndex,
            new NumericNotificationId("EntryMovedFromOtherReferenceInSameParent", 0));

        var sharedNodeMap = new SharedNodeMap();

        RegisterReferenceTargets(originalPartition.Reference_0_n, sharedNodeMap);

        CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);

        Assert.AreEqual(4, clonedPartition.Reference_0_n.Count);
        Assert.AreEqual(moved, clonedPartition.Reference_0_1);
        Assert.AreEqual(ref1.GetId(), clonedPartition.Reference_0_n[0].GetId());
    }

    [TestMethod]
    public void MoveEntryFromOtherReferenceInSameParent_multiple_to_singular_reference_last()
    {
        var ref1 = new LinkTestConcept("ref1");
        var ref2 = new LinkTestConcept("ref2");
        var ref3 = new LinkTestConcept("ref3");
        var ref4 = new LinkTestConcept("ref4");
        var moved = new LinkTestConcept("moved");

        var originalPartition = new LinkTestConcept("concept")
        {
            Reference_0_n = [ref1, ref2, ref3, ref4, moved], 
        };

        var clonedPartition = ClonePartition(originalPartition);

        var oldIndex = 4;
        var newIndex = 0;
        var notification = new EntryMovedFromOtherReferenceInSameParentNotification(originalPartition,
            TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1, newIndex, new ReferenceTarget { Reference = moved },
            TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, oldIndex,
            new NumericNotificationId("EntryMovedFromOtherReferenceInSameParent", 0));

        var sharedNodeMap = new SharedNodeMap();

        RegisterReferenceTargets(originalPartition.Reference_0_n, sharedNodeMap);

        CreatePartitionReplicator(clonedPartition, notification, sharedNodeMap);

        Assert.AreEqual(4, clonedPartition.Reference_0_n.Count);
        Assert.AreEqual(moved, clonedPartition.Reference_0_1);
        Assert.AreEqual(ref1.GetId(), clonedPartition.Reference_0_n[0].GetId());
    }

    private static void RegisterReferenceTargets<T>(IEnumerable<T> references, SharedNodeMap sharedNodeMap) where T : LinkTestConcept
    {
        foreach (var reference in references)
        {
            sharedNodeMap.RegisterNode(reference);
        }
    }

    #endregion

    #endregion
}
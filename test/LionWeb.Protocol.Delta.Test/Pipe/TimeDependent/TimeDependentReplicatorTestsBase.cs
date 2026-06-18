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

namespace LionWeb.Protocol.Delta.Test.Pipe.TimeDependent;

using Core;
using Core.M1;
using Core.M3;
using Core.Notification.Forest;
using Core.Notification.Partition;
using Core.Notification.Pipe;
using Core.Test.Languages.Generated.V2024_1.TestLanguage;
using Core.Test.Notification;

public abstract class TimeDependentReplicatorTestsBase  : ReplicatorTestsBase
{
    protected readonly Forest _originalForest;
    protected readonly Forest _clonedForest;
    protected readonly TestPartition _originalPartition;
    protected readonly TestPartition _clonedPartition;
    protected readonly NotificationCompositor _compositor;
    protected readonly List<Language> _languages = [TestLanguageLanguage.Instance];
    protected readonly LionWebVersions _lionWebVersions = LionWebVersions.v2024_1;

    protected TimeDependentReplicatorTestsBase()
    {
        _originalForest = new Forest();
        _originalPartition = new TestPartition("a");
        _originalForest.AddPartitions([_originalPartition]);

        _clonedForest = new Forest();
        _clonedPartition = ClonePartition(_originalPartition);
        _clonedForest.AddPartitions([_clonedPartition]);

        _compositor = new NotificationCompositor(null);
    }

    protected abstract void Connect();

    #region Annotation

    #region New

    [TestMethod]
    public void AnnotationAdded()
    {
        var parent = new LinkTestConcept("parent");
        var grandChild = new LinkTestConcept("grandChild");
        var annotation = new TestAnnotation("annotation") { Containment = grandChild };
        var newGrandChild = new LinkTestConcept("newGrandChild");

        Connect();
        _originalPartition.AddLinks([parent]);

        _compositor.Push();
        parent.AddAnnotations([annotation]);
        grandChild.ReplaceWith(newGrandChild);
        _compositor.Pop(true);

        AssertPartitionsEqual();
    }

    [TestMethod]
    public void AnnotationReplaced_New()
    {
        var parent = new LinkTestConcept("parent");
        var grandChild = new LinkTestConcept("grandChild");
        var firstAnnotation = new TestAnnotation("firstAnnotation");
        var annotation = new TestAnnotation("annotation") { Containment = grandChild };
        var newGrandChild = new LinkTestConcept("newGrandChild");

        Connect();
        _originalPartition.AddLinks([parent]);
        parent.AddAnnotations([firstAnnotation]);

        _compositor.Push();
        firstAnnotation.ReplaceWith(annotation);
        grandChild.ReplaceWith(newGrandChild);
        _compositor.Pop(true);

        AssertPartitionsEqual();
    }
    
    #endregion

    #region Deleted

    [TestMethod]
    public void AnnotationDeleted()
    {
        var grandChild = new LinkTestConcept("grandChild");
        var annotation = new TestAnnotation("annotation") { Containment = grandChild };
        var parent = new LinkTestConcept("parent").WithAnnotation(annotation);

        Connect();
        _compositor.Push();
        _originalPartition.AddLinks([parent]);
        annotation.DetachFromParent();
        annotation.Containment = null;
        var composite = _compositor.Pop(true);
        
        Assert.IsInstanceOfType<AnnotationDeletedNotification>(composite.Parts[^1]);

        var deleted = (AnnotationDeletedNotification)composite.Parts[^1];
        CollectionAssert.AreEqual(new List<IReadableNode>{annotation, grandChild}, deleted.DeletedNodes.ToList());

        AssertPartitionsEqual();
    }

    [TestMethod]
    public void AnnotationMovedAndReplacedFromOtherParent()
    {
        var grandChild = new LinkTestConcept("grandChild");
        var annotation = new TestAnnotation("annotation") { Containment = grandChild };
        var parent = new LinkTestConcept("parent").WithAnnotation(annotation);
        var otherAnnotation = new TestAnnotation("otherAnnotation");
        var otherParent = new LinkTestConcept("otherParent").WithAnnotation(otherAnnotation);

        Connect();
        _compositor.Push();
        _originalPartition.AddLinks([parent, otherParent]);
        annotation.ReplaceWith(otherAnnotation);
        annotation.Containment = null;
        var composite = _compositor.Pop(true);
        
        Assert.IsInstanceOfType<AnnotationMovedAndReplacedFromOtherParentNotification>(composite.Parts[^1]);

        var deleted = (AnnotationMovedAndReplacedFromOtherParentNotification)composite.Parts[^1];
        CollectionAssert.AreEqual(new List<IReadableNode>{annotation, grandChild}, deleted.DeletedNodes.ToList());
        Assert.AreSame(otherParent, deleted.OldParent);

        AssertPartitionsEqual();
    }

    [TestMethod]
    public void AnnotationMovedAndReplacedInSameParent()
    {
        var grandChild = new LinkTestConcept("grandChild");
        var annotation = new TestAnnotation("annotation") { Containment = grandChild };
        var otherAnnotation = new TestAnnotation("otherAnnotation");
        var parent = new LinkTestConcept("parent").WithAnnotation(annotation).WithAnnotation(otherAnnotation);

        Connect();
        _compositor.Push();
        _originalPartition.AddLinks([parent]);
        annotation.ReplaceWith(otherAnnotation);
        annotation.Containment = null;
        var composite = _compositor.Pop(true);
        
        Assert.IsInstanceOfType<AnnotationMovedAndReplacedInSameParentNotification>(composite.Parts[^1]);

        var deleted = (AnnotationMovedAndReplacedInSameParentNotification)composite.Parts[^1];
        CollectionAssert.AreEqual(new List<IReadableNode>{annotation, grandChild}, deleted.DeletedNodes.ToList());

        AssertPartitionsEqual();
    }

    [TestMethod]
    public void AnnotationReplaced_Deleted()
    {
        var grandChild = new LinkTestConcept("grandChild");
        var annotation = new TestAnnotation("annotation") { Containment = grandChild };
        var otherAnnotation = new TestAnnotation("otherAnnotation");
        var parent = new LinkTestConcept("parent").WithAnnotation(annotation);

        Connect();
        _compositor.Push();
        _originalPartition.AddLinks([parent]);
        annotation.ReplaceWith(otherAnnotation);
        annotation.Containment = null;
        var composite = _compositor.Pop(true);
        
        Assert.IsInstanceOfType<AnnotationReplacedNotification>(composite.Parts[^1]);

        var deleted = (AnnotationReplacedNotification)composite.Parts[^1];
        CollectionAssert.AreEqual(new List<IReadableNode>{annotation, grandChild}, deleted.DeletedNodes.ToList());

        AssertPartitionsEqual();
    }
    
    #endregion

    #endregion

    #region Child

    #region New

    [TestMethod]
    public void ChildAdded()
    {
        var parent = new LinkTestConcept("parent");
        var grandChild = new LinkTestConcept("grandChild");
        var child = new LinkTestConcept("child") { Containment_1 = grandChild };
        var newGrandChild = new LinkTestConcept("newGrandChild");

        Connect();
        _originalPartition.AddLinks([parent]);

        _compositor.Push();
        parent.Containment_0_1 = child;
        grandChild.ReplaceWith(newGrandChild);
        _compositor.Pop(true);

        AssertPartitionsEqual();
    }

    [TestMethod]
    public void ChildReplaced_New()
    {
        var parent = new LinkTestConcept("parent");
        var grandChild = new LinkTestConcept("grandChild");
        var firstChild = new LinkTestConcept("firstChild");
        var child = new LinkTestConcept("child") { Containment_1 = grandChild };
        var newGrandChild = new LinkTestConcept("newGrandChild");

        Connect();
        _originalPartition.AddLinks([parent]);
        parent.Containment_0_1 = firstChild;

        _compositor.Push();
        firstChild.ReplaceWith(child);
        grandChild.ReplaceWith(newGrandChild);
        _compositor.Pop(true);

        AssertPartitionsEqual();
    }

    #endregion

    #region Deleted


    [TestMethod]
    public void ChildDeleted()
    {
        var grandChild = new LinkTestConcept("grandChild");
        var child = new LinkTestConcept("child") { Containment_0_1 = grandChild };
        var parent = new LinkTestConcept("parent") { Containment_1 = child };

        Connect();
        _compositor.Push();
        _originalPartition.AddLinks([parent]);
        child.DetachFromParent();
        child.Containment_0_1 = null;
        var composite = _compositor.Pop(true);
        
        Assert.IsInstanceOfType<ChildDeletedNotification>(composite.Parts[^1]);

        var deleted = (ChildDeletedNotification)composite.Parts[^1];
        CollectionAssert.AreEqual(new List<IReadableNode>{child, grandChild}, deleted.DeletedNodes.ToList());

        AssertPartitionsEqual();
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainmentInSameParent()
    {
        var grandChild = new LinkTestConcept("grandChild");
        var child = new LinkTestConcept("child") { Containment_0_1 = grandChild };
        var otherChild = new LinkTestConcept("otherChild");
        var parent = new LinkTestConcept("parent") { Containment_1 = child, Containment_0_n = [otherChild]};

        Connect();
        _compositor.Push();
        _originalPartition.AddLinks([parent]);
        child.ReplaceWith(otherChild);
        child.Containment_0_1 = null;
        var composite = _compositor.Pop(true);
        
        Assert.IsInstanceOfType<ChildMovedAndReplacedFromOtherContainmentInSameParentNotification>(composite.Parts[^1]);

        var deleted = (ChildMovedAndReplacedFromOtherContainmentInSameParentNotification)composite.Parts[^1];
        CollectionAssert.AreEqual(new List<IReadableNode>{child, grandChild}, deleted.DeletedNodes.ToList());

        AssertPartitionsEqual();
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainment()
    {
        var grandChild = new LinkTestConcept("grandChild");
        var child = new LinkTestConcept("child") { Containment_0_1 = grandChild };
        var otherChild = new LinkTestConcept("otherChild");
        var parent = new LinkTestConcept("parent") { Containment_1 = child};

        Connect();
        _compositor.Push();
        _originalPartition.AddLinks([parent, otherChild]);
        child.ReplaceWith(otherChild);
        child.Containment_0_1 = null;
        var composite = _compositor.Pop(true);
        
        Assert.IsInstanceOfType<ChildMovedAndReplacedFromOtherContainmentNotification>(composite.Parts[^1]);

        var deleted = (ChildMovedAndReplacedFromOtherContainmentNotification)composite.Parts[^1];
        CollectionAssert.AreEqual(new List<IReadableNode>{child, grandChild}, deleted.DeletedNodes.ToList());

        AssertPartitionsEqual();
    }

    [TestMethod]
    public void ChildMovedAndReplacedInSameContainment()
    {
        var grandChild = new LinkTestConcept("grandChild");
        var child = new LinkTestConcept("child") { Containment_0_1 = grandChild };
        var otherChild = new LinkTestConcept("otherChild");
        var parent = new LinkTestConcept("parent") { Containment_0_n = [child, otherChild]};

        Connect();
        _compositor.Push();
        _originalPartition.AddLinks([parent]);
        child.ReplaceWith(otherChild);
        child.Containment_0_1 = null;
        var composite = _compositor.Pop(true);
        
        Assert.IsInstanceOfType<ChildMovedAndReplacedInSameContainmentNotification>(composite.Parts[^1]);

        var deleted = (ChildMovedAndReplacedInSameContainmentNotification)composite.Parts[^1];
        CollectionAssert.AreEqual(new List<IReadableNode>{child, grandChild}, deleted.DeletedNodes.ToList());

        AssertPartitionsEqual();
    }

    [TestMethod]
    public void ChildReplaced_Deleted()
    {
        var grandChild = new LinkTestConcept("grandChild");
        var child = new LinkTestConcept("child") { Containment_0_1 = grandChild };
        var otherChild = new LinkTestConcept("otherChild");
        var parent = new LinkTestConcept("parent") { Containment_0_n = [child]};

        Connect();
        _compositor.Push();
        _originalPartition.AddLinks([parent]);
        child.ReplaceWith(otherChild);
        child.Containment_0_1 = null;
        var composite = _compositor.Pop(true);
        
        Assert.IsInstanceOfType<ChildReplacedNotification>(composite.Parts[^1]);

        var deleted = (ChildReplacedNotification)composite.Parts[^1];
        CollectionAssert.AreEqual(new List<IReadableNode>{child, grandChild}, deleted.DeletedNodes.ToList());

        AssertPartitionsEqual();
    }

    #endregion
    
    #endregion


    #region Partition

    #region New

    [TestMethod]
    public void PartitionAdded()
    {
        var grandChild = new LinkTestConcept("grandChild");
        var parent = new LinkTestConcept("parent") { Containment_0_1 = grandChild };
        var partition = new TestPartition("addedPartition") { Links = [parent] };
        var newGrandChild = new LinkTestConcept("newGrandChild");

        Connect();

        _compositor.Push();
        _originalForest.AddPartitions([partition]);
        grandChild.ReplaceWith(newGrandChild);
        _compositor.Pop(true);

        AssertPartitionsEqual();
    }

    #endregion

    [TestMethod]
    public void PartitionAddedDeleted()
    {
        var grandChild = new LinkTestConcept("grandChild");
        var parent = new LinkTestConcept("parent") { Containment_0_1 = grandChild };
        var partition = new TestPartition("addedPartition") { Links = [parent] };

        Connect();
        _compositor.Push();
        _originalForest.AddPartitions([partition]);
        _originalForest.RemovePartitions([partition]);
        var composite = _compositor.Pop(true);
        
        AssertPartitionsEqual();
    }


    #region Deleted

    [TestMethod]
    public void PartitionDeleted()
    {
        var grandChild = new LinkTestConcept("grandChild");
        var parent = new LinkTestConcept("parent") { Containment_0_1 = grandChild };
        var partition = new TestPartition("addedPartition") { Links = [parent] };

        Connect();
        _originalForest.AddPartitions([partition]);
        _compositor.Push();
        _originalForest.RemovePartitions([partition]);
        parent.Containment_0_1 = null;
        var composite = _compositor.Pop(true);
        
        Assert.IsInstanceOfType<PartitionDeletedNotification>(composite.Parts[^1]);

        var deleted = (PartitionDeletedNotification)composite.Parts[^1];
        CollectionAssert.AreEqual(new List<IReadableNode>{partition, parent, grandChild}, deleted.DeletedNodes.ToList());

        AssertPartitionsEqual();
    }

    #endregion
    
    #endregion

    private void AssertPartitionsEqual() => AssertEquals([_originalPartition], [_clonedPartition]);
}
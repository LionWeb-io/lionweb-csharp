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

namespace LionWeb.Core.Test.Notification.Replicator.Annotation;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class TwowayTests : TwowayReplicatorTestsBase
{
    #region AnnotationAdded

    [TestMethod]
    public void AnnotationAdded_Multiple_Only()
    {
        var node = new TestPartition("a");

        var clone = new TestPartition("a");

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        var added = new TestAnnotation("added");
        node.AddAnnotations([added]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.GetAnnotations()[0]);
    }

    [TestMethod]
    public void AnnotationAdded_Multiple_First()
    {
        var node = new TestPartition("a");
        node.AddAnnotations([new TestAnnotation("bof")]);

        var clone = new TestPartition("a");
        clone.AddAnnotations([new TestAnnotation("bof")]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        var added = new TestAnnotation("added");
        node.InsertAnnotations(0, [added]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.GetAnnotations()[0]);
    }

    [TestMethod]
    public void AnnotationAdded_Multiple_Last()
    {
        var node = new TestPartition("a");
        node.AddAnnotations([new TestAnnotation("bof")]);

        var clone = new TestPartition("a");
        clone.AddAnnotations([new TestAnnotation("bof")]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        var added = new TestAnnotation("added");
        node.InsertAnnotations(1, [added]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.GetAnnotations()[1]);
    }

    [TestMethod]
    [Ignore("Not possible with TestLanguage")]
    public void AnnotationAdded_Deep()
    {
        var node = new TestPartition("a");

        var clone = new TestPartition("a");

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        // var added = new TestAnnotation("added")
        // {
        //     AltGroups = [new MaterialGroup("mg") { MatterState = MatterState.gas }]
        // };
        // node.AddAnnotations([added]);

        AssertEquals([node], [clone]);
        // Assert.AreNotSame(added, clone.GetAnnotations()[0]);
    }

    #endregion

    #region AnnotationDeleted

    [TestMethod]
    public void AnnotationDeleted_Multiple_Only()
    {
        var deleted = new TestAnnotation("deleted");
        var node = new TestPartition("a");
        node.AddAnnotations([deleted]);

        var clone = new TestPartition("a");
        clone.AddAnnotations([new TestAnnotation("deleted")]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.RemoveAnnotations([deleted]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void AnnotationDeleted_Multiple_First()
    {
        var deleted = new TestAnnotation("deleted");
        var node = new TestPartition("a");
        node.AddAnnotations([deleted, new TestAnnotation("bof")]);

        var clone = new TestPartition("a");
        clone.AddAnnotations([new TestAnnotation("deleted"), new TestAnnotation("bof")]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.RemoveAnnotations([deleted]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void AnnotationDeleted_Multiple_Last()
    {
        var deleted = new TestAnnotation("deleted");
        var node = new TestPartition("a");
        node.AddAnnotations([new TestAnnotation("bof"), deleted]);

        var clone = new TestPartition("a");
        clone.AddAnnotations([new TestAnnotation("bof"), new TestAnnotation("deleted")]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.RemoveAnnotations([deleted]);

        AssertEquals([node], [clone]);
    }

    #endregion

    #region AnnotationMovedFromOtherParent

    [TestMethod]
    public void AnnotationMovedFromOtherParent_Multiple()
    {
        var moved = new TestAnnotation("moved");
        var origin = new LinkTestConcept("origin");
        origin.AddAnnotations([moved]);
        var node = new TestPartition("a") { Links = [origin] };

        var cloneOrigin = new LinkTestConcept("origin");
        cloneOrigin.AddAnnotations([new TestAnnotation("moved")]);
        var clone = new TestPartition("a") { Links = [cloneOrigin] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.AddAnnotations([moved]);

        AssertEquals([node], [clone]);
    }

    #endregion

    #region AnnotationMovedInSameParent

    [TestMethod]
    public void AnnotationMovedInSameParent_Forward()
    {
        var moved = new TestAnnotation("moved");
        var node = new TestPartition("a");
        node.AddAnnotations([moved, new TestAnnotation("bof")]);

        var clone = new TestPartition("a");
        clone.AddAnnotations([new TestAnnotation("moved"), new TestAnnotation("bof")]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.AddAnnotations([moved]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void AnnotationMovedInSameParent_Backward()
    {
        var moved = new TestAnnotation("moved");
        var node = new TestPartition("a");
        node.AddAnnotations([new TestAnnotation("bof"), moved]);

        var clone = new TestPartition("a");
        clone.AddAnnotations([new TestAnnotation("bof"), new TestAnnotation("moved")]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.InsertAnnotations(0, [moved]);

        AssertEquals([node], [clone]);
    }

    #endregion
}
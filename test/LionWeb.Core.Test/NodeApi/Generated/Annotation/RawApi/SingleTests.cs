// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

namespace LionWeb.Core.Test.NodeApi.Generated.Annotation.RawApi;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class SingleTests
{
    [TestMethod]
    public void Single_Add()
    {
        var parent = new LinkTestConcept("g");
        var annotation = new TestAnnotation("myId");
        Assert.IsTrue(parent.AddAnnotationsRaw(annotation));
        Assert.AreSame(parent, annotation.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(annotation));
        Assert.Contains(annotation, ((IReadableNodeRaw)parent).GetAnnotationsRaw());
    }

    [TestMethod]
    public void Single_Set_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var annotation = new TestAnnotation("myId");
        Assert.IsFalse(parent.AddContainmentsRaw(null, annotation));
        Assert.IsNull(annotation.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(annotation));
        Assert.DoesNotContain(annotation, ((IReadableNodeRaw)parent).GetAnnotationsRaw());
    }

    #region Insert

    [TestMethod]
    public void Single_Insert_Empty()
    {
        var parent = new LinkTestConcept("g");
        var annotation = new TestAnnotation("myId");
        Assert.IsTrue(parent.InsertAnnotationsRaw(0, annotation));
        Assert.AreSame(parent, annotation.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(annotation));
        Assert.Contains(annotation, ((IReadableNodeRaw)parent).GetAnnotationsRaw());
    }

    [TestMethod]
    public void Single_Insert_Empty_UnderBounds()
    {
        var parent = new LinkTestConcept("g");
        var annotation = new TestAnnotation("myId");
        Assert.IsFalse(parent.InsertAnnotationsRaw( -1, annotation));
        Assert.IsNull(annotation.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(annotation));
    }

    [TestMethod]
    public void Single_Insert_Empty_OverBounds()
    {
        var parent = new LinkTestConcept("g");
        var annotation = new TestAnnotation("myId");
        Assert.IsFalse(parent.InsertAnnotationsRaw(1, annotation));
        Assert.IsNull(annotation.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(annotation));
    }

    [TestMethod]
    public void Single_Insert_One_Before()
    {
        var otherAnnotation = new TestAnnotation("cId");
        var parent = new LinkTestConcept("g");
        Assert.IsTrue(parent.AddAnnotationsRaw(otherAnnotation));
        var annotation = new TestAnnotation("myId");
        Assert.IsTrue(parent.InsertAnnotationsRaw(0, annotation));
        Assert.AreSame(parent, otherAnnotation.GetParent());
        Assert.AreSame(parent, annotation.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(annotation));
        CollectionAssert.AreEqual(new List<INode> { annotation, otherAnnotation }, ((IReadableNodeRaw)parent).GetAnnotationsRaw().ToList());
    }

    [TestMethod]
    public void Single_Insert_One_After()
    {
        var otherAnnotation = new TestAnnotation("cId");
        var parent = new LinkTestConcept("g");
        Assert.IsTrue(parent.AddAnnotationsRaw(otherAnnotation));
        var annotation = new TestAnnotation("myId");
        Assert.IsTrue(parent.InsertAnnotationsRaw(1, annotation));
        Assert.AreSame(parent, otherAnnotation.GetParent());
        Assert.AreSame(parent, annotation.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(annotation));
        CollectionAssert.AreEqual(new List<INode> { otherAnnotation, annotation }, ((IReadableNodeRaw)parent).GetAnnotationsRaw().ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_Before()
    {
        var otherAnnotationA = new TestAnnotation("cIdA");
        var otherAnnotationB = new TestAnnotation("cIdB");
        var parent = new LinkTestConcept("g");
        Assert.IsTrue(parent.AddAnnotationsRaw(otherAnnotationA));
        Assert.IsTrue(parent.AddAnnotationsRaw(otherAnnotationB));
        var annotation = new TestAnnotation("myId");
        Assert.IsTrue(parent.InsertAnnotationsRaw(0, annotation));
        Assert.AreSame(parent, otherAnnotationA.GetParent());
        Assert.AreSame(parent, otherAnnotationB.GetParent());
        Assert.AreSame(parent, annotation.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(annotation));
        CollectionAssert.AreEqual(new List<INode> { annotation, otherAnnotationA, otherAnnotationB }, ((IReadableNodeRaw)parent).GetAnnotationsRaw().ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_Between()
    {
        var otherAnnotationA = new TestAnnotation("cIdA");
        var otherAnnotationB = new TestAnnotation("cIdB");
        var parent = new LinkTestConcept("g");
        Assert.IsTrue(parent.AddAnnotationsRaw(otherAnnotationA));
        Assert.IsTrue(parent.AddAnnotationsRaw(otherAnnotationB));
        var annotation = new TestAnnotation("myId");
        Assert.IsTrue(parent.InsertAnnotationsRaw(1, annotation));
        Assert.AreSame(parent, otherAnnotationA.GetParent());
        Assert.AreSame(parent, otherAnnotationB.GetParent());
        Assert.AreSame(parent, annotation.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(annotation));
        CollectionAssert.AreEqual(new List<INode> { otherAnnotationA, annotation, otherAnnotationB }, ((IReadableNodeRaw)parent).GetAnnotationsRaw().ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_After()
    {
        var otherAnnotationA = new TestAnnotation("cIdA");
        var otherAnnotationB = new TestAnnotation("cIdB");
        var parent = new LinkTestConcept("g");
        Assert.IsTrue(parent.AddAnnotationsRaw(otherAnnotationA));
        Assert.IsTrue(parent.AddAnnotationsRaw(otherAnnotationB));
        var annotation = new TestAnnotation("myId");
        Assert.IsTrue(parent.InsertAnnotationsRaw(2, annotation));
        Assert.AreSame(parent, otherAnnotationA.GetParent());
        Assert.AreSame(parent, otherAnnotationB.GetParent());
        Assert.AreSame(parent, annotation.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(annotation));
        CollectionAssert.AreEqual(new List<INode> { otherAnnotationA, otherAnnotationB, annotation }, ((IReadableNodeRaw)parent).GetAnnotationsRaw().ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Single_Remove_Empty()
    {
        var parent = new LinkTestConcept("g");
        var annotation = new TestAnnotation("myId");
        Assert.IsFalse(parent.RemoveAnnotationsRaw(annotation));
        Assert.IsNull(annotation.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(annotation));
    }

    [TestMethod]
    public void Single_Remove_NotContained()
    {
        var otherAnnotation = new TestAnnotation("myC");
        var parent = new LinkTestConcept("cs");
        Assert.IsTrue(parent.AddAnnotationsRaw(otherAnnotation));
        var annotation = new TestAnnotation("myId");
        Assert.IsFalse(parent.RemoveAnnotationsRaw(annotation));
        Assert.AreSame(parent, otherAnnotation.GetParent());
        Assert.IsNull(annotation.GetParent());
        CollectionAssert.AreEqual(new List<INode> { otherAnnotation }, ((IReadableNodeRaw)parent).GetAnnotationsRaw().ToList());
    }

    [TestMethod]
    public void Single_Remove_Only()
    {
        var annotation = new TestAnnotation("myId");
        var parent = new LinkTestConcept("g");
        Assert.IsTrue(parent.AddAnnotationsRaw(annotation));
        Assert.IsTrue(parent.RemoveAnnotationsRaw(annotation));
        Assert.IsNull(annotation.GetParent());
        CollectionAssert.AreEqual(new List<INode> { }, ((IReadableNodeRaw)parent).GetAnnotationsRaw().ToList());
    }

    [TestMethod]
    public void Single_Remove_First()
    {
        var otherAnnotation = new TestAnnotation("cId");
        var annotation = new TestAnnotation("myId");
        var parent = new LinkTestConcept("g");
        Assert.IsTrue(parent.AddAnnotationsRaw(annotation));
        Assert.IsTrue(parent.AddAnnotationsRaw(otherAnnotation));
        Assert.IsTrue(parent.RemoveAnnotationsRaw(annotation));
        Assert.AreSame(parent, otherAnnotation.GetParent());
        Assert.IsNull(annotation.GetParent());
        CollectionAssert.AreEqual(new List<INode> { otherAnnotation }, ((IReadableNodeRaw)parent).GetAnnotationsRaw().ToList());
    }

    [TestMethod]
    public void Single_Remove_Last()
    {
        var otherAnnotation = new TestAnnotation("cId");
        var annotation = new TestAnnotation("myId");
        var parent = new LinkTestConcept("g");
        Assert.IsTrue(parent.AddAnnotationsRaw(otherAnnotation));
        Assert.IsTrue(parent.AddAnnotationsRaw(annotation));
        Assert.IsTrue(parent.RemoveAnnotationsRaw(annotation));
        Assert.AreSame(parent, otherAnnotation.GetParent());
        Assert.IsNull(annotation.GetParent());
        CollectionAssert.AreEqual(new List<INode> { otherAnnotation }, ((IReadableNodeRaw)parent).GetAnnotationsRaw().ToList());
    }

    [TestMethod]
    public void Single_Remove_Between()
    {
        var otherAnnotationA = new TestAnnotation("cIdA");
        var otherAnnotationB = new TestAnnotation("cIdB");
        var annotation = new TestAnnotation("myId");
        var parent = new LinkTestConcept("g");
        Assert.IsTrue(parent.AddAnnotationsRaw(otherAnnotationA));
        Assert.IsTrue(parent.AddAnnotationsRaw(annotation));
        Assert.IsTrue(parent.AddAnnotationsRaw(otherAnnotationB));
        Assert.IsTrue(parent.RemoveAnnotationsRaw(annotation));
        Assert.AreSame(parent, otherAnnotationA.GetParent());
        Assert.AreSame(parent, otherAnnotationB.GetParent());
        Assert.IsNull(annotation.GetParent());
        CollectionAssert.AreEqual(new List<INode> { otherAnnotationA, otherAnnotationB }, ((IReadableNodeRaw)parent).GetAnnotationsRaw().ToList());
    }

    #endregion
}
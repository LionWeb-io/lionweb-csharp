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

namespace LionWeb.Core.Test.NodeApi.Generated.Annotation;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class SingleTests
{
    [TestMethod]
    public void Add()
    {
        var parent = new LinkTestConcept("g");
        var annotation = new TestAnnotation("myId");
        parent.AddAnnotations([annotation]);
        Assert.AreSame(parent, annotation.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(annotation));
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new LinkTestConcept("g");
        var annotation = new TestAnnotation("myId");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(null, annotation));
        Assert.AreSame(null, annotation.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(annotation));
    }

    #region Insert

    [TestMethod]
    public void Insert_Empty()
    {
        var parent = new LinkTestConcept("g");
        var annotation = new TestAnnotation("myId");
        parent.InsertAnnotations(0, [annotation]);
        Assert.AreSame(parent, annotation.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(annotation));
    }

    [TestMethod]
    public void Insert_Empty_UnderBounds()
    {
        var parent = new LinkTestConcept("g");
        var annotation = new TestAnnotation("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.InsertAnnotations(-1, [annotation]));
        Assert.IsNull(annotation.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(annotation));
    }

    [TestMethod]
    public void Insert_Empty_OverBounds()
    {
        var parent = new LinkTestConcept("g");
        var annotation = new TestAnnotation("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.InsertAnnotations(1, [annotation]));
        Assert.IsNull(annotation.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(annotation));
    }

    [TestMethod]
    public void Insert_One_Before()
    {
        var existing = new TestAnnotation("cId");
        var parent = new LinkTestConcept("g");
        parent.AddAnnotations([existing]);
        var annotation = new TestAnnotation("myId");
        parent.InsertAnnotations(0, [annotation]);
        Assert.AreSame(parent, existing.GetParent());
        Assert.AreSame(parent, annotation.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(annotation));
        CollectionAssert.AreEqual(new List<INode> { annotation, existing }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Insert_One_After()
    {
        var existing = new TestAnnotation("cId");
        var parent = new LinkTestConcept("g");
        parent.AddAnnotations([existing]);
        var annotation = new TestAnnotation("myId");
        parent.InsertAnnotations(1, [annotation]);
        Assert.AreSame(parent, existing.GetParent());
        Assert.AreSame(parent, annotation.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(annotation));
        CollectionAssert.AreEqual(new List<INode> { existing, annotation }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Insert_Two_Before()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var parent = new LinkTestConcept("g");
        parent.AddAnnotations([existingA, existingB]);
        var annotation = new TestAnnotation("myId");
        parent.InsertAnnotations(0, [annotation]);
        Assert.AreSame(parent, existingA.GetParent());
        Assert.AreSame(parent, existingB.GetParent());
        Assert.AreSame(parent, annotation.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(annotation));
        CollectionAssert.AreEqual(new List<INode> { annotation, existingA, existingB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var parent = new LinkTestConcept("g");
        parent.AddAnnotations([existingA, existingB]);
        var annotation = new TestAnnotation("myId");
        parent.InsertAnnotations(1, [annotation]);
        Assert.AreSame(parent, existingA.GetParent());
        Assert.AreSame(parent, existingB.GetParent());
        Assert.AreSame(parent, annotation.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(annotation));
        CollectionAssert.AreEqual(new List<INode> { existingA, annotation, existingB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Insert_Two_After()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var parent = new LinkTestConcept("g");
        parent.AddAnnotations([existingA, existingB]);
        var annotation = new TestAnnotation("myId");
        parent.InsertAnnotations(2, [annotation]);
        Assert.AreSame(parent, existingA.GetParent());
        Assert.AreSame(parent, existingB.GetParent());
        Assert.AreSame(parent, annotation.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(annotation));
        CollectionAssert.AreEqual(new List<INode> { existingA, existingB, annotation }, parent.GetAnnotations().ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove_Empty()
    {
        var parent = new LinkTestConcept("g");
        var annotation = new TestAnnotation("myId");
        parent.RemoveAnnotations([annotation]);
        Assert.IsNull(annotation.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(annotation));
    }

    [TestMethod]
    public void Remove_NotContained()
    {
        var existing = new TestAnnotation("myC");
        var parent = new LinkTestConcept("cs");
        parent.AddAnnotations([existing]);
        var annotation = new TestAnnotation("myId");
        parent.RemoveAnnotations([annotation]);
        Assert.AreSame(parent, existing.GetParent());
        Assert.IsNull(annotation.GetParent());
        CollectionAssert.AreEqual(new List<INode> { existing }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Remove_Only()
    {
        var annotation = new TestAnnotation("myId");
        var parent = new LinkTestConcept("g");
        parent.AddAnnotations([annotation]);
        parent.RemoveAnnotations([annotation]);
        Assert.IsNull(annotation.GetParent());
        CollectionAssert.AreEqual(new List<INode> { }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Remove_First()
    {
        var existing = new TestAnnotation("cId");
        var annotation = new TestAnnotation("myId");
        var parent = new LinkTestConcept("g");
        parent.AddAnnotations([annotation, existing]);
        parent.RemoveAnnotations([annotation]);
        Assert.AreSame(parent, existing.GetParent());
        Assert.IsNull(annotation.GetParent());
        CollectionAssert.AreEqual(new List<INode> { existing }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Remove_Last()
    {
        var existing = new TestAnnotation("cId");
        var annotation = new TestAnnotation("myId");
        var parent = new LinkTestConcept("g");
        parent.AddAnnotations([existing, annotation]);
        parent.RemoveAnnotations([annotation]);
        Assert.AreSame(parent, existing.GetParent());
        Assert.IsNull(annotation.GetParent());
        CollectionAssert.AreEqual(new List<INode> { existing }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Remove_Between()
    {
        var existingA = new TestAnnotation("cIdA");
        var existingB = new TestAnnotation("cIdB");
        var annotation = new TestAnnotation("myId");
        var parent = new LinkTestConcept("g");
        parent.AddAnnotations([existingA, annotation, existingB]);
        parent.RemoveAnnotations([annotation]);
        Assert.AreSame(parent, existingA.GetParent());
        Assert.AreSame(parent, existingB.GetParent());
        Assert.IsNull(annotation.GetParent());
        CollectionAssert.AreEqual(new List<INode> { existingA, existingB }, parent.GetAnnotations().ToList());
    }

    #endregion
}

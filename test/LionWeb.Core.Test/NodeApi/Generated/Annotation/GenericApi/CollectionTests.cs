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

namespace LionWeb.Core.Test.NodeApi.Generated.Annotation.GenericApi;

using LionWeb.Core.Test.Languages.Generated.V2024_1.Shapes.M2;
using LionWeb.Core.Test.Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class CollectionTests
{
    #region EmptyCollection

    [TestMethod]
    public void EmptyArray()
    {
        var parent = new LinkTestConcept("g");
        var values = new INode[0];
        parent.Add(null, values);
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }


    [TestMethod]
    public void Insert_EmptyArray()
    {
        var parent = new LinkTestConcept("g");
        var values = new INode[0];
        parent.Insert(null, 0, values);
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var parent = new LinkTestConcept("g");
        var values = new INode[0];
        parent.Remove(null, values);
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray()
    {
        var parent = new LinkTestConcept("g");
        var values = new INode[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Add(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }


    [TestMethod]
    public void Insert_NullArray()
    {
        var parent = new LinkTestConcept("g");
        var values = new INode[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Insert(null, 0, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    [TestMethod]
    public void Remove_NullArray()
    {
        var parent = new LinkTestConcept("g");
        var values = new INode[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(null, values));
        Assert.IsTrue(parent.GetAnnotations().Count == 0);
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray()
    {
        var parent = new LinkTestConcept("g");
        var value = new TestAnnotation("s");
        var values = new INode[] { value };
        parent.Add(null, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void Insert_SingleArray()
    {
        var parent = new LinkTestConcept("g");
        var value = new TestAnnotation("s");
        var values = new INode[] { value };
        parent.Insert(null, 0, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(value));
    }

    #region Remove

    [TestMethod]
    public void SingleArray_Remove_Empty()
    {
        var parent = new LinkTestConcept("g");
        var bom = new TestAnnotation("myId");
        var values = new INode[] { bom };
        parent.Remove(null, values);
        Assert.IsNull(bom.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void SingleArray_Remove_Only()
    {
        var bom = new TestAnnotation("myId");
        var parent = new LinkTestConcept("g");
        parent.Add(null, [bom]);
        var values = new INode[] { bom };
        parent.Remove(null, values);
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_First()
    {
        var doc = new TestAnnotation("cId");
        var bom = new TestAnnotation("myId");
        var parent = new LinkTestConcept("g");
        parent.Add(null, [bom, doc]);
        var values = new INode[] { bom };
        parent.Remove(null, values);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_Last()
    {
        var doc = new TestAnnotation("cId");
        var bom = new TestAnnotation("myId");
        var parent = new LinkTestConcept("g");
        parent.Add(null, [doc, bom]);
        var values = new INode[] { bom };
        parent.Remove(null, values);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_Between()
    {
        var docA = new TestAnnotation("cIdA");
        var docB = new TestAnnotation("cIdB");
        var bom = new TestAnnotation("myId");
        var parent = new LinkTestConcept("g");
        parent.Add(null, [docA, bom, docB]);
        var values = new INode[] { bom };
        parent.Remove(null, values);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { docA, docB }, parent.GetAnnotations().ToList());
    }

    #endregion

    #endregion

    [TestMethod]
    public void SingleList_NotAnnotating()
    {
        var parent = new LinkTestConcept("g");
        var value = new Documentation("sA");
        var values = new List<INode>() { value };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Add(null, values));
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleList_NotAnnotating_Insert()
    {
        var parent = new LinkTestConcept("g");
        var value = new Documentation("sA");
        var values = new List<INode>() { value };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Insert(null, 0, values));
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleList_NotAnnotating_Remove()
    {
        var parent = new LinkTestConcept("g");
        var value = new Documentation("sA");
        var values = new List<INode>() { value };
        parent.Remove(null, values);
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }
}

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

using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class SingleTests
{
    [TestMethod]
    public void Add()
    {
        var parent = new Line("g");
        var bom = new BillOfMaterials("myId");
        parent.AddAnnotations([bom]);
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new Line("g");
        var bom = new BillOfMaterials("myId");
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(null, bom));
        Assert.AreSame(null, bom.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(bom));
    }

    #region Insert

    [TestMethod]
    public void Insert_Empty()
    {
        var parent = new Line("g");
        var bom = new BillOfMaterials("myId");
        parent.InsertAnnotations(0, [bom]);
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Insert_Empty_UnderBounds()
    {
        var parent = new Line("g");
        var bom = new BillOfMaterials("myId");
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => parent.InsertAnnotations(-1, [bom]));
        Assert.IsNull(bom.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Insert_Empty_OverBounds()
    {
        var parent = new Line("g");
        var bom = new BillOfMaterials("myId");
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => parent.InsertAnnotations(1, [bom]));
        Assert.IsNull(bom.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Insert_One_Before()
    {
        var doc = new Documentation("cId");
        var parent = new Line("g");
        parent.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");
        parent.InsertAnnotations(0, [bom]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { bom, doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Insert_One_After()
    {
        var doc = new Documentation("cId");
        var parent = new Line("g");
        parent.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");
        parent.InsertAnnotations(1, [bom]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { doc, bom }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Insert_Two_Before()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var parent = new Line("g");
        parent.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");
        parent.InsertAnnotations(0, [bom]);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { bom, docA, docB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Insert_Two_Between()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var parent = new Line("g");
        parent.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");
        parent.InsertAnnotations(1, [bom]);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { docA, bom, docB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Insert_Two_After()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var parent = new Line("g");
        parent.AddAnnotations([docA, docB]);
        var bom = new BillOfMaterials("myId");
        parent.InsertAnnotations(2, [bom]);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { docA, docB, bom }, parent.GetAnnotations().ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove_Empty()
    {
        var parent = new Line("g");
        var bom = new BillOfMaterials("myId");
        parent.RemoveAnnotations([bom]);
        Assert.IsNull(bom.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Remove_NotContained()
    {
        var doc = new Documentation("myC");
        var parent = new Line("cs");
        parent.AddAnnotations([doc]);
        var bom = new BillOfMaterials("myId");
        parent.RemoveAnnotations([bom]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Remove_Only()
    {
        var bom = new BillOfMaterials("myId");
        var parent = new Line("g");
        parent.AddAnnotations([bom]);
        parent.RemoveAnnotations([bom]);
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Remove_First()
    {
        var doc = new Documentation("cId");
        var bom = new BillOfMaterials("myId");
        var parent = new Line("g");
        parent.AddAnnotations([bom, doc]);
        parent.RemoveAnnotations([bom]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Remove_Last()
    {
        var doc = new Documentation("cId");
        var bom = new BillOfMaterials("myId");
        var parent = new Line("g");
        parent.AddAnnotations([doc, bom]);
        parent.RemoveAnnotations([bom]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Remove_Between()
    {
        var docA = new Documentation("cIdA");
        var docB = new Documentation("cIdB");
        var bom = new BillOfMaterials("myId");
        var parent = new Line("g");
        parent.AddAnnotations([docA, bom, docB]);
        parent.RemoveAnnotations([bom]);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { docA, docB }, parent.GetAnnotations().ToList());
    }

    #endregion
}
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

namespace LionWeb.Core.Test.NodeApi.Lenient;

[TestClass]
public class GenericApiTests: LenientNodeTestsBase
{
    #region annotation generic add/insert/remove api

    #region Single

    [TestMethod]
    public void Single_Add()
    {
        var parent = newLine("g");
        var bom = newBillOfMaterials("myId");
        parent.Add(null, [bom]);
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Single_Add_NonAnnotating()
    {
        var parent = newCoord("g");
        var value = newDocumentation("myId");
        parent.Add(null, [value]);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void Single_Add_NoAnnotation()
    {
        var parent = newCoord("g");
        var value = newLine("myId");
        parent.Add(null, [value]);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(value));
    }
    
    #region Insert

    [TestMethod]
    public void Single_Insert_Empty()
    {
        var parent = newLine("g");
        var bom = newBillOfMaterials("myId");
        parent.Insert(null, 0, [bom]);
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Single_Insert_Empty_NonAnnotating()
    {
        var parent = newCoord("g");
        var value = newDocumentation("myId");
        parent.Insert(null, 0, [value]);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void Single_Insert_Empty_NoAnnotation()
    {
        var parent = newCoord("g");
        var value = newLine("myId");
        parent.InsertAnnotations(0, [value]);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void Single_Insert_Empty_UnderBounds()
    {
        var parent = newLine("g");
        var bom = newBillOfMaterials("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.Insert(null, -1, [bom]));
        Assert.IsNull(bom.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Single_Insert_Empty_OverBounds()
    {
        var parent = newLine("g");
        var bom = newBillOfMaterials("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.Insert(null, 1, [bom]));
        Assert.IsNull(bom.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Single_Insert_One_Before()
    {
        var doc = newDocumentation("cId");
        var parent = newLine("g");
        parent.Add(null, [doc]);
        var bom = newBillOfMaterials("myId");
        parent.Insert(null, 0, [bom]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { bom, doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Insert_One_After()
    {
        var doc = newDocumentation("cId");
        var parent = newLine("g");
        parent.Add(null, [doc]);
        var bom = newBillOfMaterials("myId");
        parent.Insert(null, 1, [bom]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { doc, bom }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_Before()
    {
        var docA = newDocumentation("cIdA");
        var docB = newDocumentation("cIdB");
        var parent = newLine("g");
        parent.Add(null, [docA, docB]);
        var bom = newBillOfMaterials("myId");
        parent.Insert(null, 0, [bom]);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { bom, docA, docB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_Between()
    {
        var docA = newDocumentation("cIdA");
        var docB = newDocumentation("cIdB");
        var parent = newLine("g");
        parent.Add(null, [docA, docB]);
        var bom = newBillOfMaterials("myId");
        parent.Insert(null,1, [bom]);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { docA, bom, docB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_After()
    {
        var docA = newDocumentation("cIdA");
        var docB = newDocumentation("cIdB");
        var parent = newLine("g");
        parent.Add(null, [docA, docB]);
        var bom = newBillOfMaterials("myId");
        parent.Insert(null, 2, [bom]);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { docA, docB, bom }, parent.GetAnnotations().ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Single_Remove_Empty()
    {
        var parent = newLine("g");
        var bom = newBillOfMaterials("myId");
        parent.Remove(null, [bom]);
        Assert.IsNull(bom.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Single_Remove_Empty_NonAnnotating()
    {
        var parent = newCoord("g");
        var value = newDocumentation("myId");
        parent.Remove(null, [value]);
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void Single_Remove_Empty_NoAnnotation()
    {
        var parent = newCoord("g");
        var value = newLine("myId");
        parent.Remove(null, [value]);
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void Single_Remove_NotContained()
    {
        var doc = newDocumentation("myC");
        var parent = newLine("cs");
        parent.Add(null, [doc]);
        var bom = newBillOfMaterials("myId");
        parent.Remove(null, [bom]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Remove_Only()
    {
        var bom = newBillOfMaterials("myId");
        var parent = newLine("g");
        parent.Add(null, [bom]);
        parent.Remove(null, [bom]);
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Remove_First_NonAnnotating()
    {
        var doc = newDocumentation("cId");
        var value = newDocumentation("myId");
        var parent = newCoord("g");
        parent.Add(null, [value, doc]);
        parent.Remove(null, [value]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(value.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Remove_First_NoAnnotation()
    {
        var doc = newDocumentation("cId");
        var value = newLine("myId");
        var parent = newCoord("g");
        parent.Add(null, [value, doc]);
        parent.Remove(null, [value]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(value.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Remove_First()
    {
        var doc = newDocumentation("cId");
        var bom = newBillOfMaterials("myId");
        var parent = newCoord("g");
        parent.Add(null, [bom, doc]);
        parent.Remove(null, [bom]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Remove_Last()
    {
        var doc = newDocumentation("cId");
        var bom = newBillOfMaterials("myId");
        var parent = newLine("g");
        parent.Add(null, [doc, bom]);
        parent.Remove(null, [bom]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Remove_Between()
    {
        var docA = newDocumentation("cIdA");
        var docB = newDocumentation("cIdB");
        var bom = newBillOfMaterials("myId");
        var parent = newLine("g");
        parent.Add(null, [docA, bom, docB]);
        parent.Remove(null, [bom]);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { docA, docB }, parent.GetAnnotations().ToList());
    }

    #endregion

    #endregion

    #region Null

    //TODO: copy the rest of tests from Lenient/ContainmentTests_Annotation.cs

    #endregion
    
    #endregion
    
}
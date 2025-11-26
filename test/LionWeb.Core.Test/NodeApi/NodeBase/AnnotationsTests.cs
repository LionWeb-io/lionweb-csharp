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

namespace LionWeb.Core.Test.NodeApi.NodeBase;

using Languages.Generated.V2023_1.Shapes.M2;
using NodeBase = Core.NodeBase;

[TestClass]
public class AnnotationsTests
{
    #region AddAnnotations

    [TestMethod]
    public void AddAnnotations_NodeBase()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        NodeBase shape = new Circle("geom");
        shape.AddAnnotations([doc, bom]);
        
        Assert.AreEqual(2, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc, bom }, shape.GetAnnotations().ToList());
    }

    [TestMethod]
    public void AddAnnotations_INode()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        INode shape = new Circle("geom");
        shape.AddAnnotations([doc, bom]);
        
        Assert.AreEqual(2, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc, bom }, shape.GetAnnotations().ToList());
    }

    [TestMethod]
    public void AddAnnotations_INotifiableNode()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        INotifiableNode shape = new Circle("geom");
        shape.AddAnnotations([doc, bom]);
        
        Assert.AreEqual(2, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc, bom }, shape.GetAnnotations().ToList());
    }
    
    [TestMethod]
    public void AddAnnotations_INotifiableNode_Generic()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        INotifiableNode<INode> shape = new Circle("geom");
        shape.AddAnnotations([doc, bom]);
        
        Assert.AreEqual(2, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc, bom }, shape.GetAnnotations().ToList());
    }


    [TestMethod]
    public void AddAnnotations_IWritableNode()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        IWritableNode shape = new Circle("geom");
        shape.AddAnnotations([doc, bom]);
        
        Assert.AreEqual(2, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc, bom }, shape.GetAnnotations().ToList());
    }
    
    [TestMethod]
    public void AddAnnotations_IWritableNode_Generic()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        IWritableNode<INode> shape = new Circle("geom");
        shape.AddAnnotations([doc, bom]);
        
        Assert.AreEqual(2, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc, bom }, shape.GetAnnotations().ToList());
    }
    
    #endregion

    #region InsertAnnotations

    [TestMethod]
    public void InsertAnnotations_NodeBase()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        NodeBase shape = new Circle("geom");
        shape.InsertAnnotations(0, [doc, bom]);
        
        Assert.AreEqual(2, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc, bom }, shape.GetAnnotations().ToList());
    }

    [TestMethod]
    public void InsertAnnotations_INode()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        INode shape = new Circle("geom");
        shape.InsertAnnotations(0, [doc, bom]);
        
        Assert.AreEqual(2, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc, bom }, shape.GetAnnotations().ToList());
    }

    [TestMethod]
    public void InsertAnnotations_INotifiableNode()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        INotifiableNode shape = new Circle("geom");
        shape.InsertAnnotations(0, [doc, bom]);
        
        Assert.AreEqual(2, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc, bom }, shape.GetAnnotations().ToList());
    }

    [TestMethod]
    public void InsertAnnotations_INotifiableNode_Generic()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        INotifiableNode<INode> shape = new Circle("geom");
        shape.InsertAnnotations(0, [doc, bom]);
        
        Assert.AreEqual(2, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc, bom }, shape.GetAnnotations().ToList());
    }

    [TestMethod]
    public void InsertAnnotations_IWritableNode()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        IWritableNode shape = new Circle("geom");
        shape.InsertAnnotations(0, [doc, bom]);
        
        Assert.AreEqual(2, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc, bom }, shape.GetAnnotations().ToList());
    }

    [TestMethod]
    public void InsertAnnotations_IWritableNode_Generic()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        IWritableNode<INode> shape = new Circle("geom");
        shape.InsertAnnotations(0, [doc, bom]);
        
        Assert.AreEqual(2, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc, bom }, shape.GetAnnotations().ToList());
    }
    
    #endregion

    #region RemoveAnnotations

    [TestMethod]
    public void RemoveAnnotations_NodeBase()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        NodeBase shape = new Circle("geom");
        shape.InsertAnnotations(0, [doc, bom]);
        shape.RemoveAnnotations([bom]);
        
        Assert.AreEqual(1, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc}, shape.GetAnnotations().ToList());
    }

    [TestMethod]
    public void RemoveAnnotations_INode()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        INode shape = new Circle("geom");
        shape.InsertAnnotations(0, [doc, bom]);
        shape.RemoveAnnotations([bom]);
        
        Assert.AreEqual(1, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc}, shape.GetAnnotations().ToList());
    }

    [TestMethod]
    public void RemoveAnnotations_INotifiableNode()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        INotifiableNode shape = new Circle("geom");
        shape.InsertAnnotations(0, [doc, bom]);
        shape.RemoveAnnotations([bom]);
        
        Assert.AreEqual(1, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc}, shape.GetAnnotations().ToList());
    }

    [TestMethod]
    public void RemoveAnnotations_INotifiableNode_Generic()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        INotifiableNode<INode> shape = new Circle("geom");
        shape.InsertAnnotations(0, [doc, bom]);
        shape.RemoveAnnotations([bom]);
        
        Assert.AreEqual(1, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc}, shape.GetAnnotations().ToList());
    }

    [TestMethod]
    public void RemoveAnnotations_IWritableNode()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        IWritableNode shape = new Circle("geom");
        shape.InsertAnnotations(0, [doc, bom]);
        shape.RemoveAnnotations([bom]);
        
        Assert.AreEqual(1, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc}, shape.GetAnnotations().ToList());
    }
    
    [TestMethod]
    public void RemoveAnnotations_IWritableNode_Generic()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        IWritableNode<INode> shape = new Circle("geom");
        shape.InsertAnnotations(0, [doc, bom]);
        shape.RemoveAnnotations([bom]);
        
        Assert.AreEqual(1, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc}, shape.GetAnnotations().ToList());
    }
    
    #endregion
    
}
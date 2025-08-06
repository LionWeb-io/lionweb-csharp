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

namespace LionWeb.Core.Test.NodeApi;

using Languages.Generated.V2023_1.Shapes.M2;

[TestClass]
public class NodeBaseTests
{
    #region Set

    [TestMethod]
    public void Set_NodeBase()
    {
        var coord = new Coord("coord");
        
        NodeBase shape = new Circle("geom");
        shape.Set(ShapesLanguage.Instance.Circle_center, coord);

        Assert.AreEqual(1, shape.CollectAllSetFeatures().ToList().Count);
        Assert.IsTrue(shape.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }
    
    [TestMethod]
    public void Set_INode()
    {
        var coord = new Coord("coord");
        
        INode shape = new Circle("geom");
        shape.Set(ShapesLanguage.Instance.Circle_center, coord);

        Assert.AreEqual(1, shape.CollectAllSetFeatures().ToList().Count);
        Assert.IsTrue(shape.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }
    
    [TestMethod]
    public void Set_IEventableNode()
    {
        var coord = new Coord("coord");
        
        IEventableNode shape = new Circle("geom");
        shape.Set(ShapesLanguage.Instance.Circle_center, coord);

        Assert.AreEqual(1, shape.CollectAllSetFeatures().ToList().Count);
        Assert.IsTrue(shape.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }

    [TestMethod]
    public void Set_IEventableNode_Generic()
    {
        var coord = new Coord("coord");
        
        IEventableNode<INode> shape = new Circle("geom");
        shape.Set(ShapesLanguage.Instance.Circle_center, coord);

        Assert.AreEqual(1, shape.CollectAllSetFeatures().ToList().Count);
        Assert.IsTrue(shape.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }

    [TestMethod]
    public void Set_IWritableNode()
    {
        var coord = new Coord("coord");
        
        IWritableNode shape = new Circle("geom");
        shape.Set(ShapesLanguage.Instance.Circle_center, coord);

        Assert.AreEqual(1, shape.CollectAllSetFeatures().ToList().Count);
        Assert.IsTrue(shape.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }
    
    [TestMethod]
    public void Set_IWritableNode_Generic()
    {
        var coord = new Coord("coord");
        
        IWritableNode<INode> shape = new Circle("geom");
        shape.Set(ShapesLanguage.Instance.Circle_center, coord);

        Assert.AreEqual(1, shape.CollectAllSetFeatures().ToList().Count);
        Assert.IsTrue(shape.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }
    
    #endregion
    
    #region Set method with eventId parameter set to null

    [TestMethod]
    public void Set_NodeBase_with_null_eventId()
    {
        var coord = new Coord("coord");
        
        NodeBase shape = new Circle("geom");
        shape.Set(ShapesLanguage.Instance.Circle_center, coord, null);

        Assert.AreEqual(1, shape.CollectAllSetFeatures().ToList().Count);
        Assert.IsTrue(shape.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }
    
    [TestMethod]
    public void Set_INode_with_null_eventId()
    {
        var coord = new Coord("coord");
        
        INode shape = new Circle("geom");
        shape.Set(ShapesLanguage.Instance.Circle_center, coord, null);

        Assert.AreEqual(1, shape.CollectAllSetFeatures().ToList().Count);
        Assert.IsTrue(shape.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }
    
    [TestMethod]
    public void Set_IEventableNode_with_null_eventId()
    {
        var coord = new Coord("coord");
        
        IEventableNode shape = new Circle("geom");
        shape.Set(ShapesLanguage.Instance.Circle_center, coord, null);

        Assert.AreEqual(1, shape.CollectAllSetFeatures().ToList().Count);
        Assert.IsTrue(shape.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }

    [TestMethod]
    public void Set_IEventableNode_Generic_with_null_eventId()
    {
        var coord = new Coord("coord");
        
        IEventableNode<INode> shape = new Circle("geom");
        shape.Set(ShapesLanguage.Instance.Circle_center, coord, null);

        Assert.AreEqual(1, shape.CollectAllSetFeatures().ToList().Count);
        Assert.IsTrue(shape.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }
    
    #endregion
    
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
    public void AddAnnotations_IEventableNode()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        IEventableNode shape = new Circle("geom");
        shape.AddAnnotations([doc, bom]);
        
        Assert.AreEqual(2, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc, bom }, shape.GetAnnotations().ToList());
    }
    
    [TestMethod]
    public void AddAnnotations_IEventableNode_Generic()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        IEventableNode<INode> shape = new Circle("geom");
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
    public void InsertAnnotations_IEventableNode()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        IEventableNode shape = new Circle("geom");
        shape.InsertAnnotations(0, [doc, bom]);
        
        Assert.AreEqual(2, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc, bom }, shape.GetAnnotations().ToList());
    }

    [TestMethod]
    public void InsertAnnotations_IEventableNode_Generic()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        IEventableNode<INode> shape = new Circle("geom");
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
    public void RemoveAnnotations_IEventableNode()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        IEventableNode shape = new Circle("geom");
        shape.InsertAnnotations(0, [doc, bom]);
        shape.RemoveAnnotations([bom]);
        
        Assert.AreEqual(1, shape.GetAnnotations().Count);
        CollectionAssert.AreEqual(new List<INode> { doc}, shape.GetAnnotations().ToList());
    }

    [TestMethod]
    public void RemoveAnnotations_IEventableNode_Generic()
    {
        var doc = new Documentation("circ0");
        var bom = new BillOfMaterials("off0");
        
        IEventableNode<INode> shape = new Circle("geom");
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
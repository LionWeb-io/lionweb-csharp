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

using LionWeb.Core.Test.Languages.Generated.V2023_1.Shapes.M2;
using NodeBase = Core.NodeBase;

[TestClass]
public class SetTests
{
    #region Set

    [TestMethod]
    public void NodeBase()
    {
        var coord = new Coord("coord");
        
        NodeBase shape = new Circle("geom");
        shape.Set(ShapesLanguage.Instance.Circle_center, coord);

        Assert.AreEqual(1, shape.CollectAllSetFeatures().ToList().Count);
        Assert.IsTrue(shape.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }
    
    [TestMethod]
    public void INode()
    {
        var coord = new Coord("coord");
        
        INode shape = new Circle("geom");
        shape.Set(ShapesLanguage.Instance.Circle_center, coord);

        Assert.AreEqual(1, shape.CollectAllSetFeatures().ToList().Count);
        Assert.IsTrue(shape.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }
    
    [TestMethod]
    public void INotifiableNode()
    {
        var coord = new Coord("coord");
        
        INotifiableNode shape = new Circle("geom");
        shape.Set(ShapesLanguage.Instance.Circle_center, coord);

        Assert.AreEqual(1, shape.CollectAllSetFeatures().ToList().Count);
        Assert.IsTrue(shape.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }

    [TestMethod]
    public void INotifiableNode_Generic()
    {
        var coord = new Coord("coord");
        
        INotifiableNode<INode> shape = new Circle("geom");
        shape.Set(ShapesLanguage.Instance.Circle_center, coord);

        Assert.AreEqual(1, shape.CollectAllSetFeatures().ToList().Count);
        Assert.IsTrue(shape.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }

    [TestMethod]
    public void IWritableNode()
    {
        var coord = new Coord("coord");
        
        IWritableNode shape = new Circle("geom");
        shape.Set(ShapesLanguage.Instance.Circle_center, coord);

        Assert.AreEqual(1, shape.CollectAllSetFeatures().ToList().Count);
        Assert.IsTrue(shape.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }
    
    [TestMethod]
    public void IWritableNode_Generic()
    {
        var coord = new Coord("coord");
        
        IWritableNode<INode> shape = new Circle("geom");
        shape.Set(ShapesLanguage.Instance.Circle_center, coord);

        Assert.AreEqual(1, shape.CollectAllSetFeatures().ToList().Count);
        Assert.IsTrue(shape.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }
    
    #endregion
    
    #region Set method with notificationId parameter set to null

    [TestMethod]
    public void NodeBase_with_null_notificationId()
    {
        var coord = new Coord("coord");
        
        NodeBase shape = new Circle("geom");
        shape.Set(ShapesLanguage.Instance.Circle_center, coord, null);

        Assert.AreEqual(1, shape.CollectAllSetFeatures().ToList().Count);
        Assert.IsTrue(shape.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }
    
    [TestMethod]
    public void INode_with_null_notificationId()
    {
        var coord = new Coord("coord");
        
        INode shape = new Circle("geom");
        shape.Set(ShapesLanguage.Instance.Circle_center, coord, null);

        Assert.AreEqual(1, shape.CollectAllSetFeatures().ToList().Count);
        Assert.IsTrue(shape.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }
    
    [TestMethod]
    public void INotifiableNode_with_null_notificationId()
    {
        var coord = new Coord("coord");
        
        INotifiableNode shape = new Circle("geom");
        shape.Set(ShapesLanguage.Instance.Circle_center, coord, null);

        Assert.AreEqual(1, shape.CollectAllSetFeatures().ToList().Count);
        Assert.IsTrue(shape.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }

    [TestMethod]
    public void INotifiableNode_Generic_with_null_notificationId()
    {
        var coord = new Coord("coord");
        
        INotifiableNode<INode> shape = new Circle("geom");
        shape.Set(ShapesLanguage.Instance.Circle_center, coord, null);

        Assert.AreEqual(1, shape.CollectAllSetFeatures().ToList().Count);
        Assert.IsTrue(shape.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }
    
    #endregion
}
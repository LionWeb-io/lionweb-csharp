﻿// Copyright 2024 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Test.NodeApi.Generated;

using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class ContainmentTests_Multiple_Required_Listener
{
    #region Single

    [TestMethod]
    public void Single_Add()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildAdded += (sender, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.containment);
            Assert.AreEqual(0, args.index);
            Assert.AreEqual(line, args.newChild);
        };

        compositeShape.AddParts([line]);

        Assert.AreEqual(1, events);
    }

    #region Insert

    [TestMethod]
    public void Single_Insert_Empty()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildAdded += (sender, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.containment);
            Assert.AreEqual(0, args.index);
            Assert.AreEqual(line, args.newChild);
        };

        compositeShape.InsertParts(0, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_One_Before()
    {
        var circle = new Circle("cId");
        var compositeShape = new CompositeShape("cs") { Parts = [circle] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildAdded += (sender, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.containment);
            Assert.AreEqual(0, args.index);
            Assert.AreEqual(line, args.newChild);
        };

        compositeShape.InsertParts(0, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_One_After()
    {
        var circle = new Circle("cId");
        var compositeShape = new CompositeShape("cs") { Parts = [circle] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");

        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildAdded += (sender, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.containment);
            Assert.AreEqual(1, args.index);
            Assert.AreEqual(line, args.newChild);
        };

        compositeShape.InsertParts(1, [line]);
 
        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");
    
        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildAdded += (sender, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.containment);
            Assert.AreEqual(0, args.index);
            Assert.AreEqual(line, args.newChild);
        };

        compositeShape.InsertParts(0, [line]);
 
        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");
 
        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildAdded += (sender, args) =>
        {
            Assert.AreSame(compositeShape, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.containment);
            Assert.AreEqual(1, args.index);
            Assert.AreEqual(line, args.newChild);
            events++;
        };

        compositeShape.InsertParts(1, [line]);
 
        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");
  
        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildAdded += (sender, args) =>
        {
            Assert.AreSame(compositeShape, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.containment);
            Assert.AreEqual(2, args.index);
            Assert.AreEqual(line, args.newChild);
            events++;
        };

        compositeShape.InsertParts(2, [line]);
     
        Assert.AreEqual(1, events);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Single_Remove_Empty()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");
    
        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildDeleted += (sender, args) =>
        {
            events++;
        };

        Assert.ThrowsException<InvalidValueException>(() => compositeShape.RemoveParts([line]));

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Single_Remove_NotContained()
    {
        var circle = new Circle("myC");
        var compositeShape = new CompositeShape("cs") { Parts = [circle] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var line = new Line("myId");
    
        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildDeleted += (sender, args) =>
        {
            events++;
        };

        compositeShape.RemoveParts([line]);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Single_Remove_Only()
    {
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { Parts = [line] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };

        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildDeleted += (sender, args) =>
        {
            events++;
        };

        Assert.ThrowsException<InvalidValueException>(() => compositeShape.RemoveParts([line]));

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Single_Remove_First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { Parts = [line, circle] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
   
        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildDeleted += (sender, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.containment);
            Assert.AreEqual(0, args.index);
            Assert.AreEqual(line, args.deletedChild);
        };

        compositeShape.RemoveParts([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Last()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { Parts = [circle, line] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
  
        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildDeleted += (sender, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.containment);
            Assert.AreEqual(1, args.index);
            Assert.AreEqual(line, args.deletedChild);
        };

        compositeShape.RemoveParts([line]);
 
        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, line, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
    
        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildDeleted += (sender, args) =>
        {
            events++;
            Assert.AreSame(compositeShape, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.containment);
            Assert.AreEqual(1, args.index);
            Assert.AreEqual(line, args.deletedChild);
        };

        compositeShape.RemoveParts([line]);
   
        Assert.AreEqual(1, events);
    }

    #endregion

    #endregion

    #region EmptyCollection

    [TestMethod]
    public void EmptyArray()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[0];
 
        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildAdded += (sender, args) =>
        {
            events++;
        };

        Assert.ThrowsException<InvalidValueException>(() => compositeShape.AddParts(values));
 
        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[0];
        Assert.ThrowsException<InvalidValueException>(
            () => compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        Assert.ThrowsException<UnsetFeatureException>(() => compositeShape.Parts.Count == 0);
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[0];
 
        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildAdded += (sender, args) =>
        {
            events++;
        };

        Assert.ThrowsException<InvalidValueException>(() => compositeShape.InsertParts(0, values));

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[0];
 
        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildAdded += (sender, args) =>
        {
            events++;
        };

        Assert.ThrowsException<InvalidValueException>(() => compositeShape.RemoveParts(values));

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var value = new Circle("myId");
        compositeShape.AddParts([value]);
        var values = new List<Coord>();
        Assert.ThrowsException<InvalidValueException>(
            () => compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, values));
        CollectionAssert.AreEqual(new List<IShape> { value }, compositeShape.Parts.ToList());
    }

    #endregion

    #region MultipleCollection

    [TestMethod]
    public void MultipleArray()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildAdded += (sender, args) =>
        {
            Assert.AreSame(compositeShape, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.containment);
            Assert.AreEqual(events, args.index);
            Assert.AreEqual(values[events], args.newChild);
            events++;
        };

        compositeShape.AddParts(values);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void MultipleArray_Reflective()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        compositeShape.Set(ShapesLanguage.Instance.CompositeShape_parts, values);
        Assert.AreSame(compositeShape, valueA.GetParent());
        Assert.IsTrue(compositeShape.Parts.Contains(valueA));
        Assert.AreSame(compositeShape, valueB.GetParent());
        Assert.IsTrue(compositeShape.Parts.Contains(valueB));
    }

    #region Insert

    [TestMethod]
    public void Multiple_Insert_Empty()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
    
        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildAdded += (sender, args) =>
        {
            Assert.AreSame(compositeShape, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.containment);
            Assert.AreEqual(events, args.index);
            Assert.AreEqual(values[events], args.newChild);
            events++;
        };

        compositeShape.InsertParts(0, values);
  
        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };

        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildAdded += (sender, args) =>
        {
            Assert.AreSame(compositeShape, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.containment);
            Assert.AreEqual(1 + events, args.index);
            Assert.AreEqual(values[events], args.newChild);
            events++;
        };

        compositeShape.InsertParts(1, values);
  
        Assert.AreEqual(2, events);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Multiple_Remove_ListMatchingType()
    {
        var compositeShape = new CompositeShape("cs");
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape> { valueA, valueB };
  
        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildDeleted += (sender, args) =>
        {
            events++;
        };

        Assert.ThrowsException<InvalidValueException>(() => compositeShape.RemoveParts(values));
   
        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Multiple_Remove_Only()
    {
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [valueA, valueB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };
  
        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildDeleted += (sender, args) =>
        {
            events++;
        };

        Assert.ThrowsException<InvalidValueException>(() => compositeShape.RemoveParts(values));
    
        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Multiple_Remove_NonContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
  
        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildDeleted += (sender, args) =>
        {
            events++;
        };

        compositeShape.RemoveParts(values);
  
        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Multiple_Remove_HalfContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var valueA = new Line("sA");
        var values = new IShape[] { valueA, circleA };
   
        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildDeleted += (sender, args) =>
        {
            Assert.AreSame(compositeShape, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.containment);
            Assert.AreEqual(0, args.index);
            Assert.AreEqual(circleA, args.deletedChild);
            events++;
        };

        compositeShape.RemoveParts(values);
 
        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Multiple_Remove_Last()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [circle, valueA, valueB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };
 
        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildDeleted += (sender, args) =>
        {
            Assert.AreSame(compositeShape, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.containment);
            Assert.AreEqual(1, args.index);
            Assert.AreEqual(values[events], args.deletedChild);
            events++;
        };

        compositeShape.RemoveParts(values);
  
        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [circleA, valueA, valueB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };
 
        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildDeleted += (sender, args) =>
        {
            Assert.AreSame(compositeShape, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.containment);
            Assert.AreEqual(1, args.index);
            Assert.AreEqual(values[events], args.deletedChild);
            events++;
        };

        compositeShape.RemoveParts(values);
 
        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Multiple_Remove_Mixed()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var compositeShape = new CompositeShape("cs") { Parts = [valueA, circleA, valueB, circleB] };
        var parent = new Geometry("g") { Shapes = [compositeShape] };
        var values = new IShape[] { valueA, valueB };
 
        int events = 0;
        int[] indexes = { 0, 1 };
        ((IPartitionInstance)parent).Listener.ChildDeleted += (sender, args) =>
        {
            Assert.AreSame(compositeShape, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.CompositeShape_parts, args.containment);
            Assert.AreEqual(indexes[events], args.index);
            Assert.AreEqual(values[events], args.deletedChild);
            events++;
        };

        compositeShape.RemoveParts(values);
      
        Assert.AreEqual(2, events);
    }

    #endregion

    #endregion
}
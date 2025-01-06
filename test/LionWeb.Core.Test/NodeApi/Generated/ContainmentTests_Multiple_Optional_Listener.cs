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

namespace LionWeb.Core.Test.NodeApi.Generated;

using Languages.Generated.V2024_1.Shapes.M2;
using System.Collections;

[TestClass]
public class ContainmentTests_Multiple_Optional_Listener
{
    #region Single

    [TestMethod]
    public void Single_Add()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");

        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildAdded += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.containment);
            Assert.AreEqual(0, args.index);
            Assert.AreEqual(line, args.newChild);
        };

        parent.AddShapes([line]);

        Assert.AreEqual(1, events);
    }


    #region Insert

    [TestMethod]
    public void Single_Insert_Empty()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");
        parent.InsertShapes(0, [line]);
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Single_Insert_One_Before()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g") { Shapes = [circle] };
        var line = new Line("myId");

        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildAdded += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.containment);
            Assert.AreEqual(0, args.index);
            Assert.AreEqual(line, args.newChild);
        };

        parent.InsertShapes(0, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_One_After()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g") { Shapes = [circle] };
        var line = new Line("myId");

        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildAdded += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.containment);
            Assert.AreEqual(1, args.index);
            Assert.AreEqual(line, args.newChild);
        };

        parent.InsertShapes(1, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildAdded += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.containment);
            Assert.AreEqual(0, args.index);
            Assert.AreEqual(line, args.newChild);
        };

        parent.InsertShapes(0, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildAdded += (sender, args) =>
        {
            Assert.AreSame(parent, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.containment);
            Assert.AreEqual(1, args.index);
            Assert.AreEqual(line, args.newChild);
            events++;
        };

        parent.InsertShapes(1, [line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Insert_Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var line = new Line("myId");

        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildAdded += (sender, args) =>
        {
            Assert.AreSame(parent, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.containment);
            Assert.AreEqual(2, args.index);
            Assert.AreEqual(line, args.newChild);
            events++;
        };

        parent.InsertShapes(2, [line]);

        Assert.AreEqual(1, events);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Single_Remove_Empty()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");

        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildDeleted += (sender, args) =>
        {
            events++;
        };

        parent.RemoveShapes([line]);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Single_Remove_NotContained()
    {
        var circle = new Circle("myC");
        var parent = new Geometry("cs") { Shapes = [circle] };
        var line = new Line("myId");

        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildDeleted += (sender, args) =>
        {
            events++;
        };

        parent.RemoveShapes([line]);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Single_Remove_Only()
    {
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [line] };

        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildDeleted += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.containment);
            Assert.AreEqual(0, args.index);
            Assert.AreEqual(line, args.deletedChild);
        };

        parent.RemoveShapes([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [line, circle] };

        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildDeleted += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.containment);
            Assert.AreEqual(0, args.index);
            Assert.AreEqual(line, args.deletedChild);
        };

        parent.RemoveShapes([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Last()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [circle, line] };

        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildDeleted += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.containment);
            Assert.AreEqual(1, args.index);
            Assert.AreEqual(line, args.deletedChild);
        };

        parent.RemoveShapes([line]);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [circleA, line, circleB] };

        int events = 0;
        ((IPartitionInstance)parent).Listener.ChildDeleted += (sender, args) =>
        {
            events++;
            Assert.AreSame(parent, args.parent);
            Assert.AreSame(ShapesLanguage.Instance.Geometry_shapes, args.containment);
            Assert.AreEqual(1, args.index);
            Assert.AreEqual(line, args.deletedChild);
        };

        parent.RemoveShapes([line]);

        Assert.AreEqual(1, events);
    }

    #endregion

    #endregion

    #region Null

    [TestMethod]
    public void Null()
    {
        var parent = new Geometry("g");
        Assert.ThrowsException<InvalidValueException>(() => parent.AddShapes(null));
    }

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = new Geometry("g");
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(ShapesLanguage.Instance.Geometry_shapes, null));
    }

    [TestMethod]
    public void Null_Constructor()
    {
        Assert.ThrowsException<InvalidValueException>(() => new Geometry("g") { Shapes = [null] });
    }

    [TestMethod]
    public void Null_Insert_Empty()
    {
        var parent = new Geometry("g");
        Assert.ThrowsException<InvalidValueException>(() => parent.InsertShapes(0, null));
    }

    [TestMethod]
    public void Null_Insert_Empty_OutOfBounds()
    {
        var parent = new Geometry("g");
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => parent.InsertShapes(1, null));
    }

    [TestMethod]
    public void Null_Remove_Empty()
    {
        var parent = new Geometry("g");
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveShapes(null));
    }

    #endregion

    #region EmptyCollection

    [TestMethod]
    public void EmptyArray()
    {
        var parent = new Geometry("g");
        var values = new IShape[0];
        parent.AddShapes(values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = new Geometry("g");
        var values = new IShape[0];
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void EmptyArray_Constructor()
    {
        var values = new IShape[0];
        var parent = new Geometry("g") { Shapes = values };
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var parent = new Geometry("g");
        var values = new IShape[0];
        parent.InsertShapes(0, values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var parent = new Geometry("g");
        var values = new IShape[0];
        parent.RemoveShapes(values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void EmptyUntypedList_Reflective()
    {
        var parent = new Geometry("g");
        var values = new ArrayList();
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void EmptyListMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var values = new List<IShape>();
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void EmptyListSubtype_Reflective()
    {
        var parent = new Geometry("g");
        var values = new List<Shape>();
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void EmptySet_Reflective()
    {
        var parent = new Geometry("g");
        var values = new HashSet<IShape>();
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void EmptyListNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var values = new List<Coord>();
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void EmptyList_Reset_Reflective()
    {
        var parent = new Geometry("g");
        parent.AddShapes([new Circle("myId")]);
        var values = new List<Coord>();
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray()
    {
        var parent = new Geometry("g");
        var values = new IShape[] { null };
        Assert.ThrowsException<InvalidValueException>(() => parent.AddShapes(values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = new Geometry("g");
        var values = new IShape[] { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void NullArray_Constructor()
    {
        var values = new IShape[] { null };
        Assert.ThrowsException<InvalidValueException>(() => new Geometry("g") { Shapes = values });
    }

    [TestMethod]
    public void Insert_NullArray()
    {
        var parent = new Geometry("g");
        var values = new IShape[] { null };
        Assert.ThrowsException<InvalidValueException>(() => parent.InsertShapes(0, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void Remove_NullArray()
    {
        var parent = new Geometry("g");
        var values = new IShape[] { null };
        Assert.ThrowsException<InvalidValueException>(() => parent.RemoveShapes(values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void NullUntypedList_Reflective()
    {
        var parent = new Geometry("g");
        var values = new ArrayList() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void NullListMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var values = new List<IShape>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void NullListSubtype_Reflective()
    {
        var parent = new Geometry("g");
        var values = new List<Shape>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void NullListNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var values = new List<Coord>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void NullSet_Reflective()
    {
        var parent = new Geometry("g");
        var values = new HashSet<IShape>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray()
    {
        var parent = new Geometry("g");
        var value = new Line("s");
        var values = new IShape[] { value };
        parent.AddShapes(values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(value));
    }

    [TestMethod]
    public void SingleArray_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Line("s");
        var values = new IShape[] { value };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(value));
    }

    [TestMethod]
    public void SingleArray_Existing_Reflective()
    {
        var circle = new Circle("cc");
        var parent = new Geometry("g") { Shapes = [circle] };
        var value = new Line("s");
        var values = new IShape[] { value };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.IsNull(circle.GetParent());
        Assert.AreSame(parent, value.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { value }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void SingleArray_Constructor()
    {
        var value = new Line("s");
        var values = new IShape[] { value };
        var parent = new Geometry("g") { Shapes = values };
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(value));
    }

    [TestMethod]
    public void Insert_SingleArray()
    {
        var parent = new Geometry("g");
        var value = new Line("s");
        var values = new IShape[] { value };
        parent.InsertShapes(0, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(value));
    }

    #region Remove

    [TestMethod]
    public void SingleArray_Remove_Empty()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");
        var values = new IShape[] { line };
        parent.RemoveShapes(values);
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void SingleArray_Remove_Only()
    {
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [line] };
        var values = new IShape[] { line };
        parent.RemoveShapes(values);
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_First()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [line, circle] };
        var values = new IShape[] { line };
        parent.RemoveShapes(values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_Last()
    {
        var circle = new Circle("cId");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [circle, line] };
        var values = new IShape[] { line };
        parent.RemoveShapes(values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var line = new Line("myId");
        var parent = new Geometry("g") { Shapes = [circleA, line, circleB] };
        var values = new IShape[] { line };
        parent.RemoveShapes(values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Shapes.ToList());
    }

    #endregion

    [TestMethod]
    public void SingleUntypedArray_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Line("s");
        var values = new object[] { value };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(value));
    }

    [TestMethod]
    public void SingleUntypedList_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Line("s");
        var values = new ArrayList() { value };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(value));
    }

    [TestMethod]
    public void SingleListMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Line("s");
        var values = new List<IShape>() { value };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(value));
    }

    [TestMethod]
    public void SingleListSubtype_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Line("s");
        var values = new List<Shape>() { value };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(value));
    }

    [TestMethod]
    public void SingleSet_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Line("s");
        var values = new HashSet<IShape>() { value };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(value));
    }

    [TestMethod]
    public void SingleListNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Coord("c");
        var values = new List<Coord>() { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void SingleUntypedListNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Coord("c");
        var values = new ArrayList() { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void SingleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Coord("c");
        var values = new object[] { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    #endregion

    #region MultipleCollection

    [TestMethod]
    public void MultipleArray()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.AddShapes(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void MultipleArray_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void MultipleArray_Constructor()
    {
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape> { valueA, valueB };
        var parent = new Geometry("g") { Shapes = values };
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueB));
    }

    #region Insert

    [TestMethod]
    public void Multiple_Insert_ListMatchingType()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape> { valueA, valueB };
        parent.InsertShapes(0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Multiple_Insert_ListSubtype()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<Shape> { valueA, valueB };
        parent.InsertShapes(0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Set()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new HashSet<IShape> { valueA, valueB };
        parent.InsertShapes(0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Multiple_Insert_SingleEnumerable()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new SingleEnumerable<IShape> { valueA, valueB };
        parent.InsertShapes(0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Empty()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.InsertShapes(0, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Multiple_Insert_One_Before()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g") { Shapes = [circle] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.InsertShapes(0, values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB, circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Multiple_Insert_One_After()
    {
        var circle = new Circle("cId");
        var parent = new Geometry("g") { Shapes = [circle] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.InsertShapes(1, values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle, valueA, valueB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Two_Before()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.InsertShapes(0, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { valueA, valueB, circleA, circleB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Two_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.InsertShapes(1, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, valueA, valueB, circleB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Two_After()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var parent = new Geometry("g") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.InsertShapes(2, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB, valueA, valueB }, parent.Shapes.ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Multiple_Remove_ListMatchingType()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape>() { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(valueA));
        Assert.IsFalse(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void Multiple_Remove_ListSubtype()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<Shape>() { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(valueA));
        Assert.IsFalse(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void Multiple_Remove_Set()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new HashSet<IShape>() { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(valueA));
        Assert.IsFalse(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void Multiple_Remove_SingleEnumerable()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new SingleEnumerable<IShape>() { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(valueA));
        Assert.IsFalse(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void Multiple_Remove_Empty()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(valueA));
        Assert.IsFalse(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void Multiple_Remove_NonContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new Geometry("cs") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Multiple_Remove_HalfContained()
    {
        var circleA = new Circle("cA");
        var circleB = new Circle("cB");
        var parent = new Geometry("cs") { Shapes = [circleA, circleB] };
        var valueA = new Line("sA");
        var values = new IShape[] { valueA, circleA };
        parent.RemoveShapes(values);
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(circleA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Multiple_Remove_Only()
    {
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [valueA, valueB] };
        var values = new IShape[] { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Multiple_Remove_First()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [valueA, valueB, circle] };
        var values = new IShape[] { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Multiple_Remove_Last()
    {
        var circle = new Circle("cId");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [circle, valueA, valueB] };
        var values = new IShape[] { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circle }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Multiple_Remove_Between()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [circleA, valueA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Shapes.ToList());
    }

    [TestMethod]
    public void Multiple_Remove_Mixed()
    {
        var circleA = new Circle("cIdA");
        var circleB = new Circle("cIdB");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var parent = new Geometry("g") { Shapes = [valueA, circleA, valueB, circleB] };
        var values = new IShape[] { valueA, valueB };
        parent.RemoveShapes(values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { circleA, circleB }, parent.Shapes.ToList());
    }

    #endregion

    [TestMethod]
    public void MultipleUntypedArray_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new object[] { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void MultipleUntypedList_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new ArrayList() { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void MultipleListMatchingType()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape>() { valueA, valueB };
        parent.AddShapes(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void MultipleListMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<IShape>() { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void MultipleListSubtype()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<Shape>() { valueA, valueB };
        parent.AddShapes(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void MultipleListSubtype_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<Shape>() { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void MultipleSet()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new HashSet<IShape>() { valueA, valueB };
        parent.AddShapes(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void MultipleSet_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new HashSet<IShape>() { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void MultipleSingleEnumerable()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new SingleEnumerable<IShape> { valueA, valueB };
        parent.AddShapes(values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void MultipleSingleEnumerable_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new SingleEnumerable<IShape> { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(valueB));
    }

    [TestMethod]
    public void MultipleListNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Coord("cA");
        var valueB = new Coord("cB");
        var values = new List<Coord>() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void MultipleUntypedListNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Coord("cA");
        var valueB = new Coord("cB");
        var values = new ArrayList() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void MultipleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Coord("cA");
        var valueB = new Coord("cB");
        var values = new object[] { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_shapes, values));
        Assert.IsTrue(parent.Shapes.Count == 0);
    }

    [TestMethod]
    public void Result_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        var result = parent.Get(ShapesLanguage.Instance.Geometry_shapes);
        CollectionAssert.AreEqual(new List<INode>() { valueA, valueB }, (result as IEnumerable<INode>).ToList());
    }

    [TestMethod]
    public void ResultUnmodifiable_Set()
    {
        var parent = new Geometry("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new IShape[] { valueA, valueB };
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, values);
        var result = parent.Get(ShapesLanguage.Instance.Geometry_shapes);
        Assert.IsInstanceOfType<IReadOnlyList<INode>>(result);
    }

    [TestMethod]
    public void ResultUnmodifiable_Unset()
    {
        var parent = new Geometry("g");
        var result = parent.Get(ShapesLanguage.Instance.Geometry_shapes);
        Assert.IsInstanceOfType<IReadOnlyList<INode>>(result);
    }

    #endregion
}
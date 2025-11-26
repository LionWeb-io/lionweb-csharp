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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment;

using Languages.Generated.V2024_1.Shapes.M2;
using M3;

[TestClass]
public class SetFeaturesTests
{
    #region single

    #region optional

    [TestMethod]
    public void ContainmentSingleOptional_Init()
    {
        var parent = new Geometry("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentSingleOptional_Set()
    {
        var parent = new Geometry("g");
        var doc = new Documentation("myId");
        parent.Documentation = doc;
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.Geometry_documentation },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentSingleOptional_Set_Reflective()
    {
        var parent = new Geometry("g");
        var doc = new Documentation("myId");
        parent.Set(ShapesLanguage.Instance.Geometry_documentation, doc);
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.Geometry_documentation },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentSingleOptional_Unset()
    {
        var parent = new Geometry("g");
        var doc = new Documentation("myId");
        parent.Documentation = doc;
        parent.Documentation = null;
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentSingleOptional_Unset_Reflective()
    {
        var parent = new Geometry("g");
        var doc = new Documentation("myId");
        parent.Documentation = doc;
        parent.Set(ShapesLanguage.Instance.Geometry_documentation, null);
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region required

    [TestMethod]
    public void ContainmentSingleRequired_Init()
    {
        var parent = new OffsetDuplicate("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentSingleRequired_Set()
    {
        var parent = new OffsetDuplicate("g");
        var coord = new Coord("myId");
        parent.Offset = coord;
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.OffsetDuplicate_offset },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentSingleRequired_Set_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var coord = new Coord("myId");
        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.OffsetDuplicate_offset },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #endregion

    #region multiple

    #region optional

    [TestMethod]
    public void ContainmentMultipleOptional_Init()
    {
        var parent = new Geometry("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Add()
    {
        var parent = new Geometry("g");
        parent.AddShapes([new Circle("myId")]);
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.Geometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Insert()
    {
        var parent = new Geometry("g");
        parent.InsertShapes(0, [new Circle("myId")]);
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.Geometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Set_Reflective()
    {
        var parent = new Geometry("g");
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<IShape> { new Circle("myId") });
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.Geometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Remove()
    {
        var parent = new Geometry("g");
        var value = new Circle("myId");
        parent.AddShapes([value]);
        parent.RemoveShapes([value]);
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleOptional_RemovePart()
    {
        var parent = new Geometry("g");
        var valueA = new Circle("myA");
        var valueB = new Circle("myB");
        parent.AddShapes([valueA, valueB]);
        parent.RemoveShapes([valueA]);
        CollectionAssert.AreEqual(new List<Feature>() { ShapesLanguage.Instance.Geometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Reset_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Circle("myId");
        parent.AddShapes([value]);
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<IShape>());
        CollectionAssert.AreEqual(new List<Feature>(),
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Overwrite_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Circle("myA");
        parent.AddShapes([value]);
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<IShape> { new Circle("myB") });
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.Geometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region required

    [TestMethod]
    public void ContainmentMultipleRequired_Init()
    {
        var parent = new Geometry("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleRequired_Add()
    {
        var parent = new Geometry("g");
        parent.AddShapes([new Circle("myId")]);
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.Geometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleRequired_Insert()
    {
        var parent = new Geometry("g");
        parent.InsertShapes(0, [new Circle("myId")]);
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.Geometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleRequired_Set_Reflective()
    {
        var parent = new Geometry("g");
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<IShape> { new Circle("myId") });
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.Geometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleRequired_RemovePart()
    {
        var parent = new Geometry("g");
        var valueA = new Circle("myA");
        var valueB = new Circle("myB");
        parent.AddShapes([valueA, valueB]);
        parent.RemoveShapes([valueA]);
        CollectionAssert.AreEqual(new List<Feature>() { ShapesLanguage.Instance.Geometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }


    [TestMethod]
    public void ContainmentMultipleRequired_Overwrite_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Circle("myA");
        parent.AddShapes([valueA]);
        var valueB = new Circle("myB");
        parent.Set(ShapesLanguage.Instance.Geometry_shapes, new List<IShape> { valueB });
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.Geometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #endregion
}
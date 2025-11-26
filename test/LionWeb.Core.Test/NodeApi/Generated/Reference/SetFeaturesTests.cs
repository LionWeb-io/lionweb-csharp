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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference;

using Languages.Generated.V2024_1.Shapes.M2;
using M3;

[TestClass]
public class SetFeaturesTests
{
    #region single

    #region optional

    [TestMethod]
    public void ReferenceSingleOptional_Init()
    {
        var parent = new OffsetDuplicate("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceSingleOptional_Set()
    {
        var parent = new OffsetDuplicate("g");
        parent.AltSource = new Line("myId");
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.OffsetDuplicate_altSource },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceSingleOptional_Set_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, new Line("myId"));
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.OffsetDuplicate_altSource },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceSingleOptional_Unset()
    {
        var parent = new OffsetDuplicate("g");
        parent.AltSource = new Line("myId");
        parent.AltSource = null;
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceSingleOptional_Unset_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        parent.AltSource = new Line("myId");
        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, null);
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region required

    [TestMethod]
    public void ReferenceSingleRequired_Init()
    {
        var parent = new OffsetDuplicate("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceSingleRequired_Set()
    {
        var parent = new OffsetDuplicate("g");
        parent.Source = new Line("myId");
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.OffsetDuplicate_source },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceSingleRequired_Set_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_source, new Line("myId"));
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.OffsetDuplicate_source },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #endregion

    #region multiple

    #region optional

    [TestMethod]
    public void ReferenceMultipleOptional_Init()
    {
        var parent = new ReferenceGeometry("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleOptional_Add()
    {
        var parent = new ReferenceGeometry("g");
        parent.AddShapes([new Circle("myId")]);
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.ReferenceGeometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleOptional_Insert()
    {
        var parent = new ReferenceGeometry("g");
        parent.InsertShapes(0, [new Circle("myId")]);
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.ReferenceGeometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleOptional_Set_Reflective()
    {
        var parent = new ReferenceGeometry("g");
        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<IShape> { new Circle("myId") });
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.ReferenceGeometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleOptional_Remove()
    {
        var parent = new ReferenceGeometry("g");
        var value = new Circle("myId");
        parent.AddShapes([value]);
        parent.RemoveShapes([value]);
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleOptional_RemovePart()
    {
        var parent = new ReferenceGeometry("g");
        var valueA = new Circle("myA");
        var valueB = new Circle("myB");
        parent.AddShapes([valueA, valueB]);
        parent.RemoveShapes([valueA]);
        CollectionAssert.AreEqual(new List<Feature>() { ShapesLanguage.Instance.ReferenceGeometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleOptional_Reset_Reflective()
    {
        var parent = new ReferenceGeometry("g");
        var value = new Circle("myId");
        parent.AddShapes([value]);
        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<IShape>());
        CollectionAssert.AreEqual(new List<Feature>(),
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleOptional_Overwrite_Reflective()
    {
        var parent = new ReferenceGeometry("g");
        var value = new Circle("myA");
        parent.AddShapes([value]);
        parent.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, new List<IShape> { new Circle("myB") });
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.ReferenceGeometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region required

    [TestMethod]
    public void ReferenceMultipleRequired_Init()
    {
        var parent = new MaterialGroup("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleRequired_Add()
    {
        var parent = new MaterialGroup("g");
        parent.AddMaterials([new Circle("myId")]);
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.MaterialGroup_materials },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleRequired_Insert()
    {
        var parent = new MaterialGroup("g");
        parent.InsertMaterials(0, [new Circle("myId")]);
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.MaterialGroup_materials },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleRequired_Set_Reflective()
    {
        var parent = new MaterialGroup("g");
        parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, new List<IShape> { new Circle("myId") });
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.MaterialGroup_materials },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleRequired_RemovePart()
    {
        var parent = new MaterialGroup("g");
        var valueA = new Circle("myA");
        var valueB = new Circle("myB");
        parent.AddMaterials([valueA, valueB]);
        parent.RemoveMaterials([valueA]);
        CollectionAssert.AreEqual(new List<Feature>() { ShapesLanguage.Instance.MaterialGroup_materials },
            parent.CollectAllSetFeatures().ToList());
    }


    [TestMethod]
    public void ReferenceMultipleRequired_Overwrite_Reflective()
    {
        var parent = new MaterialGroup("g");
        var valueA = new Circle("myA");
        parent.AddMaterials([valueA]);
        var valueB = new Circle("myB");
        parent.Set(ShapesLanguage.Instance.MaterialGroup_materials, new List<IShape> { valueB });
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.MaterialGroup_materials },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #endregion
}
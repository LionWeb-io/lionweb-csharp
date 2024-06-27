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

namespace LionWeb.Core.M2.Dynamic.Test;

using Examples.Shapes.M2;
using M3;

[TestClass]
public class SetFeaturesTests:DynamicNodeTestsBase
{
    #region property

    #region string

    [TestMethod]
    public void String_Init()
    {
        var parent = newDocumentation("od");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void String_Set_Reflective()
    {
        var parent = newDocumentation("od");
        parent.Set(Documentation_text, "hello");
        CollectionAssert.AreEqual(new List<Feature> { Documentation_text },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void String_Unset_Reflective()
    {
        var parent = newDocumentation("od");
        parent.Set(Documentation_text, "hello");
        parent.Set(Documentation_text, null);
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region integer

    [TestMethod]
    public void Integer_Init()
    {
        var parent = newCircle("od");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Integer_Set_Reflective()
    {
        var parent = newCircle("od");
        parent.Set(Circle_r, 10);
        CollectionAssert.AreEqual(new List<Feature> { Circle_r },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region boolean

    [TestMethod]
    public void Boolean_Init()
    {
        var parent = newDocumentation("od");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Boolean_Set_Reflective()
    {
        var parent = newDocumentation("od");
        parent.Set(Documentation_technical, true);
        CollectionAssert.AreEqual(new List<Feature> { Documentation_technical },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Boolean_Unset_Reflective()
    {
        var parent = newDocumentation("od");
        parent.Set(Documentation_technical, true);
        parent.Set(Documentation_technical, null);
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region enum

    [TestMethod]
    public void Enum_Init()
    {
        var parent = newMaterialGroup("od");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Enum_Set_Reflective()
    {
        var parent = newMaterialGroup("od");
        parent.Set(MaterialGroup_matterState, MatterState_Gas);
        CollectionAssert.AreEqual(new List<Feature> { MaterialGroup_matterState },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Enum_Unset_Reflective()
    {
        var parent = newMaterialGroup("od");
        parent.Set(MaterialGroup_matterState, MatterState_Gas);
        parent.Set(MaterialGroup_matterState, null);
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #endregion

    #region containment

    #region single

    #region optional

    [TestMethod]
    public void ContainmentSingleOptional_Init()
    {
        var parent = newGeometry("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentSingleOptional_Set_Reflective()
    {
        var parent = newGeometry("g");
        var doc = newDocumentation("myId");
        parent.Set(Geometry_documentation, doc);
        CollectionAssert.AreEqual(new List<Feature> { Geometry_documentation },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentSingleOptional_Unset_Reflective()
    {
        var parent = newGeometry("g");
        var doc = newDocumentation("myId");
        parent.Set(Geometry_documentation, doc);
        parent.Set(Geometry_documentation, null);
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region required

    [TestMethod]
    public void ContainmentSingleRequired_Init()
    {
        var parent = newOffsetDuplicate("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentSingleRequired_Set_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var coord = newCoord("myId");
        parent.Set(OffsetDuplicate_offset, coord);
        CollectionAssert.AreEqual(new List<Feature> { OffsetDuplicate_offset },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #endregion

    #region multiple

    #region optional

    [TestMethod]
    public void ContainmentMultipleOptional_Init()
    {
        var parent = newGeometry("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Set_Reflective()
    {
        var parent = newGeometry("g");
        parent.Set(Geometry_shapes, new List<DynamicNode> { newCircle("myId") });
        CollectionAssert.AreEqual(new List<Feature> { Geometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Reset_Reflective()
    {
        var parent = newGeometry("g");
        var value = newCircle("myId");
        parent.Set(Geometry_shapes, new List<DynamicNode>{value});
        parent.Set(Geometry_shapes, new List<DynamicNode>());
        CollectionAssert.AreEqual(new List<Feature>(),
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Overwrite_Reflective()
    {
        var parent = newGeometry("g");
        var value = newCircle("myA");
        parent.Set(Geometry_shapes, new List<DynamicNode>{value});
        parent.Set(Geometry_shapes, new List<DynamicNode> { newCircle("myB") });
        CollectionAssert.AreEqual(new List<Feature> { Geometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region required

    [TestMethod]
    public void ContainmentMultipleRequired_Init()
    {
        var parent = newGeometry("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleRequired_Set_Reflective()
    {
        var parent = newGeometry("g");
        parent.Set(Geometry_shapes, new List<DynamicNode> { newCircle("myId") });
        CollectionAssert.AreEqual(new List<Feature> { Geometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleRequired_Overwrite_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newCircle("myA");
        parent.Set(Geometry_shapes, new List<DynamicNode> { valueA });
        var valueB = newCircle("myB");
        parent.Set(Geometry_shapes, new List<DynamicNode> { valueB });
        CollectionAssert.AreEqual(new List<Feature> { Geometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #endregion

    #endregion

    #region reference

    #region single

    #region optional

    [TestMethod]
    public void ReferenceSingleOptional_Init()
    {
        var parent = newOffsetDuplicate("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceSingleOptional_Set_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        parent.Set(OffsetDuplicate_altSource, newLine("myId"));
        CollectionAssert.AreEqual(new List<Feature> { OffsetDuplicate_altSource },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceSingleOptional_Unset_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        parent.Set(OffsetDuplicate_altSource, newLine("myId"));
        parent.Set(OffsetDuplicate_altSource, null);
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region required

    [TestMethod]
    public void ReferenceSingleRequired_Init()
    {
        var parent = newOffsetDuplicate("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceSingleRequired_Set_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        parent.Set(OffsetDuplicate_source, newLine("myId"));
        CollectionAssert.AreEqual(new List<Feature> { OffsetDuplicate_source },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #endregion

    #region multiple

    #region optional

    [TestMethod]
    public void ReferenceMultipleOptional_Init()
    {
        var parent = newReferenceGeometry("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleOptional_Set_Reflective()
    {
        var parent = newReferenceGeometry("g");
        parent.Set(ReferenceGeometry_shapes, new List<DynamicNode> { newCircle("myId") });
        CollectionAssert.AreEqual(new List<Feature> { ReferenceGeometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleOptional_Reset_Reflective()
    {
        var parent = newReferenceGeometry("g");
        var value = newCircle("myId");
        parent.Set(ReferenceGeometry_shapes, new List<DynamicNode>{value});
        parent.Set(ReferenceGeometry_shapes, new List<DynamicNode>());
        CollectionAssert.AreEqual(new List<Feature>(),
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleOptional_Overwrite_Reflective()
    {
        var parent = newReferenceGeometry("g");
        var value = newCircle("myA");
        parent.Set(ReferenceGeometry_shapes, new List<DynamicNode> { value });
        parent.Set(ReferenceGeometry_shapes, new List<DynamicNode> { newCircle("myB") });
        CollectionAssert.AreEqual(new List<Feature> { ReferenceGeometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region required

    [TestMethod]
    public void ReferenceMultipleRequired_Init()
    {
        var parent = newMaterialGroup("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleRequired_Set_Reflective()
    {
        var parent = newMaterialGroup("g");
        parent.Set(MaterialGroup_materials, new List<DynamicNode> { newCircle("myId") });
        CollectionAssert.AreEqual(new List<Feature> { MaterialGroup_materials },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleRequired_Overwrite_Reflective()
    {
        var parent = newMaterialGroup("g");
        var valueA = newCircle("myA");
        parent.Set(MaterialGroup_materials, new List<DynamicNode> { valueA });
        var valueB = newCircle("myB");
        parent.Set(MaterialGroup_materials, new List<DynamicNode> { valueB });
        CollectionAssert.AreEqual(new List<Feature> { MaterialGroup_materials },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #endregion

    #endregion
}
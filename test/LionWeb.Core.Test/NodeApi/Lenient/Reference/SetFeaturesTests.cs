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

namespace LionWeb.Core.Test.NodeApi.Lenient.Reference;

using M3;

[TestClass]
public class SetFeaturesTests : LenientNodeTestsBase
{
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
        parent.Set(ReferenceGeometry_shapes, new List<LenientNode> { newCircle("myId") });
        CollectionAssert.AreEqual(new List<Feature> { ReferenceGeometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleOptional_Reset_Reflective()
    {
        var parent = newReferenceGeometry("g");
        var value = newCircle("myId");
        parent.Set(ReferenceGeometry_shapes, new List<LenientNode> { value });
        parent.Set(ReferenceGeometry_shapes, new List<LenientNode>());
        CollectionAssert.AreEqual(new List<Feature>(),
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleOptional_Overwrite_Reflective()
    {
        var parent = newReferenceGeometry("g");
        var value = newCircle("myA");
        parent.Set(ReferenceGeometry_shapes, new List<LenientNode> { value });
        parent.Set(ReferenceGeometry_shapes, new List<LenientNode> { newCircle("myB") });
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
        parent.Set(MaterialGroup_materials, new List<LenientNode> { newCircle("myId") });
        CollectionAssert.AreEqual(new List<Feature> { MaterialGroup_materials },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleRequired_Overwrite_Reflective()
    {
        var parent = newMaterialGroup("g");
        var valueA = newCircle("myA");
        parent.Set(MaterialGroup_materials, new List<LenientNode> { valueA });
        var valueB = newCircle("myB");
        parent.Set(MaterialGroup_materials, new List<LenientNode> { valueB });
        CollectionAssert.AreEqual(new List<Feature> { MaterialGroup_materials },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #endregion
}
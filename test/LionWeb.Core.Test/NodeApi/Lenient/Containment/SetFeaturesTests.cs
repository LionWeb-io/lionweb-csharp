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

namespace LionWeb.Core.Test.NodeApi.Lenient.Containment;

using M3;

[TestClass]
public class SetFeaturesTests : LenientNodeTestsBase
{
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
        parent.Set(Geometry_shapes, new List<LenientNode> { newCircle("myId") });
        CollectionAssert.AreEqual(new List<Feature> { Geometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Reset_Reflective()
    {
        var parent = newGeometry("g");
        var value = newCircle("myId");
        parent.Set(Geometry_shapes, new List<LenientNode> { value });
        parent.Set(Geometry_shapes, new List<LenientNode>());
        CollectionAssert.AreEqual(new List<Feature>(),
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Overwrite_Reflective()
    {
        var parent = newGeometry("g");
        var value = newCircle("myA");
        parent.Set(Geometry_shapes, new List<LenientNode> { value });
        parent.Set(Geometry_shapes, new List<LenientNode> { newCircle("myB") });
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
        parent.Set(Geometry_shapes, new List<LenientNode> { newCircle("myId") });
        CollectionAssert.AreEqual(new List<Feature> { Geometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleRequired_Overwrite_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newCircle("myA");
        parent.Set(Geometry_shapes, new List<LenientNode> { valueA });
        var valueB = newCircle("myB");
        parent.Set(Geometry_shapes, new List<LenientNode> { valueB });
        CollectionAssert.AreEqual(new List<Feature> { Geometry_shapes },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #endregion
}
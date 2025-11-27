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

namespace LionWeb.Core.Test.NodeApi.Lenient.Property;

using M3;

[TestClass]
public class SetFeaturesTests : LenientNodeTestsBase
{
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
}
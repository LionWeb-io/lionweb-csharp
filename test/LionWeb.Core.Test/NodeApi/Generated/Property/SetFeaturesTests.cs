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

namespace LionWeb.Core.Test.NodeApi.Generated.Property;

using Languages.Generated.V2024_1.Shapes.M2;
using M3;

[TestClass]
public class SetFeaturesTests
{
    #region string

    [TestMethod]
    public void String_Init()
    {
        var parent = new Documentation("od");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void String_Set()
    {
        var parent = new Documentation("od");
        parent.Text = "hello";
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.Documentation_text },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void String_Set_Reflective()
    {
        var parent = new Documentation("od");
        parent.Set(ShapesLanguage.Instance.Documentation_text, "hello");
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.Documentation_text },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void String_Unset()
    {
        var parent = new Documentation("od");
        parent.Text = "hello";
        parent.Text = null;
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void String_Unset_Reflective()
    {
        var parent = new Documentation("od");
        parent.Text = "hello";
        parent.Set(ShapesLanguage.Instance.Documentation_text, null);
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region integer

    [TestMethod]
    public void Integer_Init()
    {
        var parent = new Circle("od");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Integer_Set_Positive()
    {
        var parent = new Circle("od");
        parent.R = 10;
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.Circle_r },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Integer_Set_Zero()
    {
        var parent = new Circle("od");
        parent.R = 0;
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.Circle_r },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Integer_Set_Negative()
    {
        var parent = new Circle("od");
        parent.R = -10;
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.Circle_r },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Integer_Set_Reflective()
    {
        var parent = new Circle("od");
        parent.Set(ShapesLanguage.Instance.Circle_r, 10);
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.Circle_r },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region boolean

    [TestMethod]
    public void Boolean_Init()
    {
        var parent = new Documentation("od");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Boolean_Set_True()
    {
        var parent = new Documentation("od");
        parent.Technical = true;
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.Documentation_technical },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Boolean_Set_False()
    {
        var parent = new Documentation("od");
        parent.Technical = true;
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.Documentation_technical },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Boolean_Set_Reflective()
    {
        var parent = new Documentation("od");
        parent.Set(ShapesLanguage.Instance.Documentation_technical, true);
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.Documentation_technical },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Boolean_Unset()
    {
        var parent = new Documentation("od");
        parent.Technical = true;
        parent.Technical = null;
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Boolean_Unset_Reflective()
    {
        var parent = new Documentation("od");
        parent.Technical = true;
        parent.Set(ShapesLanguage.Instance.Documentation_technical, null);
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region enum

    [TestMethod]
    public void Enum_Init()
    {
        var parent = new MaterialGroup("od");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Enum_Set()
    {
        var parent = new MaterialGroup("od");
        parent.MatterState = MatterState.gas;
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.MaterialGroup_matterState },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Enum_Set_Reflective()
    {
        var parent = new MaterialGroup("od");
        parent.Set(ShapesLanguage.Instance.MaterialGroup_matterState, MatterState.gas);
        CollectionAssert.AreEqual(new List<Feature> { ShapesLanguage.Instance.MaterialGroup_matterState },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Enum_Unset()
    {
        var parent = new MaterialGroup("od");
        parent.MatterState = MatterState.gas;
        parent.MatterState = null;
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Enum_Unset_Reflective()
    {
        var parent = new MaterialGroup("od");
        parent.MatterState = MatterState.gas;
        parent.Set(ShapesLanguage.Instance.MaterialGroup_matterState, null);
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    #endregion
}
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

[TestClass]
public class EnumOptionalTests
{
    #region Single

    [TestMethod]
    public void Property()
    {
        var parent = new MaterialGroup("od");
        var value = MatterState.liquid;
        parent.MatterState = value;
        Assert.AreEqual(MatterState.liquid, parent.MatterState);
    }

    [TestMethod]
    public void Setter()
    {
        var parent = new MaterialGroup("od");
        var value = MatterState.liquid;
        parent.SetMatterState(value);
        Assert.AreEqual(MatterState.liquid, parent.MatterState);
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new MaterialGroup("od");
        var value = MatterState.liquid;
        parent.Set(ShapesLanguage.Instance.MaterialGroup_matterState, value);
        Assert.AreEqual(MatterState.liquid, parent.MatterState);
    }

    [TestMethod]
    public void Get_Reflective()
    {
        var parent = new MaterialGroup("od") { MatterState = MatterState.liquid };
        Assert.AreEqual(MatterState.liquid, parent.Get(ShapesLanguage.Instance.MaterialGroup_matterState));
    }

    [TestMethod]
    public void Gas_Reflective()
    {
        var parent = new MaterialGroup("od");
        var value = MatterState.gas;
        parent.Set(ShapesLanguage.Instance.MaterialGroup_matterState, value);
        Assert.AreEqual(MatterState.gas, parent.MatterState);
    }

    [TestMethod]
    public void Boolean_Reflective()
    {
        var parent = new MaterialGroup("od");
        var value = true;
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(ShapesLanguage.Instance.MaterialGroup_matterState, value));
        Assert.AreEqual(null, parent.MatterState);
    }

    private enum TestEnum
    {
        a,
        solid,
        gas
    }
    
    [TestMethod]
    public void OtherEnum_Reflective()
    {
        
        var parent = new MaterialGroup("od");
        var value = TestEnum.a;
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(ShapesLanguage.Instance.MaterialGroup_matterState, value));
        Assert.AreEqual(null, parent.MatterState);
    }

    [TestMethod]
    public void SimilarEnum_Reflective()
    {
        
        var parent = new MaterialGroup("od");
        var value = TestEnum.solid;
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(ShapesLanguage.Instance.MaterialGroup_matterState, value));
        Assert.AreEqual(null, parent.MatterState);
    }

    [TestMethod]
    public void VerySimilarEnum_Reflective()
    {
        
        var parent = new MaterialGroup("od");
        var value = TestEnum.gas;
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(ShapesLanguage.Instance.MaterialGroup_matterState, value));
        Assert.AreEqual(null, parent.MatterState);
    }

    [TestMethod]
    public void Integer_Reflective()
    {
        var parent = new MaterialGroup("od");
        var value = 10;
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(ShapesLanguage.Instance.MaterialGroup_matterState, value));
        Assert.AreEqual(null, parent.MatterState);
    }

    [TestMethod]
    public void Constructor()
    {
        var parent = new MaterialGroup("myId") { MatterState = MatterState.liquid };
        Assert.AreEqual(MatterState.liquid, parent.MatterState);
    }

    [TestMethod]
    public void TryGet()
    {
        var parent = new MaterialGroup("myId") { MatterState = MatterState.liquid };
        Assert.IsTrue(parent.TryGetMatterState(out var o));
        Assert.AreEqual(MatterState.liquid, o);
    }

    #endregion

    #region Null

    [TestMethod]
    public void Null()
    {
        var parent = new MaterialGroup("od");
        object value = null;
        parent.MatterState = (MatterState?)value;
        Assert.AreEqual(null, parent.MatterState);
    }

    [TestMethod]
    public void Null_Setter()
    {
        var parent = new MaterialGroup("od");
        object value = null;
        parent.SetMatterState((MatterState?)value);
        Assert.AreEqual(null, parent.MatterState);
    }

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = new MaterialGroup("od");
        object value = null;
        parent.Set(ShapesLanguage.Instance.MaterialGroup_matterState, null);
        Assert.AreEqual(null, parent.MatterState);
    }

    [TestMethod]
    public void Null_Get()
    {
        var parent = new MaterialGroup("od");
        Assert.AreEqual(null, parent.MatterState);
    }

    [TestMethod]
    public void Null_Get_Reflective()
    {
        var parent = new MaterialGroup("od");
        Assert.AreEqual(null, parent.Get(ShapesLanguage.Instance.MaterialGroup_matterState));
    }

    [TestMethod]
    public void Null_Constructor()
    {
        object value = null;
        var parent = new MaterialGroup("od") { MatterState = (MatterState?)value };
        Assert.AreEqual(null, parent.MatterState);
    }

    [TestMethod]
    public void Null_TryGet()
    {
        var parent = new MaterialGroup("od");
        Assert.IsFalse(parent.TryGetMatterState(out var o));
        Assert.IsNull(o);
    }

    #endregion
}
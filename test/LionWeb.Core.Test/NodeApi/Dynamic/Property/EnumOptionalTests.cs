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

namespace LionWeb.Core.Test.NodeApi.Dynamic.Property;

[TestClass]
public class EnumOptionalTests : DynamicNodeTestsBase
{
    #region Single

    [TestMethod]
    public void Reflective()
    {
        var parent = newMaterialGroup("od");
        var value = MatterState_Liquid;
        parent.Set(MaterialGroup_matterState, value);
        Assert.AreEqual(MatterState_Liquid, parent.Get(MaterialGroup_matterState));
    }

    [TestMethod]
    public void Get_Reflective()
    {
        var parent = newMaterialGroup("od");
        parent.Set(MaterialGroup_matterState, MatterState_Liquid);
        Assert.AreEqual(MatterState_Liquid, parent.Get(MaterialGroup_matterState));
    }

    [TestMethod]
    public void Gas_Reflective()
    {
        var parent = newMaterialGroup("od");
        var value = MatterState_Gas;
        parent.Set(MaterialGroup_matterState, value);
        Assert.AreEqual(MatterState_Gas, parent.Get(MaterialGroup_matterState));
    }

    [TestMethod]
    public void Boolean_Reflective()
    {
        var parent = newMaterialGroup("od");
        var value = true;
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(MaterialGroup_matterState, value));
        Assert.AreEqual(null, parent.Get(MaterialGroup_matterState));
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
        var parent = newMaterialGroup("od");
        var value = TestEnum.a;
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(MaterialGroup_matterState, value));
        Assert.AreEqual(null, parent.Get(MaterialGroup_matterState));
    }

    [TestMethod]
    public void SimilarEnum_Reflective()
    {
        var parent = newMaterialGroup("od");
        var value = TestEnum.solid;
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(MaterialGroup_matterState, value));
        Assert.AreEqual(null, parent.Get(MaterialGroup_matterState));
    }

    [TestMethod]
    public void VerySimilarEnum_Reflective()
    {
        var parent = newMaterialGroup("od");
        var value = TestEnum.gas;
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(MaterialGroup_matterState, value));
        Assert.AreEqual(null, parent.Get(MaterialGroup_matterState));
    }

    [TestMethod]
    public void Integer_Reflective()
    {
        var parent = newMaterialGroup("od");
        var value = 10;
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(MaterialGroup_matterState, value));
        Assert.AreEqual(null, parent.Get(MaterialGroup_matterState));
    }

    #endregion

    #region Null

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = newMaterialGroup("od");
        object value = null;
        parent.Set(MaterialGroup_matterState, null);
        Assert.AreEqual(null, parent.Get(MaterialGroup_matterState));
    }

    [TestMethod]
    public void Null_Get_Reflective()
    {
        var parent = newMaterialGroup("od");
        Assert.AreEqual(null, parent.Get(MaterialGroup_matterState));
    }

    #endregion
}
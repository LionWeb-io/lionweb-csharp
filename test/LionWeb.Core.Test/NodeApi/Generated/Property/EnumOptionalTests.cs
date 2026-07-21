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

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class EnumOptionalTests
{
    #region Single

    [TestMethod]
    public void Property()
    {
        var parent = new DataTypeTestConcept("od");
        var value = TestEnumeration.literal1;
        parent.EnumValue_0_1 = value;
        Assert.AreEqual(TestEnumeration.literal1, parent.EnumValue_0_1);
    }

    [TestMethod]
    public void Setter()
    {
        var parent = new DataTypeTestConcept("od");
        var value = TestEnumeration.literal1;
        parent.SetEnumValue_0_1(value);
        Assert.AreEqual(TestEnumeration.literal1, parent.EnumValue_0_1);
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        var value = TestEnumeration.literal1;
        parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1, value);
        Assert.AreEqual(TestEnumeration.literal1, parent.EnumValue_0_1);
    }

    [TestMethod]
    public void Get_Reflective()
    {
        var parent = new DataTypeTestConcept("od") { EnumValue_0_1 = TestEnumeration.literal1 };
        Assert.AreEqual(TestEnumeration.literal1, parent.Get(TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1));
    }

    [TestMethod]
    public void Gas_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        var value = TestEnumeration.literal2;
        parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1, value);
        Assert.AreEqual(TestEnumeration.literal2, parent.EnumValue_0_1);
    }

    [TestMethod]
    public void Boolean_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        var value = true;
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1, value));
        Assert.AreEqual(null, parent.EnumValue_0_1);
    }

    private enum OtherEnum
    {
        a,
        literal1,
        literal2
    }
    
    [TestMethod]
    public void OtherEnum_Reflective()
    {
        
        var parent = new DataTypeTestConcept("od");
        var value = OtherEnum.a;
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1, value));
        Assert.AreEqual(null, parent.EnumValue_0_1);
    }

    [TestMethod]
    public void SimilarEnum_Reflective()
    {
        
        var parent = new DataTypeTestConcept("od");
        var value = OtherEnum.literal1;
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1, value));
        Assert.AreEqual(null, parent.EnumValue_0_1);
    }

    [TestMethod]
    public void VerySimilarEnum_Reflective()
    {
        
        var parent = new DataTypeTestConcept("od");
        var value = OtherEnum.literal2;
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1, value));
        Assert.AreEqual(null, parent.EnumValue_0_1);
    }

    [TestMethod]
    public void Integer_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        var value = 10;
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1, value));
        Assert.AreEqual(null, parent.EnumValue_0_1);
    }

    [TestMethod]
    public void Constructor()
    {
        var parent = new DataTypeTestConcept("myId") { EnumValue_0_1 = TestEnumeration.literal1 };
        Assert.AreEqual(TestEnumeration.literal1, parent.EnumValue_0_1);
    }

    [TestMethod]
    public void TryGet()
    {
        var parent = new DataTypeTestConcept("myId") { EnumValue_0_1 = TestEnumeration.literal1 };
        Assert.IsTrue(parent.TryGetEnumValue_0_1(out var o));
        Assert.AreEqual(TestEnumeration.literal1, o);
    }

    #endregion

    #region Null

    [TestMethod]
    public void Null()
    {
        var parent = new DataTypeTestConcept("od");
        object value = null;
        parent.EnumValue_0_1 = (TestEnumeration?)value;
        Assert.AreEqual(null, parent.EnumValue_0_1);
    }

    [TestMethod]
    public void Null_Setter()
    {
        var parent = new DataTypeTestConcept("od");
        object value = null;
        parent.SetEnumValue_0_1((TestEnumeration?)value);
        Assert.AreEqual(null, parent.EnumValue_0_1);
    }

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        object value = null;
        parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1, null);
        Assert.AreEqual(null, parent.EnumValue_0_1);
    }

    [TestMethod]
    public void Null_Get()
    {
        var parent = new DataTypeTestConcept("od");
        Assert.AreEqual(null, parent.EnumValue_0_1);
    }

    [TestMethod]
    public void Null_Get_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        Assert.AreEqual(null, parent.Get(TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1));
    }

    [TestMethod]
    public void Null_Constructor()
    {
        object value = null;
        var parent = new DataTypeTestConcept("od") { EnumValue_0_1 = (TestEnumeration?)value };
        Assert.AreEqual(null, parent.EnumValue_0_1);
    }

    [TestMethod]
    public void Null_TryGet()
    {
        var parent = new DataTypeTestConcept("od");
        Assert.IsFalse(parent.TryGetEnumValue_0_1(out var o));
        Assert.IsNull(o);
    }

    #endregion
}

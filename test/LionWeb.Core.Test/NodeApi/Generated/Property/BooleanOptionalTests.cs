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
public class BooleanOptionalTests
{
    #region Single

    [TestMethod]
    public void Property()
    {
        var parent = new DataTypeTestConcept("od");
        var value = true;
        parent.BooleanValue_0_1 = value;
        Assert.AreEqual(true, parent.BooleanValue_0_1);
    }

    [TestMethod]
    public void Setter()
    {
        var parent = new DataTypeTestConcept("od");
        var value = true;
        parent.SetBooleanValue_0_1(value);
        Assert.AreEqual(true, parent.BooleanValue_0_1);
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        var value = true;
        parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1, value);
        Assert.AreEqual(true, parent.BooleanValue_0_1);
    }

    [TestMethod]
    public void Get_Reflective()
    {
        var parent = new DataTypeTestConcept("od") { BooleanValue_0_1 = true };
        Assert.AreEqual(true, parent.Get(TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1));
    }

    [TestMethod]
    public void False_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        var value = false;
        parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1, value);
        Assert.AreEqual(false, parent.BooleanValue_0_1);
    }

    [TestMethod]
    public void String_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        var value = "10";
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1, value));
        Assert.AreEqual(null, parent.BooleanValue_0_1);
    }

    [TestMethod]
    public void Integer_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        var value = 10;
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1, value));
        Assert.AreEqual(null, parent.BooleanValue_0_1);
    }

    [TestMethod]
    public void Constructor()
    {
        var parent = new DataTypeTestConcept("myId") { BooleanValue_0_1 = true };
        Assert.AreEqual(true, parent.BooleanValue_0_1);
    }

    [TestMethod]
    public void TryGet()
    {
        var parent = new DataTypeTestConcept("myId") { BooleanValue_0_1 = true };
        Assert.IsTrue(parent.TryGetBooleanValue_0_1(out var o));
        Assert.AreEqual(true, o);
    }

    #endregion

    #region Null

    [TestMethod]
    public void Null()
    {
        var parent = new DataTypeTestConcept("od");
        object value = null;
        parent.BooleanValue_0_1 = (bool?)value;
        Assert.AreEqual(null, parent.BooleanValue_0_1);
    }

    [TestMethod]
    public void Null_Setter()
    {
        var parent = new DataTypeTestConcept("od");
        object value = null;
        parent.SetBooleanValue_0_1((bool?)value);
        Assert.AreEqual(null, parent.BooleanValue_0_1);
    }

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        object value = null;
        parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1, null);
        Assert.AreEqual(null, parent.BooleanValue_0_1);
    }

    [TestMethod]
    public void Null_Get()
    {
        var parent = new DataTypeTestConcept("od");
        Assert.AreEqual(null, parent.BooleanValue_0_1);
    }

    [TestMethod]
    public void Null_Get_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        Assert.AreEqual(null, parent.Get(TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1));
    }

    [TestMethod]
    public void Null_Constructor()
    {
        object value = null;
        var parent = new DataTypeTestConcept("od") { BooleanValue_0_1 = (bool?)value };
        Assert.AreEqual(null, parent.BooleanValue_0_1);
    }

    [TestMethod]
    public void Null_TryGet()
    {
        var parent = new DataTypeTestConcept("od");
        Assert.IsFalse(parent.TryGetBooleanValue_0_1(out var o));
        Assert.IsNull(o);
    }

    #endregion
}

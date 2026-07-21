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
public class IntegerRequiredTests
{
    #region Single

    [TestMethod]
    public void Property()
    {
        var parent = new DataTypeTestConcept("od");
        var value = 10;
        parent.IntegerValue_1 = value;
        Assert.AreEqual(10, parent.IntegerValue_1);
    }

    [TestMethod]
    public void Setter()
    {
        var parent = new DataTypeTestConcept("od");
        var value = 10;
        parent.SetIntegerValue_1(value);
        Assert.AreEqual(10, parent.IntegerValue_1);
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        var value = 10;
        parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1, value);
        Assert.AreEqual(10, parent.IntegerValue_1);
    }

    [TestMethod]
    public void Get_Reflective()
    {
        var parent = new DataTypeTestConcept("od") { IntegerValue_1 = 10 };
        Assert.AreEqual(10, parent.Get(TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1));
    }

    [TestMethod]
    public void Long_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        var value = 10L;
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1, value));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.IntegerValue_1);
    }

    [TestMethod]
    public void String_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        var value = "10";
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1, value));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.IntegerValue_1);
    }

    [TestMethod]
    public void Boolean_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        var value = true;
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1, value));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.IntegerValue_1);
    }

    [TestMethod]
    public void Constructor()
    {
        var parent = new DataTypeTestConcept("myId") { IntegerValue_1 = 10 };
        Assert.AreEqual(10, parent.IntegerValue_1);
    }

    [TestMethod]
    public void TryGet()
    {
        var parent = new DataTypeTestConcept("myId") { IntegerValue_1 = 10 };
        Assert.IsTrue(parent.TryGetIntegerValue_1(out var o));
        Assert.AreEqual(10, o);
    }

    #endregion

    #region Null

    [TestMethod]
    public void Null()
    {
        var parent = new DataTypeTestConcept("od");
        object value = null;
        Assert.ThrowsExactly<NullReferenceException>(() => parent.IntegerValue_1 = (int)value);
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.IntegerValue_1);
    }

    [TestMethod]
    public void Null_Setter()
    {
        var parent = new DataTypeTestConcept("od");
        object value = null;
        Assert.ThrowsExactly<NullReferenceException>(() => parent.SetIntegerValue_1((int)value));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.IntegerValue_1);
    }

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        object value = null;
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1, value));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.IntegerValue_1);
    }

    [TestMethod]
    public void Null_Get()
    {
        var parent = new DataTypeTestConcept("od");
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.IntegerValue_1);
    }

    [TestMethod]
    public void Null_Get_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Get(TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1));
    }

    [TestMethod]
    public void Null_Constructor()
    {
        var parent = new DataTypeTestConcept("od");
        object value = null;
        Assert.ThrowsExactly<NullReferenceException>(
            () => new DataTypeTestConcept("od") { IntegerValue_1 = (int)value });
    }

    [TestMethod]
    public void Null_TryGet()
    {
        var parent = new DataTypeTestConcept("od");
        Assert.IsFalse(parent.TryGetIntegerValue_1(out var o));
        Assert.IsNull(o);
    }

    #endregion
}

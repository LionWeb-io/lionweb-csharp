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
public class StringOptionalTests
{
    #region Single

    [TestMethod]
    public void Property()
    {
        var parent = new DataTypeTestConcept("od");
        var value = "Hi";
        parent.StringValue_0_1 = value;
        Assert.AreEqual("Hi", parent.StringValue_0_1);
    }

    [TestMethod]
    public void Setter()
    {
        var parent = new DataTypeTestConcept("od");
        var value = "Hi";
        parent.SetStringValue_0_1(value);
        Assert.AreEqual("Hi", parent.StringValue_0_1);
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        var value = "Hi";
        parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1, value);
        Assert.AreEqual("Hi", parent.StringValue_0_1);
    }

    [TestMethod]
    public void Get_Reflective()
    {
        var parent = new DataTypeTestConcept("od") { StringValue_0_1 = "Hi" };
        Assert.AreEqual("Hi", parent.Get(TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1));
    }

    [TestMethod]
    public void Bye_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        var value = "Bye";
        parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1, value);
        Assert.AreEqual("Bye", parent.StringValue_0_1);
    }

    [TestMethod]
    public void Boolean_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        var value = true;
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1, value));
        Assert.AreEqual(null, parent.StringValue_0_1);
    }

    [TestMethod]
    public void Integer_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        var value = 10;
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1, value));
        Assert.AreEqual(null, parent.StringValue_0_1);
    }

    [TestMethod]
    public void Constructor()
    {
        var parent = new DataTypeTestConcept("myId") { StringValue_0_1 = "Hi" };
        Assert.AreEqual("Hi", parent.StringValue_0_1);
    }

    [TestMethod]
    public void TryGet()
    {
        var parent = new DataTypeTestConcept("myId") { StringValue_0_1 = "Hi" };
        Assert.IsTrue(parent.TryGetStringValue_0_1(out var o));
        Assert.AreEqual("Hi", o);
    }

    #endregion

    #region Null

    [TestMethod]
    public void Null()
    {
        var parent = new DataTypeTestConcept("od");
        object value = null;
        parent.StringValue_0_1 = (string?)value;
        Assert.AreEqual(null, parent.StringValue_0_1);
    }

    [TestMethod]
    public void Null_Setter()
    {
        var parent = new DataTypeTestConcept("od");
        object value = null;
        parent.SetStringValue_0_1((string?)value);
        Assert.AreEqual(null, parent.StringValue_0_1);
    }

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        object value = null;
        parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1, null);
        Assert.AreEqual(null, parent.StringValue_0_1);
    }

    [TestMethod]
    public void Null_Get()
    {
        var parent = new DataTypeTestConcept("od");
        Assert.AreEqual(null, parent.StringValue_0_1);
    }

    [TestMethod]
    public void Null_Get_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        Assert.AreEqual(null, parent.Get(TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1));
    }

    [TestMethod]
    public void Null_Constructor()
    {
        object value = null;
        var parent = new DataTypeTestConcept("od") { StringValue_0_1 = (string?)value };
        Assert.AreEqual(null, parent.StringValue_0_1);
    }

    [TestMethod]
    public void Null_TryGet()
    {
        var parent = new DataTypeTestConcept("od");
        Assert.IsFalse(parent.TryGetStringValue_0_1(out var o));
        Assert.IsNull(o);
    }

    #endregion
}

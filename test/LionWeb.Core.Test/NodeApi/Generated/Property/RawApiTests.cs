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
public class RawApiTests
{
    #region string

    #region Optional

    [TestMethod]
    public void String_Optional_Set_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        Assert.IsTrue(parent.SetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1, "hello"));
        Assert.IsTrue(parent.TryGetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1, out var result));
        Assert.AreEqual("hello", result);
    }

    [TestMethod]
    public void String_Optional_Unset_Reflective()
    {
        var parent = new DataTypeTestConcept("od") { StringValue_0_1 = "hello" };
        Assert.IsTrue(parent.SetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1, null));
        Assert.IsTrue(parent.TryGetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1, out var result));
        Assert.IsNull(result);
    }

    #endregion

    #region Required

    [TestMethod]
    public void String_Required_Set_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        Assert.IsTrue(parent.SetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_1, "hello"));
        Assert.IsTrue(parent.TryGetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_1, out var result));
        Assert.AreEqual("hello", result);
    }

    [TestMethod]
    public void String_Required_Unset_Reflective()
    {
        var parent = new DataTypeTestConcept("od") { StringValue_1 = "hello" };
        Assert.IsTrue(parent.SetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_1, null));
        Assert.IsTrue(parent.TryGetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_1, out var result));
        Assert.IsNull(result);
    }

    #endregion

    #endregion

    #region integer
    
    #region Optional

    [TestMethod]
    public void Integer_Optional_Set_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        Assert.IsTrue(parent.SetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_0_1, 10));
        Assert.IsTrue(parent.TryGetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_0_1, out var result));
        Assert.AreEqual(10, result);
    }
    
    [TestMethod]
    public void Integer_Optional_Unset_Reflective()
    {
        var parent = new DataTypeTestConcept("od") { IntegerValue_1 = 10 };
        Assert.IsTrue(parent.SetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1, null));
        Assert.IsTrue(parent.TryGetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1, out var result));
        Assert.IsNull(result);
    }

    #endregion

    #region Required

    [TestMethod]
    public void Integer_Required_Set_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        Assert.IsTrue(parent.SetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1, 10));
        Assert.IsTrue(parent.TryGetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1, out var result));
        Assert.AreEqual(10, result);
    }
    
    [TestMethod]
    public void Integer_Required_Unset_Reflective()
    {
        var parent = new DataTypeTestConcept("od") { IntegerValue_1 = 10 };
        Assert.IsTrue(parent.SetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1, null));
        Assert.IsTrue(parent.TryGetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1, out var result));
        Assert.IsNull(result);
    }

    #endregion

    #endregion

    #region boolean

    #region Optional

    [TestMethod]
    public void Boolean_Optional_Set_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        Assert.IsTrue(parent.SetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1, true));
        Assert.IsTrue(parent.TryGetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1, out var result));
        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void Boolean_Optional_Unset_Reflective()
    {
        var parent = new DataTypeTestConcept("od") { BooleanValue_0_1 = true };
        Assert.IsTrue(parent.SetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1, null));
        Assert.IsTrue(parent.TryGetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1, out var result));
        Assert.IsNull(result);
    }

    #endregion

    #region Required

    [TestMethod]
    public void Boolean_Required_Set_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        Assert.IsTrue(parent.SetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_1, true));
        Assert.IsTrue(parent.TryGetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_1, out var result));
        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void Boolean_Required_Unset_Reflective()
    {
        var parent = new DataTypeTestConcept("od") { BooleanValue_1 = true };
        Assert.IsTrue(parent.SetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_1, null));
        Assert.IsTrue(parent.TryGetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_1, out var result));
        Assert.IsNull(result);
    }

    #endregion

    #endregion

    #region enum

    #region Optional

    [TestMethod]
    public void Enum_Optional_Set_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        Assert.IsTrue(parent.SetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1, TestEnumeration.literal2));
        Assert.IsTrue(parent.TryGetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1, out var result));
        Assert.AreEqual(TestEnumeration.literal2, result);
    }

    [TestMethod]
    public void Enum_Optional_Unset_Reflective()
    {
        var parent = new DataTypeTestConcept("od") { EnumValue_0_1 = TestEnumeration.literal2 };
        Assert.IsTrue(parent.SetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1, null));
        Assert.IsTrue(parent.TryGetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1, out var result));
        Assert.IsNull(result);
    }

    #endregion

    #region Required

    [TestMethod]
    public void Enum_Required_Set_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        Assert.IsTrue(parent.SetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_1, TestEnumeration.literal2));
        Assert.IsTrue(parent.TryGetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_1, out var result));
        Assert.AreEqual(TestEnumeration.literal2, result);
    }

    [TestMethod]
    public void Enum_Required_Unset_Reflective()
    {
        var parent = new DataTypeTestConcept("od") { EnumValue_1 = TestEnumeration.literal2 };
        Assert.IsTrue(parent.SetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_1, null));
        Assert.IsTrue(parent.TryGetPropertyRaw(TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_1, out var result));
        Assert.IsNull(result);
    }

    #endregion

    #endregion
}
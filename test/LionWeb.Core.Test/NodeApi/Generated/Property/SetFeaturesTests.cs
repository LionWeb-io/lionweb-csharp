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
using M3;

[TestClass]
public class SetFeaturesTests
{
    #region string

    [TestMethod]
    public void String_Init()
    {
        var parent = new DataTypeTestConcept("od");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void String_Set()
    {
        var parent = new DataTypeTestConcept("od");
        parent.StringValue_0_1 = "hello";
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1 },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void String_Set_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1, "hello");
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1 },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void String_Unset()
    {
        var parent = new DataTypeTestConcept("od");
        parent.StringValue_0_1 = "hello";
        parent.StringValue_0_1 = null;
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void String_Unset_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        parent.StringValue_0_1 = "hello";
        parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1, null);
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region integer

    [TestMethod]
    public void Integer_Init()
    {
        var parent = new DataTypeTestConcept("od");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Integer_Set_Positive()
    {
        var parent = new DataTypeTestConcept("od");
        parent.IntegerValue_1 = 10;
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1 },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Integer_Set_Zero()
    {
        var parent = new DataTypeTestConcept("od");
        parent.IntegerValue_1 = 0;
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1 },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Integer_Set_Negative()
    {
        var parent = new DataTypeTestConcept("od");
        parent.IntegerValue_1 = -10;
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1 },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Integer_Set_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1, 10);
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1 },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region boolean

    [TestMethod]
    public void Boolean_Init()
    {
        var parent = new DataTypeTestConcept("od");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Boolean_Set_True()
    {
        var parent = new DataTypeTestConcept("od");
        parent.BooleanValue_0_1 = true;
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1 },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Boolean_Set_False()
    {
        var parent = new DataTypeTestConcept("od");
        parent.BooleanValue_0_1 = true;
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1 },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Boolean_Set_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1, true);
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1 },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Boolean_Unset()
    {
        var parent = new DataTypeTestConcept("od");
        parent.BooleanValue_0_1 = true;
        parent.BooleanValue_0_1 = null;
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Boolean_Unset_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        parent.BooleanValue_0_1 = true;
        parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1, null);
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region enum

    [TestMethod]
    public void Enum_Init()
    {
        var parent = new DataTypeTestConcept("od");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Enum_Set()
    {
        var parent = new DataTypeTestConcept("od");
        parent.EnumValue_0_1 = TestEnumeration.literal2;
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1 },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Enum_Set_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1, TestEnumeration.literal2);
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1 },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Enum_Unset()
    {
        var parent = new DataTypeTestConcept("od");
        parent.EnumValue_0_1 = TestEnumeration.literal2;
        parent.EnumValue_0_1 = null;
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Enum_Unset_Reflective()
    {
        var parent = new DataTypeTestConcept("od");
        parent.EnumValue_0_1 = TestEnumeration.literal2;
        parent.Set(TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1, null);
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    #endregion
}

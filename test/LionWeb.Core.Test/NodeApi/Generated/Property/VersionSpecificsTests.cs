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

using Languages.Generated.V2023_1.TestLanguage;
using M3;

[TestClass]
public class VersionSpecificsTests
{
    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void String(Type versionIface)
    {
        var parent = newDataTypeTestConcept("od", versionIface);
        var value = "Hi";
        parent.Set(DataTypeTestConcept_stringValue_0_1(versionIface), value);
        Assert.AreEqual("Hi", parent.Get(DataTypeTestConcept_stringValue_0_1(versionIface)));
    }

    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void Integer(Type versionIface)
    {
        var parent = newDataTypeTestConcept("od", versionIface);
        var value = 10;
        parent.Set(DataTypeTestConcept_integerValue_1(versionIface), value);
        Assert.AreEqual(10, parent.Get(DataTypeTestConcept_integerValue_1(versionIface)));
    }

    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void Boolean(Type versionIface)
    {
        var parent = newDataTypeTestConcept("od", versionIface);
        var value = true;
        parent.Set(DataTypeTestConcept_booleanValue_0_1(versionIface), value);
        Assert.AreEqual(true, parent.Get(DataTypeTestConcept_booleanValue_0_1(versionIface)));
    }

    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void Enum(Type versionIface)
    {
        var parent = newDataTypeTestConcept("od", versionIface);
        var value = TestEnumeration_literal1(versionIface);
        parent.Set(DataTypeTestConcept_enumValue_0_1(versionIface), value);
        Assert.AreEqual(TestEnumeration_literal1(versionIface), parent.Get(DataTypeTestConcept_enumValue_0_1(versionIface)));
    }

    private INode newDataTypeTestConcept(string id, Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => new DataTypeTestConcept(id),
        IVersion2024_1 => new Languages.Generated.V2024_1.TestLanguage.DataTypeTestConcept(id),
        IVersion2024_1_Compatible => new Languages.Generated.V2024_1.TestLanguage.DataTypeTestConcept(id),
        var v => throw new UnsupportedVersionException(v)
    };

    private Feature DataTypeTestConcept_stringValue_0_1(Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1,
        IVersion2024_1 => Languages.Generated.V2024_1.TestLanguage.TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1,
        IVersion2024_1_Compatible => Languages.Generated.V2024_1.TestLanguage.TestLanguageLanguage.Instance.DataTypeTestConcept_stringValue_0_1,
        var v => throw new UnsupportedVersionException(v)
    };

    private Feature DataTypeTestConcept_booleanValue_0_1(Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1,
        IVersion2024_1 => Languages.Generated.V2024_1.TestLanguage.TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1,
        IVersion2024_1_Compatible => Languages.Generated.V2024_1.TestLanguage.TestLanguageLanguage.Instance.DataTypeTestConcept_booleanValue_0_1,
        var v => throw new UnsupportedVersionException(v)
    };

    private Feature DataTypeTestConcept_integerValue_1(Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1,
        IVersion2024_1 => Languages.Generated.V2024_1.TestLanguage.TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1,
        IVersion2024_1_Compatible => Languages.Generated.V2024_1.TestLanguage.TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1,
        var v => throw new UnsupportedVersionException(v)
    };

    private Feature DataTypeTestConcept_enumValue_0_1(Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1,
        IVersion2024_1 => Languages.Generated.V2024_1.TestLanguage.TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1,
        IVersion2024_1_Compatible => Languages.Generated.V2024_1.TestLanguage.TestLanguageLanguage.Instance.DataTypeTestConcept_enumValue_0_1,
        var v => throw new UnsupportedVersionException(v)
    };

    private object? TestEnumeration_literal1(Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => TestEnumeration.literal1,
        IVersion2024_1 => Languages.Generated.V2024_1.TestLanguage.TestEnumeration.literal1,
        IVersion2024_1_Compatible => Languages.Generated.V2024_1.TestLanguage.TestEnumeration.literal1,
        var v => throw new UnsupportedVersionException(v)
    };
}

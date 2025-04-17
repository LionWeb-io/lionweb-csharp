// Copyright 2025 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License")
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

namespace LionWeb.Core.Test.Utilities.IdentityComparer;

using Core.Utilities;
using M2;
using M3;

[TestClass]
public class FieldIdentityComparerTests
{
    readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    [TestMethod]
    public void Same()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var sdt = new DynamicStructuredDataType("id", _lionWebVersion, lang) { Key = "sdtKey" };
        var field = new DynamicField("id", _lionWebVersion, sdt) { Key = "key" };

        var comparer = new FieldIdentityComparer();
        Assert.AreSame(sdt, field.GetStructuredDataType());
        Assert.AreEqual(comparer.GetHashCode(field), comparer.GetHashCode(field));
        Assert.IsTrue(comparer.Equals(field, field));
    }

    [TestMethod]
    public void Same_Changed_Key()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var sdt = new DynamicStructuredDataType("id", _lionWebVersion, lang) { Key = "sdtKey" };
        var field = new DynamicField("id", _lionWebVersion, sdt) { Key = "key" };

        var comparer = new FieldIdentityComparer();

        var hashCodeA = comparer.GetHashCode(field);
        field.Key = "otherKey";
        var hashCodeB = comparer.GetHashCode(field);

        Assert.AreNotEqual(hashCodeA, hashCodeB);
    }

    [TestMethod]
    public void Same_Changed_ConceptKey()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var sdt = new DynamicStructuredDataType("id", _lionWebVersion, lang) { Key = "sdtKey" };
        var field = new DynamicField("id", _lionWebVersion, sdt) { Key = "key" };

        var comparer = new FieldIdentityComparer();

        var hashCodeA = comparer.GetHashCode(field);
        sdt.Key = "Y";
        var hashCodeB = comparer.GetHashCode(field);

        Assert.AreNotEqual(hashCodeA, hashCodeB);
    }

    [TestMethod]
    public void Same_Changed_Version()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var sdt = new DynamicStructuredDataType("id", _lionWebVersion, lang) { Key = "sdtKey" };
        var field = new DynamicField("id", _lionWebVersion, sdt) { Key = "key" };

        var comparer = new FieldIdentityComparer();

        var hashCodeA = comparer.GetHashCode(field);
        lang.Version = "Y";
        var hashCodeB = comparer.GetHashCode(field);

        Assert.AreNotEqual(hashCodeA, hashCodeB);
    }

    [TestMethod]
    public void Equals_SameSdt()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var sdt = new DynamicStructuredDataType("id", _lionWebVersion, lang) { Key = "sdtKey" };
        var fieldA = new DynamicField("id", _lionWebVersion, sdt) { Key = "key" };
        var fieldB = new DynamicField("id", _lionWebVersion, sdt) { Key = "key" };

        var comparer = new FieldIdentityComparer();

        Assert.AreNotSame(fieldA, fieldB);
        Assert.AreEqual(comparer.GetHashCode(fieldA), comparer.GetHashCode(fieldB));
        Assert.IsTrue(comparer.Equals(fieldA, fieldB));
    }

    [TestMethod]
    public void Equals_EqualSdt()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var sdtA = new DynamicStructuredDataType("id", _lionWebVersion, langA) { Key = "sdtKey" };
        var fieldA = new DynamicField("id", _lionWebVersion, sdtA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var sdtB = new DynamicStructuredDataType("id", _lionWebVersion, langB) { Key = "sdtKey" };
        var fieldB = new DynamicField("id", _lionWebVersion, sdtB) { Key = "key" };

        var comparer = new FieldIdentityComparer();

        Assert.AreNotSame(fieldA, fieldB);
        Assert.AreEqual(comparer.GetHashCode(fieldA), comparer.GetHashCode(fieldB));
        Assert.IsTrue(comparer.Equals(fieldA, fieldB));
    }

    [TestMethod]
    public void Different_DifferentSdt()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var sdtA = new DynamicStructuredDataType("id", _lionWebVersion, langA) { Key = "sdtKey" };
        var fieldA = new DynamicField("id", _lionWebVersion, sdtA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var sdtB = new DynamicStructuredDataType("id", _lionWebVersion, langB) { Key = "otherSdtKey" };
        var fieldB = new DynamicField("id", _lionWebVersion, sdtB) { Key = "key" };

        var comparer = new FieldIdentityComparer();

        Assert.AreNotSame(fieldA, fieldB);
        Assert.AreNotEqual(comparer.GetHashCode(fieldA), comparer.GetHashCode(fieldB));
        Assert.IsFalse(comparer.Equals(fieldA, fieldB));
    }

    [TestMethod]
    public void Different_Key()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var sdt = new DynamicStructuredDataType("id", _lionWebVersion, lang) { Key = "sdtKey" };
        var fieldA = new DynamicField("id", _lionWebVersion, sdt) { Key = "key" };
        var fieldB = new DynamicField("id", _lionWebVersion, sdt) { Key = "otherKey" };

        var comparer = new FieldIdentityComparer();

        Assert.AreNotSame(fieldA, fieldB);
        Assert.AreNotEqual(comparer.GetHashCode(fieldA), comparer.GetHashCode(fieldB));
        Assert.IsFalse(comparer.Equals(fieldA, fieldB));
    }

    [TestMethod]
    public void Equals_Different_Name()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var sdt = new DynamicStructuredDataType("id", _lionWebVersion, lang) { Key = "sdtKey" };
        var fieldA = new DynamicField("id", _lionWebVersion, sdt) { Name = "a", Key = "key" };
        var fieldB = new DynamicField("id", _lionWebVersion, sdt) { Name = "b", Key = "key" };

        var comparer = new FieldIdentityComparer();

        Assert.AreNotSame(fieldA, fieldB);
        Assert.AreEqual(comparer.GetHashCode(fieldA), comparer.GetHashCode(fieldB));
        Assert.IsTrue(comparer.Equals(fieldA, fieldB));
    }

    [TestMethod]
    public void Equals_LeftNull()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var sdt = new DynamicStructuredDataType("id", _lionWebVersion, lang) { Key = "sdtKey" };
        var field = new DynamicField("id", _lionWebVersion, sdt) { Name = "a", Key = "key" };

        var comparer = new FieldIdentityComparer();

        Assert.IsFalse(comparer.Equals(null, field));
    }

    [TestMethod]
    public void Equals_RightNull()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var sdt = new DynamicStructuredDataType("id", _lionWebVersion, lang) { Key = "sdtKey" };
        var field = new DynamicField("id", _lionWebVersion, sdt) { Name = "a", Key = "key" };

        var comparer = new FieldIdentityComparer();

        Assert.IsFalse(comparer.Equals(field, null));
    }

    [TestMethod]
    public void Equals_BothNull()
    {
        var comparer = new FieldIdentityComparer();

        Assert.IsTrue(comparer.Equals(null, null));
    }
}
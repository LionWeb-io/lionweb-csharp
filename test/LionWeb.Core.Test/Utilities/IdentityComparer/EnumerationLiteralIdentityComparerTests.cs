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
public class EnumerationLiteralIdentityComparerTests
{
    readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    [TestMethod]
    public void Same()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var enm = new DynamicEnumeration("id", _lionWebVersion, lang) { Key = "enumKey" };
        var lit = new DynamicEnumerationLiteral("id", _lionWebVersion, enm) { Key = "key" };

        var comparer = new EnumerationLiteralIdentityComparer();

        Assert.AreSame(enm, lit.GetEnumeration());
        Assert.AreEqual(comparer.GetHashCode(lit), comparer.GetHashCode(lit));
        Assert.IsTrue(comparer.Equals(lit, lit));
    }

    [TestMethod]
    public void Same_Changed_Key()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var enm = new DynamicEnumeration("id", _lionWebVersion, lang) { Key = "enumKey" };
        var lit = new DynamicEnumerationLiteral("id", _lionWebVersion, enm) { Key = "key" };

        var comparer = new EnumerationLiteralIdentityComparer();

        var hashCodeA = comparer.GetHashCode(lit);
        lit.Key = "otherKey";
        var hashCodeB = comparer.GetHashCode(lit);

        Assert.AreNotEqual(hashCodeA, hashCodeB);
    }

    [TestMethod]
    public void Same_Changed_EnumKey()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var enm = new DynamicEnumeration("id", _lionWebVersion, lang) { Key = "enumKey" };
        var lit = new DynamicEnumerationLiteral("id", _lionWebVersion, enm) { Key = "key" };

        var comparer = new EnumerationLiteralIdentityComparer();

        var hashCodeA = comparer.GetHashCode(lit);
        enm.Key = "Y";
        var hashCodeB = comparer.GetHashCode(lit);

        Assert.AreNotEqual(hashCodeA, hashCodeB);
    }

    [TestMethod]
    public void Same_Changed_Version()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var enm = new DynamicEnumeration("id", _lionWebVersion, lang) { Key = "enumKey" };
        var lit = new DynamicEnumerationLiteral("id", _lionWebVersion, enm) { Key = "key" };

        var comparer = new EnumerationLiteralIdentityComparer();

        var hashCodeA = comparer.GetHashCode(lit);
        lang.Version = "Y";
        var hashCodeB = comparer.GetHashCode(lit);

        Assert.AreNotEqual(hashCodeA, hashCodeB);
    }

    [TestMethod]
    public void Equals_SameEnum()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var enm = new DynamicEnumeration("id", _lionWebVersion, lang) { Key = "enumKey" };
        var litA = new DynamicEnumerationLiteral("id", _lionWebVersion, enm) { Key = "key" };
        var litB = new DynamicEnumerationLiteral("id", _lionWebVersion, enm) { Key = "key" };

        var comparer = new EnumerationLiteralIdentityComparer();

        Assert.AreNotSame(litA, litB);
        Assert.AreEqual(comparer.GetHashCode(litA), comparer.GetHashCode(litB));
        Assert.IsTrue(comparer.Equals(litA, litB));
    }

    [TestMethod]
    public void Equals_EqualEnum()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var enmA = new DynamicEnumeration("id", _lionWebVersion, langA) { Key = "enumKey" };
        var litA = new DynamicEnumerationLiteral("id", _lionWebVersion, enmA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var enmB = new DynamicEnumeration("id", _lionWebVersion, langB) { Key = "enumKey" };
        var litB = new DynamicEnumerationLiteral("id", _lionWebVersion, enmB) { Key = "key" };

        var comparer = new EnumerationLiteralIdentityComparer();

        Assert.AreNotSame(litA, litB);
        Assert.AreEqual(comparer.GetHashCode(litA), comparer.GetHashCode(litB));
        Assert.IsTrue(comparer.Equals(litA, litB));
    }

    [TestMethod]
    public void Different_DifferentEnum()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var enmA = new DynamicEnumeration("id", _lionWebVersion, langA) { Key = "enumKey" };
        var litA = new DynamicEnumerationLiteral("id", _lionWebVersion, enmA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var enmB = new DynamicEnumeration("id", _lionWebVersion, langB) { Key = "otherEnumKey" };
        var litB = new DynamicEnumerationLiteral("id", _lionWebVersion, enmB) { Key = "key" };

        var comparer = new EnumerationLiteralIdentityComparer();

        Assert.AreNotSame(litA, litB);
        Assert.AreNotEqual(comparer.GetHashCode(litA), comparer.GetHashCode(litB));
        Assert.IsFalse(comparer.Equals(litA, litB));
    }

    [TestMethod]
    public void Different_Key()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var enm = new DynamicEnumeration("id", _lionWebVersion, lang) { Key = "enumKey" };
        var litA = new DynamicEnumerationLiteral("id", _lionWebVersion, enm) { Key = "key" };
        var litB = new DynamicEnumerationLiteral("id", _lionWebVersion, enm) { Key = "otherKey" };

        var comparer = new EnumerationLiteralIdentityComparer();

        Assert.AreNotSame(litA, litB);
        Assert.AreNotEqual(comparer.GetHashCode(litA), comparer.GetHashCode(litB));
        Assert.IsFalse(comparer.Equals(litA, litB));
    }

    [TestMethod]
    public void Equals_Different_Name()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var enm = new DynamicEnumeration("id", _lionWebVersion, lang) { Key = "enumKey" };
        var litA = new DynamicEnumerationLiteral("id", _lionWebVersion, enm) { Name = "a", Key = "key" };
        var litB = new DynamicEnumerationLiteral("id", _lionWebVersion, enm) { Name = "b", Key = "key" };

        var comparer = new EnumerationLiteralIdentityComparer();

        Assert.AreNotSame(litA, litB);
        Assert.AreEqual(comparer.GetHashCode(litA), comparer.GetHashCode(litB));
        Assert.IsTrue(comparer.Equals(litA, litB));
    }

    [TestMethod]
    public void Equals_LeftNull()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var enm = new DynamicEnumeration("id", _lionWebVersion, lang) { Key = "enumKey" };
        var lit = new DynamicEnumerationLiteral("id", _lionWebVersion, enm) { Key = "key" };

        var comparer = new EnumerationLiteralIdentityComparer();

        Assert.IsFalse(comparer.Equals(null, lit));
    }

    [TestMethod]
    public void Equals_RightNull()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var enm = new DynamicEnumeration("id", _lionWebVersion, lang) { Key = "enumKey" };
        var lit = new DynamicEnumerationLiteral("id", _lionWebVersion, enm) { Key = "key" };

        var comparer = new EnumerationLiteralIdentityComparer();

        Assert.IsFalse(comparer.Equals(lit, null));
    }

    [TestMethod]
    public void Equals_BothNull()
    {
        var comparer = new EnumerationLiteralIdentityComparer();

        Assert.IsTrue(comparer.Equals(null, null));
    }
}
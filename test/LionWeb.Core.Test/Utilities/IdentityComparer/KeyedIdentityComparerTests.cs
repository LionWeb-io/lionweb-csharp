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
using M3;

[TestClass]
public class KeyedIdentityComparerTests
{
    readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    [TestMethod]
    public void Language()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "otherKey", Version = "langVer" };

        var comparer = new KeyedIdentityComparer();

        Assert.AreEqual(langA.GetHashCodeIdentity(), comparer.GetHashCode(langA));
        Assert.IsTrue(comparer.Equals(langA, langA));
        Assert.IsFalse(comparer.Equals(langA, langB));
    }

    [TestMethod]
    public void Annotation()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityA = new DynamicAnnotation("id", _lionWebVersion, langA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityB = new DynamicAnnotation("id", _lionWebVersion, langB) { Key = "otherKey" };

        var comparer = new KeyedIdentityComparer();

        Assert.AreEqual(entityA.GetHashCodeIdentity(), comparer.GetHashCode(entityA));
        Assert.IsTrue(comparer.Equals(entityA, entityA));
        Assert.IsFalse(comparer.Equals(entityA, entityB));
    }

    [TestMethod]
    public void Concept()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityA = new DynamicConcept("id", _lionWebVersion, langA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityB = new DynamicConcept("id", _lionWebVersion, langB) { Key = "otherKey" };

        var comparer = new KeyedIdentityComparer();

        Assert.AreEqual(entityA.GetHashCodeIdentity(), comparer.GetHashCode(entityA));
        Assert.IsTrue(comparer.Equals(entityA, entityA));
        Assert.IsFalse(comparer.Equals(entityA, entityB));
    }

    [TestMethod]
    public void Iface()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityA = new DynamicInterface("id", _lionWebVersion, langA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityB = new DynamicInterface("id", _lionWebVersion, langB) { Key = "otherKey" };

        var comparer = new KeyedIdentityComparer();

        Assert.AreEqual(entityA.GetHashCodeIdentity(), comparer.GetHashCode(entityA));
        Assert.IsTrue(comparer.Equals(entityA, entityA));
        Assert.IsFalse(comparer.Equals(entityA, entityB));
    }

    [TestMethod]
    public void Enumeration()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityA = new DynamicEnumeration("id", _lionWebVersion, langA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityB = new DynamicEnumeration("id", _lionWebVersion, langB) { Key = "otherKey" };

        var comparer = new KeyedIdentityComparer();

        Assert.AreEqual(entityA.GetHashCodeIdentity(), comparer.GetHashCode(entityA));
        Assert.IsTrue(comparer.Equals(entityA, entityA));
        Assert.IsFalse(comparer.Equals(entityA, entityB));
    }

    [TestMethod]
    public void Primitive()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityA = new DynamicPrimitiveType("id", _lionWebVersion, langA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityB = new DynamicPrimitiveType("id", _lionWebVersion, langB) { Key = "otherKey" };

        var comparer = new KeyedIdentityComparer();

        Assert.AreEqual(entityA.GetHashCodeIdentity(), comparer.GetHashCode(entityA));
        Assert.IsTrue(comparer.Equals(entityA, entityA));
        Assert.IsFalse(comparer.Equals(entityA, entityB));
    }

    [TestMethod]
    public void Containment()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityA = new DynamicInterface("id", _lionWebVersion, langA) { Key = "key" };
        var featureA = new DynamicContainment("id", _lionWebVersion, entityA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityB = new DynamicInterface("id", _lionWebVersion, langB) { Key = "key" };
        var featureB = new DynamicContainment("id", _lionWebVersion, entityB) { Key = "otherKey" };

        var comparer = new KeyedIdentityComparer();

        Assert.AreEqual(featureA.GetHashCodeIdentity(), comparer.GetHashCode(featureA));
        Assert.IsTrue(comparer.Equals(featureA, featureA));
        Assert.IsFalse(comparer.Equals(featureA, featureB));
    }

    [TestMethod]
    public void Property()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityA = new DynamicInterface("id", _lionWebVersion, langA) { Key = "key" };
        var featureA = new DynamicProperty("id", _lionWebVersion, entityA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityB = new DynamicInterface("id", _lionWebVersion, langB) { Key = "key" };
        var featureB = new DynamicProperty("id", _lionWebVersion, entityB) { Key = "otherKey" };

        var comparer = new KeyedIdentityComparer();

        Assert.AreEqual(featureA.GetHashCodeIdentity(), comparer.GetHashCode(featureA));
        Assert.IsTrue(comparer.Equals(featureA, featureA));
        Assert.IsFalse(comparer.Equals(featureA, featureB));
    }

    [TestMethod]
    public void Reference()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityA = new DynamicInterface("id", _lionWebVersion, langA) { Key = "key" };
        var featureA = new DynamicReference("id", _lionWebVersion, entityA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityB = new DynamicInterface("id", _lionWebVersion, langB) { Key = "key" };
        var featureB = new DynamicReference("id", _lionWebVersion, entityB) { Key = "otherKey" };

        var comparer = new KeyedIdentityComparer();

        Assert.AreEqual(featureA.GetHashCodeIdentity(), comparer.GetHashCode(featureA));
        Assert.IsTrue(comparer.Equals(featureA, featureA));
        Assert.IsFalse(comparer.Equals(featureA, featureB));
    }

    [TestMethod]
    public void Literal()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var enmA = new DynamicEnumeration("id", _lionWebVersion, langA) { Key = "key" };
        var litA = new DynamicEnumerationLiteral("id", _lionWebVersion, enmA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var enmB = new DynamicEnumeration("id", _lionWebVersion, langB) { Key = "key" };
        var litB = new DynamicEnumerationLiteral("id", _lionWebVersion, enmB) { Key = "otherKey" };

        var comparer = new KeyedIdentityComparer();

        Assert.AreEqual(litA.GetHashCodeIdentity(), comparer.GetHashCode(litA));
        Assert.IsTrue(comparer.Equals(litA, litA));
        Assert.IsFalse(comparer.Equals(litA, litB));
    }

    [TestMethod]
    public void Field()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var sdtA = new DynamicStructuredDataType("id", _lionWebVersion, langA) { Key = "key" };
        var fieldA = new DynamicField("id", _lionWebVersion, sdtA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var sdtB = new DynamicStructuredDataType("id", _lionWebVersion, langB) { Key = "key" };
        var fieldB = new DynamicField("id", _lionWebVersion, sdtB) { Key = "otherKey" };

        var comparer = new KeyedIdentityComparer();

        Assert.AreEqual(fieldA.GetHashCodeIdentity(), comparer.GetHashCode(fieldA));
        Assert.IsTrue(comparer.Equals(fieldA, fieldA));
        Assert.IsFalse(comparer.Equals(fieldA, fieldB));
    }

    [TestMethod]
    public void Sdt()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityA = new DynamicStructuredDataType("id", _lionWebVersion, langA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityB = new DynamicStructuredDataType("id", _lionWebVersion, langB) { Key = "otherKey" };

        var comparer = new KeyedIdentityComparer();

        Assert.AreEqual(entityA.GetHashCodeIdentity(), comparer.GetHashCode(entityA));
        Assert.IsTrue(comparer.Equals(entityA, entityA));
        Assert.IsFalse(comparer.Equals(entityA, entityB));
    }

    [TestMethod]
    public void Equals_LeftNull()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };

        var comparer = new KeyedIdentityComparer();

        Assert.IsFalse(comparer.Equals(null, lang));
    }

    [TestMethod]
    public void Equals_RightNull()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };

        var comparer = new KeyedIdentityComparer();

        Assert.IsFalse(comparer.Equals(lang, null));
    }

    [TestMethod]
    public void Equals_BothNull()
    {
        var comparer = new KeyedIdentityComparer();

        Assert.IsTrue(comparer.Equals(null, null));
    }
}
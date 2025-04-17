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
public class LanguageEntityIdentityComparerTests
{
    readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    [TestMethod]
    public void Same()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entity = new DynamicConcept("id", _lionWebVersion, lang) { Key = "key" };

        var comparer = new LanguageEntityIdentityComparer();
        Assert.AreSame(lang, entity.GetLanguage());
        Assert.AreEqual(comparer.GetHashCode(entity), comparer.GetHashCode(entity));
        Assert.IsTrue(comparer.Equals(entity, entity));
    }

    [TestMethod]
    public void Same_Changed_Key()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entity = new DynamicConcept("id", _lionWebVersion, lang) { Key = "key" };

        var comparer = new LanguageEntityIdentityComparer();

        var hashCodeA = comparer.GetHashCode(entity);
        entity.Key = "otherKey";
        var hashCodeB = comparer.GetHashCode(entity);

        Assert.AreNotEqual(hashCodeA, hashCodeB);
    }

    [TestMethod]
    public void Same_Changed_Version()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entity = new DynamicConcept("id", _lionWebVersion, lang) { Key = "key" };

        var comparer = new LanguageEntityIdentityComparer();

        var hashCodeA = comparer.GetHashCode(entity);
        lang.Version = "Y";
        var hashCodeB = comparer.GetHashCode(entity);

        Assert.AreNotEqual(hashCodeA, hashCodeB);
    }

    [TestMethod]
    public void Equals_SameLanguage()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityA = new DynamicConcept("id", _lionWebVersion, lang) { Key = "key" };
        var entityB = new DynamicConcept("id", _lionWebVersion, lang) { Key = "key" };

        var comparer = new LanguageEntityIdentityComparer();

        Assert.AreNotSame(entityA, entityB);
        Assert.AreEqual(comparer.GetHashCode(entityA), comparer.GetHashCode(entityB));
        Assert.IsTrue(comparer.Equals(entityA, entityB));
    }

    [TestMethod]
    public void Equals_EqualLanguage()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityA = new DynamicConcept("id", _lionWebVersion, langA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityB = new DynamicConcept("id", _lionWebVersion, langB) { Key = "key" };

        var comparer = new LanguageEntityIdentityComparer();

        Assert.AreNotSame(entityA, entityB);
        Assert.AreEqual(comparer.GetHashCode(entityA), comparer.GetHashCode(entityB));
        Assert.IsTrue(comparer.Equals(entityA, entityB));
    }

    [TestMethod]
    public void Different_DifferentLanguage()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityA = new DynamicConcept("id", _lionWebVersion, langA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langOtherKey", Version = "langVer" };
        var entityB = new DynamicConcept("id", _lionWebVersion, langB) { Key = "key" };

        var comparer = new LanguageEntityIdentityComparer();

        Assert.AreNotSame(entityA, entityB);
        Assert.AreNotEqual(comparer.GetHashCode(entityA), comparer.GetHashCode(entityB));
        Assert.IsFalse(comparer.Equals(entityA, entityB));
    }

    [TestMethod]
    public void Equals_Different_Type()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        LanguageEntity entityA = new DynamicConcept("id", _lionWebVersion, langA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        LanguageEntity entityB = new DynamicEnumeration("id", _lionWebVersion, langB) { Key = "key" };

        var comparer = new LanguageEntityIdentityComparer();

        Assert.AreNotSame(entityA, entityB);
        Assert.AreEqual(comparer.GetHashCode(entityA), comparer.GetHashCode(entityB));
        Assert.IsTrue(comparer.Equals(entityA, entityB));
    }

    [TestMethod]
    public void Different_Key()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityA = new DynamicConcept("id", _lionWebVersion, lang) { Key = "key" };
        var entityB = new DynamicConcept("id", _lionWebVersion, lang) { Key = "otherKey" };

        var comparer = new LanguageEntityIdentityComparer();

        Assert.AreNotSame(entityA, entityB);
        Assert.AreNotEqual(comparer.GetHashCode(entityA), comparer.GetHashCode(entityB));
        Assert.IsFalse(comparer.Equals(entityA, entityB));
    }

    [TestMethod]
    public void Equals_Different_Name()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityA = new DynamicConcept("id", _lionWebVersion, lang) { Name = "a", Key = "key" };
        var entityB = new DynamicConcept("id", _lionWebVersion, lang) { Name = "b", Key = "key" };

        var comparer = new LanguageEntityIdentityComparer();

        Assert.AreNotSame(entityA, entityB);
        Assert.AreEqual(comparer.GetHashCode(entityA), comparer.GetHashCode(entityB));
        Assert.IsTrue(comparer.Equals(entityA, entityB));
    }

    [TestMethod]
    public void Equals_LeftNull()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entity = new DynamicConcept("id", _lionWebVersion, lang) { Key = "key" };

        var comparer = new LanguageEntityIdentityComparer();

        Assert.IsFalse(comparer.Equals(null, entity));
    }

    [TestMethod]
    public void Equals_RightNull()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entity = new DynamicConcept("id", _lionWebVersion, lang) { Key = "key" };

        var comparer = new LanguageEntityIdentityComparer();

        Assert.IsFalse(comparer.Equals(entity, null));
    }

    [TestMethod]
    public void Equals_BothNull()
    {
        var comparer = new LanguageEntityIdentityComparer();

        Assert.IsTrue(comparer.Equals(null, null));
    }
}
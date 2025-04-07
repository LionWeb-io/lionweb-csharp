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

namespace LionWeb.Core.Test.Utilities;

using Core.Utilities;
using M3;

[TestClass]
public class EqualityComparerTests
{
    LionWebVersions _lionWebVersion = LionWebVersions.Current;

    [TestMethod]
    public void LanguageIdentityComparer_Same()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "key", Version = "x" };

        var comparer = new LanguageIdentityComparer();
        Assert.AreEqual(comparer.GetHashCode(lang), comparer.GetHashCode(lang));
        Assert.IsTrue(comparer.Equals(lang, lang));
    }

    [TestMethod]
    public void LanguageIdentityComparer_Same_Changed_Key()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "key", Version = "x" };

        var comparer = new LanguageIdentityComparer();

        var hashCodeA = comparer.GetHashCode(lang);
        lang.Key = "otherKey";
        var hashCodeB = comparer.GetHashCode(lang);

        Assert.AreNotEqual(hashCodeA, hashCodeB);
    }

    [TestMethod]
    public void LanguageIdentityComparer_Same_Changed_Version()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "key", Version = "x" };

        var comparer = new LanguageIdentityComparer();

        var hashCodeA = comparer.GetHashCode(lang);
        lang.Version = "Y";
        var hashCodeB = comparer.GetHashCode(lang);

        Assert.AreNotEqual(hashCodeA, hashCodeB);
    }

    [TestMethod]
    public void LanguageIdentityComparer_Equals()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "key", Version = "x" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "key", Version = "x" };

        var comparer = new LanguageIdentityComparer();

        Assert.AreNotSame(langA, langB);
        Assert.AreEqual(comparer.GetHashCode(langA), comparer.GetHashCode(langB));
        Assert.IsTrue(comparer.Equals(langA, langB));
    }

    [TestMethod]
    public void LanguageIdentityComparer_Different_Key()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "key", Version = "x" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "otherKey", Version = "x" };

        var comparer = new LanguageIdentityComparer();

        Assert.AreNotSame(langA, langB);
        Assert.AreNotEqual(comparer.GetHashCode(langA), comparer.GetHashCode(langB));
        Assert.IsFalse(comparer.Equals(langA, langB));
    }

    [TestMethod]
    public void LanguageIdentityComparer_Different_Version()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "key", Version = "x" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "key", Version = "y" };

        var comparer = new LanguageIdentityComparer();

        Assert.AreNotSame(langA, langB);
        Assert.AreNotEqual(comparer.GetHashCode(langA), comparer.GetHashCode(langB));
        Assert.IsFalse(comparer.Equals(langA, langB));
    }

    [TestMethod]
    public void LanguageIdentityComparer_Equals_Different_Name()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Name = "a", Key = "key", Version = "x" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Name = "b", Key = "key", Version = "x" };

        var comparer = new LanguageIdentityComparer();

        Assert.AreNotSame(langA, langB);
        Assert.AreEqual(comparer.GetHashCode(langA), comparer.GetHashCode(langB));
        Assert.IsTrue(comparer.Equals(langA, langB));
    }

    [TestMethod]
    public void LanguageIdentityComparer_Equals_LeftNull()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Name = "a", Key = "key", Version = "x" };

        var comparer = new LanguageIdentityComparer();

        Assert.IsFalse(comparer.Equals(null, lang));
    }

    [TestMethod]
    public void LanguageIdentityComparer_Equals_RightNull()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Name = "a", Key = "key", Version = "x" };

        var comparer = new LanguageIdentityComparer();

        Assert.IsFalse(comparer.Equals(lang, null));
    }

    [TestMethod]
    public void LanguageIdentityComparer_Equals_BothNull()
    {
        var comparer = new LanguageIdentityComparer();

        Assert.IsTrue(comparer.Equals(null, null));
    }
}
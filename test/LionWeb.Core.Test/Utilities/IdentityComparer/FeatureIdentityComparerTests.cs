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
public class FeatureIdentityComparerTests
{
    readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    [TestMethod]
    public void Same()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entity = new DynamicConcept("id", _lionWebVersion, lang) { Key = "conceptKey" };
        var feature = new DynamicProperty("id", _lionWebVersion, entity) { Key = "key" };

        var comparer = new FeatureIdentityComparer();
        Assert.AreSame(entity, feature.GetFeatureClassifier());
        Assert.AreEqual(comparer.GetHashCode(feature), comparer.GetHashCode(feature));
        Assert.IsTrue(comparer.Equals(feature, feature));
    }

    [TestMethod]
    public void Same_Changed_Key()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entity = new DynamicConcept("id", _lionWebVersion, lang) { Key = "conceptKey" };
        var feature = new DynamicProperty("id", _lionWebVersion, entity) { Key = "key" };

        var comparer = new FeatureIdentityComparer();

        var hashCodeA = comparer.GetHashCode(feature);
        feature.Key = "otherKey";
        var hashCodeB = comparer.GetHashCode(feature);

        Assert.AreNotEqual(hashCodeA, hashCodeB);
    }

    [TestMethod]
    public void Same_Changed_ConceptKey()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entity = new DynamicConcept("id", _lionWebVersion, lang) { Key = "conceptKey" };
        var feature = new DynamicProperty("id", _lionWebVersion, entity) { Key = "key" };

        var comparer = new FeatureIdentityComparer();

        var hashCodeA = comparer.GetHashCode(feature);
        entity.Key = "Y";
        var hashCodeB = comparer.GetHashCode(feature);

        Assert.AreNotEqual(hashCodeA, hashCodeB);
    }

    [TestMethod]
    public void Same_Changed_Version()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entity = new DynamicConcept("id", _lionWebVersion, lang) { Key = "conceptKey" };
        var feature = new DynamicProperty("id", _lionWebVersion, entity) { Key = "key" };

        var comparer = new FeatureIdentityComparer();

        var hashCodeA = comparer.GetHashCode(feature);
        lang.Version = "Y";
        var hashCodeB = comparer.GetHashCode(feature);

        Assert.AreNotEqual(hashCodeA, hashCodeB);
    }

    [TestMethod]
    public void Equals_SameConcept()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entity = new DynamicConcept("id", _lionWebVersion, lang) { Key = "conceptKey" };
        var featureA = new DynamicProperty("id", _lionWebVersion, entity) { Key = "key" };
        var featureB = new DynamicProperty("id", _lionWebVersion, entity) { Key = "key" };

        var comparer = new FeatureIdentityComparer();

        Assert.AreNotSame(featureA, featureB);
        Assert.AreEqual(comparer.GetHashCode(featureA), comparer.GetHashCode(featureB));
        Assert.IsTrue(comparer.Equals(featureA, featureB));
    }

    [TestMethod]
    public void Equals_EqualConcept()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityA = new DynamicConcept("id", _lionWebVersion, langA) { Key = "key" };
        var featureA = new DynamicProperty("id", _lionWebVersion, entityA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityB = new DynamicConcept("id", _lionWebVersion, langB) { Key = "key" };
        var featureB = new DynamicProperty("id", _lionWebVersion, entityB) { Key = "key" };

        var comparer = new FeatureIdentityComparer();

        Assert.AreNotSame(featureA, featureB);
        Assert.AreEqual(comparer.GetHashCode(featureA), comparer.GetHashCode(featureB));
        Assert.IsTrue(comparer.Equals(featureA, featureB));
    }

    [TestMethod]
    public void Different_DifferentConcept()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityA = new DynamicConcept("id", _lionWebVersion, langA) { Key = "key" };
        var featureA = new DynamicProperty("id", _lionWebVersion, entityA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityB = new DynamicConcept("id", _lionWebVersion, langB) { Key = "otherKey" };
        var featureB = new DynamicProperty("id", _lionWebVersion, entityB) { Key = "key" };

        var comparer = new FeatureIdentityComparer();

        Assert.AreNotSame(featureA, featureB);
        Assert.AreNotEqual(comparer.GetHashCode(featureA), comparer.GetHashCode(featureB));
        Assert.IsFalse(comparer.Equals(featureA, featureB));
    }

    [TestMethod]
    public void Equals_Different_Type()
    {
        var langA = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityA = new DynamicConcept("id", _lionWebVersion, langA) { Key = "key" };
        Feature featureA = new DynamicContainment("id", _lionWebVersion, entityA) { Key = "key" };
        var langB = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entityB = new DynamicConcept("id", _lionWebVersion, langB) { Key = "key" };
        Feature featureB = new DynamicReference("id", _lionWebVersion, entityB) { Key = "key" };

        var comparer = new FeatureIdentityComparer();

        Assert.AreNotSame(featureA, featureB);
        Assert.AreEqual(comparer.GetHashCode(featureA), comparer.GetHashCode(featureB));
        Assert.IsTrue(comparer.Equals(featureA, featureB));
    }

    [TestMethod]
    public void Different_Key()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entity = new DynamicConcept("id", _lionWebVersion, lang) { Key = "conceptKey" };
        var featureA = new DynamicProperty("id", _lionWebVersion, entity) { Key = "key" };
        var featureB = new DynamicProperty("id", _lionWebVersion, entity) { Key = "otherKey" };

        var comparer = new FeatureIdentityComparer();

        Assert.AreNotSame(featureA, featureB);
        Assert.AreNotEqual(comparer.GetHashCode(featureA), comparer.GetHashCode(featureB));
        Assert.IsFalse(comparer.Equals(featureA, featureB));
    }

    [TestMethod]
    public void Equals_Different_Name()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entity = new DynamicConcept("id", _lionWebVersion, lang) { Key = "conceptKey" };
        var featureA = new DynamicProperty("id", _lionWebVersion, entity) { Name = "a", Key = "key" };
        var featureB = new DynamicProperty("id", _lionWebVersion, entity) { Name = "b", Key = "key" };

        var comparer = new FeatureIdentityComparer();

        Assert.AreNotSame(featureA, featureB);
        Assert.AreEqual(comparer.GetHashCode(featureA), comparer.GetHashCode(featureB));
        Assert.IsTrue(comparer.Equals(featureA, featureB));
    }

    [TestMethod]
    public void Equals_LeftNull()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entity = new DynamicConcept("id", _lionWebVersion, lang) { Key = "conceptKey" };
        var feature = new DynamicProperty("id", _lionWebVersion, entity) { Key = "key" };

        var comparer = new FeatureIdentityComparer();

        Assert.IsFalse(comparer.Equals(null, feature));
    }

    [TestMethod]
    public void Equals_RightNull()
    {
        var lang = new DynamicLanguage("id", _lionWebVersion) { Key = "langKey", Version = "langVer" };
        var entity = new DynamicConcept("id", _lionWebVersion, lang) { Key = "conceptKey" };
        var feature = new DynamicProperty("id", _lionWebVersion, entity) { Key = "key" };

        var comparer = new FeatureIdentityComparer();

        Assert.IsFalse(comparer.Equals(feature, null));
    }

    [TestMethod]
    public void Equals_BothNull()
    {
        var comparer = new FeatureIdentityComparer();

        Assert.IsTrue(comparer.Equals(null, null));
    }
}
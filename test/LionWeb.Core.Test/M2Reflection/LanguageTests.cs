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

// ReSharper disable InconsistentNaming

namespace LionWeb.Core.Test.M2Reflection;

using M1;
using M3;

[TestClass]
public class LanguageTests : M2ReflectionTestsBase
{
    [TestMethod]
    public void Meta()
    {
        Assert.AreEqual(_m3.Language, lang.GetClassifier());

        CollectionAssert.AreEqual(new List<Feature>
            {
                INamedName,
                IKeyedKey,
                LanguageVersion,
                LanguageEntities,
                LanguageDependsOn
            },
            lang.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Get()
    {
        Assert.AreEqual(lang.Name, lang.Get(INamedName));
        Assert.AreEqual(lang.Key, lang.Get(IKeyedKey));
        Assert.AreEqual(lang.Version, lang.Get(LanguageVersion));
        CollectionAssert.AreEqual(lang.Entities.ToList(),
            (lang.Get(LanguageEntities) as IReadOnlyList<LanguageEntity>).ToList());
        CollectionAssert.AreEqual(lang.DependsOn.ToList(),
            (lang.Get(LanguageDependsOn) as IReadOnlyList<Language>).ToList());
        Assert.ThrowsExactly<UnknownFeatureException>(() => lang.Get(AnnotationAnnotates));
    }

    [TestMethod]
    public void Set_Name()
    {
        lang.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", lang.Name);
        Assert.ThrowsExactly<InvalidValueException>(() => lang.Set(INamedName, null));
        Assert.AreEqual("Hello", lang.Name);
        Assert.ThrowsExactly<InvalidValueException>(() => lang.Set(INamedName, 123));
    }

    [TestMethod]
    public void TryGet_Name()
    {
        var node = new DynamicLanguage("a", _lionWebVersion);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Key()
    {
        var language = lang;
        language.Set(IKeyedKey, "Hello");
        Assert.AreEqual("Hello", language.Key);
        Assert.ThrowsExactly<InvalidValueException>(() => language.Set(IKeyedKey, null));
        Assert.AreEqual("Hello", language.Key);
        Assert.ThrowsExactly<InvalidValueException>(() => language.Set(IKeyedKey, 123));
    }

    [TestMethod]
    public void TryGet_Key()
    {
        var node = new DynamicLanguage("a", _lionWebVersion);
        Assert.IsFalse(node.TryGetKey(out _));

        node.Key = "Hello";
        Assert.IsTrue(node.TryGetKey(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Version()
    {
        lang.Set(LanguageVersion, "453");
        Assert.AreEqual("453", lang.Version);
        Assert.ThrowsExactly<InvalidValueException>(() => lang.Set(LanguageVersion, null));
        Assert.AreEqual("453", lang.Version);
        Assert.ThrowsExactly<InvalidValueException>(() => lang.Set(LanguageVersion, 123));
    }

    [TestMethod]
    public void TryGet_Version()
    {
        var node = new DynamicLanguage("a", _lionWebVersion);
        Assert.IsFalse(node.TryGetVersion(out _));

        node.Version = "Hello";
        Assert.IsTrue(node.TryGetVersion(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Entities()
    {
        Concept conceptA = new DynamicConcept("my-id", _lionWebVersion, lang) { Key = "my-key", Name = "SomeName" };
        Enumeration enumA = new DynamicEnumeration("enum-id", _lionWebVersion, lang) { Key = "enum-key", Name = "SomeEnum" };
        lang.Set(LanguageEntities, new List<LanguageEntity> { conceptA, enumA });
        CollectionAssert.AreEqual(new List<object> { conceptA, enumA }, lang.Entities.ToList());
        lang.Set(LanguageEntities, Enumerable.Empty<LanguageEntity>());
        CollectionAssert.AreEqual(Enumerable.Empty<LanguageEntity>().ToList(), lang.Entities.ToList());
        Assert.ThrowsExactly<InvalidValueException>(() => lang.Set(LanguageEntities, null));
    }

    [TestMethod]
    public void TryGet_Entities()
    {
        var node = new DynamicLanguage("a", _lionWebVersion);
        Assert.IsFalse(((Language)node).TryGetEntities(out var empty));
        Assert.IsTrue(empty?.Count == 0);

        node.AddEntities([new DynamicConcept("b", _lionWebVersion, null)]);
        Assert.IsTrue(((Language)node).TryGetEntities(out var value));
        Assert.IsFalse(value.Count == 0);
    }

    [TestMethod]
    public void Replace_Entity()
    {
        DynamicConcept conceptA = new DynamicConcept("my-id", _lionWebVersion, lang) { Key = "my-key", Name = "SomeName" };
        DynamicEnumeration enumA = new DynamicEnumeration("enum-id", _lionWebVersion, lang) { Key = "enum-key", Name = "SomeEnum" };
        lang.Set(LanguageEntities, new List<LanguageEntity> { conceptA });
        CollectionAssert.AreEqual(new List<object> { conceptA }, lang.Entities.ToList());
        Assert.AreSame(_lionWebVersion.LionCore.Language_entities, lang.GetContainmentOf(conceptA));
        conceptA.ReplaceWith(enumA);
        CollectionAssert.AreEqual(new List<object> { enumA }, lang.Entities.ToList());
        Assert.IsNull(conceptA.GetParent());
        Assert.IsNull(lang.GetContainmentOf(conceptA));
        Assert.AreSame(lang, enumA.GetParent());
    }
    
    [TestMethod]
    public void Set_DependsOn()
    {
        Language langA =
            new DynamicLanguage("langAId", _lionWebVersion) { Version = "123", Key = "langAKey", Name = "LangA" };
        Language langB =
            new DynamicLanguage("langBId", _lionWebVersion) { Version = "23", Key = "langBKey", Name = "LangB" };
        lang.Set(LanguageDependsOn, new List<Language> { langA, langB });
        CollectionAssert.AreEqual(new List<Language> { langA, langB }, lang.DependsOn.ToList());
        lang.Set(LanguageDependsOn, Enumerable.Empty<Language>());
        CollectionAssert.AreEqual(Enumerable.Empty<Language>().ToList(), lang.DependsOn.ToList());
        Assert.ThrowsExactly<InvalidValueException>(() => lang.Set(LanguageDependsOn, null));
    }

    [TestMethod]
    public void TryGet_DependsOn()
    {
        var node = new DynamicLanguage("a", _lionWebVersion);
        Assert.IsFalse(((Language)node).TryGetDependsOn(out var empty));
        Assert.IsTrue(empty?.Count == 0);

        node.AddDependsOn([node]);
        Assert.IsTrue(((Language)node).TryGetDependsOn(out var value));
        Assert.IsFalse(value.Count == 0);
    }

    [TestMethod]
    public void Set_Invalid()
    {
        Assert.ThrowsExactly<UnknownFeatureException>(() => lang.Set(AnnotationExtends, null));
    }
}
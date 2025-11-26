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

using M3;

[TestClass]
public class EnumerationTests : M2ReflectionTestsBase
{
    [TestMethod]
    public void Meta()
    {
        Assert.AreEqual(_m3.Enumeration, enm.GetClassifier());

        CollectionAssert.AreEqual(new List<Feature> { INamedName, IKeyedKey, EnumerationLiterals },
            enm.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Get()
    {
        Assert.AreEqual(enm.Name, enm.Get(INamedName));
        Assert.AreEqual(enm.Key, enm.Get(IKeyedKey));
        CollectionAssert.AreEqual(enm.Literals.ToList(),
            (enm.Get(EnumerationLiterals) as IReadOnlyList<EnumerationLiteral>).ToList());
        Assert.ThrowsException<UnknownFeatureException>(() => enm.Get(LanguageVersion));
    }

    [TestMethod]
    public void Set_Name()
    {
        enm.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", enm.Name);
        Assert.ThrowsException<InvalidValueException>(() => enm.Set(INamedName, null));
        Assert.AreEqual("Hello", enm.Name);
        Assert.ThrowsException<InvalidValueException>(() => enm.Set(INamedName, 123));
    }

    [TestMethod]
    public void TryGet_Name()
    {
        var node = new DynamicEnumeration("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Key()
    {
        var enumeration = enm;
        enumeration.Set(IKeyedKey, "Hello");
        Assert.AreEqual("Hello", enumeration.Key);
        Assert.ThrowsException<InvalidValueException>(() => enumeration.Set(IKeyedKey, null));
        Assert.AreEqual("Hello", enumeration.Key);
        Assert.ThrowsException<InvalidValueException>(() => enumeration.Set(IKeyedKey, 123));
    }

    [TestMethod]
    public void TryGet_Key()
    {
        var node = new DynamicEnumeration("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetKey(out _));

        node.Key = "Hello";
        Assert.IsTrue(node.TryGetKey(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Literals()
    {
        EnumerationLiteral litA = new DynamicEnumerationLiteral("my-id", _lionWebVersion, enm) { Key = "my-key", Name = "SomeName" };
        EnumerationLiteral litB = new DynamicEnumerationLiteral("ref-id", _lionWebVersion, enm) { Key = "ref-key", Name = "SomeRef" };
        enm.Set(EnumerationLiterals, new List<EnumerationLiteral> { litA, litB });
        CollectionAssert.AreEqual(new List<object> { litA, litB }, enm.Literals.ToList());
        enm.Set(EnumerationLiterals, Enumerable.Empty<EnumerationLiteral>());
        CollectionAssert.AreEqual(Enumerable.Empty<EnumerationLiteral>().ToList(), enm.Literals.ToList());
        Assert.ThrowsException<InvalidValueException>(() => enm.Set(EnumerationLiterals, null));
    }

    [TestMethod]
    public void TryGet_Literals()
    {
        var node = new DynamicEnumeration("a", _lionWebVersion, lang);
        Assert.IsFalse(((Enumeration)node).TryGetLiterals(out var empty));
        Assert.IsTrue(empty?.Count == 0);

        node.AddLiterals([new DynamicEnumerationLiteral("b", _lionWebVersion, null)]);
        Assert.IsTrue(((Enumeration)node).TryGetLiterals(out var value));
        Assert.IsFalse(value.Count == 0);
    }

    [TestMethod]
    public void Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => enm.Set(LanguageVersion, "asdf"));
    }
}
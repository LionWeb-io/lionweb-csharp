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
public class EnumerationLiteralTests : M2ReflectionTestsBase
{
    [TestMethod]
    public void Meta()
    {
        Assert.AreEqual(_m3.EnumerationLiteral, enLit.GetClassifier());

        CollectionAssert.AreEqual(new List<Feature> { INamedName, IKeyedKey },
            enLit.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Get()
    {
        Assert.AreEqual(enLit.Name, enLit.Get(INamedName));
        Assert.AreEqual(enLit.Key, enLit.Get(IKeyedKey));
        Assert.ThrowsExactly<UnknownFeatureException>(() => enLit.Get(LanguageVersion));
    }

    [TestMethod]
    public void Set_Name()
    {
        enLit.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", enLit.Name);
        Assert.ThrowsExactly<InvalidValueException>(() => enLit.Set(INamedName, null));
        Assert.AreEqual("Hello", enLit.Name);
        Assert.ThrowsExactly<InvalidValueException>(() => enLit.Set(INamedName, 123));
    }

    [TestMethod]
    public void TryGet_Name()
    {
        var node = new DynamicEnumerationLiteral("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Key()
    {
        var literal = enLit;
        literal.Set(IKeyedKey, "Hello");
        Assert.AreEqual("Hello", literal.Key);
        Assert.ThrowsExactly<InvalidValueException>(() => literal.Set(IKeyedKey, null));
        Assert.AreEqual("Hello", literal.Key);
        Assert.ThrowsExactly<InvalidValueException>(() => literal.Set(IKeyedKey, 123));
    }

    [TestMethod]
    public void TryGet_Key()
    {
        var node = new DynamicEnumerationLiteral("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetKey(out _));

        node.Key = "Hello";
        Assert.IsTrue(node.TryGetKey(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Invalid()
    {
        Assert.ThrowsExactly<UnknownFeatureException>(() => enLit.Set(LanguageVersion, "asdf"));
    }
}
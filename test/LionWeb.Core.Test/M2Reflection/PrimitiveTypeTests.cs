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
public class PrimitiveTypeTests : M2ReflectionTestsBase
{
    [TestMethod]
    public void Meta()
    {
        Assert.AreEqual(_m3.PrimitiveType, prim.GetClassifier());

        CollectionAssert.AreEqual(new List<Feature> { INamedName, IKeyedKey },
            prim.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Get()
    {
        Assert.AreEqual(prim.Name, prim.Get(INamedName));
        Assert.AreEqual(prim.Key, prim.Get(IKeyedKey));
        Assert.ThrowsException<UnknownFeatureException>(() => prim.Get(LanguageVersion));
    }

    [TestMethod]
    public void Set_Name()
    {
        prim.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", prim.Name);
        Assert.ThrowsException<InvalidValueException>(() => prim.Set(INamedName, null));
        Assert.AreEqual("Hello", prim.Name);
        Assert.ThrowsException<InvalidValueException>(() => prim.Set(INamedName, 123));
    }

    [TestMethod]
    public void TryGet_Name()
    {
        var node = new DynamicPrimitiveType("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Key()
    {
        var primitive = prim;
        primitive.Set(IKeyedKey, "Hello");
        Assert.AreEqual("Hello", primitive.Key);
        Assert.ThrowsException<InvalidValueException>(() => primitive.Set(IKeyedKey, null));
        Assert.AreEqual("Hello", primitive.Key);
        Assert.ThrowsException<InvalidValueException>(() => primitive.Set(IKeyedKey, 123));
    }

    [TestMethod]
    public void TryGet_Key()
    {
        var node = new DynamicPrimitiveType("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetKey(out _));

        node.Key = "Hello";
        Assert.IsTrue(node.TryGetKey(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => prim.Set(LanguageVersion, "asdf"));
    }
}
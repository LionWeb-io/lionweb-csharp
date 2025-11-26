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
public class FieldTests : M2ReflectionTestsBase
{
    [TestMethod]
    public void Meta()
    {
        Assert.AreEqual(_m3.Field, fld.GetClassifier());

        CollectionAssert.AreEqual(new List<Feature> { INamedName, IKeyedKey, FieldType },
            fld.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Get()
    {
        Assert.AreEqual(fld.Name, fld.Get(INamedName));
        Assert.AreEqual(fld.Key, fld.Get(IKeyedKey));
        Assert.ThrowsException<UnknownFeatureException>(() => fld.Get(LanguageVersion));
    }

    [TestMethod]
    public void Set_Name()
    {
        fld.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", fld.Name);
        Assert.ThrowsException<InvalidValueException>(() => fld.Set(INamedName, null));
        Assert.AreEqual("Hello", fld.Name);
        Assert.ThrowsException<InvalidValueException>(() => fld.Set(INamedName, 123));
    }

    [TestMethod]
    public void TryGet_Name()
    {
        var node = new DynamicField("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Key()
    {
        fld.Set(IKeyedKey, "Hello");
        Assert.AreEqual("Hello", fld.Key);
        Assert.ThrowsException<InvalidValueException>(() => fld.Set(IKeyedKey, null));
        Assert.AreEqual("Hello", fld.Key);
        Assert.ThrowsException<InvalidValueException>(() => fld.Set(IKeyedKey, 123));
    }

    [TestMethod]
    public void TryGet_Key()
    {
        var node = new DynamicField("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetKey(out _));

        node.Key = "Hello";
        Assert.IsTrue(node.TryGetKey(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => fld.Set(LanguageVersion, "asdf"));
    }

    [TestMethod]
    public void Set_Type()
    {
        fld.Set(FieldType, prim);
        Assert.AreEqual(prim, fld.Type);
        Assert.ThrowsException<InvalidValueException>(() => fld.Set(FieldType, lang));
        Assert.AreEqual(prim, fld.Type);
    }

    [TestMethod]
    public void TryGet_Type()
    {
        var node = new DynamicField("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetType(out _));

        var type = new DynamicPrimitiveType("b", _lionWebVersion, null);
        node.Type = type;
        Assert.IsTrue(node.TryGetType(out var value));
        Assert.AreEqual(type, value);
    }
}
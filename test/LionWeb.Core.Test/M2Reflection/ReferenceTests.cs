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
public class ReferenceTests : M2ReflectionTestsBase
{
    [TestMethod]
    public void Meta()
    {
        Assert.AreEqual(_m3.Reference, refe.GetClassifier());

        var collection = refe.CollectAllSetFeatures().ToList();
        CollectionAssert.AreEqual(new List<Feature>
            {
                INamedName,
                IKeyedKey,
                FeatureOptional,
                LinkMultiple,
                LinkType
            },
            collection);
    }

    [TestMethod]
    public void Get()
    {
        Assert.AreEqual(refe.Name, refe.Get(INamedName));
        Assert.AreEqual(refe.Key, refe.Get(IKeyedKey));
        Assert.AreEqual(refe.Optional, refe.Get(FeatureOptional));
        Assert.AreEqual(refe.Multiple, refe.Get(LinkMultiple));
        Assert.AreEqual(refe.Type, refe.Get(LinkType));
        Assert.ThrowsException<UnknownFeatureException>(() => refe.Get(LanguageVersion));
    }

    [TestMethod]
    public void Set_Name()
    {
        refe.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", refe.Name);
        Assert.ThrowsException<InvalidValueException>(() => refe.Set(INamedName, null));
        Assert.AreEqual("Hello", refe.Name);
        Assert.ThrowsException<InvalidValueException>(() => refe.Set(INamedName, 123));
    }

    [TestMethod]
    public void TryGet_Name()
    {
        var node = new DynamicReference("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Key()
    {
        var reference = refe;
        reference.Set(IKeyedKey, "Hello");
        Assert.AreEqual("Hello", reference.Key);
        Assert.ThrowsException<InvalidValueException>(() => reference.Set(IKeyedKey, null));
        Assert.AreEqual("Hello", reference.Key);
        Assert.ThrowsException<InvalidValueException>(() => reference.Set(IKeyedKey, 123));
    }

    [TestMethod]
    public void TryGet_Key()
    {
        var node = new DynamicReference("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetKey(out _));

        node.Key = "Hello";
        Assert.IsTrue(node.TryGetKey(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Optional()
    {
        refe.Set(FeatureOptional, true);
        Assert.AreEqual(true, refe.Optional);
        refe.Set(FeatureOptional, false);
        Assert.AreEqual(false, refe.Optional);
        Assert.ThrowsException<InvalidValueException>(() => refe.Set(FeatureOptional, null));
    }

    [TestMethod]
    public void TryGet_Optional()
    {
        var node = new DynamicReference("a", _lionWebVersion, null);
        Assert.IsTrue(((Reference)node).TryGetOptional(out var empty));
        Assert.AreEqual(false, empty);

        node.Optional = true;
        Assert.IsTrue(((Reference)node).TryGetOptional(out var value));
        Assert.AreEqual(true, value);
    }

    [TestMethod]
    public void Set_Multiple()
    {
        refe.Set(LinkMultiple, true);
        Assert.AreEqual(true, refe.Multiple);
        refe.Set(LinkMultiple, false);
        Assert.AreEqual(false, refe.Multiple);
        Assert.ThrowsException<InvalidValueException>(() => refe.Set(LinkMultiple, null));
    }

    [TestMethod]
    public void TryGet_Multiple()
    {
        var node = new DynamicReference("a", _lionWebVersion, null);
        Assert.IsTrue(((Reference)node).TryGetMultiple(out var empty));
        Assert.AreEqual(false, empty);

        node.Multiple = true;
        Assert.IsTrue(((Reference)node).TryGetMultiple(out var value));
        Assert.AreEqual(true, value);
    }

    [TestMethod]
    public void Set_Type()
    {
        refe.Set(LinkType, ann);
        Assert.AreEqual(ann, refe.Type);
        Assert.ThrowsException<InvalidValueException>(() => refe.Set(LinkType, lang));
        Assert.AreEqual(ann, refe.Type);
    }

    [TestMethod]
    public void TryGet_Type()
    {
        var node = new DynamicReference("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetType(out _));

        var type = new DynamicConcept("b", _lionWebVersion, null);
        node.Type = type;
        Assert.IsTrue(node.TryGetType(out var value));
        Assert.AreEqual(type, value);
    }

    [TestMethod]
    public void Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => refe.Set(LanguageVersion, "asdf"));
    }
}
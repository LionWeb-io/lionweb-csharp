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
public class ContainmentTests : M2ReflectionTestsBase
{
    [TestMethod]
    public void Meta()
    {
        Assert.AreEqual(_m3.Containment, cont.GetClassifier());

        CollectionAssert.AreEqual(new List<Feature>
            {
                INamedName,
                IKeyedKey,
                FeatureOptional,
                LinkMultiple,
                LinkType
            },
            cont.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Get()
    {
        Assert.AreEqual(cont.Name, cont.Get(INamedName));
        Assert.AreEqual(cont.Key, cont.Get(IKeyedKey));
        Assert.AreEqual(cont.Optional, cont.Get(FeatureOptional));
        Assert.AreEqual(cont.Multiple, cont.Get(LinkMultiple));
        Assert.AreEqual(cont.Type, cont.Get(LinkType));
        Assert.ThrowsException<UnknownFeatureException>(() => cont.Get(LanguageVersion));
    }

    [TestMethod]
    public void Set_Name()
    {
        cont.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", cont.Name);
        Assert.ThrowsException<InvalidValueException>(() => cont.Set(INamedName, null));
        Assert.AreEqual("Hello", cont.Name);
        Assert.ThrowsException<InvalidValueException>(() => cont.Set(INamedName, 123));
    }

    [TestMethod]
    public void TryGet_Name()
    {
        var node = new DynamicContainment("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Key()
    {
        var containment = cont;
        containment.Set(IKeyedKey, "Hello");
        Assert.AreEqual("Hello", containment.Key);
        Assert.ThrowsException<InvalidValueException>(() => containment.Set(IKeyedKey, null));
        Assert.AreEqual("Hello", containment.Key);
        Assert.ThrowsException<InvalidValueException>(() => containment.Set(IKeyedKey, 123));
    }

    [TestMethod]
    public void TryGet_Key()
    {
        var node = new DynamicContainment("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetKey(out _));

        node.Key = "Hello";
        Assert.IsTrue(node.TryGetKey(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Optional()
    {
        cont.Set(FeatureOptional, true);
        Assert.AreEqual(true, cont.Optional);
        cont.Set(FeatureOptional, false);
        Assert.AreEqual(false, cont.Optional);
        Assert.ThrowsException<InvalidValueException>(() => cont.Set(FeatureOptional, null));
    }

    [TestMethod]
    public void TryGet_Optional()
    {
        var node = new DynamicContainment("a", _lionWebVersion, null);
        Assert.IsTrue(((Containment)node).TryGetOptional(out var empty));
        Assert.AreEqual(false, empty);

        node.Optional = true;
        Assert.IsTrue(((Containment)node).TryGetOptional(out var value));
        Assert.AreEqual(true, value);
    }

    [TestMethod]
    public void Set_Multiple()
    {
        cont.Set(LinkMultiple, true);
        Assert.AreEqual(true, cont.Multiple);
        cont.Set(LinkMultiple, false);
        Assert.AreEqual(false, cont.Multiple);
        Assert.ThrowsException<InvalidValueException>(() => cont.Set(LinkMultiple, null));
    }

    [TestMethod]
    public void TryGet_Multiple()
    {
        var node = new DynamicContainment("a", _lionWebVersion, null);
        Assert.IsTrue(((Containment)node).TryGetMultiple(out var empty));
        Assert.AreEqual(false, empty);

        node.Multiple = true;
        Assert.IsTrue(((Containment)node).TryGetMultiple(out var value));
        Assert.AreEqual(true, value);
    }

    [TestMethod]
    public void Set_Type()
    {
        cont.Set(LinkType, ann);
        Assert.AreEqual(ann, cont.Type);
        Assert.ThrowsException<InvalidValueException>(() => cont.Set(LinkType, lang));
        Assert.AreEqual(ann, cont.Type);
    }

    [TestMethod]
    public void TryGet_Type()
    {
        var node = new DynamicContainment("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetType(out _));

        var type = new DynamicConcept("b", _lionWebVersion, null);
        node.Type = type;
        Assert.IsTrue(node.TryGetType(out var value));
        Assert.AreEqual(type, value);
    }

    [TestMethod]
    public void Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => cont.Set(LanguageVersion, "asdf"));
    }
}
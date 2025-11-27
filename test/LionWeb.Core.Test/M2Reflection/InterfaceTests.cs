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
public class InterfaceTests : M2ReflectionTestsBase
{
    [TestMethod]
    public void Meta()
    {
        Assert.AreEqual(_m3.Interface, iface.GetClassifier());

        CollectionAssert.AreEqual(new List<Feature> { INamedName, IKeyedKey, ClassifierFeatures, InterfaceExtends },
            iface.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Get()
    {
        Assert.AreEqual(iface.Name, iface.Get(INamedName));
        Assert.AreEqual(iface.Key, iface.Get(IKeyedKey));
        CollectionAssert.AreEqual(iface.Features.ToList(),
            (iface.Get(ClassifierFeatures) as IReadOnlyList<Feature>).ToList());
        CollectionAssert.AreEqual(iface.Extends.ToList(),
            (iface.Get(InterfaceExtends) as IReadOnlyList<Interface>).ToList());
        Assert.ThrowsExactly<UnknownFeatureException>(() => iface.Get(LanguageVersion));
    }

    [TestMethod]
    public void Set_Name()
    {
        iface.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", iface.Name);
        Assert.ThrowsExactly<InvalidValueException>(() => iface.Set(INamedName, null));
        Assert.AreEqual("Hello", iface.Name);
        Assert.ThrowsExactly<InvalidValueException>(() => iface.Set(INamedName, 123));
    }

    [TestMethod]
    public void TryGet_Name()
    {
        var node = new DynamicInterface("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Key()
    {
        var ifac = iface;
        ifac.Set(IKeyedKey, "Hello");
        Assert.AreEqual("Hello", ifac.Key);
        Assert.ThrowsExactly<InvalidValueException>(() => ifac.Set(IKeyedKey, null));
        Assert.AreEqual("Hello", ifac.Key);
        Assert.ThrowsExactly<InvalidValueException>(() => ifac.Set(IKeyedKey, 123));
    }

    [TestMethod]
    public void TryGet_Key()
    {
        var node = new DynamicInterface("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetKey(out _));

        node.Key = "Hello";
        Assert.IsTrue(node.TryGetKey(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Features()
    {
        Property propA = new DynamicProperty("my-id", _lionWebVersion, ann) { Key = "my-key", Name = "SomeName" };
        Reference refB = new DynamicReference("ref-id", _lionWebVersion, ann) { Key = "ref-key", Name = "SomeRef" };
        iface.Set(ClassifierFeatures, new List<Feature> { propA, refB });
        CollectionAssert.AreEqual(new List<object> { propA, refB }, iface.Features.ToList());
        iface.Set(ClassifierFeatures, Enumerable.Empty<Feature>());
        CollectionAssert.AreEqual(Enumerable.Empty<Feature>().ToList(), iface.Features.ToList());
        Assert.ThrowsExactly<InvalidValueException>(() => iface.Set(ClassifierFeatures, null));
    }

    [TestMethod]
    public void TryGet_Features()
    {
        var node = new DynamicInterface("a", _lionWebVersion, lang);
        Assert.IsFalse(((Interface)node).TryGetFeatures(out var empty));
        Assert.IsTrue(empty?.Count == 0);

        node.AddFeatures([new DynamicProperty("b", _lionWebVersion, null)]);
        Assert.IsTrue(((Interface)node).TryGetFeatures(out var value));
        Assert.IsFalse(value.Count == 0);
    }

    [TestMethod]
    public void Set_Extends()
    {
        Interface ifaceA = new DynamicInterface("my-id", _lionWebVersion, lang) { Key = "my-key", Name = "SomeName" };
        Interface ifaceB = new DynamicInterface("ref-id", _lionWebVersion, lang) { Key = "ref-key", Name = "SomeRef" };
        iface.Set(InterfaceExtends, new List<Interface> { ifaceA, ifaceB });
        CollectionAssert.AreEqual(new List<object> { ifaceA, ifaceB }, iface.Extends.ToList());
        iface.Set(InterfaceExtends, Enumerable.Empty<Interface>());
        CollectionAssert.AreEqual(Enumerable.Empty<Interface>().ToList(), iface.Extends.ToList());
        Assert.ThrowsExactly<InvalidValueException>(() => iface.Set(InterfaceExtends, null));
    }

    [TestMethod]
    public void TryGet_Extends()
    {
        var node = new DynamicInterface("a", _lionWebVersion, lang);
        Assert.IsFalse(((Interface)node).TryGetExtends(out var empty));
        Assert.IsTrue(empty?.Count == 0);

        node.AddExtends([new DynamicInterface("b", _lionWebVersion, null)]);
        Assert.IsTrue(((Interface)node).TryGetExtends(out var value));
        Assert.IsFalse(value.Count == 0);
    }

    [TestMethod]
    public void Set_Invalid()
    {
        Assert.ThrowsExactly<UnknownFeatureException>(() => iface.Set(LanguageVersion, "asdf"));
    }
}
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
public class StructuredDataTypeTests : M2ReflectionTestsBase
{
    [TestMethod]
    public void Meta()
    {
        Assert.AreEqual(_m3.StructuredDataType, sdt.GetClassifier());

        CollectionAssert.AreEqual(new List<Feature> { INamedName, IKeyedKey, Fields },
            sdt.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Get()
    {
        Assert.AreEqual(sdt.Name, sdt.Get(INamedName));
        Assert.AreEqual(sdt.Key, sdt.Get(IKeyedKey));
        CollectionAssert.AreEqual(sdt.Fields.ToList(),
            (sdt.Get(Fields) as IReadOnlyList<Field>).ToList());
        Assert.ThrowsException<UnknownFeatureException>(() => sdt.Get(LanguageVersion));
    }

    [TestMethod]
    public void Set_Name()
    {
        sdt.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", sdt.Name);
        Assert.ThrowsException<InvalidValueException>(() => sdt.Set(INamedName, null));
        Assert.AreEqual("Hello", sdt.Name);
        Assert.ThrowsException<InvalidValueException>(() => sdt.Set(INamedName, 123));
    }

    [TestMethod]
    public void TryGet_Name()
    {
        var node = new DynamicStructuredDataType("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Key()
    {
        sdt.Set(IKeyedKey, "Hello");
        Assert.AreEqual("Hello", sdt.Key);
        Assert.ThrowsException<InvalidValueException>(() => sdt.Set(IKeyedKey, null));
        Assert.AreEqual("Hello", sdt.Key);
        Assert.ThrowsException<InvalidValueException>(() => sdt.Set(IKeyedKey, 123));
    }

    [TestMethod]
    public void TryGet_Key()
    {
        var node = new DynamicStructuredDataType("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetKey(out _));

        node.Key = "Hello";
        Assert.IsTrue(node.TryGetKey(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Fields()
    {
        Field litA = new DynamicField("my-id", _lionWebVersion, sdt) { Key = "my-key", Name = "SomeName" };
        Field litB = new DynamicField("ref-id", _lionWebVersion, sdt) { Key = "ref-key", Name = "SomeRef" };
        sdt.Set(Fields, new List<Field> { litA, litB });
        CollectionAssert.AreEqual(new List<object> { litA, litB }, sdt.Fields.ToList());
        sdt.Set(Fields, Enumerable.Empty<Field>());
        CollectionAssert.AreEqual(Enumerable.Empty<Feature>().ToList(), sdt.Fields.ToList());
        Assert.ThrowsException<InvalidValueException>(() => sdt.Set(Fields, null));
    }

    [TestMethod]
    public void TryGet_Fields()
    {
        var node = new DynamicStructuredDataType("a", _lionWebVersion, lang);
        Assert.IsFalse(((StructuredDataType)node).TryGetFields(out var empty));
        Assert.IsTrue(empty.Count == 0);

        node.AddFields([new DynamicField("b", _lionWebVersion, null)]);
        Assert.IsTrue(((StructuredDataType)node).TryGetFields(out var value));
        Assert.IsFalse(value.Count == 0);
    }

    [TestMethod]
    public void Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => sdt.Set(LanguageVersion, "asdf"));
    }
}
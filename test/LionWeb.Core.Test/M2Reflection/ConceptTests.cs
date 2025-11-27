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
public class ConceptTests : M2ReflectionTestsBase
{
    [TestMethod]
    public void Meta()
    {
        Assert.AreEqual(Concept, conc.GetClassifier());

        CollectionAssert.AreEqual(new List<Feature>
            {
                INamedName,
                IKeyedKey,
                ClassifierFeatures,
                ConceptAbstract,
                ConceptPartition,
                ConceptExtends,
                ConceptImplements
            },
            conc.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Get()
    {
        Assert.AreEqual(conc.Name, conc.Get(INamedName));
        Assert.AreEqual(conc.Key, conc.Get(IKeyedKey));
        CollectionAssert.AreEqual(conc.Features.ToList(),
            (conc.Get(ClassifierFeatures) as IReadOnlyList<Feature>).ToList());
        Assert.AreEqual(conc.Abstract, conc.Get(ConceptAbstract));
        Assert.AreEqual(conc.Partition, conc.Get(ConceptPartition));
        Assert.AreEqual(conc.Extends, conc.Get(ConceptExtends));
        CollectionAssert.AreEqual(conc.Implements.ToList(),
            (conc.Get(ConceptImplements) as IReadOnlyList<Interface>).ToList());
        Assert.ThrowsExactly<UnknownFeatureException>(() => conc.Get(LanguageVersion));
    }

    [TestMethod]
    public void Set_Name()
    {
        conc.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", conc.Name);
        Assert.ThrowsExactly<InvalidValueException>(() => conc.Set(INamedName, null));
        Assert.AreEqual("Hello", conc.Name);
        Assert.ThrowsExactly<InvalidValueException>(() => conc.Set(INamedName, 123));
    }

    [TestMethod]
    public void TryGet_Name()
    {
        var node = new DynamicConcept("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Key()
    {
        var concept = conc;
        concept.Set(IKeyedKey, "myKey");
        Assert.AreEqual("myKey", concept.Key);
        concept.Set(IKeyedKey, "otherKey");
        Assert.AreEqual("otherKey", concept.Key);
        Assert.ThrowsExactly<InvalidValueException>(() => concept.Set(IKeyedKey, null));
    }

    [TestMethod]
    public void TryGet_Key()
    {
        var node = new DynamicConcept("a", _lionWebVersion, null);
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
        conc.Set(ClassifierFeatures, new List<Feature> { propA, refB });
        CollectionAssert.AreEqual(new List<object> { propA, refB }, conc.Features.ToList());
        conc.Set(ClassifierFeatures, Enumerable.Empty<Feature>());
        CollectionAssert.AreEqual(Enumerable.Empty<Feature>().ToList(), conc.Features.ToList());
        Assert.ThrowsExactly<InvalidValueException>(() => conc.Set(ClassifierFeatures, null));
    }

    [TestMethod]
    public void TryGet_Features()
    {
        var node = new DynamicConcept("a", _lionWebVersion, lang);
        Assert.IsFalse(((Concept)node).TryGetFeatures(out var empty));
        Assert.IsTrue(empty?.Count == 0);

        node.AddFeatures([new DynamicProperty("b", _lionWebVersion, null)]);
        Assert.IsTrue(((Concept)node).TryGetFeatures(out var value));
        Assert.IsFalse(value.Count == 0);
    }

    [TestMethod]
    public void Set_Abstract()
    {
        conc.Set(ConceptAbstract, true);
        Assert.AreEqual(true, conc.Abstract);
        conc.Set(ConceptAbstract, false);
        Assert.AreEqual(false, conc.Abstract);
        Assert.ThrowsExactly<InvalidValueException>(() => conc.Set(ConceptAbstract, null));
    }

    [TestMethod]
    public void TryGet_Abstract()
    {
        var node = new DynamicConcept("a", _lionWebVersion, null);
        Assert.IsTrue(((Concept)node).TryGetAbstract(out var empty));
        Assert.AreEqual(false, empty);

        node.Abstract = true;
        Assert.IsTrue(((Concept)node).TryGetAbstract(out var value));
        Assert.AreEqual(true, value);
    }

    [TestMethod]
    public void Set_Partition()
    {
        conc.Set(ConceptPartition, true);
        Assert.AreEqual(true, conc.Partition);
        conc.Set(ConceptPartition, false);
        Assert.AreEqual(false, conc.Partition);
        Assert.ThrowsExactly<InvalidValueException>(() => conc.Set(ConceptPartition, null));
    }

    [TestMethod]
    public void TryGet_Partition()
    {
        var node = new DynamicConcept("a", _lionWebVersion, null);
        Assert.IsTrue(((Concept)node).TryGetPartition(out var empty));
        Assert.AreEqual(false, empty);

        node.Partition = true;
        Assert.IsTrue(((Concept)node).TryGetPartition(out var value));
        Assert.AreEqual(true, value);
    }

    [TestMethod]
    public void Set_Extends()
    {
        Concept sup = new DynamicConcept("my-id", _lionWebVersion, lang) { Key = "my-key", Name = "SomeName" };
        conc.Set(ConceptExtends, sup);
        Assert.AreEqual(sup, conc.Extends);
        conc.Set(ConceptExtends, null);
        Assert.AreEqual(null, conc.Extends);
        Assert.ThrowsExactly<InvalidValueException>(() => conc.Set(ConceptExtends, lang));
    }

    [TestMethod]
    public void TryGet_Extends()
    {
        var node = new DynamicConcept("a", _lionWebVersion, null);
        Assert.IsFalse(((Concept)node).TryGetExtends(out var empty));
        Assert.AreEqual(null, empty);

        node.Extends = node;
        Assert.IsTrue(((Concept)node).TryGetExtends(out var value));
        Assert.AreEqual(node, value);
    }

    [TestMethod]
    public void Set_Implements()
    {
        Interface ifaceA = new DynamicInterface("my-id", _lionWebVersion, lang) { Key = "my-key", Name = "SomeName" };
        Interface ifaceB = new DynamicInterface("ref-id", _lionWebVersion, lang) { Key = "ref-key", Name = "SomeRef" };
        conc.Set(ConceptImplements, new List<Interface> { ifaceA, ifaceB });
        CollectionAssert.AreEqual(new List<object> { ifaceA, ifaceB }, conc.Implements.ToList());
        conc.Set(ConceptImplements, Enumerable.Empty<Interface>());
        CollectionAssert.AreEqual(Enumerable.Empty<Interface>().ToList(), conc.Implements.ToList());
        Assert.ThrowsExactly<InvalidValueException>(() => conc.Set(ConceptImplements, null));
    }

    [TestMethod]
    public void TryGet_Implements()
    {
        var node = new DynamicConcept("a", _lionWebVersion, lang);
        Assert.IsFalse(((Concept)node).TryGetImplements(out var empty));
        Assert.IsTrue(empty?.Count == 0);

        node.AddImplements([new DynamicInterface("b", _lionWebVersion, null)]);
        Assert.IsTrue(((Concept)node).TryGetImplements(out var value));
        Assert.IsFalse(value.Count == 0);
    }

    [TestMethod]
    public void Set_Invalid()
    {
        Assert.ThrowsExactly<UnknownFeatureException>(() => conc.Set(LanguageVersion, "asdf"));
    }
}
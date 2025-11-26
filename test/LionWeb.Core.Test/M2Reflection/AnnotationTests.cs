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
public class AnnotationTests : M2ReflectionTestsBase
{
    [TestMethod]
    public void Meta()
    {
        Assert.AreEqual(_m3.Annotation, ann.GetClassifier());

        CollectionAssert.AreEqual(new List<Feature>
            {
                INamedName,
                IKeyedKey,
                ClassifierFeatures,
                AnnotationAnnotates,
                AnnotationExtends,
                AnnotationImplements
            },
            ann.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Get()
    {
        Assert.AreEqual(ann.Name, ann.Get(INamedName));
        Assert.AreEqual(ann.Key, ann.Get(IKeyedKey));
        CollectionAssert.AreEqual(ann.Features.ToList(),
            (ann.Get(ClassifierFeatures) as IReadOnlyList<Feature>).ToList());
        Assert.AreEqual(ann.Annotates, ann.Get(AnnotationAnnotates));
        Assert.AreEqual(ann.Extends, ann.Get(AnnotationExtends));
        CollectionAssert.AreEqual(ann.Implements.ToList(),
            (ann.Get(AnnotationImplements) as IReadOnlyList<Interface>).ToList());
        Assert.ThrowsException<UnknownFeatureException>(() => ann.Get(LanguageVersion));
    }

    [TestMethod]
    public void Set_Name()
    {
        ann.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", ann.Name);
        Assert.ThrowsException<InvalidValueException>(() => ann.Set(INamedName, null));
        Assert.AreEqual("Hello", ann.Name);
        Assert.ThrowsException<InvalidValueException>(() => ann.Set(INamedName, 123));
        Assert.AreEqual("Hello", ann.Name);
    }

    [TestMethod]
    public void TryGet_Name()
    {
        var node = new DynamicAnnotation("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Set_Key()
    {
        var annotation = ann;
        annotation.Set(IKeyedKey, "asdf");
        Assert.AreEqual("asdf", annotation.Key);
        Assert.ThrowsException<InvalidValueException>(() => annotation.Set(IKeyedKey, null));
        Assert.AreEqual("asdf", annotation.Key);
        Assert.ThrowsException<InvalidValueException>(() => annotation.Set(IKeyedKey, 123));
        Assert.AreEqual("asdf", annotation.Key);
    }

    [TestMethod]
    public void TryGet_Key()
    {
        var node = new DynamicAnnotation("a", _lionWebVersion, null);
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
        ann.Set(ClassifierFeatures, new List<Feature> { propA, refB });
        CollectionAssert.AreEqual(new List<object> { propA, refB }, ann.Features.ToList());
        ann.Set(ClassifierFeatures, Enumerable.Empty<Feature>());
        CollectionAssert.AreEqual(Enumerable.Empty<Feature>().ToList(), ann.Features.ToList());
        Assert.ThrowsException<InvalidValueException>(() => ann.Set(ClassifierFeatures, null));
    }

    [TestMethod]
    public void TryGet_Features()
    {
        var node = new DynamicAnnotation("a", _lionWebVersion, lang);
        Assert.IsFalse(((Annotation)node).TryGetFeatures(out var empty));
        Assert.IsTrue(empty?.Count == 0);

        node.AddFeatures([new DynamicProperty("b", _lionWebVersion, null)]);
        Assert.IsTrue(((Annotation)node).TryGetFeatures(out var value));
        Assert.IsFalse(value.Count == 0);
    }

    [TestMethod]
    public void Set_Annotates()
    {
        Annotation tgt = new DynamicAnnotation("my-id", _lionWebVersion, lang) { Key = "my-key", Name = "SomeName" };
        ann.Set(AnnotationAnnotates, tgt);
        Assert.AreEqual(tgt, ann.Annotates);
        Assert.ThrowsException<InvalidValueException>(() => ann.Set(AnnotationAnnotates, null));
        Assert.AreEqual(tgt, ann.Annotates);
    }

    [TestMethod]
    public void TryGet_Annotates()
    {
        var node = new DynamicAnnotation("a", _lionWebVersion, lang);
        Assert.IsTrue(((Annotation)node).TryGetAnnotates(out var empty));
        Assert.AreEqual(_builtIns.Node, empty);

        node.Annotates = _builtIns.INamed;
        Assert.IsTrue(((Annotation)node).TryGetAnnotates(out var value));
        Assert.AreEqual(_builtIns.INamed, value);
    }

    [TestMethod]
    public void Set_Extends()
    {
        Annotation sup = new DynamicAnnotation("my-id", _lionWebVersion, lang) { Key = "my-key", Name = "SomeName" };
        ann.Set(AnnotationExtends, sup);
        Assert.AreEqual(sup, ann.Extends);
        ann.Set(AnnotationExtends, null);
        Assert.AreEqual(null, ann.Extends);
        Assert.ThrowsException<InvalidValueException>(() => ann.Set(AnnotationExtends, lang));
    }

    [TestMethod]
    public void TryGet_Extends()
    {
        var node = new DynamicAnnotation("a", _lionWebVersion, lang);
        Assert.IsFalse(((Annotation)node).TryGetExtends(out var empty));
        Assert.IsNull(empty);

        node.Extends = node;
        Assert.IsTrue(((Annotation)node).TryGetExtends(out var value));
        Assert.AreEqual(node, value);
    }

    [TestMethod]
    public void Set_Implements()
    {
        Interface ifaceA = new DynamicInterface("my-id", _lionWebVersion, lang) { Key = "my-key", Name = "SomeName" };
        Interface ifaceB = new DynamicInterface("ref-id", _lionWebVersion, lang) { Key = "ref-key", Name = "SomeRef" };
        ann.Set(AnnotationImplements, new List<Interface> { ifaceA, ifaceB });
        CollectionAssert.AreEqual(new List<object> { ifaceA, ifaceB }, ann.Implements.ToList());
        ann.Set(AnnotationImplements, Enumerable.Empty<Interface>());
        CollectionAssert.AreEqual(Enumerable.Empty<Interface>().ToList(), ann.Implements.ToList());
        Assert.ThrowsException<InvalidValueException>(() => ann.Set(AnnotationImplements, null));
    }

    [TestMethod]
    public void TryGet_Implements()
    {
        var node = new DynamicAnnotation("a", _lionWebVersion, lang);
        Assert.IsFalse(((Annotation)node).TryGetImplements(out var empty));
        Assert.IsTrue(empty?.Count == 0);

        node.AddImplements([new DynamicInterface("b", _lionWebVersion, null)]);
        Assert.IsTrue(((Annotation)node).TryGetImplements(out var value));
        Assert.IsFalse(value.Count == 0);
    }

    [TestMethod]
    public void Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => ann.Set(LanguageVersion, "asdf"));
    }
}
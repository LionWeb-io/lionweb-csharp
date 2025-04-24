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

namespace LionWeb.Core.Test.NodeApi.Lenient;

using Core.Utilities;
using Languages;
using M2;
using M3;

[TestClass]
public class ReflectionTests : LenientNodeTestsBase
{
    private static readonly IBuiltInsLanguage _builtIns = LionWebVersions.Current.BuiltIns;

    [TestMethod]
    public void GetClassifier()
    {
        var node = newCircle("id");
        Assert.AreEqual(ShapesDynamic.Language.ClassifierByKey("key-Circle"), node.GetClassifier(),
            new LanguageEntityIdentityComparer());
    }

    [TestMethod]
    public void SetClassifier()
    {
        var node = newCircle("id");
        Assert.AreEqual(ShapesDynamic.Language.ClassifierByKey("key-Circle"), node.GetClassifier(),
            new LanguageEntityIdentityComparer());

        node.SetClassifier(ShapesDynamic.Language.ClassifierByKey("key-Line"));
        Assert.AreEqual(ShapesDynamic.Language.ClassifierByKey("key-Line"), node.GetClassifier(),
            new LanguageEntityIdentityComparer());
    }

    #region inherited

    #region property

    [TestMethod]
    public void SetGetInheritedProperty()
    {
        var node = newCircle("id");
        node.Set(_builtIns.INamed_name, "hi");
        Assert.AreEqual("hi", node.Get(_builtIns.INamed_name));
    }

    [TestMethod]
    public void GetInheritedProperty_Unset()
    {
        var node = newCircle("id");
        Assert.ThrowsException<UnsetFeatureException>(() => node.Get(_builtIns.INamed_name));
    }

    #endregion

    #region containment

    [TestMethod]
    public void SetGetInheritedContainment()
    {
        var child = newDocumentation("c");
        var parent = newCircle("id");
        parent.Set(Shape_shapeDocs, child);
        Assert.AreSame(child, parent.Get(Shape_shapeDocs));
    }

    [TestMethod]
    public void GetInheritedContainment_Unset()
    {
        var parent = newCircle("id");
        Assert.AreSame(null, parent.Get(Shape_shapeDocs));
    }

    [TestMethod]
    public void InheritedContainment_DetachChild()
    {
        var child = newDocumentation("c");
        var parent = newCircle("id");
        parent.Set(Shape_shapeDocs, child);
        child.DetachFromParent();
        Assert.IsNull(child.GetParent());
        Assert.IsNull(parent.Get(Shape_shapeDocs));
    }

    [TestMethod]
    public void InheritedContainment_GetContainmentOf()
    {
        var child = newDocumentation("c");
        var parent = newCircle("id");
        parent.Set(Shape_shapeDocs, child);
        Assert.AreEqual(Shape_shapeDocs, parent.GetContainmentOf(child));
    }

    [TestMethod]
    public void InheritedContainment_CollectAllSetFeatures()
    {
        var child = newDocumentation("c");
        var parent = newCircle("id");
        parent.Set(Shape_shapeDocs, child);
        CollectionAssert.AreEqual(new List<Feature> { Shape_shapeDocs },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #endregion

    #region metamodelViolation

    #region property

    [TestMethod]
    public void SetGetUnknownProperty()
    {
        var node = newCircle("id");
        node.Set(Documentation_text, "hi");
        Assert.AreEqual("hi", node.Get(Documentation_text));
    }

    private struct TestStruct(int a, string b);

    [TestMethod]
    public void SetValueTypeProperty()
    {
        var node = newCircle("id");
        node.Set(Documentation_text, new TestStruct(2, "hi"));
        Assert.AreEqual(new TestStruct(2, "hi"), node.Get(Documentation_text));
    }

    [TestMethod]
    public void SetValueTypeListProperty()
    {
        var node = newCircle("id");
        Assert.ThrowsException<InvalidValueException>(() =>
            node.Set(Documentation_text, new List<TestStruct> { new TestStruct(2, "hi") }));
        Assert.ThrowsException<UnknownFeatureException>(() => node.Get(Documentation_text));
    }

    [TestMethod]
    public void SetObjectProperty()
    {
        var node = newCircle("id");
        Assert.ThrowsException<InvalidValueException>(() => node.Set(Documentation_text, new object()));
        Assert.ThrowsException<UnknownFeatureException>(() => node.Get(Documentation_text));
    }

    [TestMethod]
    public void GetUnknownProperty_Unset()
    {
        var node = newCircle("id");
        Assert.ThrowsException<UnknownFeatureException>(() => node.Get(Documentation_text));
    }

    #endregion

    #region containment

    [TestMethod]
    public void SetGetUnknownContainment()
    {
        var child = newDocumentation("c");
        var parent = newCircle("id");
        parent.Set(MaterialGroup_defaultShape, child);
        Assert.AreSame(child, parent.Get(MaterialGroup_defaultShape));
    }

    [TestMethod]
    public void SetValueTypeContainment()
    {
        var node = newCircle("id");
        node.Set(MaterialGroup_defaultShape, new TestStruct(2, "hi"));
        Assert.AreEqual(new TestStruct(2, "hi"), node.Get(MaterialGroup_defaultShape));
    }

    [TestMethod]
    public void SetValueTypeListContainment()
    {
        var node = newCircle("id");
        Assert.ThrowsException<InvalidValueException>(() =>
            node.Set(MaterialGroup_defaultShape, new List<TestStruct> { new TestStruct(2, "hi") }));
        Assert.ThrowsException<UnknownFeatureException>(() => node.Get(MaterialGroup_defaultShape));
    }

    [TestMethod]
    public void SetObjectContainment()
    {
        var node = newCircle("id");
        Assert.ThrowsException<InvalidValueException>(() => node.Set(MaterialGroup_defaultShape, new object()));
        Assert.ThrowsException<UnknownFeatureException>(() => node.Get(MaterialGroup_defaultShape));
    }

    [TestMethod]
    public void GetUnknownContainment_Unset()
    {
        var parent = newCircle("id");
        Assert.ThrowsException<UnknownFeatureException>(() => parent.Get(MaterialGroup_defaultShape));
    }

    [TestMethod]
    public void UnknownContainment_DetachChild()
    {
        var child = newDocumentation("c");
        var parent = newCircle("id");
        parent.Set(MaterialGroup_defaultShape, child);
        child.DetachFromParent();
        Assert.IsNull(child.GetParent());
        Assert.ThrowsException<UnknownFeatureException>(() => parent.Get(MaterialGroup_defaultShape));
        Assert.AreEqual(null, parent.GetContainmentOf(child));
        Assert.IsFalse(parent.CollectAllSetFeatures().Any());
    }

    [TestMethod]
    public void UnknownContainment_GetContainmentOf()
    {
        var child = newDocumentation("c");
        var parent = newCircle("id");
        parent.Set(MaterialGroup_defaultShape, child);
        Assert.AreEqual(MaterialGroup_defaultShape, parent.GetContainmentOf(child));
    }

    [TestMethod]
    public void UnknownContainment_CollectAllSetFeatures()
    {
        var child = newDocumentation("c");
        var parent = newCircle("id");
        parent.Set(MaterialGroup_defaultShape, child);
        CollectionAssert.AreEqual(new List<Feature> { MaterialGroup_defaultShape },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #endregion
}
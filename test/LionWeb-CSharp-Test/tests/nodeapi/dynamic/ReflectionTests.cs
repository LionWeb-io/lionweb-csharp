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

namespace LionWeb.Core.M2.Dynamic.Test;

using Examples.Shapes.Dynamic;
using M3;
using Utilities;

[TestClass]
public class ReflectionTests : DynamicNodeTestsBase
{
    [TestMethod]
    public void GetClassifier()
    {
        var node = newCircle("id");
        Assert.AreEqual(ShapesDynamic.Language.ClassifierByKey("key-Circle"), node.GetClassifier(),
            new LanguageEntityIdentityComparer());
    }

    #region inherited

    #region property

    [TestMethod]
    public void SetGetInheritedProperty()
    {
        var node = newCircle("id");
        node.Set(BuiltInsLanguage.Instance.INamed_name, "hi");
        Assert.AreEqual("hi", node.Get(BuiltInsLanguage.Instance.INamed_name));
    }

    [TestMethod]
    public void GetInheritedProperty_Unset()
    {
        var node = newCircle("id");
        Assert.ThrowsException<UnsetFeatureException>(() => node.Get(BuiltInsLanguage.Instance.INamed_name));
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
}
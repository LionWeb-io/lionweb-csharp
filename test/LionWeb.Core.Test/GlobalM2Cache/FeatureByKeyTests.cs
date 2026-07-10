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

namespace LionWeb.Core.Test.GlobalM2Cache;

using Languages.Generated.V2024_1.SDTLang;
using Languages.Generated.V2024_1.Shapes.M2;
using M2;

[TestClass]
public class FeatureByKeyTests
{
    private static readonly IBuiltInsLanguage _builtIns = LionWebVersions.Current.BuiltIns;

    [TestMethod]
    public void DirectProperty()
    {
        var cache = new M2Cache();
        var language = ShapesLanguage.Instance;
        cache.Register([language]);
        var actual = cache.FeatureByKey(language.Circle, language.Circle_r.Key);
        Assert.AreSame(language.Circle_r, actual);
    }

    [TestMethod]
    public void DirectContainment()
    {
        var cache = new M2Cache();
        var language = ShapesLanguage.Instance;
        cache.Register([language]);
        var actual = cache.FeatureByKey(language.Circle, language.Circle_center.Key);
        Assert.AreSame(language.Circle_center, actual);
    }

    [TestMethod]
    public void DirectReference()
    {
        var cache = new M2Cache();
        var language = ShapesLanguage.Instance;
        cache.Register([language]);
        var actual = cache.FeatureByKey(language.BillOfMaterials, language.BillOfMaterials_materials.Key);
        Assert.AreSame(language.BillOfMaterials_materials, actual);
    }

    [TestMethod]
    public void InheritedProperty()
    {
        var cache = new M2Cache();
        var language = ShapesLanguage.Instance;
        cache.Register([language]);
        var actual = cache.FeatureByKey(language.Circle, _builtIns.INamed_name.Key);
        Assert.AreSame(_builtIns.INamed_name, actual);
    }

    [TestMethod]
    public void InheritedContainment()
    {
        var cache = new M2Cache();
        var language = ShapesLanguage.Instance;
        cache.Register([language]);
        var actual = cache.FeatureByKey(language.Circle, language.Shape_shapeDocs.Key);
        Assert.AreSame(language.Shape_shapeDocs, actual);
    }

    [TestMethod]
    public void WrongClassifier()
    {
        var cache = new M2Cache();
        var language = ShapesLanguage.Instance;
        cache.Register([language]);
        var actual = cache.FeatureByKey(language.Circle, language.Line_start.Key);
        Assert.IsNull(actual);
    }

    [TestMethod]
    public void UnknownKey()
    {
        var cache = new M2Cache();
        var language = ShapesLanguage.Instance;
        cache.Register([language]);
        var actual = cache.FeatureByKey(language.Circle, "asdf");
        Assert.IsNull(actual);
    }

    [TestMethod]
    public void UnknownClassifier()
    {
        var cache = new M2Cache();
        var language = ShapesLanguage.Instance;
        cache.Register([language]);
        var actual = cache.FeatureByKey(SDTLangLanguage.Instance.SDTConcept, "asdf");
        Assert.IsNull(actual);
    }
}
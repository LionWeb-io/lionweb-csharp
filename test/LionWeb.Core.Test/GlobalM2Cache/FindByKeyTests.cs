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

using Languages.Generated.V2024_1.CustomPrimitiveTypeLang;
using Languages.Generated.V2024_1.SDTLang;
using Languages.Generated.V2024_1.Shapes.M2;
using M2;
using M3;

[TestClass]
public class FindByKeyTests
{
    [TestMethod]
    public void Language()
    {
        var cache = new M2Cache();
        var language = ShapesLanguage.Instance;
        cache.Register([language]);
        var actual = cache.FindByKey<Language>(language, language.Key);
        Assert.AreSame(language, actual);
    }

    [TestMethod]
    public void Classifier()
    {
        var cache = new M2Cache();
        var language = ShapesLanguage.Instance;
        cache.Register([language]);
        var actual = cache.FindByKey<Classifier>(language, language.Circle.Key);
        Assert.AreSame(language.Circle, actual);
    }

    [TestMethod]
    public void Feature()
    {
        var cache = new M2Cache();
        var language = ShapesLanguage.Instance;
        cache.Register([language]);
        var actual = cache.FindByKey<Feature>(language, language.Circle_center.Key);
        Assert.AreSame(language.Circle_center, actual);
    }

    [TestMethod]
    public void Enumeration()
    {
        var cache = new M2Cache();
        var language = ShapesLanguage.Instance;
        cache.Register([language]);
        var actual = language.FindByKey<Enumeration>(language.MatterState.Key);
        Assert.AreSame(language.MatterState, actual);
    }

    [TestMethod]
    public void EnumerationLiteral()
    {
        var cache = new M2Cache();
        var language = ShapesLanguage.Instance;
        cache.Register([language]);
        var actual = cache.FindByKey<EnumerationLiteral>(language, language.MatterState_liquid.Key);
        Assert.AreSame(language.MatterState_liquid, actual);
    }

    [TestMethod]
    public void PrimitiveType()
    {
        var cache = new M2Cache();
        var language = LionWebVersions.v2024_1.BuiltIns;
        cache.Register([language]);
        var actual = cache.FindByKey<PrimitiveType>(language, language.Boolean.Key);
        Assert.AreSame(language.Boolean, actual);
    }

    [TestMethod]
    public void PrimitiveType_generated()
    {
        var cache = new M2Cache();
        var language = CustomPrimitiveTypeLangLanguage.Instance;
        cache.Register([language]);
        var actual = language.FindByKey<PrimitiveType>(language.CustomType.Key);
        Assert.AreSame(language.CustomType, actual);
    }

    [TestMethod]
    public void Sdt()
    {
        var cache = new M2Cache();
        var language = SDTLangLanguage.Instance;
        cache.Register([language]);
        var actual = cache.FindByKey<StructuredDataType>(language, SDTLangLanguage.Instance.A.Key);
        Assert.AreSame(SDTLangLanguage.Instance.A, actual);
    }

    [TestMethod]
    public void Field()
    {
        var cache = new M2Cache();
        var language = SDTLangLanguage.Instance;
        cache.Register([language]);
        var actual = cache.FindByKey<Field>(language, language.A_a2b.Key);
        Assert.AreSame(language.A_a2b, actual);
    }

    [TestMethod]
    public void Supertype()
    {
        var cache = new M2Cache();
        var language = SDTLangLanguage.Instance;
        cache.Register([language]);
        var actual = cache.FindByKey<IKeyed>(language, language.A_a2b.Key);
        Assert.AreSame(language.A_a2b, actual);
    }

    [TestMethod]
    public void WrongType()
    {
        var cache = new M2Cache();
        var language = SDTLangLanguage.Instance;
        cache.Register([language]);
        Assert.IsNull(cache.FindByKey<EnumerationLiteral>(language, language.A_a2b.Key));
    }

    [TestMethod]
    public void UnknownKey()
    {
        var cache = new M2Cache();
        var language = ShapesLanguage.Instance;
        cache.Register([language]);
        Assert.IsNull(cache.FindByKey<IKeyed>(language, "asdf"));
    }
}
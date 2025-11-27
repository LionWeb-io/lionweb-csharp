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

namespace LionWeb.Core.Test.NodeApi;

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
        var language = ShapesLanguage.Instance;
        var actual = language.FindByKey<Language>(language.Key);
        Assert.AreSame(language, actual);
    }

    [TestMethod]
    public void Classifier()
    {
        var language = ShapesLanguage.Instance;
        var actual = language.FindByKey<Classifier>(language.Circle.Key);
        Assert.AreSame(language.Circle, actual);
    }

    [TestMethod]
    public void Feature()
    {
        var language = ShapesLanguage.Instance;
        var actual = language.FindByKey<Feature>(language.Circle_center.Key);
        Assert.AreSame(language.Circle_center, actual);
    }

    [TestMethod]
    public void Enumeration()
    {
        var language = ShapesLanguage.Instance;
        var actual = language.FindByKey<Enumeration>(language.MatterState.Key);
        Assert.AreSame(language.MatterState, actual);
    }

    [TestMethod]
    public void EnumerationLiteral()
    {
        var language = ShapesLanguage.Instance;
        var actual = language.FindByKey<EnumerationLiteral>(language.MatterState_liquid.Key);
        Assert.AreSame(language.MatterState_liquid, actual);
    }

    [TestMethod]
    public void PrimitiveType()
    {
        var language = LionWebVersions.v2024_1.BuiltIns;
        var actual = language.FindByKey<PrimitiveType>(language.Boolean.Key);
        Assert.AreSame(language.Boolean, actual);
    }

    [TestMethod]
    public void Sdt()
    {
        var language = SDTLangLanguage.Instance;
        var actual = language.FindByKey<StructuredDataType>(SDTLangLanguage.Instance.A.Key);
        Assert.AreSame(SDTLangLanguage.Instance.A, actual);
    }

    [TestMethod]
    public void Field()
    {
        var language = SDTLangLanguage.Instance;
        var actual = language.FindByKey<Field>(language.A_a2b.Key);
        Assert.AreSame(language.A_a2b, actual);
    }

    [TestMethod]
    public void Supertype()
    {
        var language = SDTLangLanguage.Instance;
        var actual = language.FindByKey<IKeyed>(language.A_a2b.Key);
        Assert.AreSame(language.A_a2b, actual);
    }

    [TestMethod]
    public void WrongType()
    {
        var language = SDTLangLanguage.Instance;
        Assert.ThrowsExactly<KeyNotFoundException>(() => language.FindByKey<EnumerationLiteral>(language.A_a2b.Key));
    }

    [TestMethod]
    public void UnknownKey()
    {
        var language = ShapesLanguage.Instance;
        Assert.ThrowsExactly<KeyNotFoundException>(() => language.FindByKey<IKeyed>("asdf"));
    }
}
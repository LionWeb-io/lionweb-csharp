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
        var target = ShapesLanguage.Instance;
        Assert.AreSame(target, ShapesLanguage.Instance.FindByKey<Language>(target.Key));
    }

    [TestMethod]
    public void Annotation()
    {
        var target = ShapesLanguage.Instance.Documentation;
        Assert.AreSame(target, ShapesLanguage.Instance.FindByKey<Annotation>(target.Key));
    }

    [TestMethod]
    public void Concept()
    {
        var target = ShapesLanguage.Instance.Circle;
        Assert.AreSame(target, ShapesLanguage.Instance.FindByKey<Concept>(target.Key));
    }

    [TestMethod]
    public void Interface()
    {
        var target = ShapesLanguage.Instance.IShape;
        Assert.AreSame(target, ShapesLanguage.Instance.FindByKey<Interface>(target.Key));
    }

    [TestMethod]
    public void Feature()
    {
        var target = ShapesLanguage.Instance.IShape_uuid;
        Assert.AreSame(target, ShapesLanguage.Instance.FindByKey<Feature>(target.Key));
    }

    [TestMethod]
    public void Enumeration()
    {
        var target = ShapesLanguage.Instance.MatterState;
        Assert.AreSame(target, ShapesLanguage.Instance.FindByKey<Enumeration>(target.Key));
    }

    [TestMethod]
    public void EnumerationLiteral()
    {
        var target = ShapesLanguage.Instance.MatterState_liquid;
        Assert.AreSame(target, ShapesLanguage.Instance.FindByKey<EnumerationLiteral>(target.Key));
    }

    [TestMethod]
    public void Sdt()
    {
        var target = SDTLangLanguage.Instance.A;
        Assert.AreSame(target, SDTLangLanguage.Instance.FindByKey<StructuredDataType>(target.Key));
    }

    [TestMethod]
    public void Field()
    {
        var target = SDTLangLanguage.Instance.A_a2b;
        Assert.AreSame(target, SDTLangLanguage.Instance.FindByKey<Field>(target.Key));
    }

    [TestMethod]
    public void PrimitiveType()
    {
        var target = LionWebVersions.v2024_1.BuiltIns.Boolean;
        Assert.AreSame(target, LionWebVersions.v2024_1.BuiltIns.FindByKey<PrimitiveType>(target.Key));
    }
}
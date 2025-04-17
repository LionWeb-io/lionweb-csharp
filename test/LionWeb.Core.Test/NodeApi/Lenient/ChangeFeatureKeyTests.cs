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

using M3;

[TestClass]
public class ChangeFeatureKeyTests
{
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    private readonly DynamicLanguage _lang;
    private readonly DynamicConcept _concept;
    private readonly DynamicProperty _property;

    public ChangeFeatureKeyTests()
    {
        _lang = new DynamicLanguage("lang", _lionWebVersion) { Key = "key-lang", Version = "0" };
        _concept = _lang.Concept("concept", "key-concept", "concept");
        _property = _concept.Property("property", "key-property", "property").OfType(_lionWebVersion.BuiltIns.String);
    }

    [TestMethod]
    public void LangKey()
    {
        var node = new LenientNode("node", _concept);
        node.Set(_property, "value");

        Assert.AreEqual("value", node.Get(_property));

        _lang.Key = "newLangKey";

        Assert.AreEqual("value", node.Get(_property));
    }

    [TestMethod]
    public void ConceptKey()
    {
        var node = new LenientNode("node", _concept);
        node.Set(_property, "value");

        Assert.AreEqual("value", node.Get(_property));

        _concept.Key = "newConceptKey";

        Assert.AreEqual("value", node.Get(_property));
    }

    [TestMethod]
    public void FeatureKey()
    {
        var node = new LenientNode("node", _concept);
        node.Set(_property, "value");

        Assert.AreEqual("value", node.Get(_property));

        _property.Key = "newPropertyKey";

        Assert.AreEqual("value", node.Get(_property));
    }
}
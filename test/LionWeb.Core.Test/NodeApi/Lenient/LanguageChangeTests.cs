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
public class LanguageChangeTests
{
    [TestMethod]
    public void GetFeatureAfterLanguageChange()
    {
        var lionWebVersion = LionWebVersions.Current;

        var lang = new DynamicLanguage("l", lionWebVersion)
        {
            Name = "LangChange", Key = "langChange-Key", Version = "0"
        };
        var coord = new DynamicConcept("coord", lionWebVersion, lang) { Name = "Coord", Key = "Coord-Key" };
        var line = new DynamicConcept("line", lionWebVersion, lang) { Name = "Line", Key = "Line-Key" };
        var lineStart = new DynamicContainment("lineStart", lionWebVersion, line)
        {
            Name = "start", Key = "lineStart-Key", Type = coord
        };

        var node = lang.GetFactory().CreateNode("id", line);
        var startCoord = lang.GetFactory().CreateNode("start", coord);
        node.Set(lineStart, startCoord);

        Assert.AreSame(startCoord, node.Get(lineStart));

        var prop = new DynamicProperty("propId", lang.LionWebVersion, null)
        {
            Key = "myKey", Name = "myName", Optional = true, Type = lang.LionWebVersion.BuiltIns.String
        };
        line.AddFeatures([prop]);
        node.Set(prop, "hello");

        Assert.AreEqual("hello", node.Get(prop));
    }

    [TestMethod]
    public void GetFeatureAfterLanguageChange_NameMixup()
    {
        var lionWebVersion = LionWebVersions.Current;

        var lang = new DynamicLanguage("l", lionWebVersion)
        {
            Name = "LangChange", Key = "langChange-Key", Version = "0"
        };
        var coord = new DynamicConcept("coord", lionWebVersion, lang) { Name = "Coord", Key = "Coord-Key" };
        var line = new DynamicConcept("line", lionWebVersion, lang) { Name = "Line", Key = "Line-Key" };
        var lineStart = new DynamicContainment("lineStart", lionWebVersion, line)
        {
            Name = "start", Key = "lineStart-Key", Type = coord
        };
        var node = lang.GetFactory().CreateNode("id", line);
        var startCoord = lang.GetFactory().CreateNode("start", coord);
        node.Set(lineStart, startCoord);

        Assert.AreSame(startCoord, node.Get(lineStart));

        lineStart.Key = "key-start_old";
        var prop = new DynamicProperty("propId", lang.LionWebVersion, null)
        {
            Key = "key-start", Name = "startProperty", Optional = true, Type = lang.LionWebVersion.BuiltIns.String
        };
        line.AddFeatures([prop]);
        node.Set(prop, "hello");
        var newStart = lang.GetFactory().CreateNode("newStart", coord);
        node.Set(lineStart, newStart);

        Assert.AreEqual("hello", node.Get(prop));
        Assert.AreEqual(newStart, node.Get(lineStart));
    }
}
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

namespace LionWeb_CSharp_Test.tests.serialization;

using Examples.Shapes.M2;
using LionWeb.Core;
using LionWeb.Core.M1;
using LionWeb.Core.M3;
using System.Collections.Concurrent;

[TestClass]
public class HandlerSerializationTests
{
    class DuplicateNodeHandler(Action incrementer) : ISerializerHandler
    {
        Language? ISerializerHandler.DuplicateUsedLanguage(Language a, Language b) =>
            throw new NotImplementedException();

        public void DuplicateNodeId(IReadableNode n) => incrementer();
        
        public string? UnknownDatatype(IReadableNode node, Feature property, object? value) => throw new NotImplementedException();
    }

    [TestMethod]
    public void DuplicateId_CustomHandler()
    {
        var materialGroup = new MaterialGroup("duplicate") { DefaultShape = new Circle("duplicate") };

        int count = 0;

        var serializer =
            new Serializer { Handler = new DuplicateNodeHandler(() => Interlocked.Increment(ref count)) };

        try
        {
            ISerializerExtensions.Serialize(serializer, materialGroup.Descendants(true, true));
        } catch (InvalidOperationException _)
        {
        }

        Assert.AreEqual(1, count);
    }

    class DuplicateLanguageHandler(Func<Language, Language, Language?> incrementer) : ISerializerHandler
    {
        Language? ISerializerHandler.DuplicateUsedLanguage(Language a, Language b) => incrementer(a,b);

        public void DuplicateNodeId(IReadableNode n) => throw new NotImplementedException();
        
        public string? UnknownDatatype(IReadableNode node, Feature property, object? value) => throw new NotImplementedException();
    }

    [TestMethod]
    public void DuplicateLanguage_CustomHandler()
    {
        var lang = new DynamicLanguage("abc")
        {
            Key = ShapesLanguage.Instance.Key, Version = ShapesLanguage.Instance.Version
        };
        var materialGroup = lang.Concept("efg", ShapesLanguage.Instance.MaterialGroup.Key,
            ShapesLanguage.Instance.MaterialGroup.Name);
        var defaultShape = materialGroup.Containment("ijk", ShapesLanguage.Instance.MaterialGroup_defaultShape.Key,
            ShapesLanguage.Instance.MaterialGroup_defaultShape.Name);

        var a = lang.GetFactory().CreateNode("a", materialGroup);
        var b = new Circle("b");
        a.Set(defaultShape, b);

        var dictionary = new ConcurrentDictionary<Language, byte>();

        var serializer =
            new Serializer
            {
                Handler = new DuplicateLanguageHandler((a, b) =>
                {
                    dictionary[a] = 1;
                    dictionary[b] = 1;
                    return null;
                })
            };

        try
        {
            ISerializerExtensions.Serialize(serializer, a.Descendants(true, true));
        } catch (InvalidOperationException _)
        {
        }

        Assert.AreEqual(2, dictionary.Count);
    }

    [TestMethod]
    public void DuplicateLanguage_CustomHandler_Heal()
    {
        var lang = new DynamicLanguage("abc")
        {
            Key = ShapesLanguage.Instance.Key, Version = ShapesLanguage.Instance.Version
        };
        var materialGroup = lang.Concept("efg", ShapesLanguage.Instance.MaterialGroup.Key,
            ShapesLanguage.Instance.MaterialGroup.Name);
        var defaultShape = materialGroup.Containment("ijk", ShapesLanguage.Instance.MaterialGroup_defaultShape.Key,
            ShapesLanguage.Instance.MaterialGroup_defaultShape.Name);

        var a = lang.GetFactory().CreateNode("a", materialGroup);
        var b = new Circle("b");
        a.Set(defaultShape, b);

        var dictionary = new ConcurrentDictionary<Language, byte>();

        var serializer =
            new Serializer
            {
                Handler = new DuplicateLanguageHandler((a, b) =>
                {
                    dictionary[a] = 1;
                    dictionary[b] = 1;
                    return ShapesLanguage.Instance;
                })
            };

        ISerializerExtensions.Serialize(serializer, a.Descendants(true, true));

        Assert.AreEqual(2, dictionary.Count);
    }

    private TestContext testContextInstance;

    /// <summary>
    /// Gets or sets the test context which provides
    /// information about and functionality for the current test run.
    /// </summary>
    public TestContext TestContext
    {
        get { return testContextInstance; }
        set { testContextInstance = value; }
    }
}
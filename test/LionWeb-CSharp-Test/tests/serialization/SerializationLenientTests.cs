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

using Examples.Circular.B;
using Examples.Library.M2;
using Examples.Shapes.M2;
using Examples.WithEnum.M2;
using LionWeb.Core;
using LionWeb.Core.M1;
using LionWeb.Core.M2;
using LionWeb.Core.Serialization;
using LionWeb.Core.Utilities;
using Comparer = LionWeb.Core.Utilities.Comparer;

[TestClass]
public class SerializationLenientTests
{
    [TestMethod]
    public void InvalidValues()
    {
        var rootNode = new LenientNode("a", ShapesLanguage.Instance.Line);

        var childA = new Book("bookA");
        var childB = new BConcept("bConceptB");
        rootNode.Set(BuiltInsLanguage.Instance.INamed_name, new List<INode> { childA, childB });
        rootNode.Set(ShapesLanguage.Instance.IShape_uuid, new List<IReadableNode> { childA, childB });
        rootNode.Set(ShapesLanguage.Instance.Shape_shapeDocs, "hello");
        rootNode.Set(ShapesLanguage.Instance.Line_start, new List<INode> { childA, childB });
        rootNode.Set(ShapesLanguage.Instance.Line_end, MyEnum.literal1);

        var serializationChunk = Serializer.SerializeToChunk(new List<INode> { rootNode, childA, childB });
        Console.WriteLine(JsonUtils.WriteJsonToString(serializationChunk));

        // var readableNodes = new DeserializerBuilder()
        //     .WithLanguage(ShapesLanguage.Instance)
        //     .WithLanguage(LibraryLanguage.Instance)
        //     .WithLanguage(BLangLanguage.Instance)
        //     .WithLanguage(WithEnumLanguage.Instance)
        //     .Build()
        //     .Deserialize(serializationChunk);
        //
        // Assert.AreEqual(1, readableNodes.Count);
        //
        // var comparer = new Comparer(new List<IReadableNode> { rootNode }, readableNodes);
        // Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(new ComparerOutputConfig()));
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
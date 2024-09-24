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

[TestClass]
public class SerializationDuplicateNodeTests
{
    [TestMethod]
    public void DuplicateId()
    {
        var materialGroup = new MaterialGroup("duplicate") { DefaultShape = new Circle("duplicate") };

        Assert.ThrowsException<ArgumentException>(() => Serializer.SerializeToChunk([materialGroup]));
    }

    [TestMethod]
    [Ignore]
    public void DuplicateNode()
    {
        var b = new Circle("b");
        var a = new MaterialGroup("a") { DefaultShape = b };
        var b2 = new Circle("b");

        var x = new Serializer().Serialize([a, b, b]).ToList();
        var serializedNodes = new Serializer().Serialize([a, b, b2]).ToList();
        Assert.AreEqual(2, serializedNodes.Count);
    }

    class DuplicateNodeIdHandler(Action incrementer) : ISerializerHandler
    {
        Language? ISerializerHandler.DuplicateUsedLanguage(Language a, Language b) =>
            throw new NotImplementedException();

        public void DuplicateNodeId(IReadableNode n) => incrementer();

        public string? UnknownDatatype(IReadableNode node, Property property, object? value) =>
            throw new NotImplementedException();
    }

    [TestMethod]
    public void DuplicateId_CustomHandler()
    {
        var materialGroup = new MaterialGroup("duplicate") { DefaultShape = new Circle("duplicate") };

        int count = 0;

        var serializer =
            new Serializer { Handler = new DuplicateNodeIdHandler(() => Interlocked.Increment(ref count)) };

        try
        {
            ISerializerExtensions.Serialize(serializer, materialGroup.Descendants(true, true));
        } catch (InvalidOperationException _)
        {
        }

        Assert.AreEqual(1, count);
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
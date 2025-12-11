// Copyright 2025 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License")
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

namespace LionWeb.Core.Test.Serialization;

using Core.Serialization;
using Core.Utilities;
using Languages.Generated.V2024_1.TestLanguage;
using M3;

public abstract class SerializationTestsBase
{
    protected readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;
    protected readonly Language _language;

    private TestContext testContextInstance;

    protected SerializationTestsBase()
    {
        _language = TestLanguageLanguage.Instance;
    }


    /// <summary>
    /// Gets or sets the test context which provides
    /// information about and functionality for the current test run.
    /// </summary>
    public TestContext TestContext
    {
        get { return testContextInstance; }
        set { testContextInstance = value; }
    }

    protected void AssertEquals(IList<IReadableNode?> expected, List<IReadableNode> actual)
    {
        var comparer = new Comparer(expected, actual);
        Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(new ComparerOutputConfig()));
    }


    protected void AssertEquals(SerializationChunk expected, SerializationChunk actual)
    {
        Assert.AreEqual(expected.SerializationFormatVersion, actual.SerializationFormatVersion);
        CollectionAssert.AreEqual(expected.Languages, actual.Languages);
        Assert.AreEqual(expected.Nodes.Length, actual.Nodes.Length);
        var expectedNodes = expected.Nodes.GetEnumerator();
        var actualNodes = actual.Nodes.GetEnumerator();
        while (expectedNodes.MoveNext() && actualNodes.MoveNext())
        {
            var expectedNode = (SerializedNode)expectedNodes.Current!;
            var actualNode = (SerializedNode)actualNodes.Current!;
            Assert.AreEqual(expectedNode.Id, actualNode.Id);
            Assert.AreEqual(expectedNode.Classifier, actualNode.Classifier);
            CollectionAssert.AreEqual(
                expectedNode.Properties.OrderBy(p => p.Property.Key).ToList(),
                actualNode.Properties.OrderBy(p => p.Property.Key).ToList()
            );

            Assert.AreEqual(expectedNode.Containments.Length, actualNode.Containments.Length);
            using var expectedContainments = expectedNode.Containments.OrderBy(c => c.Containment.Key).GetEnumerator();
            using var actualContainments = actualNode.Containments.OrderBy(c => c.Containment.Key).GetEnumerator();
            while (expectedContainments.MoveNext() && actualContainments.MoveNext())
            {
                var expectedContainment = expectedContainments.Current;
                var actualContainment = actualContainments.Current;
                Assert.AreEqual(expectedContainment.Containment, actualContainment.Containment);
                CollectionAssert.AreEqual(expectedContainment.Children, actualContainment.Children);
            }

            Assert.AreEqual(expectedNode.References.Length, actualNode.References.Length);
            using var expectedReferences = expectedNode.References.OrderBy(r => r.Reference.Key).GetEnumerator();
            using var actualReferences = actualNode.References.OrderBy(r => r.Reference.Key).GetEnumerator();
            while (expectedReferences.MoveNext() && actualReferences.MoveNext())
            {
                var expectedReference = expectedReferences.Current;
                var actualReference = actualReferences.Current;
                Assert.AreEqual(expectedReference.Reference, actualReference.Reference);
                CollectionAssert.AreEqual(expectedReference.Targets, actualReference.Targets);
            }

            CollectionAssert.AreEqual(expectedNode.Annotations, actualNode.Annotations);

            Assert.AreEqual(expectedNode.Parent, actualNode.Parent);
        }
    }
}
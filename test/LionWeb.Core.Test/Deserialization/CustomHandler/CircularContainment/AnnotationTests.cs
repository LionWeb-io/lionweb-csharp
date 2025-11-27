// Copyright 2024 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Test.Deserialization.CustomHandler.CircularContainment;

using Core.Serialization;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;

/// <summary>
/// Tests for <see cref="IDeserializerHandler.CircularContainment"/>
/// </summary>
[TestClass]
public class AnnotationTests : CircularContainmentTestBase
{
    [TestMethod]
    public void self_circular_annotation_heals()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "A",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-BillOfMaterials"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment()
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-default-group"), Children = []
                        }
                    ],
                    References = [],
                    Annotations = ["A"],
                    Parent = "A"
                },
            ]
        };

        var deserializerHealingHandler = new DeserializerHealingHandler((node, writableNode) => null);
        List<IReadableNode> nodes = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        Assert.AreEqual(1, nodes.Count);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        Assert.IsFalse(a.GetAnnotations().Any());
    }

    /// <summary>
    /// Test case:
    /// Direct circular dependency: Annotation A has B as a child and B is annotated by A
    /// Healing:
    /// Removes annotation instance from B ( B is no longer annotated by A)
    /// </summary>
    [TestMethod]
    public void direct_circular_annotation_heals()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "A",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-BillOfMaterials"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment()
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-default-group"), Children = ["B"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = "B"
                },
                new SerializedNode
                {
                    Id = "B",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-MaterialGroup"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations =
                    [
                        "A"
                    ],
                    Parent = "A"
                },
            ]
        };

        var deserializerHealingHandler = new DeserializerHealingHandler((node, writableNode) => null);
        List<IReadableNode> nodes = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        Assert.AreEqual(1, nodes.Count);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());

        Assert.AreEqual(1, a.Children().Count());
        INode b = a.Children().First();
        Assert.AreEqual("B", b.GetId());
        Assert.AreSame(a, b.GetParent());

        Assert.IsFalse(b.GetAnnotations().Any());
    }

    /// <summary>
    /// Test case:
    /// Indirect circular dependency: A has B as a child, annotation C annotates B and has A as a child
    /// Healing:
    /// Breaks containment link from C to A
    /// </summary>
    [TestMethod]
    public void indirect_circular_annotation_heals()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "A",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-MaterialGroup"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment()
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-default-shape"), Children = ["B"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = null
                },

                new SerializedNode
                {
                    Id = "B",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = ["C"],
                    Parent = "A"
                },

                new SerializedNode
                {
                    Id = "C",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-BillOfMaterials"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment()
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-default-group"), Children = ["A"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = "B"
                },
            ]
        };

        var deserializerHealingHandler = new DeserializerHealingHandler((node, writableNode) => null);
        List<IReadableNode> nodes = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        Assert.AreEqual(1, nodes.Count);

        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        Assert.AreEqual(1, a.Children().Count());
        Assert.AreEqual(1, a.Descendants().Count());

        INode b = a.Children().First();
        Assert.AreEqual("B", b.GetId());
        Assert.AreSame(a, b.GetParent());
        Assert.IsFalse(b.Children().Any());
        Assert.IsTrue(b.GetAnnotations().Any());

        INode c = b.GetAnnotations()[0];
        Assert.AreEqual("C", c.GetId());
        Assert.AreSame(b, c.GetParent());
        Assert.IsFalse(c.Children().Any());
    }
}
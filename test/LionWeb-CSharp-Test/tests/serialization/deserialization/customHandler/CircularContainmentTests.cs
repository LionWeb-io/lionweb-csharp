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

namespace LionWeb_CSharp_Test.tests.serialization.deserialization;

using Examples.Shapes.M2;
using LionWeb.Core;
using LionWeb.Core.M1;
using LionWeb.Core.Serialization;

/// <summary>
/// Tests for <see cref="IDeserializerHandler.CircularContainment"/>
/// </summary>
[TestClass]
public class CircularContainmentTests
{
    private class DeserializerHealingHandler(Func<IWritableNode, IWritableNode, IWritableNode?> heal)
        : DeserializerExceptionHandler
    {
        public override IWritableNode? CircularContainment(IWritableNode containedNode, IWritableNode parent)
            => heal(containedNode, parent);
    }

    #region containment

    [TestMethod]
    public void self_circular_containment_heals()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "A",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment()
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-parts"), Children = ["A"]
                        }
                    ],
                    References = [],
                    Annotations = [],
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
        Assert.AreEqual(0, a.Children().Count());
    }

    /// <summary>
    /// Test case:
    /// Direct circular dependency: A has B as child, B has A as child
    /// Healing:
    /// Break containment link from B to A. As a result, B does not have A as a child  
    /// </summary>
    /// <remarks>Instead of breaking containment link from B to A, a reference link can be created from B to A</remarks>
    [TestMethod]
    public void direct_circular_containment_heals_case_1()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "A",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment()
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-parts"), Children = ["B"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = "B"
                },
                new SerializedNode
                {
                    Id = "B",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment()
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-parts"), Children = ["A"]
                        }
                    ],
                    References = [],
                    Annotations = [],
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
        Assert.IsFalse(b.Children().Any());
    }

    /// <summary>
    /// Test case:
    /// Direct circular dependency: A has B as child, B has A as child 
    /// Healing:
    /// Break containment link from A to B. As a result, A does not have B as a child and B has A as a child  
    /// </summary>
    /// <remarks>Instead of breaking containment link from A to B, a reference link can be created from A to B</remarks>
    [TestMethod]
    public void direct_circular_containment_heals_case_2()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "A",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment()
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-parts"), Children = ["B"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = "B"
                },
                new SerializedNode
                {
                    Id = "B",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment()
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-parts"), Children = ["A"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = "A"
                },
            ]
        };

        var deserializerHealingHandler = new DeserializerHealingHandler((containedNode, parent) =>
        {
            parent.DetachFromParent();
            return containedNode;
        });
        List<IReadableNode> nodes = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        Assert.AreEqual(1, nodes.Count);
        INode b = nodes.OfType<INode>().First();
        Assert.AreEqual("B", b.GetId());
        Assert.IsNull(b.GetParent());
        Assert.AreEqual(1, b.Children().Count());

        INode a = b.Children().First();
        Assert.AreEqual("A", a.GetId());
        Assert.AreSame(b, a.GetParent());
        Assert.IsFalse(a.Children().Any());
    }

    /// <summary>
    /// Test case:
    /// Direct circular dependency: A has B, C as children. B has C as a child. C has A as a child.
    /// Healing:
    /// Break containment link from C to A.
    /// </summary>
    [TestMethod]
    public void direct_circular_containment_multiple_children_heals_case_1()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "A",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment()
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-parts"), Children = ["B", "C"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = null
                },

                new SerializedNode
                {
                    Id = "B",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment()
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-parts"), Children = ["D"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = "A"
                },

                new SerializedNode
                {
                    Id = "C",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment()
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-parts"), Children = ["A"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = "A"
                },

                new SerializedNode
                {
                    Id = "D",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties = [],
                    Containments = [],
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
        Assert.AreEqual(2, a.Children().Count());
        Assert.AreEqual(3, a.Descendants().Count());

        INode b = a.Children().First();
        Assert.AreEqual("B", b.GetId());
        Assert.AreSame(a, b.GetParent());
        Assert.IsTrue(b.Children().Any());
        
        INode c = a.Children().Last();
        Assert.AreEqual("C", c.GetId());
        Assert.AreSame(a, c.GetParent());
        Assert.IsFalse(c.Children().Any());

        INode d = b.Children().First();
        Assert.AreEqual("D", d.GetId());
        Assert.AreSame(b, d.GetParent());
        Assert.IsFalse(d.Children().Any());
    }

    /// <summary>
    /// Test case:
    /// Direct circular dependency: A has B, C as children. B has C as a child. C has A as a child.
    /// Healing:
    /// Break containment link from A to C.
    /// </summary>
    [TestMethod]
    public void direct_circular_containment_multiple_children_heals_case_2()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "C",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment()
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-parts"), Children = ["A"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = "A"
                },

                new SerializedNode
                {
                    Id = "A",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment()
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-parts"), Children = ["B", "C"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = null
                },

                new SerializedNode
                {
                    Id = "B",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment()
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-parts"), Children = ["D"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = "A"
                },


                new SerializedNode
                {
                    Id = "D",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties = [],
                    Containments = [],
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

        INode c = nodes.OfType<INode>().First();
        Assert.AreEqual("C", c.GetId());
        Assert.IsNull(c.GetParent());
        Assert.AreEqual(1, c.Children().Count());
        Assert.AreEqual(3, c.Descendants().Count());

        INode a = c.Children().First();
        Assert.AreEqual("A", a.GetId());
        Assert.AreSame(c, a.GetParent());
        Assert.IsTrue(a.Children().Any());

        INode b = a.Children().First();
        Assert.AreEqual("B", b.GetId());
        Assert.AreSame(a, b.GetParent());
        Assert.IsTrue(b.Children().Any());
        
        INode d = b.Children().First();
        Assert.AreEqual("D", d.GetId());
        Assert.AreSame(b, d.GetParent());
        Assert.IsFalse(d.Children().Any());
    }

    
    #endregion

    #region annotation

    [TestMethod]
    public void circular_annotation()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
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

    #endregion
}
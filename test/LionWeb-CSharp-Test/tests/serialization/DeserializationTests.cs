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
using LionWeb.Core.M2;
using LionWeb.Core.Serialization;
using LionWeb.Core.Utilities;

[TestClass]
public class DeserializationTests
{
    [TestMethod]
    public void test_deserialization_of_a_node_with_missing_parent_does_not_fail()
    {
        SerializationChunk serializationChunk = new SerializationChunk
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
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Geometry"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "bar"
                }
            ]
        };

        var nodes = new DeserializerBuilder()
            .WithHandler(new DeserializerIgnoringHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);
        Assert.AreEqual(1, nodes.Count);
        var node = nodes[0];
        Assert.IsInstanceOfType<Geometry>(node);
        Assert.IsNull(node.GetParent());
    }

    [TestMethod]
    public void test_deserialization_of_a_node_with_a_missing_child_does_not_fail()
    {
        SerializationChunk serializationChunk = new SerializationChunk
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
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Geometry"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-shapes"), Children = ["bar"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = null
                }
            ]
        };

        var nodes = new DeserializerBuilder()
            .WithHandler(new DeserializerIgnoringHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);
        Assert.AreEqual(1, nodes.Count);
        var node = nodes[0];
        Assert.IsInstanceOfType<Geometry>(node);
        Assert.AreEqual(0, (node as Geometry).Shapes.Count);
    }

    [TestMethod]
    public void test_deserialization_of_a_node_with_a_missing_reference_target_does_not_fail()
    {
        SerializationChunk serializationChunk = new SerializationChunk
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
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Geometry"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-shapes"),
                            Children =
                            [
                                "bar"
                            ]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode()
                {
                    Id = "bar",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-OffsetDuplicate"),
                    Properties = [],
                    Containments = [], // should have an offset:Coord but can leave that one off
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("key-Shapes", "1", "key-source"),
                            Targets =
                            [
                                new SerializedReferenceTarget { Reference = "lizard", ResolveInfo = "lizard" }
                            ]
                        }
                    ],
                    Annotations = [],
                    Parent = "foo"
                }
            ]
        };

        var nodes = new DeserializerBuilder()
            .WithHandler(new DeserializerIgnoringHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);
        Assert.AreEqual(1, nodes.Count);
        var node = nodes[0];
        Assert.IsInstanceOfType<Geometry>(node);
        var geometry = node as Geometry;
        Assert.AreEqual(1, geometry.Shapes.Count);
        var shape = geometry.Shapes[0];
        Assert.IsInstanceOfType<OffsetDuplicate>(shape);
        var offsetDuplicate = shape as OffsetDuplicate;
        Assert.IsFalse(offsetDuplicate.CollectAllSetFeatures().Contains(ShapesLanguage.Instance
            .ClassifierByKey("key-OffsetDuplicate").FeatureByKey("key-source")));
    }

    [TestMethod]
    public void test_deserialization_of_a_node_referencing_a_dependent_node_succeeds()
    {
        SerializationChunk serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode()
                {
                    Id = "bar",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-OffsetDuplicate"),
                    Properties = [],
                    Containments = [], // should have an offset:Coord but can leave that one off
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("key-Shapes", "1", "key-source"),
                            Targets =
                            [
                                new SerializedReferenceTarget { Reference = "lizard", ResolveInfo = "lizard" }
                            ]
                        }
                    ],
                    Annotations = [],
                    Parent = null
                }
            ]
        };

        var lizard = ShapesLanguage.Instance.GetFactory().NewLine("lizard");
        var dependentGeometry = ShapesLanguage.Instance.GetFactory().CreateGeometry()
            .AddShapes([lizard]);

        var nodes = new DeserializerBuilder()
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk, dependentGeometry.Descendants(true, true));
        Assert.AreEqual(1, nodes.Count);
        var node = nodes[0];
        Assert.IsInstanceOfType<OffsetDuplicate>(node);
        var offsetDuplicate = node as OffsetDuplicate;
        Assert.AreEqual(lizard, offsetDuplicate.Source);
    }

    [TestMethod]
    public void test_deserialization_of_a_node_with_missing_annotation_does_not_fail()
    {
        SerializationChunk serializationChunk = new SerializationChunk
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
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Geometry"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations =
                    [
                        "lizard"
                    ],
                    Parent = null
                }
            ]
        };

        var nodes = new DeserializerBuilder()
            .WithHandler(new DeserializerIgnoringHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);
        Assert.AreEqual(1, nodes.Count);
        var node = nodes[0];
        Assert.AreEqual(0, node.GetAnnotations().Count);
    }

    [TestMethod]
    public void DeserializeUnsetRequiredContainment()
    {
        var line = new Line("line") { Start = new Coord("coord") { X = 1, Y = 2, Z = 3 } };

        var serializationChunk = Serializer.SerializeToChunk([line]);
        var nodes = new DeserializerBuilder()
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        var comparer = new Comparer([line], nodes);
        Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(new ComparerOutputConfig()));
    }

    #region circular containment

    [TestMethod]
    public void CircularContainment()
    {
        SerializationChunk serializationChunk = new SerializationChunk
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

        var nodes = new DeserializerBuilder()
            .WithHandler(new DeserializerIgnoringHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        Assert.AreEqual(1, nodes.Count);
        var a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());

        Assert.AreEqual(1, a.Children().Count());
        var b = a.Children().First();
        Assert.AreEqual("B", b.GetId());
        Assert.AreSame(a, b.GetParent());

        Assert.IsFalse(b.Children().Any());
    }

    [TestMethod]
    public void CircularAnnotation()
    {
        SerializationChunk serializationChunk = new SerializationChunk
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

        var nodes = new DeserializerBuilder()
            .WithHandler(new DeserializerIgnoringHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        Assert.AreEqual(1, nodes.Count);
        var a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());

        Assert.AreEqual(1, a.Children().Count());
        var b = a.Children().First();
        Assert.AreEqual("B", b.GetId());
        Assert.AreSame(a, b.GetParent());

        Assert.IsFalse(b.GetAnnotations().Any());
    }

    #endregion

    #region duplicate containment

    [TestMethod]
    public void DoubleContainment()
    {
        SerializationChunk serializationChunk = new SerializationChunk
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
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "B",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments = [],
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
                            Containment = new MetaPointer("key-Shapes", "1", "key-parts"), Children = ["B"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
            ]
        };

        var nodes = new DeserializerBuilder()
            .WithHandler(new DeserializerIgnoringHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        Assert.AreEqual(2, nodes.Count);
        var a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        Assert.AreEqual(1, a.Children().Count());

        var c = nodes.OfType<INode>().Last();
        Assert.AreEqual("C", c.GetId());
        Assert.IsNull(c.GetParent());
        Assert.IsFalse(c.Children().Any());

        var b = a.Children().First();
        Assert.AreEqual("B", b.GetId());
        Assert.AreSame(a, b.GetParent());
        Assert.IsFalse(b.Children().Any());
    }

    [TestMethod]
    public void DoubleAnnotation()
    {
        SerializationChunk serializationChunk = new SerializationChunk
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
                    Containments = [],
                    References = [],
                    Annotations =
                    [
                        "B"
                    ],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "B",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-BillOfMaterials"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "A"
                },
                new SerializedNode
                {
                    Id = "C",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations =
                    [
                        "B"
                    ],
                    Parent = null
                },
            ]
        };

        var nodes = new DeserializerBuilder()
            .WithHandler(new DeserializerIgnoringHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        Assert.AreEqual(2, nodes.Count);
        var a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        Assert.AreEqual(1, a.GetAnnotations().Count());

        var c = nodes.OfType<INode>().Last();
        Assert.AreEqual("C", c.GetId());
        Assert.IsNull(c.GetParent());
        Assert.IsFalse(c.GetAnnotations().Any());

        var b = a.GetAnnotations()[0];
        Assert.AreEqual("B", b.GetId());
        Assert.AreSame(a, b.GetParent());
        Assert.IsFalse(b.GetAnnotations().Any());
    }

    #endregion

    #region duplicate node id

    [TestMethod]
    public void DuplicateNodeId()
    {
        SerializationChunk serializationChunk = new SerializationChunk
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-OffsetDuplicate"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-builtins", "2023.1",
                                "LionCore-builtins-INamed-name"),
                            Value = "First"
                        }
                    ],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-offset"),
                            Children =
                            [
                                "ChildFirst"
                            ]
                        }
                    ],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("key-Shapes", "1", "key-source"),
                            Targets =
                            [
                                new SerializedReferenceTarget { Reference = "RefFirst" }
                            ]
                        }
                    ],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "ChildFirst",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "A"
                },
                new SerializedNode
                {
                    Id = "RefFirst",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "A",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-OffsetDuplicate"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-builtins", "2023.1",
                                "LionCore-builtins-INamed-name"),
                            Value = "Second"
                        }
                    ],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-offset"),
                            Children =
                            [
                                "ChildSecond"
                            ]
                        }
                    ],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("key-Shapes", "1", "key-source"),
                            Targets =
                            [
                                new SerializedReferenceTarget { Reference = "RefSecond" }
                            ]
                        }
                    ],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "ChildSecond",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "A"
                },
                new SerializedNode
                {
                    Id = "RefSecond",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
            ]
        };

        var nodes = new DeserializerBuilder()
            .WithHandler(new DeserializerIgnoringHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        Assert.AreEqual(4, nodes.Count);
        var a = nodes.OfType<OffsetDuplicate>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.AreEqual("First", a.Name);

        Assert.IsNotNull(a.Offset);
        Assert.AreEqual("ChildFirst", a.Offset.GetId());

        Assert.IsNotNull(a.Source);
        Assert.AreEqual("RefFirst", a.Source.GetId());

        Assert.AreEqual(2, nodes.OfType<Circle>().Count());

        Assert.AreEqual(1, nodes.OfType<Coord>().Count());
        Assert.AreEqual("ChildSecond", nodes.OfType<Coord>().First().GetId());
    }


    private class DeserializerHealingHandler(Func<CompressedId, IWritableNode, INode, string?> heal)
        : DeserializerExceptionHandler
    {
        public override string? DuplicateNodeId(CompressedId nodeId, IWritableNode existingNode, INode node) =>
            heal(nodeId, existingNode, node);
    }


    [TestMethod]
    public void DuplicateNodeId_Heal()
    {
        SerializationChunk serializationChunk = new SerializationChunk
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-OffsetDuplicate"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-builtins", "2023.1",
                                "LionCore-builtins-INamed-name"),
                            Value = "First"
                        }
                    ],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-offset"),
                            Children =
                            [
                                "ChildFirst"
                            ]
                        }
                    ],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("key-Shapes", "1", "key-source"),
                            Targets =
                            [
                                new SerializedReferenceTarget { Reference = "RefFirst" }
                            ]
                        }
                    ],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "ChildFirst",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "A"
                },
                new SerializedNode
                {
                    Id = "RefFirst",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "A",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-OffsetDuplicate"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-builtins", "2023.1",
                                "LionCore-builtins-INamed-name"),
                            Value = "Second"
                        }
                    ],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-offset"),
                            Children =
                            [
                                "ChildSecond"
                            ]
                        }
                    ],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("key-Shapes", "1", "key-source"),
                            Targets =
                            [
                                new SerializedReferenceTarget { Reference = "RefSecond" }
                            ]
                        }
                    ],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "ChildSecond",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "A"
                },
                new SerializedNode
                {
                    Id = "RefSecond",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
            ]
        };


        var deserializerHealingHandler = new DeserializerHealingHandler((id, node, arg3) => "renamed-A");

        var nodes = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        Assert.AreEqual(4, nodes.Count);
        var first = nodes.OfType<OffsetDuplicate>().First();
        Assert.AreEqual("A", first.GetId());
        Assert.AreEqual("First", first.Name);

        Assert.IsNotNull(first.Offset);
        Assert.AreEqual("ChildFirst", first.Offset.GetId());

        Assert.IsNotNull(first.Source);
        Assert.AreEqual("RefFirst", first.Source.GetId());

        var last = nodes.OfType<OffsetDuplicate>().Last();
        Assert.AreEqual("renamed-A", last.GetId());
        Assert.AreEqual("Second", last.Name);

        Assert.IsNotNull(last.Offset);
        Assert.AreEqual("ChildSecond", last.Offset.GetId());

        Assert.IsNotNull(last.Source);
        Assert.AreEqual("RefSecond", last.Source.GetId());

        Assert.AreEqual(2, nodes.OfType<Circle>().Count());
        Assert.AreEqual(0, nodes.OfType<Coord>().Count());
    }

    #endregion
}
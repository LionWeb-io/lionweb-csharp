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

namespace LionWeb.Core.Test;

using Examples.Shapes.M2;
using M1;
using M2;
using Serialization;
using Utilities;

[TestClass]
public class DeserializationTests
{
    [TestMethod]
    public void test_deserialization_of_a_node_with_missing_parent_does_not_fail()
    {
        SerializationChunk serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages = [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes = [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer
                    {
                        Key = "key-Geometry",
                        Language = "key-Shapes",
                        Version = "1"
                    },
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "bar"
                }
            ]
        };

        var nodes = new Deserializer([ShapesLanguage.Instance]).Deserialize(serializationChunk);
        Assert.AreEqual(1, nodes.Count);
        var node = nodes.First();
        Assert.IsInstanceOfType<Geometry>(node);
        Assert.IsNull(node.GetParent());
    }

    [TestMethod]
    public void test_deserialization_of_a_node_with_a_missing_child_does_not_fail()
    {
        SerializationChunk serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages = [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes = [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer
                    {
                        Key = "key-Geometry",
                        Language = "key-Shapes",
                        Version = "1"
                    },
                    Properties = [],
                    Containments = [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer
                            {
                                Key = "key-shapes",
                                Language = "key-Shapes",
                                Version = "1"
                            },
                            Children = [ "bar" ]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = null
                }
            ]
        };

        var nodes = new Deserializer([ShapesLanguage.Instance]).Deserialize(serializationChunk);
        Assert.AreEqual(1, nodes.Count);
        var node = nodes.First();
        Assert.IsInstanceOfType<Geometry>(node);
        Assert.AreEqual(0, (node as Geometry).Shapes.Count);
    }

    [TestMethod]
    public void test_deserialization_of_a_node_with_a_missing_reference_target_does_not_fail()
    {
        SerializationChunk serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages = [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes = [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer
                    {
                        Key = "key-Geometry",
                        Language = "key-Shapes",
                        Version = "1"
                    },
                    Properties = [],
                    Containments = [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer
                            {
                                Key = "key-shapes",
                                Language = "key-Shapes",
                                Version = "1"
                            },
                            Children = [
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
                    Classifier = new MetaPointer
                    {
                        Key = "key-OffsetDuplicate",
                        Language = "key-Shapes",
                        Version = "1"
                    },
                    Properties = [],
                    Containments = [],  // should have an offset:Coord but can leave that one off
                    References = [
                        new SerializedReference
                        {
                            Reference = new MetaPointer
                            {
                                Key = "key-source",
                                Language = "key-Shapes",
                                Version = "1"
                            },
                            Targets = [
                                new SerializedReferenceTarget
                                {
                                    Reference = "lizard",
                                    ResolveInfo = "lizard"
                                }
                            ]
                        }
                    ],
                    Annotations = [],
                    Parent = "foo"
                }
            ]
        };

        var nodes = new Deserializer([ShapesLanguage.Instance]).Deserialize(serializationChunk);
        Assert.AreEqual(1, nodes.Count);
        var node = nodes.First();
        Assert.IsInstanceOfType<Geometry>(node);
        var geometry = node as Geometry;
        Assert.AreEqual(1, geometry.Shapes.Count);
        var shape = geometry.Shapes.First();
        Assert.IsInstanceOfType<OffsetDuplicate>(shape);
        var offsetDuplicate = shape as OffsetDuplicate;
        Assert.IsFalse(offsetDuplicate.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.ClassifierByKey("key-OffsetDuplicate").FeatureByKey("key-source")));
    }

    [TestMethod]
    public void test_deserialization_of_a_node_referencing_a_dependent_node_succeeds()
    {
        SerializationChunk serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages = [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes = [
                new SerializedNode()
                {
                    Id = "bar",
                    Classifier = new MetaPointer
                    {
                        Key = "key-OffsetDuplicate",
                        Language = "key-Shapes",
                        Version = "1"
                    },
                    Properties = [],
                    Containments = [],  // should have an offset:Coord but can leave that one off
                    References = [
                        new SerializedReference
                        {
                            Reference = new MetaPointer
                            {
                                Key = "key-source",
                                Language = "key-Shapes",
                                Version = "1"
                            },
                            Targets = [
                                new SerializedReferenceTarget
                                {
                                    Reference = "lizard",
                                    ResolveInfo = "lizard"
                                }
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

        var nodes = new Deserializer([ShapesLanguage.Instance]).Deserialize(serializationChunk, [dependentGeometry]);
        Assert.AreEqual(1, nodes.Count);
        var node = nodes.First();
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
            Languages = [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes = [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer
                    {
                        Key = "key-Geometry",
                        Language = "key-Shapes",
                        Version = "1"
                    },
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [
                        "lizard"
                    ],
                    Parent = null
                }
            ]
        };

        var nodes = new Deserializer([ShapesLanguage.Instance]).Deserialize(serializationChunk);
        Assert.AreEqual(1, nodes.Count);
        var node = nodes.First();
        Assert.AreEqual(0, node.GetAnnotations().Count);
    }

    [TestMethod]
    public void deserializeUnsetRequiredContainment()
    {
        var line = new Line("line") { Start = new Coord("coord") { X = 1, Y = 2, Z = 3 } };

        var serializationChunk = Serializer.Serialize([line]);
        var nodes = new Deserializer([ShapesLanguage.Instance]).Deserialize(serializationChunk);

        var comparer = new Comparer([line], nodes);
        Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(new ComparerOutputConfig()));
    }
}
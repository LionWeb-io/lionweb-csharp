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

namespace LionWeb.Core.Test.Deserialization;

using Core.Serialization;
using Core.Utilities;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;
using M2;
using M3;

[TestClass]
public class DeserializationTests
{
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    [TestMethod]
    public void node_with_missing_parent_does_not_fail()
    {
        SerializationChunk serializationChunk = new SerializationChunk
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
    public void node_with_a_missing_child_does_not_fail()
    {
        SerializationChunk serializationChunk = new SerializationChunk
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
    public void node_with_a_missing_reference_target_does_not_fail()
    {
        SerializationChunk serializationChunk = new SerializationChunk
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
    public void node_referencing_a_dependent_node_succeeds()
    {
        SerializationChunk serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
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
    public void node_with_missing_annotation_does_not_fail()
    {
        SerializationChunk serializationChunk = new SerializationChunk
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
    public void UnsetRequiredContainment()
    {
        var line = new Line("line") { Start = new Coord("coord") { X = 1, Y = 2, Z = 3 } };

        var serializationChunk = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build()
            .SerializeToChunk([line]);
        var nodes = new DeserializerBuilder()
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        var comparer = new Comparer([line], nodes);
        Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(new ComparerOutputConfig()));
    }

    private class ClosestVersionDeserializerHandler : DeserializerExceptionHandler
    {
        public override T? SelectVersion<T>(CompressedMetaPointer metaPointer, List<Language> languages)
            where T : class =>
            DeserializerHandlerSelectOtherLanguageVersion.SelectVersion<T>(metaPointer, languages);
    }

    [TestMethod]
    public void UnfittingLanguageVersion()
    {
        var v1 = new DynamicLanguage("id-A", _lionWebVersion) { Key = "lang", Version = "1" };
        var v2 = new DynamicLanguage("id-B", _lionWebVersion) { Key = "lang", Version = "2" };
        var v3 = new DynamicLanguage("id-C", _lionWebVersion) { Key = "lang", Version = "3" };

        v1.Concept("id-A-concept", "key-A-concept", "AConcept");
        v1.Concept("id-A-concept2", "key-D-concept", "DConcept-A");
        v2.Concept("id-B-concept", "key-B-concept", "BConcept");
        v3.Concept("id-C-concept", "key-C-concept", "CConcept");
        v3.Concept("id-C-concept2", "key-D-concept", "DConcept-C");

        var chunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages =
            [
                new SerializedLanguageReference { Key = "lang", Version = "1" },
                new SerializedLanguageReference { Key = "lang", Version = "2" },
                new SerializedLanguageReference { Key = "lang", Version = "3" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "A-id",
                    Classifier = new MetaPointer("lang", "x", "key-A-concept"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "B-id",
                    Classifier = new MetaPointer("lang", "x", "key-B-concept"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "C-id",
                    Classifier = new MetaPointer("lang", "x", "key-C-concept"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "D-id",
                    Classifier = new MetaPointer("lang", "x", "key-D-concept"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
            ]
        };

        var deserializer = new DeserializerBuilder()
            .WithLanguage(v1)
            .WithLanguage(v2)
            .WithLanguage(v3)
            .WithCompressedIds(new(KeepOriginal: true))
            .WithHandler(new ClosestVersionDeserializerHandler())
            .Build();

        var nodes = deserializer.Deserialize(chunk);

        Assert.AreEqual(4, nodes.Count);
        Assert.AreSame(v1, nodes[0].GetClassifier().GetLanguage());
        Assert.AreSame(v2, nodes[1].GetClassifier().GetLanguage());
        Assert.AreSame(v3, nodes[2].GetClassifier().GetLanguage());
        Assert.AreSame(v3, nodes[3].GetClassifier().GetLanguage());
    }

    [TestMethod]
    public void UnfittingLanguageVersion_StrangeVersions()
    {
        var v1 = new DynamicLanguage("id-A", _lionWebVersion) { Key = "lang", Version = "1" };
        var v2 = new DynamicLanguage("id-B", _lionWebVersion) { Key = "lang", Version = "hä? llÖ" };
        var v3 = new DynamicLanguage("id-C", _lionWebVersion) { Key = "lang", Version = "\ud83d\ude00" };

        v1.Concept("id-A-concept", "key-A-concept", "AConcept");
        v1.Concept("id-A-concept2", "key-D-concept", "DConcept-A");
        v2.Concept("id-B-concept", "key-B-concept", "BConcept");
        v3.Concept("id-C-concept", "key-C-concept", "CConcept");
        v3.Concept("id-C-concept2", "key-D-concept", "DConcept-C");

        var chunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages =
            [
                new SerializedLanguageReference { Key = "lang", Version = v1.Version },
                new SerializedLanguageReference { Key = "lang", Version = v2.Version },
                new SerializedLanguageReference { Key = "lang", Version = v3.Version }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "A-id",
                    Classifier = new MetaPointer("lang", "x", "key-A-concept"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "B-id",
                    Classifier = new MetaPointer("lang", "x", "key-B-concept"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "C-id",
                    Classifier = new MetaPointer("lang", "x", "key-C-concept"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "D-id",
                    Classifier = new MetaPointer("lang", "x", "key-D-concept"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
            ]
        };

        var deserializer = new DeserializerBuilder()
            .WithLanguage(v1)
            .WithLanguage(v2)
            .WithLanguage(v3)
            .WithHandler(new ClosestVersionDeserializerHandler())
            .Build();

        var nodes = deserializer.Deserialize(chunk);

        Assert.AreEqual(4, nodes.Count);
        Assert.AreSame(v1, nodes[0].GetClassifier().GetLanguage());
        Assert.AreSame(v2, nodes[1].GetClassifier().GetLanguage());
        Assert.AreSame(v3, nodes[2].GetClassifier().GetLanguage());
        Assert.AreSame(v3, nodes[3].GetClassifier().GetLanguage());
    }
}
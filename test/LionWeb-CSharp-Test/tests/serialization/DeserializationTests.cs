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
using LionWeb.Core.M3;
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
        var node = nodes.First();
        Assert.IsInstanceOfType<Geometry>(node);
        var geometry = node as Geometry;
        Assert.AreEqual(1, geometry.Shapes.Count);
        var shape = geometry.Shapes.First();
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
        var node = nodes.First();
        Assert.AreEqual(0, node.GetAnnotations().Count);
    }

    [TestMethod]
    public void deserializeUnsetRequiredContainment()
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

    #region unknown_classifier

    [TestMethod]
    public void test_deserialization_of_a_node_with_unknown_classifier_throws_exception_does_not_fail()
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
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-unknown"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<UnsupportedClassifierException>(() => deserializer.Deserialize(serializationChunk));
    }

    class UnknownClassifierHandler(Func<Classifier?> incrementer) : IDeserializerHandler
    {
        public Classifier? UnknownClassifier(string id, MetaPointer metaPointer) => incrementer();

        public Feature? UnknownFeature(Classifier classifier, CompressedMetaPointer compressedMetaPointer,
            IReadableNode node) => throw new NotImplementedException();

        public INode? UnknownParent(CompressedId parentId, INode node) => throw new NotImplementedException();

        public INode? UnknownChild(CompressedId childId, IWritableNode node) => throw new NotImplementedException();

        public IReadableNode? UnknownReference(CompressedId targetId, string? resolveInfo, IWritableNode node) =>
            throw new NotImplementedException();

        public INode? UnknownAnnotation(CompressedId annotationId, INode node) => throw new NotImplementedException();

        public INode? InvalidAnnotation(IReadableNode annotation, IWritableNode node) =>
            throw new NotImplementedException();

        public Enum? UnknownEnumerationLiteral(string nodeId, Enumeration enumeration, string key) =>
            throw new NotImplementedException();

        public object? UnknownDatatype(string nodeId, Property property, string? value) =>
            throw new NotImplementedException();

        public bool SkipDeserializingDependentNode(string id) => throw new NotImplementedException();

        public TFeature? InvalidFeature<TFeature>(Classifier classifier, CompressedMetaPointer compressedMetaPointer,
            IReadableNode node) where TFeature : class, Feature =>
            throw new NotImplementedException();

        public void InvalidContainment(IReadableNode node) => throw new NotImplementedException();

        public void InvalidReference(IReadableNode node) => throw new NotImplementedException();

        public IWritableNode? InvalidAnnotationParent(IReadableNode annotation, string parentId) =>
            throw new NotImplementedException();
    }

    [TestMethod]
    public void test_deserialization_of_a_node_with_unknown_classifier_custom_handler_does_not_fail()
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
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-unknown"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var count = 0;

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new UnknownClassifierHandler(() =>
            {
                Interlocked.Increment(ref count);
                return null;
            }))
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        try
        {
            deserializer.Deserialize(serializationChunk);
        } catch (InvalidOperationException _)
        {
        }

        Assert.AreEqual(1, count);
    }

    #endregion
}
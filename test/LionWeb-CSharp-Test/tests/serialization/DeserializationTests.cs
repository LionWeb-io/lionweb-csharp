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
using Examples.WithEnum.M2;
using Examples.Library.M2;
using Examples.TinyRefLang;
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

    private abstract class NotImplementedDeserializerHandler : IDeserializerHandler
    {
        public virtual Classifier? UnknownClassifier(string id, MetaPointer metaPointer) =>
            throw new NotImplementedException();

        public virtual Feature? UnknownFeature(Classifier classifier, CompressedMetaPointer compressedMetaPointer,
            IReadableNode node) => throw new NotImplementedException();

        public TFeature? UnknownFeature<TFeature>(Classifier classifier, CompressedMetaPointer compressedMetaPointer,
            IReadableNode node) where TFeature : class, Feature =>
            throw new NotImplementedException();

        public virtual INode? UnknownParent(CompressedId parentId, INode node) => throw new NotImplementedException();

        public virtual INode? UnknownChild(CompressedId childId, IWritableNode node) =>
            throw new NotImplementedException();

        public virtual IReadableNode?
            UnknownReference(CompressedId targetId, string? resolveInfo, IWritableNode node) =>
            throw new NotImplementedException();

        public virtual INode? UnknownAnnotation(CompressedId annotationId, INode node) =>
            throw new NotImplementedException();

        public virtual INode? InvalidAnnotation(IReadableNode annotation, IWritableNode node) =>
            throw new NotImplementedException();

        public virtual Enum? UnknownEnumerationLiteral(string nodeId, Enumeration enumeration, string key) =>
            throw new NotImplementedException();

        public object? UnknownDatatype(string nodeId, Feature property, string? value) =>
            throw new NotImplementedException();

        public bool SkipDeserializingDependentNode(string id) => throw new NotImplementedException();

        public virtual TFeature? InvalidFeature<TFeature>(Classifier classifier,
            CompressedMetaPointer compressedMetaPointer,
            IReadableNode node) where TFeature : class, Feature =>
            throw new NotImplementedException();

        public virtual void InvalidContainment(IReadableNode node) => throw new NotImplementedException();

        public virtual void InvalidReference(IReadableNode node) => throw new NotImplementedException();

        public virtual IWritableNode? InvalidAnnotationParent(IReadableNode annotation, string parentId) =>
            throw new NotImplementedException();
    }

    #region unknown_classifier

    [TestMethod]
    public void test_deserialization_of_a_node_with_unknown_classifier_throws_exception_does_not_fail()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages = [new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }],
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

    private class UnknownClassifierHandler(Func<Classifier?> incrementer) : NotImplementedDeserializerHandler
    {
        public override Classifier? UnknownClassifier(string id, MetaPointer metaPointer) => incrementer();
    }

    [TestMethod]
    public void test_deserialization_of_a_node_with_unknown_classifier_custom_handler_returns_null_does_not_fail()
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

    #region unknown_feature

    [TestMethod]
    public void test_deserialization_of_a_node_with_unknown_feature_throws_exception_does_not_fail()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages = [new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }],
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
                            Containment = new MetaPointer("key-Shapes", "1", "key-unknown"),
                            Children =
                            [
                                "bar"
                            ]
                        }
                    ],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<UnknownFeatureException>(() => deserializer.Deserialize(serializationChunk));
    }

    private class UnknownFeatureHandler(Func<Feature?> incrementer) : NotImplementedDeserializerHandler
    {
        public override Feature? UnknownFeature(Classifier classifier, CompressedMetaPointer compressedMetaPointer,
            IReadableNode node)
            => incrementer();
    }

    [TestMethod]
    [Ignore]
    public void test_deserialization_of_a_node_with_unknown_feature_custom_handler_returns_null_does_not_fail()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages = [new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }],
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
                            Containment = new MetaPointer("key-Shapes", "1", "key-unknown"),
                            Children =
                            [
                                "bar"
                            ]
                        }
                    ],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var count = 0;

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new UnknownFeatureHandler(() =>
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

    #region unknown_parent

    [TestMethod]
    public void test_deserialization_of_a_node_with_unknown_parent_throws_exception_does_not_fail()
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Geometry"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "bar"
                }
            ]
        };

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<DeserializerException>(() => deserializer.Deserialize(serializationChunk));
    }

    private class UnknownParentHandler(Func<INode?> incrementer) : NotImplementedDeserializerHandler
    {
        public override INode? UnknownParent(CompressedId parentId, INode node)
            => incrementer();
    }

    [TestMethod]
    public void test_deserialization_of_a_node_with_unknown_parent_custom_handler_returns_null_does_not_fail()
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Geometry"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "bar"
                }
            ]
        };

        var count = 0;

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new UnknownParentHandler(() =>
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

    #region unknown_child

    [TestMethod]
    public void test_deserialization_of_a_node_with_unknown_child_throws_exception_does_not_fail()
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
                }
            ]
        };

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<DeserializerException>(() => deserializer.Deserialize(serializationChunk));
    }

    private class UnknownChildHandler(Func<INode?> incrementer) : NotImplementedDeserializerHandler
    {
        public override INode? UnknownChild(CompressedId childId, IWritableNode node)
            => incrementer();
    }

    [TestMethod]
    public void test_deserialization_of_a_node_with_unknown_child_custom_handler_returns_null_does_not_fail()
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
                }
            ]
        };

        var count = 0;

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new UnknownChildHandler((() =>
            {
                Interlocked.Increment(ref count);
                return null;
            })))
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

    #region unknown_reference

    [TestMethod]
    public void test_deserialization_of_a_node_with_unknown_reference_throws_exception_does_not_fail()
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Geometry"),
                    Properties = [],
                    Containments = [],
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
                }
            ]
        };

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<DeserializerException>(() => deserializer.Deserialize(serializationChunk));
    }

    private class UnknownReferenceHandler(Func<IReadableNode?> incrementer) : NotImplementedDeserializerHandler
    {
        public override IReadableNode? UnknownReference(CompressedId targetId, string? resolveInfo, IWritableNode node)
            => incrementer();
    }

    [TestMethod]
    public void test_deserialization_of_a_node_with_unknown_reference_custom_handler_returns_null_does_not_fail()
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Geometry"),
                    Properties = [],
                    Containments = [],
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
                }
            ]
        };

        var count = 0;

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new UnknownReferenceHandler((() =>
            {
                Interlocked.Increment(ref count);
                return null;
            })))
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        try
        {
            deserializer.Deserialize(serializationChunk);
        } catch (InvalidOperationException)
        {
        }

        Assert.AreEqual(1, count);
    }

    #endregion

    #region unknown_annotation

    [TestMethod]
    public void test_deserialization_of_a_node_with_unknown_annotation_throws_exception_does_not_fail()
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Geometry"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = ["lizard"],
                }
            ]
        };

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<DeserializerException>(() => deserializer.Deserialize(serializationChunk));
    }

    private class UnknownAnnotationHandler(Func<INode?> incrementer) : NotImplementedDeserializerHandler
    {
        public override INode? UnknownAnnotation(CompressedId annotationId, INode node)
            => incrementer();
    }

    [TestMethod]
    public void test_deserialization_of_a_node_with_unknown_annotation_custom_handler_returns_null_does_not_fail()
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Geometry"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = ["lizard"],
                }
            ]
        };

        var count = 0;

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new UnknownAnnotationHandler((() =>
            {
                Interlocked.Increment(ref count);
                return null;
            })))
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        try
        {
            deserializer.Deserialize(serializationChunk);
        } catch (InvalidOperationException)
        {
        }

        Assert.AreEqual(1, count);
    }

    #endregion

    #region invalid_feature

    [TestMethod]
    [Ignore("ThrowFormatException is invalid feature case ?")]
    // Remark: what is the difference between UnknownFeature and InvalidFeature ? For both cases
    // UnknownFeatureException is thrown.
    public void test_deserialization_of_a_node_with_invalid_feature_throws_exception_does_not_fail()
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("key-Shapes", "1", "key-x"), Value = "not a integer"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        /*IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerIgnoringHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);*/

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<UnknownFeatureException>(() => deserializer.Deserialize(serializationChunk));
    }

    private class InvalidFeatureHandler(Func<Feature?> incrementer) : NotImplementedDeserializerHandler
    {
        public override TFeature? InvalidFeature<TFeature>(Classifier classifier,
            CompressedMetaPointer compressedMetaPointer,
            IReadableNode node) where TFeature : class => (TFeature?)incrementer();
    }

    [TestMethod]
    [Ignore("UnknownFeature is recognized instead of InvalidFeature")]
    public void test_deserialization_of_a_node_with_invalid_feature_custom_handler_returns_null_does_not_fail()
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("key-Shapes", "2", "key-x"), Value = "not an integer"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var count = 0;

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new InvalidFeatureHandler(() =>
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

    #region unknown_enumeration_literal

    [TestMethod]
    public void test_deserialization_of_a_node_with_unknown_enumeration_literal_throws_exception_does_not_fail()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "WithEnum", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("WithEnum", "1", "EnumHolder"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("WithEnum", "1", "enumValue"), Value = "unknown"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguage(WithEnumLanguage.Instance)
            .Build();

        Assert.ThrowsException<DeserializerException>(() => deserializer.Deserialize(serializationChunk));
    }

    private class UnknownEnumerationLiteralHandler(Func<Enum?> incrementer) : NotImplementedDeserializerHandler
    {
        public override Enum? UnknownEnumerationLiteral(string nodeId, Enumeration enumeration, string key)
            => incrementer();
    }


    [TestMethod]
    public void
        test_deserialization_of_a_node_with_unknown_enumeration_literal_custom_handler_returns_null_does_not_fail()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "WithEnum", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("WithEnum", "1", "EnumHolder"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("WithEnum", "1", "enumValue"), Value = "unknown"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var count = 0;

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new UnknownEnumerationLiteralHandler(() =>
            {
                Interlocked.Increment(ref count);
                return null;
            }))
            .WithLanguage(WithEnumLanguage.Instance)
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

    #region invalid_containment

    [TestMethod]
    public void test_deserialization_of_a_node_with_invalid_containment_throws_exception_does_not_fail()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" },
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-center"), Children = ["bar"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                },

                new SerializedNode
                {
                    Id = "bar",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Line"),
                    Properties = [],
                    Containments =
                    [
                    ],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<InvalidValueException>(() => deserializer.Deserialize(serializationChunk));
    }

    [TestMethod]
    [Ignore(message: "no exception thrown")]
    public void test_deserialization_of_a_node_with_single_containment_expected_throws_exception_does_not_fail()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" },
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-center"),
                            Children = ["bar1", "bar2"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                },

                new SerializedNode
                {
                    Id = "bar1",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "foo"
                },

                new SerializedNode
                {
                    Id = "bar2",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "foo"
                }
            ]
        };

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<InvalidValueException>(() => deserializer.Deserialize(serializationChunk));
    }

    private class InvalidContainmentHandler(Action incrementer) : NotImplementedDeserializerHandler
    {
        public override void InvalidContainment(IReadableNode node) => incrementer();
    }

    [TestMethod]
    [Ignore("fails, InvalidContainment is not considered in M1 deserialization, this might be the reason ?")]
    public void test_deserialization_of_a_node_with_invalid_containment_custom_handler_does_not_fail()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" },
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-center"), Children = ["bar"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                },

                new SerializedNode
                {
                    Id = "bar",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Line"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var count = 0;

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new InvalidContainmentHandler(() =>
            {
                Interlocked.Increment(ref count);
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

    #region invalid_reference

    [TestMethod]
    // rename test: reference target type mismatch exception handler / ignoring handler / custom handler
    public void test_deserialization_of_a_node_with_invalid_reference_throws_exception_does_not_fail()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "library", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("library", "1", "Book"),
                    Properties = [],
                    Containments = [],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("library", "1", "author"),
                            Targets =
                            [
                                new SerializedReferenceTarget { Reference = "book_1", ResolveInfo = "author" },
                            ]
                        }
                    ],
                    Annotations = [],
                },

                new SerializedNode
                {
                    Id = "book_1",
                    Classifier = new MetaPointer("library", "1", "Book"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                },
            ]
        };

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguage(LibraryLanguage.Instance)
            .Build();

        Assert.ThrowsException<InvalidValueException>(() => deserializer.Deserialize(serializationChunk));
    }

    [TestMethod]
    [Ignore(message: "no exception thrown")]
    public void test_deserialization_of_a_node_with_single_reference_expected_throws_exception_does_not_fail()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-tinyRefLang", Version = "0" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("key-tinyRefLang", "0", "key-MyConcept"),
                    Properties = [],
                    Containments = [],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("key-tinyRefLang", "0", "key-MyConcept-singularRef"),
                            Targets =
                            [
                                new SerializedReferenceTarget { Reference = "ref_1", ResolveInfo = "ref" },
                                new SerializedReferenceTarget { Reference = "ref_2", ResolveInfo = "ref" },
                            ]
                        }
                    ],
                    Annotations = [],
                },

                new SerializedNode
                {
                    Id = "ref_1",
                    Classifier = new MetaPointer("key-tinyRefLang", "0", "key-MyConcept"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                },

                new SerializedNode
                {
                    Id = "ref_2",
                    Classifier = new MetaPointer("key-tinyRefLang", "0", "key-MyConcept"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                },
            ]
        };

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguage(TinyRefLangLanguage.Instance)
            .Build();

        Assert.ThrowsException<InvalidValueException>(() => deserializer.Deserialize(serializationChunk));
    }

    private class InvalidReferenceHandler(Action incrementer) : NotImplementedDeserializerHandler
    {
        public override void InvalidReference(IReadableNode node) => incrementer();
    }

    [TestMethod]
    [Ignore("fails, InvalidReference is not considered in M1 deserialization, this might be the reason ?")]
    public void test_deserialization_of_a_node_with_invalid_reference_custom_handler_does_not_fail()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "library", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("library", "1", "Book"),
                    Properties = [],
                    Containments = [],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("library", "1", "author"),
                            Targets =
                            [
                                new SerializedReferenceTarget { Reference = "author_1", ResolveInfo = "author" },
                            ]
                        }
                    ],
                    Annotations = [],
                },

                new SerializedNode
                {
                    Id = "author_1",
                    Classifier = new MetaPointer("library", "1", "Book"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                },
            ]
        };

        var count = 0;

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new InvalidReferenceHandler(() =>
            {
                Interlocked.Increment(ref count);
            }))
            .WithLanguage(LibraryLanguage.Instance)
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

    #region invalid_annotation

    [TestMethod]
    public void test_deserialization_of_a_node_with_invalid_annotation_throws_exception_does_not_fail()
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Geometry"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = ["lizard"],
                },

                new SerializedNode
                {
                    Id = "lizard",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Geometry"),
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

        Assert.ThrowsException<DeserializerException>(() => deserializer.Deserialize(serializationChunk));
    }

    private class InvalidAnnotationHandler(Func<INode?> incrementer) : NotImplementedDeserializerHandler
    {
        public override INode? InvalidAnnotation(IReadableNode annotation, IWritableNode node) =>
            incrementer();
    }

    [TestMethod]
    public void test_deserialization_of_a_node_with_invalid_annotation_custom_handler_returns_null_does_not_fail()
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Geometry"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = ["lizard"],
                },

                new SerializedNode
                {
                    Id = "lizard",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Geometry"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var count = 0;

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new InvalidAnnotationHandler(() =>
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

    #region invalid_annotation_parent

    [TestMethod]
    public void test_deserialization_of_a_node_with_invalid_annotation_parent_throws_exception_does_not_fail()
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Geometry"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = ["lizard"],
                },

                new SerializedNode
                {
                    Id = "lizard",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Documentation"),
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

        Assert.ThrowsException<DeserializerException>(() => deserializer.Deserialize(serializationChunk));
    }

    private class InvalidAnnotationParentHandler(Func<IWritableNode?> incrementer) : NotImplementedDeserializerHandler
    {
        public override IWritableNode? InvalidAnnotationParent(IReadableNode annotation, string parentId)
            => incrementer();
    }


    [TestMethod]
    [Ignore(message: "the method or operation is not implemented")]
    public void test_deserialization_of_a_node_with_invalid_annotation_parent_custom_handler_returns_null_does_not_fail()
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Geometry"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = ["lizard"],
                },

                new SerializedNode
                {
                    Id = "lizard",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Documentation"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var count = 0;

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new InvalidAnnotationParentHandler(() =>
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

    #region skip_deserializing_dependent_node

    [TestMethod]
    [Ignore("no exception is thrown in M1 deserializer")]
    public void test_skip_deserializing_dependent_node_throws_exception_does_not_fail()
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
                new SerializedNode()
                {
                    Id = "bar",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-OffsetDuplicate"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-docs"), Children = ["doc"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "doc",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Documentation"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                },
            ]
        };


        Geometry dependentGeometry = ShapesLanguage.Instance.GetFactory().CreateGeometry();
        Documentation documentation = ShapesLanguage.Instance.GetFactory().NewDocumentation("doc");
        dependentGeometry.Documentation = documentation;

        /*
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerIgnoringHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk,
            dependentGeometry.Descendants(true, true));
            */

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build();


        Assert.ThrowsException<DeserializerException>(() =>
            deserializer.Deserialize(serializationChunk,
                dependentGeometry.Descendants(true, true)));
    }

    #endregion

    #region unknown_datatype

    [TestMethod]
    [Ignore("test case is not proper, how to write a test case with unknown datatype")]
    public void test_deserialization_of_a_node_with_unknown_datatype_throws_exception_does_not_fail()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "WithEnum", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("WithEnum", "1", "EnumHolder"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("WithEnum", "1", "enumValue"), Value = "lit1"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                },
            ]
        };

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerIgnoringHandler())
            .WithLanguage(WithEnumLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);

        /*IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguage(WithEnumLanguage.Instance)
            .Build();

        Assert.ThrowsException<DeserializerException>(() => deserializer.Deserialize(serializationChunk));*/
    }

    #endregion
}
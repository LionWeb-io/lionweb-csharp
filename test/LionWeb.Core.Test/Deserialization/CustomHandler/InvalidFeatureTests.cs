﻿// Copyright 2024 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Test.Deserialization.CustomHandler;

using Core.Serialization;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;
using M3;

/// <summary>
/// Tests for <see cref="IDeserializerHandler.InvalidFeature{TFeature}"/>
/// </summary>
[TestClass]
public class InvalidFeatureTests
{
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    private class DeserializerHealingHandler(Func<CompressedMetaPointer, Classifier, IWritableNode, Feature?> heal)
        : DeserializerExceptionHandler
    {
        public override Feature? InvalidFeature<TFeature>(CompressedMetaPointer feature, Classifier classifier,
            IReadableNode node) => heal(feature, classifier, (IWritableNode)node);
    }

    /// <summary>
    /// Invalid feature: using existing feature in a wrong feature type: r is a property feature type,
    /// but used in reference feature type 
    /// </summary>
    [TestMethod]
    public void invalid_feature_does_not_heal()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
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
                    Containments = [],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("key-Shapes", "1", "key-r"), Targets = []
                        }
                    ],
                    Annotations = [],
                },
            ]
        };

        var deserializerHealingHandler = new DeserializerHealingHandler((pointer, classifier, arg3) => null);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(1, deserializedNodes.Count);
        Assert.AreEqual(0,
            deserializedNodes.OfType<Circle>().FirstOrDefault()?.CollectAllSetFeatures().OfType<Reference>().Count());
    }

    /// <summary>
    /// Tries to heal to a feature which is not part of the classifier
    /// </summary>
    [TestMethod]
    public void invalid_feature_tries_to_heal_to_invalid_feature()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
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
                    Containments = [],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("key-Shapes", "1", "key-r"),
                            Targets =
                            [
                                new SerializedReferenceTarget
                                {
                                    Reference = "center", ResolveInfo = "center-resolve-info"
                                }
                            ]
                        }
                    ],
                    Annotations = [],
                },

                new SerializedNode
                {
                    Id = "center",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var deserializerHealingHandler =
            new DeserializerHealingHandler((pointer, classifier, arg3) => ShapesLanguage.Instance.CompositeShape_parts);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<UnknownFeatureException>(() => deserializer.Deserialize(serializationChunk));
    }

    /// <summary>
    /// Heals from using a <see cref="Property"/> feature type in <see cref="Reference"/> type
    /// Heals to a <see cref="Containment"/> feature
    /// </summary>
    [TestMethod]
    public void invalid_feature_heals_to_a_containment()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
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
                    Containments = [],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("key-Shapes", "1", "key-r"),
                            Targets =
                            [
                                new SerializedReferenceTarget
                                {
                                    Reference = "center", ResolveInfo = "center-resolve-info"
                                }
                            ]
                        }
                    ],
                    Annotations = [],
                },

                new SerializedNode
                {
                    Id = "center",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var deserializerHealingHandler =
            new DeserializerHealingHandler((pointer, classifier, arg3) => ShapesLanguage.Instance.Circle_center);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(1,
            deserializedNodes.OfType<Circle>().FirstOrDefault()?.CollectAllSetFeatures().OfType<Containment>().Count());
        Assert.AreEqual(1, deserializedNodes.Count);
    }

    /// <summary>
    /// Heals from using a <see cref="Containment"/> feature type in <see cref="Reference"/> type
    /// Heals to a <see cref="Reference"/> feature
    /// </summary>
    [TestMethod]
    public void invalid_feature_heals_to_a_reference()
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-OffsetDuplicate"),
                    Properties = [],
                    Containments = [],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("key-Shapes", "1", "key-offset"),
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
                    Id = "RefFirst",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
            ]
        };

        var deserializerHealingHandler =
            new DeserializerHealingHandler(
                (pointer, classifier, arg3) => ShapesLanguage.Instance.OffsetDuplicate_source);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(1,
            deserializedNodes.OfType<OffsetDuplicate>().FirstOrDefault()?.CollectAllSetFeatures().OfType<Reference>()
                .Count());
        Assert.AreEqual(2, deserializedNodes.Count);
    }
}
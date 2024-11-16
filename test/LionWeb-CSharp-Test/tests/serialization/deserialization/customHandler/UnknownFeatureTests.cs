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

using Examples.Library.M2;
using Examples.Shapes.M2;
using LionWeb.Core;
using LionWeb.Core.M1;
using LionWeb.Core.M3;
using LionWeb.Core.Serialization;

/// <summary>
/// Tests for <see cref="IDeserializerHandler.UnknownFeature{TFeature}"/>
/// </summary>
[TestClass]
public class UnknownFeatureTests
{
    private class DeserializerHealingHandler(Func<CompressedMetaPointer, Classifier, IWritableNode, Feature?> heal)
        : DeserializerExceptionHandler
    {
        public override Feature? UnknownFeature<TFeature>(CompressedMetaPointer feature, Classifier classifier,
            IWritableNode node) => heal(feature, classifier, node);
    }


    [TestMethod]
    public void unknown_containment_does_not_heal()
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-unknown-containment"),
                            Children = ["child"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                },

                new SerializedNode
                {
                    Id = "child",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
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
        Assert.AreEqual(2, deserializedNodes.Count);
        Assert.AreEqual(0, deserializedNodes.OfType<Circle>().FirstOrDefault()?.CollectAllSetFeatures()
            .OfType<Containment>().Count());
    }

    [TestMethod]
    public void unknown_containment_heals()
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-unknown-containment"),
                            Children = ["child"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                },

                new SerializedNode
                {
                    Id = "child",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                },
            ]
        };

        var deserializerHealingHandler =
            new DeserializerHealingHandler((pointer, classifier, arg3) => ShapesLanguage.Instance.Circle_center);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(1, deserializedNodes.Count);
        Assert.AreEqual(1, deserializedNodes.OfType<Circle>().FirstOrDefault()?.CollectAllSetFeatures()
            .OfType<Containment>().Count());
    }

    [TestMethod]
    public void unknown_containment_tries_to_heal_to_invalid_feature()
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-unknown-containment"),
                            Children = ["child"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                },

                new SerializedNode
                {
                    Id = "child",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                },
            ]
        };

        var deserializerHealingHandler =
            new DeserializerHealingHandler((pointer, classifier, arg3) => ShapesLanguage.Instance.Circle_r);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<UnknownFeatureException>(() => deserializer.Deserialize(serializationChunk));
    }

    [TestMethod]
    public void unknown_property_does_not_heal()
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
                            Property = new MetaPointer("key-Shapes", "1", "key-unknown-property"), Value = "1"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var deserializerHealingHandler = new DeserializerHealingHandler((pointer, classifier, arg3) => null);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(1, deserializedNodes.Count);
        Assert.AreEqual(0, deserializedNodes.OfType<Coord>().FirstOrDefault()?.CollectAllSetFeatures()
            .OfType<Property>().Count());
    }

    [TestMethod]
    public void unknown_property_heals()
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
                            Property = new MetaPointer("key-Shapes", "1", "key-unknown-property"), Value = "1"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var deserializerHealingHandler =
            new DeserializerHealingHandler((pointer, classifier, arg3) => ShapesLanguage.Instance.Coord_x);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(1, deserializedNodes.Count);
        Assert.AreEqual(1, deserializedNodes.OfType<Coord>().FirstOrDefault()?.CollectAllSetFeatures()
            .OfType<Property>().Count());
    }

    [TestMethod]
    public void unknown_reference_does_not_heal()
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
                            Reference = new MetaPointer("library", "1", "key-unknown-reference"),
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
                    Classifier = new MetaPointer("library", "1", "Writer"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("library", "1", "library_Writer_name"), Value = "author-name"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                },
            ]
        };

        var deserializerHealingHandler = new DeserializerHealingHandler((pointer, classifier, arg3) => null);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(LibraryLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(2, deserializedNodes.Count);
        Assert.AreEqual(0,
            deserializedNodes.OfType<Book>().FirstOrDefault()?.CollectAllSetFeatures().OfType<Reference>().Count());
    }

    [TestMethod]
    public void unknown_reference_heals()
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
                            Reference = new MetaPointer("library", "1", "key-unknown-reference"),
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
                    Classifier = new MetaPointer("library", "1", "Writer"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("library", "1", "library_Writer_name"), Value = "author-name"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                },
            ]
        };

        var deserializerHealingHandler =
            new DeserializerHealingHandler((pointer, classifier, arg3) => LibraryLanguage.Instance.Book_author);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(LibraryLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(2, deserializedNodes.Count);
        Assert.AreEqual(1,
            deserializedNodes.OfType<Book>().FirstOrDefault()?.CollectAllSetFeatures().OfType<Reference>().Count());
    }
}
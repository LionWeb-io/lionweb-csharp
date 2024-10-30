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
using Examples.TinyRefLang;
using LionWeb.Core;
using LionWeb.Core.M1;
using LionWeb.Core.M3;
using LionWeb.Core.Serialization;

[TestClass]
public class InvalidLinkValueTests
{
    /// <summary>
    /// <see cref="IDeserializerHandler.InvalidLinkValue{T}"/>
    /// </summary>

    #region invalid link value

    private class InvalidLinkValueDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }

        public override List<T>? InvalidLinkValue<T>(List<T> value, Feature link, IWritableNode node)
        {
            Called = true;
            return null;
        }
    }

    [TestMethod]
    public void invalid_containment_type()
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
                            Children = ["invalid-child"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                },

                new SerializedNode
                {
                    Id = "invalid-child",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Line"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var invalidLinkValueDeserializerHandler = new InvalidLinkValueDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(invalidLinkValueDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(invalidLinkValueDeserializerHandler.Called);
    }

    [TestMethod]
    public void invalid_containment_cardinality_expects_single()
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
                            Children = ["child1", "child2"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                },

                new SerializedNode
                {
                    Id = "child1",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "foo"
                },

                new SerializedNode
                {
                    Id = "child2",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "foo"
                }
            ]
        };

        var invalidLinkValueDeserializerHandler = new InvalidLinkValueDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(invalidLinkValueDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(invalidLinkValueDeserializerHandler.Called);

        Circle circle = deserializedNodes.OfType<Circle>().First();
        Assert.IsFalse(circle.CollectAllSetFeatures().Contains(ShapesLanguage.Instance.Circle_center));
    }

    [TestMethod]
    public void invalid_reference_type()
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

        var invalidLinkValueDeserializerHandler = new InvalidLinkValueDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(invalidLinkValueDeserializerHandler)
            .WithLanguage(LibraryLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(invalidLinkValueDeserializerHandler.Called);
    }

    [TestMethod]
    public void invalid_reference_cardinality_expects_single()
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

        var invalidLinkValueDeserializerHandler = new InvalidLinkValueDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(invalidLinkValueDeserializerHandler)
            .WithLanguage(TinyRefLangLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(invalidLinkValueDeserializerHandler.Called);

        MyConcept myConcept = deserializedNodes.OfType<MyConcept>().First();
        Assert.IsFalse(myConcept.CollectAllSetFeatures().Contains(TinyRefLangLanguage.Instance.MyConcept_singularRef));
    }

    #endregion
}
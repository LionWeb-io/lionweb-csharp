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

[TestClass]
public class UnknownFeatureTests
{
    /// <summary>
    /// <see cref="IDeserializerHandler.UnknownFeature{TFeature}"/>
    /// </summary>

    #region unknown feature

    private class UnknownFeatureDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }

        public override Feature? UnknownFeature<TFeature>(CompressedMetaPointer feature, Classifier classifier,
            IWritableNode node)
        {
            Called = true;
            return null;
        }
    }


    [TestMethod]
    public void unknown_containment()
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
                            Containment = new MetaPointer("key-Shapes", "1", "key-unknown-containment"),
                            Children = []
                        }
                    ],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var unknownFeatureDeserializerHandler = new UnknownFeatureDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unknownFeatureDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unknownFeatureDeserializerHandler.Called);
    }

    [TestMethod]
    public void unknown_property_with_assigned_value()
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

        var unknownFeatureDeserializerHandler = new UnknownFeatureDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unknownFeatureDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unknownFeatureDeserializerHandler.Called);
    }

    [TestMethod]
    public void unknown_property_without_assigned_value()
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
                        new SerializedProperty { Property = new MetaPointer("key-Shapes", "1", "key-unknown-property") }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var unknownFeatureDeserializerHandler = new UnknownFeatureDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unknownFeatureDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unknownFeatureDeserializerHandler.Called);
    }

    [TestMethod]
    public void unknown_property_with_null_value()
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
                            Property = new MetaPointer("key-Shapes", "1", "key-unknown-property"), Value = null
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var unknownFeatureDeserializerHandler = new UnknownFeatureDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unknownFeatureDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unknownFeatureDeserializerHandler.Called);
    }

    [TestMethod]
    public void unknown_reference()
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

        var unknownFeatureDeserializerHandler = new UnknownFeatureDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unknownFeatureDeserializerHandler)
            .WithLanguage(LibraryLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unknownFeatureDeserializerHandler.Called);
    }

    #endregion
}
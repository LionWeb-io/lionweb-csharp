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
using Examples.WithEnum.M2;
using LionWeb.Core;
using LionWeb.Core.M1;
using LionWeb.Core.M2;
using LionWeb.Core.Serialization;

[TestClass]
public class DeserializationWithExceptionHandlerTests
{
    #region unknown_classifier

    [TestMethod]
    public void unknown_classifier_does_not_fail()
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

    #endregion

    #region unknown_feature

    [TestMethod]
    public void unknown_containment_does_not_fail()
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
                            Containment = new MetaPointer("key-Shapes", "1", "key-unknown"), Children = []
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

    [TestMethod]
    [Ignore(message: "no exception thrown")]
    public void unknown_property_does_not_fail()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" },
                new SerializedLanguageReference { Key = "LionCore_builtins", Version = ReleaseVersion.Current }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Geometry"),
                    Properties =
                    [
                        new SerializedProperty { Property = new MetaPointer("LionCore_builtins", "1", "key-unknown"), }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguages([ShapesLanguage.Instance, BuiltInsLanguage.Instance])
            .Build();

        Assert.ThrowsException<UnknownFeatureException>(() => deserializer.Deserialize(serializationChunk));
    }

    [TestMethod]
    public void unknown_reference_does_not_fail()
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
                            Reference = new MetaPointer("library", "1", "key-unknown"),
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

        Assert.ThrowsException<UnknownFeatureException>(() => deserializer.Deserialize(serializationChunk));
    }

    #endregion

    #region unknown_parent

    [TestMethod]
    public void unknown_parent_does_not_fail()
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
                    Parent = "unknown-parent"
                }
            ]
        };

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<DeserializerException>(() => deserializer.Deserialize(serializationChunk));
    }

    #endregion

    #region unknown_child

    [TestMethod]
    public void unknown_child_does_not_fail()
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
                            Containment = new MetaPointer("key-Shapes", "1", "key-shapes"),
                            Children = ["unknown-child"]
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

    #endregion

    #region unknown_reference_target

    [TestMethod]
    public void unknown_reference_target_does_not_fail()
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
                                new SerializedReferenceTarget
                                {
                                    Reference = "unknown-reference", ResolveInfo = "unknown-reference-resolve-info"
                                }
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

    #endregion

    #region unknown_annotation

    [TestMethod]
    public void unknown_annotation_does_not_fail()
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
                    Annotations = ["unknown-annotation"],
                }
            ]
        };

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<DeserializerException>(() => deserializer.Deserialize(serializationChunk));
    }

    #endregion

    #region unknown_enumeration_literal

    [TestMethod]
    public void unknown_enumeration_literal_does_not_fail()
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
                            Property = new MetaPointer("WithEnum", "1", "enumValue"),
                            Value = "unknown-enumeration-literal"
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

    #endregion

    #region invalid_containment

    [TestMethod]
    public void invalid_containment_classifier_mismatch_does_not_fail()
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

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<InvalidValueException>(() => deserializer.Deserialize(serializationChunk));
    }

    [TestMethod]
    [Ignore(message: "no exception thrown")]
    public void single_containment_expected_does_not_fail()
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

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<InvalidValueException>(() => deserializer.Deserialize(serializationChunk));
    }

    #endregion

    #region invalid_reference_target

    [TestMethod]
    public void reference_target_type_mismatch_does_not_fail()
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
    public void single_reference_expected_does_not_fail()
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

    #endregion

    #region invalid_feature

    [TestMethod]
    [Ignore("throws FormatException")]
    public void invalid_feature_does_not_fail()
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
                            Property = new MetaPointer("key-Shapes", "1", "key-x"), Value = "not an integer"
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
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<InvalidValueException>(() => deserializer.Deserialize(serializationChunk));
    }

    #endregion

    #region invalid_annotation

    [TestMethod]
    public void invalid_annotation_classifier_mismatch_does_not_fail()
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
                    Annotations = ["annotation"],
                },

                new SerializedNode
                {
                    Id = "annotation",
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

    #endregion

    #region invalid_annotation_parent

    [TestMethod]
    public void invalid_annotation_parent_does_not_fail()
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
                    Annotations = ["annotation"],
                },

                new SerializedNode
                {
                    Id = "annotation",
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

    #endregion

    #region unknown_datatype

    [TestMethod]
    [Ignore("how to write a test case with unknown datatype")]
    public void unknown_datatype_does_not_fail()
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
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguage(WithEnumLanguage.Instance)
            .Build();

        Assert.ThrowsException<DeserializerException>(() => deserializer.Deserialize(serializationChunk));
    }

    #endregion

    #region skip_deserializing_dependent_node

    [TestMethod]
    [Ignore(message: "no exception thrown")]
    public void skip_deserializing_dependent_node_does_not_fail()
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
                            Containment = new MetaPointer("key-Shapes", "1", "key-docs"), Children = ["repeated-id"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "repeated-id",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Documentation"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                },
            ]
        };

        Geometry dependentGeometry = ShapesLanguage.Instance.GetFactory().CreateGeometry();
        Documentation documentation = ShapesLanguage.Instance.GetFactory().NewDocumentation("repeated-id");
        dependentGeometry.Documentation = documentation;

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<DeserializerException>(() =>
            deserializer.Deserialize(serializationChunk,
                dependentGeometry.Descendants(true, true)));
    }

    #endregion
}
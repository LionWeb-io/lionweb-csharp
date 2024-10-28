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
using LionWeb.Core.Serialization;

[TestClass]
public class DeserializationWithExceptionHandlerTests
{
    /// <summary>
    /// <see cref="IDeserializerHandler.UnknownClassifier"/>
    /// </summary>

    #region unknown classifier

    [TestMethod]
    public void unknown_classifier()
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

    /// <summary>
    /// <see cref="IDeserializerHandler.UnknownFeature{TFeature}"/>
    /// </summary>

    #region unknown feature

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
    public void unknown_property_with_value()
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
                            Property = new MetaPointer("key-Shapes", "1", "key-unknown"), Value = "1"
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
            .WithLanguages([ShapesLanguage.Instance])
            .Build();

        Assert.ThrowsException<UnknownFeatureException>(() => deserializer.Deserialize(serializationChunk));
    }

    [TestMethod]
    [Ignore(message: "no exception thrown, expects an assigned value to evaluate, requires implementation")]
    public void unknown_property_without_value()
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
                        new SerializedProperty { Property = new MetaPointer("key-Shapes", "1", "key-unknown") }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new DeserializerExceptionHandler())
            .WithLanguages([ShapesLanguage.Instance])
            .Build();

        Assert.ThrowsException<UnknownFeatureException>(() => deserializer.Deserialize(serializationChunk));
    }

    [TestMethod]
    [Ignore(message: "no exception thrown, expects an assigned value to evaluate, requires implementation")]
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
                            Property = new MetaPointer("key-Shapes", "1", "key-unknown"), Value = null
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
            .WithLanguages([ShapesLanguage.Instance])
            .Build();

        Assert.ThrowsException<UnknownFeatureException>(() => deserializer.Deserialize(serializationChunk));
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

    /// <summary>
    /// <see cref="IDeserializerHandler.UnknownParent"/>
    /// </summary>

    #region unknown parent

    [TestMethod]
    public void unknown_parent()
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

    /// <summary>
    /// <see cref="IDeserializerHandler.UnknownChild"/>
    /// </summary>

    #region unknown child

    [TestMethod]
    public void unknown_child()
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

    [TestMethod]
    public void unknown_child_deserializer()
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

        var unknownChildDeserializer = new UnknownChildDeserializer();

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unknownChildDeserializer)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unknownChildDeserializer.Called);
        
    }

    private class UnknownChildDeserializer : DeserializerExceptionHandler
    {
        public bool Called;

        public override INode? UnknownChild(CompressedId childId, IWritableNode node)
        {
            Called = true;
            return null;
        }
    }

    [TestMethod]
    public void unknown_child_exception_handler() =>
        Assert.ThrowsException<DeserializerException>(() =>
            new DeserializerExceptionHandler().UnknownChild(CompressedId.Create("xx", true), new Line("a")));

    #endregion

    #region unknown reference target

    [TestMethod]
    public void unknown_reference_target()
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
                                    Reference = "unknown-reference-target",
                                    ResolveInfo = "unknown-reference-target-resolve-info"
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

    /// <summary>
    /// <see cref="IDeserializerHandler.UnknownAnnotation"/>
    /// </summary>

    #region unknown annotation

    [TestMethod]
    public void unknown_annotation()
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

    /// <summary>
    /// <see cref="IDeserializerHandler.UnknownEnumerationLiteral"/>
    /// </summary>

    #region unknown enumeration literal

    [TestMethod]
    public void unknown_enumeration_literal()
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

    /// <summary>
    /// <see cref="IDeserializerHandler.InvalidContainment"/>
    /// </summary>

    #region invalid containment

    [TestMethod]
    public void containment_classifier_mismatch()
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
    [Ignore(message:"requires implementation")]
    public void single_containment_expected()
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

    #region invalid reference

    [TestMethod]
    public void reference_classifier_mismatch()
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
    [Ignore(message:"requires implementation")]
    public void single_reference_expected()
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

    /// <summary>
    /// <see cref="IDeserializerHandler.InvalidFeature{TFeature}"/>
    /// </summary>

    #region invalid property value

    [TestMethod]
    [Ignore(message: "requires implementation")]
    public void invalid_property_value()
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

    /// <summary>
    /// <see cref="IDeserializerHandler.InvalidAnnotation"/>
    /// </summary>

    #region invalid annotation

    [TestMethod]
    public void invalid_annotation_classifier_mismatch()
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

    /// <summary>
    /// <see cref="IDeserializerHandler.InvalidAnnotationParent"/>
    /// </summary>

    #region invalid annotation parent

    [TestMethod]
    public void invalid_annotation_parent()
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

    /// <summary>
    /// <see cref="IDeserializerHandler.UnknownDatatype"/>
    /// </summary>

    #region unknown datatype

    [TestMethod]
    public void unknown_datatype()
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
                            Property = new MetaPointer("WithEnum", "1", "enumValue"), Value = "xxx"
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

    /// <summary>
    /// <see cref="IDeserializerHandler.SkipDeserializingDependentNode"/>
    /// </summary>

    #region skip deserializing dependent node

    [TestMethod]
    [Ignore(message:"requires implementation")]
    public void skip_deserializing_dependent_node()
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
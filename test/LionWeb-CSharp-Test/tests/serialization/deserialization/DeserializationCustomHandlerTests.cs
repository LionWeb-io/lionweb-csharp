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
using Examples.WithEnum.M2;
using LionWeb.Core;
using LionWeb.Core.M1;
using LionWeb.Core.M3;
using LionWeb.Core.Serialization;

[TestClass]
public class DeserializationCustomHandlerTests
{
    /// <summary>
    /// <see cref="IDeserializerHandler.UnknownClassifier"/>
    /// </summary>

    #region unknown classifier

    private class UnknownClassifierDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }

        public override Classifier? UnknownClassifier(string id, MetaPointer metaPointer)
        {
            Called = true;
            return null;
        }
    }

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

        var unknownClassifierDeserializerHandler = new UnknownClassifierDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unknownClassifierDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unknownClassifierDeserializerHandler.Called);
    }

    #endregion

    /// <summary>
    /// <see cref="IDeserializerHandler.UnknownFeature{TFeature}"/>
    /// </summary>

    #region unknown feature

    private class UnknownFeatureDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }

        public override TFeature? UnknownFeature<TFeature>(Classifier classifier,
            CompressedMetaPointer compressedMetaPointer,
            IReadableNode node) where TFeature : class
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
                            Containment = new MetaPointer("key-Shapes", "1", "key-unknown"), Children = []
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

        var unknownFeatureDeserializerHandler = new UnknownFeatureDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unknownFeatureDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unknownFeatureDeserializerHandler.Called);
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

        var unknownFeatureDeserializerHandler = new UnknownFeatureDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unknownFeatureDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unknownFeatureDeserializerHandler.Called);
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

        var unknownFeatureDeserializerHandler = new UnknownFeatureDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unknownFeatureDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unknownFeatureDeserializerHandler.Called);
    }

    #endregion

    /// <summary>
    /// <see cref="IDeserializerHandler.UnknownParent"/>
    /// </summary>

    #region unknown parent

    private class UnknownParentDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }

        public override INode? UnknownParent(CompressedId parentId, INode node)
        {
            Called = true;
            return null;
        }
    }

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

        var unknownParentDeserializerHandler = new UnknownParentDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unknownParentDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unknownParentDeserializerHandler.Called);
    }

    #endregion

    /// <summary>
    /// <see cref="IDeserializerHandler.UnknownChild"/>
    /// </summary>

    #region unknown child

    private class UnknownChildDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }

        public override INode? UnknownChild(CompressedId childId, IWritableNode node)
        {
            Called = true;
            return null;
        }
    }

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

        var unknownChildDeserializerHandler = new UnknownChildDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unknownChildDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unknownChildDeserializerHandler.Called);
    }

    #endregion

    #region unknown reference target

    private class UnknownReferenceTargetDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }
        
        public override IReadableNode? UnknownReferenceTarget(CompressedId targetId, string? resolveInfo,
            IWritableNode node)
        {
            Called = true;
            return null;
        }
    }

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

        var unknownReferenceTargetDeserializerHandler = new UnknownReferenceTargetDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unknownReferenceTargetDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unknownReferenceTargetDeserializerHandler.Called);
    }

    #endregion

    /// <summary>
    /// <see cref="IDeserializerHandler.UnknownAnnotation"/>
    /// </summary>

    #region unknown annotation

    private class UnknownAnnotationDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }
        
        public override INode? UnknownAnnotation(CompressedId annotationId, INode node)
        {
            Called = true;
            return null;
        }
    }

    
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

        var unknownAnnotationDeserializerHandler = new UnknownAnnotationDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unknownAnnotationDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unknownAnnotationDeserializerHandler.Called);
    }

    #endregion

    /// <summary>
    /// <see cref="IDeserializerHandler.UnknownEnumerationLiteral"/>
    /// </summary>

    #region unknown enumeration literal

    private class UnknownEnumerationLiteralDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }
        
        public override Enum? UnknownEnumerationLiteral(string nodeId, Enumeration enumeration, string key)
        {
            Called = true;
            return null;
        }
    }
    
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

        var unknownEnumerationLiteralDeserializerHandler = new UnknownEnumerationLiteralDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unknownEnumerationLiteralDeserializerHandler)
            .WithLanguage(WithEnumLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unknownEnumerationLiteralDeserializerHandler.Called);
    }

    #endregion

    /// <summary>
    /// <see cref="IDeserializerHandler.InvalidContainment"/>
    /// </summary>

    #region invalid containment

    private class InvalidContainmentDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }
        
        public override void InvalidContainment(IReadableNode node) => Called = true;
    }
    
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

        var invalidContainmentDeserializerHandler = new InvalidContainmentDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(invalidContainmentDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(invalidContainmentDeserializerHandler.Called);
    }

    [TestMethod]
    [Ignore(message: "requires implementation")]
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

        var invalidContainmentDeserializerHandler = new InvalidContainmentDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(invalidContainmentDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(invalidContainmentDeserializerHandler.Called);
    }

    #endregion

    #region invalid reference

    private class InvalidReferenceDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }
        
        public override void InvalidReference(IReadableNode node) => Called = true;
    }
    
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

        var invalidReferenceDeserializerHandler = new InvalidReferenceDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(invalidReferenceDeserializerHandler)
            .WithLanguage(LibraryLanguage.Instance)
            .Build();
        
        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(invalidReferenceDeserializerHandler.Called);
    }

    [TestMethod]
    [Ignore(message: "requires implementation")]
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

        var invalidReferenceDeserializerHandler = new InvalidReferenceDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(invalidReferenceDeserializerHandler)
            .WithLanguage(LibraryLanguage.Instance)
            .Build();
        
        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(invalidReferenceDeserializerHandler.Called);
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

    private class InvalidAnnotationDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }

        public override INode? InvalidAnnotation(IReadableNode annotation, IWritableNode node)
        {
            Called = true;
            return null;
        }
    }
    
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

        var invalidAnnotationDeserializerHandler = new InvalidAnnotationDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(invalidAnnotationDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(invalidAnnotationDeserializerHandler.Called);
    }

    #endregion

    /// <summary>
    /// <see cref="IDeserializerHandler.InvalidAnnotationParent"/>
    /// </summary>

    #region invalid annotation parent

    private class InvalidAnnotationParentDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }
        
        public override IWritableNode? InvalidAnnotationParent(IReadableNode annotation, string parentId)
        {
            Called = true;
            return null;
        }
    }
    
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

        var invalidAnnotationParentDeserializerHandler = new InvalidAnnotationParentDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(invalidAnnotationParentDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(invalidAnnotationParentDeserializerHandler.Called);
    }

    #endregion

    /// <summary>
    /// <see cref="IDeserializerHandler.UnknownDatatype"/>
    /// </summary>

    #region unknown datatype

    private class UnknownDatatypeDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }

        public override object? UnknownDatatype(string nodeId, Feature property, string? value)
        {
            Called = true;
            return null;
        }
    }

    
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

        var unknownDatatypeDeserializerHandler = new UnknownDatatypeDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unknownDatatypeDeserializerHandler)
            .WithLanguage(WithEnumLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unknownDatatypeDeserializerHandler.Called);
    }

    #endregion

    /// <summary>
    /// <see cref="IDeserializerHandler.SkipDeserializingDependentNode"/>
    /// </summary>

    #region skip deserializing dependent node

    private class SkipDeserializingDependentNodeDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }
        
        public override bool SkipDeserializingDependentNode(string id)
        {
            Called = true;
            return true;
        }
    }
    
    
    [TestMethod]
    [Ignore(message: "requires implementation")]
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

        var skipDeserializingDependentNodeDeserializerHandler = new SkipDeserializingDependentNodeDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(skipDeserializingDependentNodeDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk, dependentGeometry.Descendants(true, true));
        Assert.IsTrue(skipDeserializingDependentNodeDeserializerHandler.Called);
    }

    #endregion
}
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

        public override Classifier? UnknownClassifier(CompressedMetaPointer classifier, CompressedId id)
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
    //[Ignore(message: "no exception thrown, expects an assigned value to evaluate, requires implementation")]
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
    //[Ignore(message: "no exception thrown, expects an assigned value to evaluate, requires implementation")]
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
    /// <see cref="IDeserializerHandler.UnresolvableParent"/>
    /// </summary>

    #region unresolvable parent

    private class UnresolvableParentDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }
        
        public override IWritableNode? UnresolvableParent(CompressedId parentId, IWritableNode node)
        {
            Called = true;
            return null;
        }
    }

    [TestMethod]
    public void unresolvable_parent()
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
                    Parent = "unresolvable-parent"
                }
            ]
        };

        var unresolvableParentDeserializerHandler = new UnresolvableParentDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unresolvableParentDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unresolvableParentDeserializerHandler.Called);
    }

    #endregion

    /// <summary>
    /// <see cref="IDeserializerHandler.UnresolvableChild"/>
    /// </summary>

    #region unresolvable child

    private class UnresolvableChildDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }
        
        public override IWritableNode? UnresolvableChild(CompressedId childId, Feature containment, IWritableNode node)
        {
            Called = true;
            return null;
        }
    }

    [TestMethod]
    public void unresolvable_child()
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

        var unresolvableChildDeserializerHandler = new UnresolvableChildDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unresolvableChildDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unresolvableChildDeserializerHandler.Called);
    }

    #endregion

    /// <summary>
    /// <see cref="IDeserializerHandler.UnresolvableReferenceTarget"/>
    /// </summary>
    #region unresolvable reference target

    private class UnresolvableReferenceTargetDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }

        public override IReadableNode? UnresolvableReferenceTarget(CompressedId? targetId, string? resolveInfo,
            Feature reference,
            IWritableNode node)
        {
            Called = true;
            return null;
        }
    }

    [TestMethod]
    public void unresolvable_reference_target()
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

        var unresolvableReferenceTargetDeserializerHandler = new UnresolvableReferenceTargetDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unresolvableReferenceTargetDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unresolvableReferenceTargetDeserializerHandler.Called);
    }

    #endregion

    /// <summary>
    /// <see cref="IDeserializerHandler.UnresolvableAnnotation"/>
    /// </summary>

    #region unresolvable annotation

    private class UnresolvableAnnotationDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }
        
        public override IWritableNode? UnresolvableAnnotation(CompressedId annotationId, IWritableNode node)
        {
            Called = true;
            return null;
        }
    }

    
    [TestMethod]
    public void unresolvable_annotation()
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

        var unresolvableAnnotationDeserializerHandler = new UnresolvableAnnotationDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unresolvableAnnotationDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unresolvableAnnotationDeserializerHandler.Called);
    }

    #endregion

    /// <summary>
    /// <see cref="IDeserializerHandler.UnknownEnumerationLiteral"/>
    /// </summary>

    #region unknown enumeration literal

    private class UnknownEnumerationLiteralDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }
        
        public override Enum? UnknownEnumerationLiteral(string key, Enumeration enumeration, Feature property,
            IWritableNode nodeId)
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
    //[Ignore(message: "requires implementation")]
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
    //[Ignore(message: "requires implementation")]
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
    /// <see cref="IDeserializerHandler.InvalidPropertyValue{TValue}"/>
    /// </summary>

    #region invalid property value

    private class InvalidPropertyValueDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }
        
        public override object? InvalidPropertyValue<TValue>(string? value, Feature property, CompressedId nodeId)
        {
            Called = true;
            return null;
        }
    }
    
    [TestMethod]
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

        var invalidPropertyValueDeserializerHandler = new InvalidPropertyValueDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(invalidPropertyValueDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(invalidPropertyValueDeserializerHandler.Called);
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
        
        public override void InvalidAnnotationParent(IWritableNode annotation, IReadableNode? parent) => Called = true;
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
        
        public override object? UnknownDatatype(Feature property, string? value, IWritableNode node)
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
        
        public override bool SkipDeserializingDependentNode(CompressedId id)
        {
            Called = true;
            return true;
        }
    }
    
    
    [TestMethod]
    //[Ignore(message: "requires implementation")]
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
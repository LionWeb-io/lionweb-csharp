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

/// <summary>
/// Tests for <see cref="IDeserializerHandler.DuplicateNodeId"/>
/// </summary>>
[TestClass]
public class DuplicateNodeIdTests
{
    private class DeserializerHealingHandler(Func<ICompressedId, IWritableNode, INode, string?> heal)
        : DeserializerExceptionHandler
    {
        public override string? DuplicateNodeId(ICompressedId nodeId, IReadableNode existingNode, IReadableNode node) =>
            heal(nodeId, (IWritableNode)existingNode, (INode)node);
    }

    [TestMethod]
    public void duplicate_node_id_does_not_heal()
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
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-builtins", "2024.1",
                                "LionCore-builtins-INamed-name"),
                            Value = "First"
                        }
                    ],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-offset"),
                            Children =
                            [
                                "ChildFirst"
                            ]
                        }
                    ],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("key-Shapes", "1", "key-source"),
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
                    Id = "ChildFirst",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "A"
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
                new SerializedNode
                {
                    Id = "A",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-OffsetDuplicate"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-builtins", "2024.1",
                                "LionCore-builtins-INamed-name"),
                            Value = "Second"
                        }
                    ],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-offset"),
                            Children =
                            [
                                "ChildSecond"
                            ]
                        }
                    ],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("key-Shapes", "1", "key-source"),
                            Targets =
                            [
                                new SerializedReferenceTarget { Reference = "RefSecond" }
                            ]
                        }
                    ],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "ChildSecond",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "A"
                },
                new SerializedNode
                {
                    Id = "RefSecond",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
            ]
        };

        List<IReadableNode> nodes = new DeserializerBuilder()
            .WithHandler(new DeserializerIgnoringHandler())
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        Assert.AreEqual(4, nodes.Count);
        OffsetDuplicate a = nodes.OfType<OffsetDuplicate>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.AreEqual("First", a.Name);

        Assert.IsNotNull(a.Offset);
        Assert.AreEqual("ChildFirst", a.Offset.GetId());

        Assert.IsNotNull(a.Source);
        Assert.AreEqual("RefFirst", a.Source.GetId());

        Assert.AreEqual(2, nodes.OfType<Circle>().Count());

        Assert.AreEqual(1, nodes.OfType<Coord>().Count());
        Assert.AreEqual("ChildSecond", nodes.OfType<Coord>().First().GetId());
    }

    [TestMethod]
    public void duplicate_node_id_heals()
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
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-builtins", "2024.1",
                                "LionCore-builtins-INamed-name"),
                            Value = "First"
                        }
                    ],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-offset"),
                            Children =
                            [
                                "ChildFirst"
                            ]
                        }
                    ],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("key-Shapes", "1", "key-source"),
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
                    Id = "ChildFirst",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "A"
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
                new SerializedNode
                {
                    Id = "A",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-OffsetDuplicate"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-builtins", "2024.1",
                                "LionCore-builtins-INamed-name"),
                            Value = "Second"
                        }
                    ],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-offset"),
                            Children =
                            [
                                "ChildSecond"
                            ]
                        }
                    ],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("key-Shapes", "1", "key-source"),
                            Targets =
                            [
                                new SerializedReferenceTarget { Reference = "RefSecond" }
                            ]
                        }
                    ],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "ChildSecond",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "A"
                },
                new SerializedNode
                {
                    Id = "RefSecond",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
            ]
        };
        
        var deserializerHealingHandler = new DeserializerHealingHandler((id, node, arg3) => "renamed-A");

        List<IReadableNode> nodes = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        Assert.AreEqual(4, nodes.Count);
        OffsetDuplicate first = nodes.OfType<OffsetDuplicate>().First();
        Assert.AreEqual("A", first.GetId());
        Assert.AreEqual("First", first.Name);

        Assert.IsNotNull(first.Offset);
        Assert.AreEqual("ChildFirst", first.Offset.GetId());

        Assert.IsNotNull(first.Source);
        Assert.AreEqual("RefFirst", first.Source.GetId());

        OffsetDuplicate last = nodes.OfType<OffsetDuplicate>().Last();
        Assert.AreEqual("renamed-A", last.GetId());
        Assert.AreEqual("Second", last.Name);

        Assert.IsNotNull(last.Offset);
        Assert.AreEqual("ChildSecond", last.Offset.GetId());

        Assert.IsNotNull(last.Source);
        Assert.AreEqual("RefSecond", last.Source.GetId());

        Assert.AreEqual(2, nodes.OfType<Circle>().Count());
        Assert.AreEqual(0, nodes.OfType<Coord>().Count());
    }

    private readonly Dictionary<ICompressedId, int> _healingHandlerCallCount = new();
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    [TestMethod]
    public void duplicate_node_id_heals_after_second_attempt()
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
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-builtins", "2024.1",
                                "LionCore-builtins-INamed-name"),
                            Value = "First"
                        }
                    ],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-offset"),
                            Children =
                            [
                                "ChildFirst"
                            ]
                        }
                    ],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("key-Shapes", "1", "key-source"),
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
                    Id = "ChildFirst",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "A"
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
                new SerializedNode
                {
                    Id = "A",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-OffsetDuplicate"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("LionCore-builtins", "2024.1",
                                "LionCore-builtins-INamed-name"),
                            Value = "Second"
                        }
                    ],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-offset"),
                            Children =
                            [
                                "ChildSecond"
                            ]
                        }
                    ],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("key-Shapes", "1", "key-source"),
                            Targets =
                            [
                                new SerializedReferenceTarget { Reference = "RefSecond" }
                            ]
                        }
                    ],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "ChildSecond",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "A"
                },
                new SerializedNode
                {
                    Id = "RefSecond",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
            ]
        };

        var deserializerHealingHandler = new DeserializerHealingHandler((nodeId, existingNode, node) =>
        {
            if (!_healingHandlerCallCount.TryAdd(nodeId, 1))
            {
                _healingHandlerCallCount[nodeId] += 1;
            }

            return _healingHandlerCallCount[nodeId] == 1 ? "A" : "renamed-A";
        });

        List<IReadableNode> nodes = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        Assert.AreEqual(4, nodes.Count);
        OffsetDuplicate first = nodes.OfType<OffsetDuplicate>().First();
        Assert.AreEqual("A", first.GetId());
        Assert.AreEqual("First", first.Name);

        Assert.IsNotNull(first.Offset);
        Assert.AreEqual("ChildFirst", first.Offset.GetId());

        Assert.IsNotNull(first.Source);
        Assert.AreEqual("RefFirst", first.Source.GetId());

        OffsetDuplicate last = nodes.OfType<OffsetDuplicate>().Last();
        Assert.AreEqual("renamed-A", last.GetId());
        Assert.AreEqual("Second", last.Name);

        Assert.IsNotNull(last.Offset);
        Assert.AreEqual("ChildSecond", last.Offset.GetId());

        Assert.IsNotNull(last.Source);
        Assert.AreEqual("RefSecond", last.Source.GetId());

        Assert.AreEqual(2, nodes.OfType<Circle>().Count());
        Assert.AreEqual(0, nodes.OfType<Coord>().Count());
    }
}
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

using Examples.Shapes.M2;
using LionWeb.Core;
using LionWeb.Core.M1;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Serialization;

/// <summary>
/// Tests for <see cref="IDeserializerHandler.UnresolvableReferenceTarget"/>
/// </summary>
[TestClass]
public class UnresolvableReferenceTargetTests
{
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    private class DeserializerHealingHandler(Func<CompressedId?, string?, Feature, IWritableNode, IReadableNode?> heal)
        : DeserializerExceptionHandler
    {
        public override IReadableNode? UnresolvableReferenceTarget(CompressedId? targetId, string? resolveInfo,
            Feature reference, IReadableNode node) => heal(targetId, resolveInfo, reference, (IWritableNode)node);
    }

    [TestMethod]
    public void unresolvable_reference_target_does_not_heal()
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
                                    Reference = "unresolvable-reference-target",
                                    ResolveInfo = "unresolvable-reference-target-resolve-info"
                                }
                            ]
                        }
                    ],
                    Annotations = [],
                }
            ]
        };

        var deserializerHealingHandler = new DeserializerHealingHandler((id, s, arg3, arg4) => null);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(1, deserializedNodes.Count);
    }

    [TestMethod]
    public void unresolvable_reference_target_heals()
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
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-OffsetDuplicate"),
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
                                    Reference = "unresolvable-reference-target",
                                    ResolveInfo = "unresolvable-reference-target-resolve-info"
                                }
                            ]
                        }
                    ],
                    Annotations = [],
                }
            ]
        };

        var circle = new Circle("new-ref-target");
        var deserializerHealingHandler = new DeserializerHealingHandler((id, s, arg3, arg4) => circle);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(1, deserializedNodes.Count);

        Shape? source = deserializedNodes.OfType<OffsetDuplicate>().FirstOrDefault()?.Source;
        Assert.AreSame(circle, source);
        Assert.AreSame(circle.GetId(), source?.GetId());
    }

    [TestMethod]
    public void unresolvable_reference_target_heals_based_on_resolveInfo()
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
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-OffsetDuplicate"),
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
                                    Reference = "unresolvable-reference-target", ResolveInfo = "circle"
                                }
                            ]
                        }
                    ],
                    Annotations = [],
                }
            ]
        };

        var deserializerHealingHandler = new DeserializerHealingHandler((targetId, resolveInfo, reference, node) =>
            resolveInfo == "circle" ? new Circle("new-ref-target") : null);

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(1, deserializedNodes.Count);

        Shape? source = deserializedNodes.OfType<OffsetDuplicate>().FirstOrDefault()?.Source;
        Assert.IsTrue(source?.GetId() == "new-ref-target");
        Assert.AreSame(source.GetClassifier(), ShapesLanguage.Instance.Circle);
    }

    [TestMethod]
    public void unresolvable_reference_target_tries_to_heal_to_invalid_target()
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
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-OffsetDuplicate"),
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
                                    Reference = "unresolvable-reference-target",
                                    ResolveInfo = "unresolvable-reference-target-resolve-info"
                                }
                            ]
                        }
                    ],
                    Annotations = [],
                }
            ]
        };

        var coord = new Coord("invalid-target");
        var deserializerHealingHandler = new DeserializerHealingHandler((id, s, arg3, arg4) => coord);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<InvalidValueException>(() => deserializer.Deserialize(serializationChunk));
    }
}
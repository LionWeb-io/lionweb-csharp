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
using LionWeb.Core.M3;
using LionWeb.Core.Serialization;

/// <summary>
/// Tests for <see cref="IDeserializerHandler.UnresolvableChild"/>
/// </summary>
[TestClass]
public class UnresolvableChildTests
{
    private class DeserializerHealingHandler(Func<CompressedId, Feature, IWritableNode, IWritableNode?> heal)
        : DeserializerExceptionHandler
    {
        public override IWritableNode? UnresolvableChild(CompressedId childId, Feature containment, IWritableNode node)
            => heal(childId, containment, node);
    }

    [TestMethod]
    public void unresolvable_child_does_not_heal()
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
                            Children = ["unresolvable-child"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var deserializerHealingHandler = new DeserializerHealingHandler((id, feature, arg3) => null);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(1, deserializedNodes.Count);
    }

    [TestMethod]
    public void unresolvable_child_heals()
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
                            Children = ["unresolvable-child"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var circle = new Circle("new-child");

        var deserializerHealingHandler = new DeserializerHealingHandler((id, feature, arg3) => circle);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreSame(circle, deserializedNodes.OfType<Geometry>().First().Shapes[0]);
    }

    [TestMethod]
    public void unresolvable_child_tries_to_heal_to_invalid_child()
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
                            Children = ["unresolvable-child"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var coord = new Coord("invalid-child");

        var deserializerHealingHandler = new DeserializerHealingHandler((id, feature, arg3) => coord);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<InvalidValueException>(() => deserializer.Deserialize(serializationChunk));
    }
}
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
using LionWeb.Core.Serialization;

/// <summary>
/// <see cref="IDeserializerHandler.InvalidAnnotation"/>
/// </summary>
[TestClass]
public class InvalidAnnotationTests
{
    private class DeserializerHealingHandler(Func<IReadableNode, IWritableNode, INode?> heal)
        : DeserializerExceptionHandler
    {
        public override IWritableNode? InvalidAnnotation(IReadableNode annotation, IWritableNode node) =>
            heal(annotation, node);
    }

    [TestMethod]
    public void invalid_annotation_does_not_heal()
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = ["annotation"],
                },

                new SerializedNode
                {
                    Id = "annotation",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Line"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var deserializerHealingHandler = new DeserializerHealingHandler((node, writableNode) => null);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(2, deserializedNodes.Count);
        Assert.AreEqual(0, deserializedNodes.OfType<Circle>().FirstOrDefault()?.GetAnnotations().Count);
    }

    [TestMethod]
    public void invalid_annotation_heals()
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = ["annotation"],
                },

                new SerializedNode
                {
                    Id = "annotation",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Line"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var billOfMaterials = new BillOfMaterials("new-annotation") { };

        var deserializerHealingHandler = new DeserializerHealingHandler((node, writableNode) => billOfMaterials);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(2, deserializedNodes.Count);
        Assert.AreEqual(billOfMaterials, deserializedNodes.OfType<Circle>().FirstOrDefault()?.GetAnnotations()[0]);
    }
}
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
/// Tests for <see cref="IDeserializerHandler.UnresolvableAnnotation"/>
/// </summary>
[TestClass]
public class UnresolvableAnnotationTests
{
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    private class DeserializerHealingHandler(Func<ICompressedId, IWritableNode, IWritableNode?> heal)
        : DeserializerExceptionHandler
    {
        public override IWritableNode? UnresolvableAnnotation(ICompressedId annotationId, IReadableNode node) =>
            heal(annotationId, (IWritableNode)node);
    }


    [TestMethod]
    public void unresolvable_annotation_does_not_heal()
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
                    References = [],
                    Annotations = ["unresolvable-annotation"],
                }
            ]
        };

        var deserializerHealingHandler = new DeserializerHealingHandler((id, node) => null);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(1, deserializedNodes.Count);
    }

    [TestMethod]
    public void unresolvable_annotation_heals()
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Circle"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = ["unresolvable-annotation"],
                }
            ]
        };

        var documentation = new Documentation("new-annotation") { Technical = true, Text = "this is a doc" };

        var deserializerHealingHandler = new DeserializerHealingHandler((id, node) => documentation);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk);
        Assert.AreEqual(1, deserializedNodes.Count);
        INode? annotation = deserializedNodes.OfType<Circle>().FirstOrDefault()?.GetAnnotations()[0];
        Assert.AreSame(documentation, annotation);
        Assert.AreSame(documentation.GetId(), annotation?.GetId());
    }

    [TestMethod]
    public void unresolvable_annotation_tries_to_heal_to_invalid_annotation()
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
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = ["unresolvable-annotation"],
                }
            ]
        };

        var documentation = new Documentation("invalid-annotation") { Technical = true, Text = "this is a doc" };
        var deserializerHealingHandler = new DeserializerHealingHandler((id, node) => documentation);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        Assert.ThrowsException<DeserializerException>(() => deserializer.Deserialize(serializationChunk));
    }
}
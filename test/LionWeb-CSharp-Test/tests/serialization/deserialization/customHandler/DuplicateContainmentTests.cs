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
/// Tests for <see cref="DeserializerExceptionHandler.DuplicateContainment"/>
/// </summary>
[TestClass]
public class DuplicateContainmentTests
{
    private class DeserializerHealingHandler(Func<IWritableNode, IWritableNode, IReadableNode, bool> heal)
        : DeserializerExceptionHandler
    {
        public override bool DuplicateContainment(IWritableNode containedNode, IWritableNode newParent,
            IReadableNode existingParent) =>
            heal(containedNode, newParent, existingParent);
    }

    [TestMethod]
    public void duplicate_containment_keeps_current_parent()
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
                    Id = "A",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment()
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-parts"), Children = ["B"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "B",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "A"
                },
                new SerializedNode
                {
                    Id = "C",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment()
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-parts"), Children = ["B"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
            ]
        };

        var deserializerHealingHandler = new DeserializerHealingHandler((node, writableNode, arg3) => false);
        List<IReadableNode> nodes = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        Assert.AreEqual(2, nodes.Count);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        Assert.AreEqual(1, a.Children().Count());

        INode c = nodes.OfType<INode>().Last();
        Assert.AreEqual("C", c.GetId());
        Assert.IsNull(c.GetParent());
        Assert.IsFalse(c.Children().Any());

        INode b = a.Children().First();
        Assert.AreEqual("B", b.GetId());
        Assert.AreSame(a, b.GetParent());
        Assert.IsFalse(b.Children().Any());
    }

    [TestMethod]
    public void duplicate_containment_gets_new_parent()
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
                    Id = "A",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment()
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-parts"), Children = ["B"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "B",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "A"
                },
                new SerializedNode
                {
                    Id = "C",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment()
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-parts"), Children = ["B"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = null
                },
            ]
        };

        var deserializerHealingHandler = new DeserializerHealingHandler((node, writableNode, arg3) => true);
        List<IReadableNode> nodes = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        Assert.AreEqual(2, nodes.Count);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        Assert.AreEqual(0, a.Children().Count());

        INode c = nodes.OfType<INode>().Last();
        Assert.AreEqual("C", c.GetId());
        Assert.IsNull(c.GetParent());
        Assert.IsTrue(c.Children().Any());

        INode b = c.Children().First();
        Assert.AreEqual("B", b.GetId());
        Assert.AreSame(c, b.GetParent());
        Assert.IsFalse(b.Children().Any());
    }
    
    [TestMethod]
    public void duplicate_annotation_keeps_current_parent()
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
                    Id = "A",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations =
                    [
                        "B"
                    ],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "B",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-BillOfMaterials"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "A"
                },
                new SerializedNode
                {
                    Id = "C",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations =
                    [
                        "B"
                    ],
                    Parent = null
                },
            ]
        };

        var deserializerHealingHandler = new DeserializerHealingHandler((node, writableNode, arg3) => false);
        List<IReadableNode> nodes = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        Assert.AreEqual(2, nodes.Count);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        Assert.AreEqual(1, a.GetAnnotations().Count);

        INode c = nodes.OfType<INode>().Last();
        Assert.AreEqual("C", c.GetId());
        Assert.IsNull(c.GetParent());
        Assert.IsFalse(c.GetAnnotations().Any());

        INode b = a.GetAnnotations()[0];
        Assert.AreEqual("B", b.GetId());
        Assert.AreSame(a, b.GetParent());
        Assert.IsFalse(b.GetAnnotations().Any());
    }    
    
    [TestMethod]
    public void duplicate_annotation_gets_new_parent()
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
                    Id = "A",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations =
                    [
                        "B"
                    ],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "B",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-BillOfMaterials"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "A"
                },
                new SerializedNode
                {
                    Id = "C",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-CompositeShape"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations =
                    [
                        "B"
                    ],
                    Parent = null
                },
            ]
        };

        var deserializerHealingHandler = new DeserializerHealingHandler((node, writableNode, arg3) => true);
        List<IReadableNode> nodes = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        Assert.AreEqual(2, nodes.Count);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        Assert.AreEqual(0, a.GetAnnotations().Count);

        INode c = nodes.OfType<INode>().Last();
        Assert.AreEqual("C", c.GetId());
        Assert.IsNull(c.GetParent());
        Assert.AreEqual(1, c.GetAnnotations().Count);

        INode b = c.GetAnnotations()[0];
        Assert.AreEqual("B", b.GetId());
        Assert.AreSame(c, b.GetParent());
        Assert.IsFalse(b.GetAnnotations().Any());
    }
}
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
using Examples.TinyRefLang;
using LionWeb.Core;
using LionWeb.Core.M1;
using LionWeb.Core.M2;
using LionWeb.Core.Serialization;

/// <summary>
/// Tests for <see cref="IDeserializerHandler.SkipDeserializingDependentNode"/>
/// </summary>
[TestClass]
public class SkipDeserializingDependentNodeTests
{
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    private class DeserializerHealingHandler(Func<CompressedId, bool> heal) : DeserializerExceptionHandler
    {
        public override bool SkipDeserializingDependentNode(CompressedId id) => heal(id);
    }


    [TestMethod]
    [Ignore(message: " are we going to look for containment and annotations in dependent nodes ? ")]
    public void skip_deserializing_dependent_containment_node()
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
                    Id = "bar",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-OffsetDuplicate"),
                    Properties = [],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer("key-Shapes", "1", "key-offset"),
                            Children = ["repeated-id"]
                        }
                    ],
                    References = [],
                    Annotations = [],
                    Parent = null
                },

                new SerializedNode
                {
                    Id = "repeated-id",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-Coord"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "bar"
                },
            ]
        };

        Coord coord = ShapesLanguage.Instance.GetFactory().NewCoord("repeated-id");

        var deserializerHealingHandler = new DeserializerHealingHandler(id => true);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .WithUncompressedIds(true)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk, [coord]);

        Assert.AreEqual(1, deserializedNodes.Count);
        OffsetDuplicate deserializedOffsetDuplicate =
            deserializedNodes.OfType<OffsetDuplicate>().First(n => n.GetId() == "bar");
        Assert.AreSame(coord, deserializedOffsetDuplicate.Offset);
    }

    [TestMethod]
    public void skip_deserializing_dependent_reference_node()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
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
                                new SerializedReferenceTarget { Reference = "repeated-id", ResolveInfo = "ref" },
                            ]
                        }
                    ],
                    Annotations = [],
                },

                new SerializedNode
                {
                    Id = "repeated-id",
                    Classifier = new MetaPointer("key-tinyRefLang", "0", "key-MyConcept"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                },
            ]
        };

        MyConcept myConcept = TinyRefLangLanguage.Instance.GetFactory().NewMyConcept("repeated-id");

        var deserializerHealingHandler = new DeserializerHealingHandler(id => true);
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(deserializerHealingHandler)
            .WithLanguage(TinyRefLangLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk, [myConcept]);

        Assert.AreEqual(1, deserializedNodes.Count);
        MyConcept deserializedMyConcept = deserializedNodes.OfType<MyConcept>().First(n => n.GetId() == "foo");
        Assert.AreSame(myConcept, deserializedMyConcept.SingularRef);
    }
}
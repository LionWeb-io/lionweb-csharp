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
using LionWeb.Core.Serialization;

public class SkipDeserializingDependentNodeTests
{
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
    [Ignore(message:"do we need to implement SkipDeserializingDependentNode for other nodes such as containment and annotations")]
    public void skip_deserializing_dependent_containment_node()
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
        
        Documentation documentation = ShapesLanguage.Instance.GetFactory().NewDocumentation("repeated-id");

        var skipDeserializingDependentNodeDeserializerHandler = new SkipDeserializingDependentNodeDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(skipDeserializingDependentNodeDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk, [documentation]);
        Assert.IsTrue(skipDeserializingDependentNodeDeserializerHandler.Called);

        OffsetDuplicate deserializedOffsetDuplicate = deserializedNodes.OfType<OffsetDuplicate>().First();
        Assert.AreSame(documentation, deserializedOffsetDuplicate.Docs);
    }

    [TestMethod]
    public void skip_deserializing_dependent_reference_node()
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
            
        var skipDeserializingDependentNodeDeserializerHandler = new SkipDeserializingDependentNodeDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(skipDeserializingDependentNodeDeserializerHandler)
            .WithLanguage(TinyRefLangLanguage.Instance)
            .Build();

        List<IReadableNode> deserializedNodes = deserializer.Deserialize(serializationChunk, [myConcept]);
        Assert.IsTrue(skipDeserializingDependentNodeDeserializerHandler.Called);

        MyConcept deserializedMyConcept = deserializedNodes.OfType<MyConcept>().First(n => n.GetId() == "foo");
        Assert.AreSame(myConcept, deserializedMyConcept.SingularRef);

    }
    
    #endregion
}
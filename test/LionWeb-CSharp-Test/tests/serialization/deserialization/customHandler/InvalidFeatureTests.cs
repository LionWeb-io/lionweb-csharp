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

[TestClass]
public class InvalidFeatureTests
{
    /// <summary>
    /// <see cref="IDeserializerHandler.InvalidFeature{TFeature}"/>
    /// </summary>

    #region invalid feature

    private class InvalidFeatureDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }

        public override Feature? InvalidFeature<TFeature>(CompressedMetaPointer feature, Classifier classifier,
            IWritableNode node)
        {
            Called = true;
            return null;
        }
    }

    [TestMethod]
    public void invalid_feature()
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
                    Containments = [],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer("key-Shapes", "1", "key-r"), Targets = []
                        }
                    ],
                    Annotations = [],
                },
            ]
        };

        var invalidFeatureDeserializerHandler = new InvalidFeatureDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(invalidFeatureDeserializerHandler)
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(invalidFeatureDeserializerHandler.Called);
    }

    #endregion
}
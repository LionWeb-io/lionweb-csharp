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

public class InvalidPropertyValueTests
{
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
                            Property = new MetaPointer("key-Shapes", "1", "key-x"), Value = "expects an integer"
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

}
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

using Examples.WithEnum.M2;
using LionWeb.Core;
using LionWeb.Core.M1;
using LionWeb.Core.M3;
using LionWeb.Core.Serialization;

[TestClass]
public class UnknownEnumerationLiteralTests
{
    /// <summary>
    /// <see cref="IDeserializerHandler.UnknownEnumerationLiteral"/>
    /// </summary>

    #region unknown enumeration literal

    private class UnknownEnumerationLiteralDeserializerHandler : DeserializerExceptionHandler
    {
        public bool Called { get; private set; }

        public override Enum? UnknownEnumerationLiteral(string key, Enumeration enumeration, Feature property,
            IWritableNode nodeId)
        {
            Called = true;
            return null;
        }
    }

    [TestMethod]
    public void unknown_enumeration_literal()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "WithEnum", Version = "1" }
            ],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("WithEnum", "1", "EnumHolder"),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer("WithEnum", "1", "enumValue"),
                            Value = "unknown-enumeration-literal"
                        }
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var unknownEnumerationLiteralDeserializerHandler = new UnknownEnumerationLiteralDeserializerHandler();
        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(unknownEnumerationLiteralDeserializerHandler)
            .WithLanguage(WithEnumLanguage.Instance)
            .Build();

        deserializer.Deserialize(serializationChunk);
        Assert.IsTrue(unknownEnumerationLiteralDeserializerHandler.Called);
    }

    #endregion
}
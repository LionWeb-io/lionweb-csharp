// Copyright 2025 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Test.Delta;

using Core.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using TargetNode = NodeId;
using CommandId = NodeId;
using QueryId = NodeId;
using FreeId = NodeId;
using MessageKind = NodeId;
using MessageDataKey = NodeId;

public abstract class JsonSerializationTestsBase
{
    private int _nextNodeId = 0;
    private int _nextKey = 0;
    private int _nextPropertyValue = 0;
    private int _nextResolveInfo = 0;
    private int _nextIndex = 0;

    protected void AssertSerialization(IDeltaContent input)
    {
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            TypeInfoResolver = new DeltaProtocolTypeResolver()
        };
        var serialized = JsonSerializer.Serialize(input, jsonSerializerOptions);
        var deserialized = JsonSerializer.Deserialize<IDeltaContent>(serialized, jsonSerializerOptions);

        Assert.AreEqual(input, deserialized);
    }

    private class DeltaProtocolTypeResolver : DefaultJsonTypeInfoResolver
    {
        private static readonly List<JsonDerivedType> _queries = [];
        private static readonly List<JsonDerivedType> _commands = [];
        private static readonly List<JsonDerivedType> _singleCommands = [];
        private static readonly List<JsonDerivedType> _events = [];
        private static readonly List<JsonDerivedType> _singleEvents = [];
        private static readonly List<JsonDerivedType> _all = [];

        static DeltaProtocolTypeResolver()
        {
            Fill(_queries, typeof(IDeltaQuery));
            Fill(_commands, typeof(IDeltaCommand));
            Fill(_singleCommands, typeof(ISingleDeltaCommand));
            Fill(_events, typeof(IDeltaEvent));
            Fill(_singleEvents, typeof(ISingleDeltaEvent));
            Fill(_all, typeof(IDeltaContent));
        }

        private static void Fill(List<JsonDerivedType> container, Type baseType) =>
            container.AddRange(GetDerivedTypes(baseType).Select(t => new JsonDerivedType(t, t.Name)));

        private static IEnumerable<Type> GetDerivedTypes(Type type) =>
            type.Assembly.GetTypes()
                .Where(t => type.IsAssignableFrom(t) && !t.IsAbstract);

        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

            var jsonPolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "messageKind",
                IgnoreUnrecognizedTypeDiscriminators = false,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization
            };

            if (jsonTypeInfo.Type == typeof(IDeltaContent))
            {
                foreach (var derivedType in _all)
                {
                    jsonPolymorphismOptions.DerivedTypes.Add(derivedType);
                }

                jsonTypeInfo.PolymorphismOptions = jsonPolymorphismOptions;
            } else if (jsonTypeInfo.Type == typeof(ISingleDeltaCommand))
            {
                foreach (var derivedType in _singleCommands)
                {
                    jsonPolymorphismOptions.DerivedTypes.Add(derivedType);
                }

                jsonTypeInfo.PolymorphismOptions = jsonPolymorphismOptions;
            } else if (jsonTypeInfo.Type == typeof(ISingleDeltaEvent))
            {
                foreach (var derivedType in _singleEvents)
                {
                    jsonPolymorphismOptions.DerivedTypes.Add(derivedType);
                }

                jsonTypeInfo.PolymorphismOptions = jsonPolymorphismOptions;
            }

            return jsonTypeInfo;
        }
    }

    protected TargetNode CreateTargetNode() =>
        (++_nextNodeId).ToString();

    protected DeltaSerializationChunk CreateChunk() =>
        new DeltaSerializationChunk([
            CreateNode(),
            CreateNode()
        ]);

    private SerializedNode CreateNode() =>
        new SerializedNode
        {
            Id = CreateTargetNode(),
            Classifier = CreateMetaPointer(),
            Properties = [],
            Containments = [],
            References = [],
            Annotations = []
        };

    protected MetaPointer CreateMetaPointer() =>
        new MetaPointer("myLang", "v0", CreateKey());

    private string CreateKey() =>
        (++_nextKey).ToString();

    protected ProtocolMessage CreateProtocolMessage() =>
        new ProtocolMessage("MyKind", "MyMessage",
            [new ProtocolMessageData("key0", "value0"), new ProtocolMessageData("key1", "value1")]
        );

    protected DeltaProperty CreateProperty() =>
        new DeltaProperty(CreateTargetNode(), CreateMetaPointer());

    protected String CreatePropertyValue() =>
        (++_nextPropertyValue).ToString();

    protected DeltaContainment CreateContainment() =>
        new DeltaContainment(CreateTargetNode(), CreateMetaPointer(), CreateIndex());

    protected DeltaAnnotation CreateAnnotation() =>
        new DeltaAnnotation(CreateTargetNode(), CreateIndex());

    protected DeltaReference CreateReference() =>
        new DeltaReference(CreateTargetNode(), CreateMetaPointer(), CreateIndex());

    protected SerializedReferenceTarget CreateTarget() =>
        new SerializedReferenceTarget() { Reference = CreateTargetNode(), ResolveInfo = CreateResolveInfo() };

    protected String CreateResolveInfo() =>
        $"resolve{++_nextResolveInfo}";

    protected Int32 CreateIndex() =>
        ++_nextIndex;
}
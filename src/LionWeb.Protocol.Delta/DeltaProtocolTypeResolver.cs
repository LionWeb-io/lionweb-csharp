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

namespace LionWeb.Protocol.Delta;

using Message;
using Message.Command;
using Message.Event;
using Message.Query;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

public class DeltaProtocolTypeResolver : DefaultJsonTypeInfoResolver
{
    private static readonly List<JsonDerivedType> _queries = [];
    private static readonly List<JsonDerivedType> _commands = [];
    // private static readonly List<JsonDerivedType> _singleCommands = [];
    private static readonly List<JsonDerivedType> _events = [];
    // private static readonly List<JsonDerivedType> _singleEvents = [];
    private static readonly List<JsonDerivedType> _all = [];

    static DeltaProtocolTypeResolver()
    {
        Fill(_queries, typeof(IDeltaQuery));
        Fill(_commands, typeof(IDeltaCommand));
        // Fill(_singleCommands, typeof(ISingleDeltaCommand));
        Fill(_events, typeof(IDeltaEvent));
        // Fill(_singleEvents, typeof(ISingleDeltaEvent));
        Fill(_all, typeof(IDeltaContent));
    }

    private static void Fill(List<JsonDerivedType> container, Type baseType) =>
        container.AddRange(GetDerivedTypes(baseType).Select(t => new JsonDerivedType(t, t.Name)));

    private static IEnumerable<Type> GetDerivedTypes(Type type) =>
        type.Assembly.GetTypes()
            .Where(t => type.IsAssignableFrom(t) && !t.IsAbstract);

    /// <inheritdoc />
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
            FillDerived(jsonPolymorphismOptions, _all);
            jsonTypeInfo.PolymorphismOptions = jsonPolymorphismOptions;
        } else if (jsonTypeInfo.Type == typeof(IDeltaCommand))
        {
            FillDerived(jsonPolymorphismOptions, _commands);
            jsonTypeInfo.PolymorphismOptions = jsonPolymorphismOptions;
            // } else if (jsonTypeInfo.Type == typeof(ISingleDeltaCommand))
            // {
            //     FillDerived(jsonPolymorphismOptions, _singleCommands);
            //     jsonTypeInfo.PolymorphismOptions = jsonPolymorphismOptions;
        } else if (jsonTypeInfo.Type == typeof(IDeltaEvent))
        {
            FillDerived(jsonPolymorphismOptions, _events);
            jsonTypeInfo.PolymorphismOptions = jsonPolymorphismOptions;
            // } else if (jsonTypeInfo.Type == typeof(ISingleDeltaEvent))
            // {
            //     FillDerived(jsonPolymorphismOptions, _singleEvents);
            //     jsonTypeInfo.PolymorphismOptions = jsonPolymorphismOptions;
        }

        return jsonTypeInfo;
    }

    private static void FillDerived(JsonPolymorphismOptions jsonPolymorphismOptions, IEnumerable<JsonDerivedType> derivedTypes)
    {
        foreach (var derivedType in derivedTypes)
        {
            jsonPolymorphismOptions.DerivedTypes.Add(derivedType);
        }
    }
}
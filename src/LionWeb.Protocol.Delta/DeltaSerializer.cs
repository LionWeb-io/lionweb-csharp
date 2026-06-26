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

using Core.M1;
using Message;
using Message.Command;
using Message.Event;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// <see cref="Serialize"/>s and <see cref="Deserialize"/>s between <see cref="IDeltaContent"/> and UTF-8 JSON strings. 
/// </summary>
public class DeltaSerializer
{
    /// <summary>
    /// Serialize <paramref name="content"/> to UTF-8 JSON string.
    /// </summary>
    public string Serialize(IDeltaContent content) =>
        JsonSerializer.Serialize(content, typeof(IDeltaContent), LionWebDeltaSerializerContext.Default);

    /// <summary>
    /// Deserialize UTF-8 <paramref name="json"/> string to <see cref="IDeltaContent"/>.
    /// </summary>
    /// <exception cref="DeserializerException">If no content was deserialized.</exception>
    public T Deserialize<T>(string json) where T : class, IDeltaContent =>
        JsonSerializer.Deserialize(json, typeof(T), LionWebDeltaSerializerContext.Default) as T ?? throw new DeserializerException("deserialization yielded no content");
}

/// Source generator for efficient, AOT-optimizable JSON (de)serialization of LionWeb delta messages.
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(IDeltaContent))]
[JsonSerializable(typeof(IDeltaCommand))]
[JsonSerializable(typeof(IDeltaEvent))]
public partial class LionWebDeltaSerializerContext : JsonSerializerContext;
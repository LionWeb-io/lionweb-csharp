// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

namespace LionWeb.Core.Serialization;

using M1;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Stream;

/// <summary>
/// Utility methods for working with JSON.
/// </summary>
public static class JsonUtils
{
    // TODO  write and read JSON as UTF-8
    private static readonly JsonSerializerOptions _readOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Parses the given string as JSON.
    /// </summary>
    public static T ReadJsonFromString<T>(string json) =>
        JsonSerializer.Deserialize<T>(json, _readOptions)!;

    private static readonly JsonSerializerOptions _writeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true
    };

    /// <summary>
    /// Returns the given data rendered as JSON.
    /// </summary>
    public static string WriteJsonToString(object data) =>
        JsonSerializer.Serialize(data, _writeOptions);

    /// <summary>
    /// Writes the given data to a file with the given path.
    /// </summary>
    public static void WriteJsonToFile(string path, object data) =>
        File.WriteAllText(path, WriteJsonToString(data));

    public static void WriteNodesToStream(Stream stream, ISerializer serializer, IEnumerable<INode> nodes)
    {
        object data = new LazySerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Nodes = serializer.Serialize(nodes),
            Languages = serializer.UsedLanguages
        };
        JsonSerializer.Serialize(stream, data, _writeOptions);
    }

    public static async Task<List<IReadableNode>> ReadNodesFromStream(Stream stream, IDeserializer deserializer)
    {
        var streamReader = new Utf8JsonAsyncStreamReader(stream, leaveOpen: true);

        bool insideNodes = false;
        while (await Advance())
        {
            switch (streamReader.TokenType)
            {
                case JsonTokenType.PropertyName when streamReader.GetString() == "serializationFormatVersion":
                    await Advance();
                    string? version = streamReader.GetString();
                    if (version != ReleaseVersion.Current)
                    {
                    }

                    break;

                case JsonTokenType.PropertyName when streamReader.GetString() == "nodes":
                    insideNodes = true;
                    break;

                case JsonTokenType.PropertyName when streamReader.GetString() != "nodes":
                    insideNodes = false;
                    break;

                case JsonTokenType.StartObject when insideNodes:
                    var serializedNode = await streamReader.DeserializeAsync<SerializedNode>(_readOptions);
                    if (serializedNode != null)
                        deserializer.Process(serializedNode);

                    break;
            }
        }

        return deserializer.Finish().ToList();

        async Task<bool> Advance() => await streamReader.ReadAsync();
    }
}

/// <inheritdoc cref="SerializationChunk"/>
internal class LazySerializationChunk
{
    /// <inheritdoc cref="SerializationChunk.SerializationFormatVersion"/>
    [JsonPropertyOrder(0)]
    public string SerializationFormatVersion { get; init; }

    /// <inheritdoc cref="SerializationChunk.Nodes"/>
    [JsonPropertyOrder(1)]
    public IEnumerable<SerializedNode> Nodes { get; init; }

    /// <inheritdoc cref="SerializationChunk.Languages"/>
    [JsonPropertyOrder(2)]
    public IEnumerable<SerializedLanguageReference> Languages { get; init; }
}
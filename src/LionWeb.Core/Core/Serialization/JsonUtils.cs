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
    private static readonly JsonSerializerOptions _readOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// Parses <paramref name="json"/> as JSON.
    public static T ReadJsonFromString<T>(string json) =>
        JsonSerializer.Deserialize<T>(json, _readOptions)!;

    private static readonly JsonSerializerOptions _writeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// Returns <paramref name="data"/> rendered as JSON.
    public static string WriteJsonToString(object data) =>
        JsonSerializer.Serialize(data, _writeOptions);

    /// Writes <paramref name="data"/> to a file at <paramref name="path"/>.
    public static void WriteJsonToFile(string path, object data) =>
        File.WriteAllText(path, WriteJsonToString(data));

    /// <summary>
    /// Uses <paramref name="serializer"/> to write <paramref name="nodes"/> to <paramref name="utf8JsonStream"/> efficiently.
    /// </summary>
    /// <param name="utf8JsonStream">Stream to serialize <paramref name="nodes"/> to.
    /// <i>Only</i> includes these nodes, no descendants etc.</param>
    /// <param name="serializer">Serializer to use.</param>
    /// <param name="nodes">Nodes to serialize.</param>
    public static void WriteNodesToStream(Stream utf8JsonStream, ISerializer serializer, IEnumerable<IReadableNode> nodes)
    {
        object data = new LazySerializationChunk
        {
            SerializationFormatVersion = serializer.LionWebVersion.VersionString,
            Nodes = serializer.Serialize(nodes),
            Languages = serializer.UsedLanguages
        };
        JsonSerializer.Serialize(utf8JsonStream, data, _writeOptions);
    }

    /// <summary>
    /// Uses <paramref name="serializer"/> to write <paramref name="nodes"/> to <paramref name="utf8JsonStream"/> efficiently.
    /// </summary>
    /// <param name="utf8JsonStream">Stream to serialize <paramref name="nodes"/> to.
    /// <i>Only</i> includes these nodes, no descendants etc.</param>
    /// <param name="serializer">Serializer to use.</param>
    /// <param name="nodes">Nodes to serialize.</param>
    public static async Task WriteNodesToStreamAsync(Stream utf8JsonStream, ISerializer serializer, IEnumerable<IReadableNode> nodes)
    {
        object data = new LazySerializationChunk
        {
            SerializationFormatVersion = serializer.LionWebVersion.VersionString,
            Nodes = serializer.Serialize(nodes),
            Languages = serializer.UsedLanguages
        };
        await JsonSerializer.SerializeAsync(utf8JsonStream, data, _writeOptions);
    }

    /// <summary>
    /// Uses <paramref name="deserializer"/> to read nodes from <paramref name="utf8JsonStream"/>.  
    /// </summary>
    /// <param name="utf8JsonStream">Stream to read from.</param>
    /// <param name="deserializer">Deserializer to use.</param>
    /// <param name="lionWebVersionChecker">Optional action to check the <see cref="SerializationChunk.SerializationFormatVersion"/>.
    /// If <c>null</c>, we use <see cref="LionWebVersionsExtensions.AssureCompatible(LionWeb.Core.LionWebVersions,string,string?)"/>.</param>
    /// <returns>Nodes as returned from <see cref="IDeserializer.Finish"/>.</returns>
    public static async Task<List<IReadableNode>> ReadNodesFromStreamAsync(Stream utf8JsonStream, IDeserializer deserializer,
        Action<string>? lionWebVersionChecker = null)
    {
        var streamReader = new Utf8JsonAsyncStreamReader(utf8JsonStream, leaveOpen: true);

        bool insideNodes = false;
        while (await Advance())
        {
            switch (streamReader.TokenType)
            {
                case JsonTokenType.PropertyName when streamReader.GetString() == "serializationFormatVersion":
                    await Advance();
                    string? version = streamReader.GetString();
                    if (version != null)
                    {
                        if (lionWebVersionChecker != null)
                        {
                            lionWebVersionChecker(version);
                        } else
                        {
                            deserializer.LionWebVersion.AssureCompatible(version);
                        }
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

/// Variant of <see cref="SerializationChunk"/> that moves <see cref="Languages"/> as last entry,
/// so we can fill it _after_ we've processed all <see cref="Nodes"/>.
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
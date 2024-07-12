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

    /// <summary>
    /// Parses the contents of the file with the given path as JSON. 
    /// </summary>
    public static T ReadJsonFromFile<T>(string path) =>
        ReadJsonFromString<T>(File.ReadAllText(path));

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

    /// <summary>
    /// Writes the given <paramref name="data"/> to a <paramref name="stream"/>.
    /// </summary>
    public static void WriteJsonToStream(Stream stream, object data) =>
        JsonSerializer.Serialize(stream, data, _writeOptions);

    public static void WriteNodesToStream(Stream stream, ISerializer serializer, IEnumerable<INode> nodes)
    {
        WriteJsonToStream(stream,
            new LazySerializationChunk
            {
                SerializationFormatVersion = ReleaseVersion.Current,
                Nodes = serializer.SerializeToNodes(nodes),
                Languages = serializer.UsedLanguages
            });
    }
}

class LazySerializationChunk
{
    [JsonPropertyOrder(0)] public string SerializationFormatVersion { get; init; }

    [JsonPropertyOrder(1)] public IEnumerable<SerializedNode> Nodes { get; init; }

    [JsonPropertyOrder(2)] public IEnumerable<SerializedLanguageReference> Languages { get; init; }
}
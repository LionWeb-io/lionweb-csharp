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

namespace LionWeb.Core.Serialization;

using M1;
using System.Text.Json;
using System.Text.Json.Stream;

/// Partially successful attempt to share logic for synchronous and <c>async</c>chronous deserialization.
/// Not supposed to be used directly, but through <see cref="JsonUtils.ReadNodesFromStream"/> or <see cref="JsonUtils.ReadNodesFromStreamAsync"/>. 
internal abstract class StreamReaderBase(IDeserializer deserializer)
{
    protected readonly IDeserializer Deserializer = deserializer;

    public Action<string>? LionWebVersionChecker { get; init; }

    public Func<List<SerializedLanguageReference>, bool>? LanguageReferencesChecker { get; init; }

    public required JsonSerializerOptions JsonSerializerOptions { get; init; }

    protected bool IsInsideNodes(string? stringFromReader) => stringFromReader == "nodes";

    protected void Process(SerializedNode? serializedNode)
    {
        if (serializedNode != null)
            Deserializer.Process(serializedNode);
    }

    protected bool IsInsideLanguages(string? stringFromReader) => stringFromReader == "languages";

    protected bool IsSerializationFormatVersion(string? stringFromReader) =>
        stringFromReader == "serializationFormatVersion";

    protected void CheckSerializationFormatVersion(string? stringFromReader)
    {
        if (stringFromReader == null)
            return;

        if (LionWebVersionChecker == null)
            Deserializer.LionWebVersion.AssureCompatible(stringFromReader);
        else
            LionWebVersionChecker(stringFromReader);
    }
}

internal sealed class StreamReaderAsync(Stream utf8JsonStream, IDeserializer deserializer)
    : StreamReaderBase(deserializer)
{
    private readonly Utf8JsonAsyncStreamReader _streamReader = new(utf8JsonStream, leaveOpen: true);

    public async Task<List<IReadableNode>> ReadNodes()
    {
        var insideNodes = false;
        var insideLanguages = false;
        List<SerializedLanguageReference> languageReferences = [];

        while (await Advance())
        {
            switch (_streamReader.TokenType, insideNodes, insideLanguages)
            {
                case (JsonTokenType.PropertyName, _, _) when IsSerializationFormatVersion(_streamReader.GetString()):
                    await Advance();
                    CheckSerializationFormatVersion(_streamReader.GetString());
                    break;

                case (JsonTokenType.PropertyName, false, _) when IsInsideNodes(_streamReader.GetString()):
                    insideNodes = true;
                    insideLanguages = false;
                    break;

                case (JsonTokenType.PropertyName, _, false) when IsInsideLanguages(_streamReader.GetString()):
                    insideLanguages = true;
                    insideNodes = false;
                    break;

                case (JsonTokenType.PropertyName, true, _):
                    insideNodes = false;
                    break;

                case (JsonTokenType.PropertyName, _, true):
                    insideLanguages = false;
                    break;

                case (JsonTokenType.StartObject, true, false):
                    var serializedNode = await _streamReader.DeserializeAsync<SerializedNode>(JsonSerializerOptions);
                    Process(serializedNode);
                    break;

                case (JsonTokenType.StartObject, false, true) when LanguageReferencesChecker is not null:
                    var languageReference = await _streamReader.DeserializeAsync<SerializedLanguageReference>(JsonSerializerOptions);
                    if (languageReference is not null)
                        languageReferences.Add(languageReference);
                    break;

                case (JsonTokenType.EndArray, false, true) when LanguageReferencesChecker is not null:
                    if (!LanguageReferencesChecker(languageReferences))
                        return [];
                    break;
            }
        }

        return Deserializer.Finish().ToList();
    }

    private async Task<bool> Advance() => await _streamReader.ReadAsync();
}

internal sealed class StreamReaderSync(Stream utf8JsonStream, IDeserializer deserializer)
    : StreamReaderBase(deserializer)
{
    private const int _bufferSize = 1024 * 8;

    public List<IReadableNode> ReadNodes()
    {
        using var jsonStreamReader = new Utf8JsonStreamReader(utf8JsonStream, _bufferSize);

        var insideNodes = false;
        var insideLanguages = false;
        List<SerializedLanguageReference> languageReferences = [];

        while (jsonStreamReader.Read())
        {
            switch (jsonStreamReader.TokenType, insideNodes, insideLanguages)
            {
                case (JsonTokenType.PropertyName, _, _) when IsSerializationFormatVersion(jsonStreamReader.GetString()):
                    jsonStreamReader.Read();
                    CheckSerializationFormatVersion(jsonStreamReader.GetString());
                    break;

                case (JsonTokenType.PropertyName, false, _) when IsInsideNodes(jsonStreamReader.GetString()):
                    insideNodes = true;
                    insideLanguages = false;
                    break;

                case (JsonTokenType.PropertyName, _, false) when IsInsideLanguages(jsonStreamReader.GetString()):
                    insideLanguages = true;
                    insideNodes = false;
                    break;

                case (JsonTokenType.PropertyName, true, _):
                    insideNodes = false;
                    break;

                case (JsonTokenType.PropertyName, _, true):
                    insideLanguages = false;
                    break;

                case (JsonTokenType.StartObject, true, false):
                    var serializedNode = jsonStreamReader.Deserialize<SerializedNode>(JsonSerializerOptions);
                    Process(serializedNode);
                    break;

                case (JsonTokenType.StartObject, false, true) when LanguageReferencesChecker is not null:
                    var languageReference = jsonStreamReader.Deserialize<SerializedLanguageReference>(JsonSerializerOptions);
                    if (languageReference is not null)
                        languageReferences.Add(languageReference);
                    break;

                case (JsonTokenType.EndArray, false, true) when LanguageReferencesChecker is not null:
                    if (!LanguageReferencesChecker(languageReferences))
                        return [];
                    break;
            }
        }

        return Deserializer.Finish().ToList();
    }
}
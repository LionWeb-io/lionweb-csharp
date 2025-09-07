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

using System.Text.Json;
using System.Text.Json.Serialization;

/// Source generator for efficient, AOT-optimizable JSON (de)serialization of LionWeb chunks.
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,Converters = [typeof(InternedStringConverter)])]
[JsonSerializable(typeof(SerializationChunk))]
[JsonSerializable(typeof(LazySerializationChunk))]
public partial class LionWebJsonSerializerContext : JsonSerializerContext;

public class InternedStringConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString();
        return string.IsInterned(s) ?? s;
    }
    

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        if (value != null)
            writer.WriteStringValue(value.AsSpan());
        else
            writer.WriteNullValue();
    }
}

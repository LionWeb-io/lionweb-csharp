// Copyright 2026 TRUMPF Laser SE and other contributors
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

public sealed class NullableArrayConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        var underlyingType = Nullable.GetUnderlyingType(typeToConvert);
        var baseType = underlyingType ?? typeToConvert;
        if (!baseType.IsArray)
            return false;
        var elementType = baseType.GetElementType()!;

        return 
            elementType == typeof(SerializedProperty) ||
            elementType == typeof(SerializedContainment) ||
            elementType == typeof(SerializedReference) ||
            elementType == typeof(SerializedReferenceTarget) ||
            elementType == typeof(string);
    }

    /// <inheritdoc />
    public override JsonConverter CreateConverter(
        Type type,
        JsonSerializerOptions options)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);
        var baseType = underlyingType ?? type;
        var elementType = baseType.GetElementType()!;

        return elementType switch
        {
            _ when elementType == typeof(SerializedProperty) => new NullableArrayConverter<SerializedProperty>(
                (JsonConverter<SerializedProperty>)LionWebJsonSerializerContext.Default.SerializedProperty.Converter),
            _ when elementType == typeof(SerializedContainment) => new NullableArrayConverter<SerializedContainment>(
                (JsonConverter<SerializedContainment>)LionWebJsonSerializerContext.Default.SerializedContainment.Converter),
            _ when elementType == typeof(SerializedReference) => new NullableArrayConverter<SerializedReference>(
                (JsonConverter<SerializedReference>)LionWebJsonSerializerContext.Default.SerializedReference.Converter),
            _ when elementType == typeof(SerializedReferenceTarget) => new NullableArrayConverter<SerializedReferenceTarget>(
                (JsonConverter<SerializedReferenceTarget>)LionWebJsonSerializerContext.Default.SerializedReferenceTarget.Converter),
            _ when elementType == typeof(string) => new NullableArrayConverter<string>(
                (JsonConverter<string>)LionWebJsonSerializerContext.Default.String.Converter),
            _ => throw new JsonException()
        };
    }

    private sealed class NullableArrayConverter<T>(JsonConverter<T> baseConverter) : JsonConverter<T[]?>
    {
        /// <inheritdoc />
        public override bool HandleNull => true;

        /// <inheritdoc />
        public override T[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.StartArray:
                    List<T>? list = null;
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                            break;
                        T? read = baseConverter.Read(ref reader, typeof(T), options);
                        if (read is not null)
                        {
                            if (list is not null)
                                list.Add(read);
                            else
                                list = [read];
                        }
                    }

                    return list?.ToArray();
                default:
                    throw new JsonException();
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, T[]? value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            if (value is not null)
            {
                foreach (T v in value)
                {
                    baseConverter.Write(writer, v, options);
                }
            }

            writer.WriteEndArray();
        }
    }
}
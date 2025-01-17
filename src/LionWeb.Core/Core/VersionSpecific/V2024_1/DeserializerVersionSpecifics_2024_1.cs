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

// ReSharper disable InconsistentNaming

namespace LionWeb.Core.VersionSpecific.V2024_1;

using M1;
using M2;
using M3;
using System.Text.Json;
using System.Text.Json.Nodes;

/// <see cref="IDeserializer"/> parts specific to LionWeb <see cref="IVersion2023_1"/>.  
internal class DeserializerVersionSpecifics_2024_1<T, H>(
    DeserializerBase<T, H> deserializer,
    DeserializerMetaInfo metaInfo,
    IDeserializerHandler handler)
    : DeserializerVersionSpecificsBase<T, H>(deserializer, metaInfo, handler)
    where T : class, IReadableNode where H : class, IDeserializerHandler
{
    public override LionWebVersions Version => LionWebVersions.v2024_1;

    public override void RegisterBuiltins() =>
        RegisterLanguage(BuiltInsLanguage_2024_1.Instance);

    public override object? ConvertDatatype(IWritableNode node, Feature property,
        LanguageEntity datatype, string? value)
    {
        var convertedValue = (datatype, value) switch
        {
            (_, null) => null,
            (PrimitiveType p, { } v) => ConvertPrimitiveType(node, property, p, v),
            (Enumeration enumeration, { } v) => ConvertEnumeration(node, property, enumeration, v),
            (StructuredDataType sdt, { } v) => ConvertStructuredDataType(node, property, sdt, v),
            var (_, v) => _handler.UnknownDatatype(v, datatype, property, node)
        };
        return convertedValue;
    }

    protected virtual object? ConvertPrimitiveType(IWritableNode node, Feature property, PrimitiveType datatype,
        string value)
    {
        ICompressedId compressedId = _metaInfo.Compress(node.GetId());
        return datatype switch
        {
            var b when b == BuiltInsLanguage_2024_1.Instance.Boolean => bool.TryParse(value, out var result)
                ? result
                : _handler.InvalidPropertyValue<bool>(value, property, compressedId),
            var i when i == BuiltInsLanguage_2024_1.Instance.Integer => int.TryParse(value, out var result)
                ? result
                : _handler.InvalidPropertyValue<int>(value, property, compressedId),
            // leave a String value as a string:
            var s when s == BuiltInsLanguage_2024_1.Instance.String => value,
            _ => _handler.UnknownDatatype(value, datatype, property, node)
        };
    }

    private IStructuredDataTypeInstance? ConvertStructuredDataType(IWritableNode node, Feature property,
        StructuredDataType sdt, string s)
    {
        try
        {
            JsonObject? jsonObject = JsonSerializer.Deserialize<JsonObject>(s);
            if (jsonObject == null)
                return _handler.UnknownDatatype(s, sdt, property, node) as IStructuredDataTypeInstance;
            return ConvertStructuredDataType(node, property, sdt, jsonObject);
        } catch (Exception e) when (e is JsonException or NotSupportedException)
        {
            return _handler.UnknownDatatype(s, sdt, property, node) as IStructuredDataTypeInstance;
        }
    }

    private IStructuredDataTypeInstance? ConvertStructuredDataType(IWritableNode node, Feature property,
        StructuredDataType sdt, JsonObject jsonObject)
    {
        var fieldValues = new FieldValues();

        foreach ((var key, JsonNode? jsonNode) in jsonObject)
        {
            var field = sdt.Fields.FirstOrDefault(f => f.Key == key);
            if (field == null)
            {
                field = _handler.UnknownField(key, sdt, property, node);
                if (field == null)
                    continue;
            }

            var value = (field.Type, jsonNode) switch
            {
                (_, null) => null,
                (not StructuredDataType, JsonValue j) when j.GetValueKind() == JsonValueKind.String =>
                    ConvertDatatype(node, property, field.Type, j.GetValue<string>()),
                (StructuredDataType s, JsonObject o) => ConvertStructuredDataType(node, property, s, o),
                _ => _handler.UnknownDatatype(jsonNode.ToString(), sdt, property, node)
            };

            fieldValues.Add(field, value);
        }

        if (_metaInfo.LookupFactory(property.GetLanguage(), out var factory))
        {
            return factory.CreateStructuredDataTypeInstance(sdt, fieldValues);
        }

        return _handler.UnknownDatatype(jsonObject.ToString(), sdt, property, node) as
            IStructuredDataTypeInstance;
    }
}
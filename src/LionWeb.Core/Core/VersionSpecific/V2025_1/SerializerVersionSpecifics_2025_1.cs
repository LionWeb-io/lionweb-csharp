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

namespace LionWeb.Core.VersionSpecific.V2025_1;

using M1;
using M2;
using M3;
using System.Text.Json.Nodes;

internal class SerializerVersionSpecifics_2025_1 : SerializerVersionSpecificsBase
{
    public override LionWebVersions Version => LionWebVersions.v2025_1;

    public override string? ConvertDatatype(IReadableNode node, Feature property, object? value) =>
        value switch
        {
            null => null,
            Enum e => ConvertEnumeration(e),
            int or bool or string => ConvertPrimitiveType(value),
            IStructuredDataTypeInstance s => ConvertStructuredDataType(node, property, s),
            _ => _handler?.UnknownDatatype(node, property, value)
        };

    private string ConvertStructuredDataType(IReadableNode node, Feature feature, IStructuredDataTypeInstance sdt) =>
        SerializeStructuredDataType(node, feature, sdt).ToJsonString();

    private JsonObject SerializeStructuredDataType(IReadableNode node, Feature feature, IStructuredDataTypeInstance sdt)
    {
        _serializer?.RegisterUsedLanguage(sdt.GetStructuredDataType().GetLanguage());

        return new JsonObject(
            sdt
                .CollectAllSetFields()
                .Select(f => SerializeField(node, feature, sdt, f))
        );
    }

    private KeyValuePair<string, JsonNode?> SerializeField(IReadableNode node, Feature feature,
        IStructuredDataTypeInstance sdt, Field field)
    {
        var key = field.Key;
        JsonNode? value = sdt.Get(field) switch
        {
            null => JsonValue.Create((string?)null),
            IStructuredDataTypeInstance s => SerializeStructuredDataType(node, feature, s),
            var v => JsonValue.Create(ConvertDatatype(node, feature, v))
        };
        return KeyValuePair.Create(key, value);
    }
}
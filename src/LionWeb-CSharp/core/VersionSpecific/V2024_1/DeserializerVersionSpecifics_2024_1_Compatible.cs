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
using M3;
using V2023_1;

/// <see cref="IDeserializer"/> parts specific to LionWeb <see cref="IVersion2024_1_Compatible"/>.  
internal class DeserializerVersionSpecifics_2024_1_Compatible<T, H>(
    DeserializerBase<T, H> deserializer,
    DeserializerMetaInfo metaInfo,
    IDeserializerHandler handler)
    : DeserializerVersionSpecifics_2024_1<T, H>(deserializer, metaInfo, handler)
    where T : class, IReadableNode where H : class, IDeserializerHandler
{
    public override LionWebVersions Version => LionWebVersions.v2024_1_Compatible;

    protected override object? ConvertPrimitiveType(IWritableNode node, Feature property, PrimitiveType datatype, string value)
    {
        CompressedId compressedId = _metaInfo.Compress(node.GetId());
        return datatype switch
        {
            var b when b == BuiltInsLanguage_2023_1.Instance.Boolean || b == BuiltInsLanguage_2024_1.Instance.Boolean =>
                bool.TryParse(value, out var result)
                    ? result
                    : _handler.InvalidPropertyValue<bool>(value, property, compressedId),
            var i when i == BuiltInsLanguage_2023_1.Instance.Integer || i == BuiltInsLanguage_2024_1.Instance.Integer =>
                int.TryParse(value, out var result)
                    ? result
                    : _handler.InvalidPropertyValue<int>(value, property, compressedId),
            // leave a String value as a string:
            var s when s == BuiltInsLanguage_2023_1.Instance.String || s == BuiltInsLanguage_2024_1.Instance.String =>
                value,
            _ => _handler.UnknownDatatype(value, datatype, property, node)
        };
    }

    public override void RegisterBuiltins()
    {
        RegisterLanguage(BuiltInsLanguage_2023_1.Instance);
        RegisterLanguage(BuiltInsLanguage_2024_1.Instance);
    }
}
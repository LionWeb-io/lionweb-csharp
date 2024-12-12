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

namespace LionWeb.Core.M1;

using M2;
using M3;

public interface IDeserializerVersionSpecifics
{
    public static IDeserializerVersionSpecifics Create(LionWebVersions lionWebVersion) =>lionWebVersion switch
    {
        LionWebVersions.IVersion2023_1 => new DeserializerVersionSpecifics_2023_1(),
        LionWebVersions.IVersion2024_1 => new DeserializerVersionSpecifics_2024_1(),
        _ => throw new UnsupportedVersionException(lionWebVersion)
    };
    
    LionWebVersions Version { get; }

    object? ConvertPrimitiveType<T>(DeserializerBase<T> self, T node, Property property, string value) where T : IReadableNode;
}

class DeserializerVersionSpecifics_2023_1 : IDeserializerVersionSpecifics
{
    public LionWebVersions Version => LionWebVersions.v2023_1;

    public object? ConvertPrimitiveType<T>(DeserializerBase<T> self, T node,
        Property property, string value) where T : IReadableNode
    {
        CompressedId compressedId = self.Compress(node.GetId());
        return property.Type switch
        {
            var b when b == BuiltInsLanguage_2023_1.Instance.Boolean => bool.TryParse(value, out var result)
                ? result
                : self.Handler.InvalidPropertyValue<bool>(value, property, compressedId),
            var i when i == BuiltInsLanguage_2023_1.Instance.Integer => int.TryParse(value, out var result)
                ? result
                : self.Handler.InvalidPropertyValue<int>(value, property, compressedId),
            // leave both a String and JSON value as a string:
            var s when s == BuiltInsLanguage_2023_1.Instance.String ||
                       s == BuiltInsLanguage_2023_1.Instance.Json => value,
            _ => self.Handler.UnknownDatatype(property, value, node)
        };
    }
}

class DeserializerVersionSpecifics_2024_1 : IDeserializerVersionSpecifics
{
    public LionWebVersions Version => LionWebVersions.v2024_1;

    public object? ConvertPrimitiveType<T>(DeserializerBase<T> self, T node,
        Property property, string value) where T : IReadableNode
    {
        CompressedId compressedId = self.Compress(node.GetId());
        return property.Type switch
        {
            var b when b == BuiltInsLanguage_2024_1.Instance.Boolean => bool.TryParse(value, out var result)
                ? result
                : self.Handler.InvalidPropertyValue<bool>(value, property, compressedId),
            var i when i == BuiltInsLanguage_2024_1.Instance.Integer => int.TryParse(value, out var result)
                ? result
                : self.Handler.InvalidPropertyValue<int>(value, property, compressedId),
            // leave a String value as a string:
            var s when s == BuiltInsLanguage_2024_1.Instance.String => value,
            _ => self.Handler.UnknownDatatype(property, value, node)
        };
    }
}
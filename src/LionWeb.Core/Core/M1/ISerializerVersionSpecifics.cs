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
using Serialization;
using VersionSpecific.V2023_1;
using VersionSpecific.V2024_1_Compatible;
using VersionSpecific.V2024_1;
using VersionSpecific.V2025_1_Compatible;
using VersionSpecific.V2025_1;

/// Externalized logic of <see cref="ISerializer"/>, specific to one version of LionWeb standard.
internal interface ISerializerVersionSpecifics : IVersionSpecifics
{
    /// <summary>
    /// Creates an instance of <see cref="ISerializerVersionSpecifics"/> that implements <paramref name="lionWebVersion"/>.
    /// </summary>
    /// <exception cref="UnsupportedVersionException"></exception>
    public static ISerializerVersionSpecifics Create(LionWebVersions lionWebVersion) => lionWebVersion switch
    {
        IVersion2023_1 => new SerializerVersionSpecifics_2023_1(),
        IVersion2024_1 => new SerializerVersionSpecifics_2024_1(),
        IVersion2024_1_Compatible => new SerializerVersionSpecifics_2024_1_Compatible(),
        IVersion2025_1 => new SerializerVersionSpecifics_2025_1(),
        IVersion2025_1_Compatible => new SerializerVersionSpecifics_2025_1_Compatible(),
        _ => throw new UnsupportedVersionException(lionWebVersion)
    };

    /// Initializes this VersionSpecifics.
    void Initialize(Serializer serializer, ISerializerHandler handler);

    /// Converts <paramref name="value"/> from <paramref name="node"/>'s <paramref name="property"/> to a <see cref="SerializeProperty"/>.
    SerializedProperty SerializeProperty(IReadableNode node, Feature property, object? value);
    
    /// Converts <paramref name="value"/> from <paramref name="node"/>'s <paramref name="property"/> to the string representation as used in a <see cref="SerializeProperty"/>.
    public PropertyValue? ConvertDatatype(IReadableNode node, Feature property, object? value);
}

internal abstract class SerializerVersionSpecificsBase : ISerializerVersionSpecifics
{
    protected Serializer? _serializer;
    protected ISerializerHandler? _handler;

    public abstract LionWebVersions Version { get; }

    public void Initialize(Serializer serializer, ISerializerHandler handler)
    {
        _serializer = serializer;
        _handler = handler;
    }

    public SerializedProperty SerializeProperty(IReadableNode node, Feature property, object? value) =>
        new SerializedProperty { Property = property.ToMetaPointer(), Value = ConvertDatatype(node, property, value) };

    public abstract PropertyValue? ConvertDatatype(IReadableNode node, Feature property, object? value);

    protected PropertyValue? ConvertEnumeration(Enum e)
    {
        var lionCoreMetaPointer = AttributeExtensions.GetAttributeOfType<LionCoreMetaPointer>(e);

        if (lionCoreMetaPointer == null)
            return null;

        var languageInstanceField = lionCoreMetaPointer.Language.GetField("Instance");
        if (languageInstanceField != null)
        {
            var languageInstance = languageInstanceField.GetValue(null);
            if (languageInstance is Language language)
            {
                _serializer?.RegisterUsedLanguage(language);
            }
        }

        return lionCoreMetaPointer.Key;
    }

    /// <summary>
    /// Serializes the given <paramref name="value">runtime value</paramref> as a string,
    /// conforming to the LionWeb JSON serialization format.
    /// </summary>
    protected PropertyValue? ConvertPrimitiveType(object value) => value switch
    {
        null => null,
        bool boolean => boolean ? "true" : "false",
        string @string => @string,
        _ => value.ToString()
    };
}
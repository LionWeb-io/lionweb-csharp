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
using VersionSpecific.V2023_1;
using VersionSpecific.V2024_1_Compatible;
using VersionSpecific.V2024_1;

/// Externalized logic of <see cref="IDeserializer"/>, specific to one version of LionWeb standard.
public interface IDeserializerVersionSpecifics : IVersionSpecifics
{
    /// <summary>
    /// Creates an instance of <see cref="IDeserializerVersionSpecifics"/> that implements <paramref name="lionWebVersion"/>.
    /// </summary>
    /// <exception cref="UnsupportedVersionException"></exception>
    public static IDeserializerVersionSpecifics Create<T, H>(LionWebVersions lionWebVersion, DeserializerBase<T, H> deserializer, DeserializerMetaInfo metaInfo, IDeserializerHandler handler) where T : class, IReadableNode where H : class, IDeserializerHandler => lionWebVersion switch
    {
        IVersion2023_1 => new DeserializerVersionSpecifics_2023_1<T, H>(deserializer,metaInfo,handler),
        IVersion2024_1 => new DeserializerVersionSpecifics_2024_1<T, H>(deserializer,metaInfo,handler),
        IVersion2024_1_Compatible => new DeserializerVersionSpecifics_2024_1_Compatible<T, H>(deserializer,metaInfo,handler),
        _ => throw new UnsupportedVersionException(lionWebVersion)
    };

    /// Registers all relevant builtins. 
    void RegisterBuiltins();

    /// Converts the low-level string representation <paramref name="value"/> of <paramref name="property"/> into the internal LionWeb-C# representation.
    /// <seealso cref="Deserializer.DeserializeProperties"/>
    object? ConvertDatatype(IReadableNode node, Feature property, LanguageEntity datatype, PropertyValue? value);
}

internal abstract class DeserializerVersionSpecificsBase<T, H>(
    DeserializerBase<T, H> deserializer,
    DeserializerMetaInfo metaInfo,
    IDeserializerHandler handler)
    : IDeserializerVersionSpecifics
    where T : class, IReadableNode where H : class, IDeserializerHandler
{
    private readonly IDeserializer _deserializer = deserializer;
    internal readonly DeserializerMetaInfo _metaInfo = metaInfo;
    internal readonly IDeserializerHandler _handler = handler;

    public abstract LionWebVersions Version { get; }

    public abstract void RegisterBuiltins();

    public abstract object? ConvertDatatype(IReadableNode node, Feature property, LanguageEntity datatype,
        PropertyValue? value);

    protected void RegisterLanguage(Language language)
    {
        switch (_deserializer)
        {
            case ILanguageDeserializer l:
                l.RegisterDependentLanguage(language);
                break;
            default:
                _deserializer.RegisterInstantiatedLanguage(language);
                break;
        }
    }

    protected Enum? ConvertEnumeration(IReadableNode nodeId, Feature property, Enumeration enumeration, PropertyValue value)
    {
        var literal = enumeration.Literals.FirstOrDefault(literal => literal.Key == value);

        if (literal != null && _metaInfo.LookupFactory(enumeration.GetLanguage(), out var factory))
        {
            Enum? result = factory.GetEnumerationLiteral(literal);
            if (result != null)
            {
                return result;
            }
        }

        return _handler.UnknownEnumerationLiteral(value, enumeration, property, nodeId);
    }
}
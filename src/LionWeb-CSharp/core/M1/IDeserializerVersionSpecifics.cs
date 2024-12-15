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
using VersionSpecific.V2024_1;

/// Externalized logic of <see cref="IDeserializer"/>, specific to one version of LionWeb standard.
public interface IDeserializerVersionSpecifics : IVersionSpecifics
{
    /// <summary>
    /// Creates an instance of <see cref="IDeserializerVersionSpecifics"/> that implements <paramref name="lionWebVersion"/>.
    /// </summary>
    /// <exception cref="UnsupportedVersionException"></exception>
    public static IDeserializerVersionSpecifics Create(LionWebVersions lionWebVersion) => lionWebVersion switch
    {
        IVersion2023_1 => new DeserializerVersionSpecifics_2023_1(),
        IVersion2024_1 => new DeserializerVersionSpecifics_2024_1(),
        IVersion2024_1_Compatible => new DeserializerVersionSpecifics_2024_1_Compatible(),
        _ => throw new UnsupportedVersionException(lionWebVersion)
    };

    /// Converts the low-level string representation <paramref name="value"/> of <paramref name="property"/> into the internal LionWeb-C# representation.
    object? ConvertPrimitiveType<T>(DeserializerBase<T> self, T node, Property property, string value)
        where T : class, IReadableNode;

    /// Registers all relevant builtins to <paramref name="self"/>. 
    void RegisterBuiltins(IDeserializer self);
}

internal static class IDeserializerVersionSpecificsExtensions
{
    public static void RegisterLanguage(this IDeserializerVersionSpecifics specifics, IDeserializer self,
        Language language)
    {
        switch (self)
        {
            case ILanguageDeserializer l:
                l.RegisterDependentLanguage(language);
                break;
            default:
                self.RegisterInstantiatedLanguage(language);
                break;
        }
    }
}
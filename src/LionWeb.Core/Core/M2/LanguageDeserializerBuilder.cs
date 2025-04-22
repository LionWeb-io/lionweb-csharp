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

namespace LionWeb.Core.M2;

using LionWeb.Core.M3;
using M1;
using M3;

/// Builds an M2 (i.e. language-level) <see cref="ILanguageDeserializer"/>.
public class LanguageDeserializerBuilder
{
    private readonly Dictionary<Language, INodeFactory> _languages = new();
    private readonly HashSet<IReadableNode> _dependentNodes = new();
    private readonly HashSet<Language> _dependentLanguages = new();
    private CompressedIdConfig _compressedIdConfig = new();
    private ReferenceResolveInfoHandling _referenceResolveInfoHandling = ReferenceResolveInfoHandling.None;

    /// <inheritdoc cref="WithLionWebVersion"/>
    public LionWebVersions LionWebVersion { get; set; } = LionWebVersions.Current;

    /// <inheritdoc cref="WithHandler"/>
    public ILanguageDeserializerHandler? Handler { get; set; }

    /// Registers a custom handler.
    /// Defaults to <see cref="DeserializerExceptionHandler"/>.
    public LanguageDeserializerBuilder WithHandler(ILanguageDeserializerHandler handler)
    {
        Handler = handler;
        return this;
    }

    /// Registers an <see cref="IDeserializer.RegisterInstantiatedLanguage">instantiated language</see>.
    public LanguageDeserializerBuilder WithLanguage(Language language)
    {
        WithCustomFactory(language, language.GetFactory());
        return this;
    }

    /// Registers several <see cref="IDeserializer.RegisterInstantiatedLanguage">instantiated languages</see>.
    public LanguageDeserializerBuilder WithLanguages(IEnumerable<Language> languages)
    {
        foreach (Language language in languages)
        {
            WithLanguage(language);
        }

        return this;
    }

    /// Registers an <see cref="IDeserializer.RegisterInstantiatedLanguage">instantiated language</see> with custom factory.
    public LanguageDeserializerBuilder WithCustomFactory(Language language, INodeFactory factory)
    {
        _languages[language] = factory;
        return this;
    }

    /// Registers <see cref="IDeserializer.RegisterDependentNodes">dependent nodes</see>.
    public LanguageDeserializerBuilder WithDependentNodes(IEnumerable<IReadableNode> dependentNodes)
    {
        foreach (var dependentNode in dependentNodes)
        {
            _dependentNodes.Add(dependentNode);
        }

        return this;
    }

    /// Registers <see cref="ILanguageDeserializer.RegisterDependentLanguage">dependent languages</see>.
    public LanguageDeserializerBuilder WithDependentLanguages(IEnumerable<Language> dependentLanguages)
    {
        foreach (var dependentLanguage in dependentLanguages)
        {
            _dependentLanguages.Add(dependentLanguage);
        }

        return this;
    }

    /// Whether to compress ids, and whether to store uncompressed node and meta-pointer ids
    /// alongside the compressed ones during processing.
    public LanguageDeserializerBuilder WithCompressedIds(CompressedIdConfig? config = null)
    {
        _compressedIdConfig = config ?? new();
        return this;
    }

    /// Whether we try to resolve references by <see cref="LionWeb.Core.Serialization.SerializedReferenceTarget.ResolveInfo"/>.
    /// Defaults to <see cref="ReferenceResolveInfoHandling.None"/>.
    public LanguageDeserializerBuilder WithReferenceResolveInfoHandling(
        ReferenceResolveInfoHandling referenceResolveInfoHandling)
    {
        _referenceResolveInfoHandling = referenceResolveInfoHandling;
        return this;
    }

    /// The version of LionWeb standard to use.
    /// Defaults to <see cref="LionWebVersions.Current"/>.
    public LanguageDeserializerBuilder WithLionWebVersion(LionWebVersions lionWebVersion)
    {
        LionWebVersion = lionWebVersion;
        return this;
    }

    /// <summary>Builds the deserializer.</summary>
    /// <exception cref="VersionMismatchException">
    /// If any <see cref="WithLanguage">registered language</see>'s LionWeb version is not compatible with
    /// the <see cref="WithLionWebVersion">selected version of LionWeb standard</see>.
    /// </exception>
    public ILanguageDeserializer Build()
    {
        var result = new LanguageDeserializer(LionWebVersion, Handler, _compressedIdConfig)
        {
            ResolveInfoHandling = _referenceResolveInfoHandling
        };
        foreach ((Language language, INodeFactory factory) in _languages)
        {
            result.RegisterInstantiatedLanguage(language, factory);
        }

        result.RegisterDependentNodes(_dependentNodes);
        foreach (var dependentLanguage in _dependentLanguages)
        {
            result.RegisterDependentLanguage(dependentLanguage);
        }

        return result;
    }
}
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

/// Builds an M1 (i.e. instance-level) <see cref="IDeserializer"/>.
public class DeserializerBuilder
{
    /// <inheritdoc cref="WithLionWebVersion"/>
    public LionWebVersions LionWebVersion { get; set; } = LionWebVersions.Current;

    /// <inheritdoc cref="WithHandler"/>
    public IDeserializerHandler? Handler { get; set; }

    /// <inheritdoc cref="WithReferenceResolveInfoHandling"/>
    public ReferenceResolveInfoHandling ResolveInfoHandling { get; set; } = ReferenceResolveInfoHandling.None;

    /// <inheritdoc cref="WithLanguage"/>
    public Dictionary<Language, INodeFactory> Languages { get; } = [];

    /// <inheritdoc cref="WithLanguageReferences"/>
    public bool LanguageReferences { get; set; } = false;

    /// <inheritdoc cref="WithDependentNodes"/>
    public HashSet<IReadableNode> DependentNodes { get; } = [];

    /// Registers a custom handler.
    /// Defaults to <see cref="DeserializerExceptionHandler"/>.
    public DeserializerBuilder WithHandler(IDeserializerHandler handler)
    {
        Handler = handler;
        return this;
    }

    /// Registers an <see cref="IDeserializer.RegisterInstantiatedLanguage">instantiated language</see>.
    public DeserializerBuilder WithLanguage(Language language)
    {
        WithCustomFactory(language, language.GetFactory());
        return this;
    }

    /// Registers several <see cref="IDeserializer.RegisterInstantiatedLanguage">instantiated languages</see>.
    public DeserializerBuilder WithLanguages(IEnumerable<Language> languages)
    {
        foreach (Language language in languages)
        {
            WithLanguage(language);
        }

        return this;
    }

    /// Enables references to language elements of <see cref="IDeserializer.RegisterInstantiatedLanguage">instantiated languages</see>.
    public DeserializerBuilder WithLanguageReferences(bool languageReferences = true)
    {
        LanguageReferences = languageReferences;
        return this;
    }

    /// Registers an <see cref="IDeserializer.RegisterInstantiatedLanguage">instantiated language</see> with custom factory.
    public DeserializerBuilder WithCustomFactory(Language language, INodeFactory factory)
    {
        Languages[language] = factory;
        return this;
    }

    /// Registers <see cref="IDeserializer.RegisterDependentNodes">dependent nodes</see>.
    public DeserializerBuilder WithDependentNodes(IEnumerable<IReadableNode> dependentNodes)
    {
        foreach (var dependentNode in dependentNodes)
        {
            DependentNodes.Add(dependentNode);
        }

        return this;
    }

    /// Whether to compress ids, and whether to store uncompressed node and meta-pointer ids
    /// alongside the compressed ones during processing.
    [Obsolete("Not supported anymore, has no effect.")]
    public DeserializerBuilder WithCompressedIds(CompressedIdConfig? config = null) => this;

    /// <inheritdoc cref="WithCompressedIds(LionWeb.Core.M1.CompressedIdConfig?)"/>
    /// <param name="compress">Whether we compress ids at all.</param>
    /// <param name="keepOriginal">Whether we keep the original around for compressed ids. Uses more memory, but eases debugging.</param>
    [Obsolete("Not supported anymore, has no effect.")]
    public DeserializerBuilder WithCompressedIds(bool keepOriginal = true, bool compress = true) => this;
    
    /// Whether we try to resolve references by <see cref="LionWeb.Core.Serialization.SerializedReferenceTarget.ResolveInfo"/>.
    /// Defaults to <see cref="ReferenceResolveInfoHandling.None"/>.
    public DeserializerBuilder WithReferenceResolveInfoHandling(
        ReferenceResolveInfoHandling referenceResolveInfoHandling)
    {
        ResolveInfoHandling = referenceResolveInfoHandling;
        return this;
    }

    /// The version of LionWeb standard to use.
    /// Defaults to <see cref="LionWebVersions.Current"/>.
    public DeserializerBuilder WithLionWebVersion(LionWebVersions lionWebVersion)
    {
        LionWebVersion = lionWebVersion;
        return this;
    }

    /// <summary>Builds the deserializer.</summary>
    /// <exception cref="VersionMismatchException">
    /// If any <see cref="WithLanguage">registered language</see>'s LionWeb version is not compatible with
    /// the <see cref="WithLionWebVersion">selected version of LionWeb standard</see>.
    /// </exception>
    public IDeserializer Build()
    {
        IDeserializer result = new Deserializer(LionWebVersion, Handler)
        {
            ResolveInfoHandling = ResolveInfoHandling
        };
        foreach ((Language language, INodeFactory factory) in Languages)
        {
            result.RegisterInstantiatedLanguage(language, factory);
        }

        result.RegisterDependentNodes(DependentNodes);

        if (LanguageReferences)
            result.RegisterDependentNodes(Languages.Keys.SelectMany(k =>
                M1Extensions.Descendants<IReadableNode>(k, true, true)));

        return result;
    }
}
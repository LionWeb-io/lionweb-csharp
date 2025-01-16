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
    private readonly Dictionary<Language, INodeFactory> _languages = new();
    private readonly HashSet<IReadableNode> _dependentNodes = new();
    private IDeserializerHandler? _handler;
    private CompressedIdConfig _compressedIdConfig = new();
    private LionWebVersions _lionWebVersion = LionWebVersions.Current;
    private ReferenceResolveInfoHandling _referenceResolveInfoHandling = ReferenceResolveInfoHandling.None;

    /// Registers a custom handler.
    /// Defaults to <see cref="DeserializerExceptionHandler"/>.
    public DeserializerBuilder WithHandler(IDeserializerHandler handler)
    {
        _handler = handler;
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

    /// Registers an <see cref="IDeserializer.RegisterInstantiatedLanguage">instantiated language</see> with custom factory.
    public DeserializerBuilder WithCustomFactory(Language language, INodeFactory factory)
    {
        _languages[language] = factory;
        return this;
    }

    /// Registers <see cref="IDeserializer.RegisterDependentNodes">dependent nodes</see>.
    public DeserializerBuilder WithDependentNodes(IEnumerable<IReadableNode> dependentNodes)
    {
        foreach (var dependentNode in dependentNodes)
        {
            _dependentNodes.Add(dependentNode);
        }

        return this;
    }

    /// Whether to compress ids, and whether to store uncompressed node and meta-pointer ids
    /// alongside the compressed ones during processing.
    public DeserializerBuilder WithCompressedIds(CompressedIdConfig? config = null)
    {
        _compressedIdConfig = config ?? new ();
        return this;
    }

    /// <inheritdoc cref="WithCompressedIds(LionWeb.Core.M1.CompressedIdConfig?)"/>
    /// <param name="compress">Whether we compress ids at all.</param>
    /// <param name="keepOriginal">Whether we keep the original around for compressed ids. Uses more memory, but eases debugging.</param>
    [Obsolete("Use WithCompressedIds(CompressedIdConfig) instead.")]
    public DeserializerBuilder WithCompressedIds(bool keepOriginal = true, bool compress = true) =>
        WithCompressedIds(new CompressedIdConfig(compress, keepOriginal));
    
    /// Whether we try to resolve references by <see cref="LionWeb.Core.Serialization.SerializedReferenceTarget.ResolveInfo"/>.
    /// Defaults to <see cref="ReferenceResolveInfoHandling.None"/>.
    public DeserializerBuilder WithReferenceResolveInfoHandling(
        ReferenceResolveInfoHandling referenceResolveInfoHandling)
    {
        _referenceResolveInfoHandling = referenceResolveInfoHandling;
        return this;
    }

    /// The version of LionWeb standard to use.
    /// Defaults to <see cref="LionWebVersions.Current"/>.
    public DeserializerBuilder WithLionWebVersion(LionWebVersions lionWebVersion)
    {
        _lionWebVersion = lionWebVersion;
        return this;
    }

    /// <summary>Builds the deserializer.</summary>
    /// <exception cref="VersionMismatchException">
    /// If any <see cref="WithLanguage">registered language</see>'s LionWeb version is not compatible with
    /// the <see cref="WithLionWebVersion">selected version of LionWeb standard</see>.
    /// </exception>
    public IDeserializer Build()
    {
        IDeserializer result = new Deserializer(_lionWebVersion, _handler)
        {
            CompressedIdConfig = _compressedIdConfig, ResolveInfoHandling = _referenceResolveInfoHandling
        };
        foreach ((Language language, INodeFactory factory) in _languages)
        {
            result.RegisterInstantiatedLanguage(language, factory);
        }

        result.RegisterDependentNodes(_dependentNodes);

        return result;
    }
}
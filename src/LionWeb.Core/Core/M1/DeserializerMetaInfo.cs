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

// ReSharper disable SuggestVarOrType_SimpleTypes

namespace LionWeb.Core.M1;

using M2;
using M3;
using Serialization;
using System.Diagnostics.CodeAnalysis;

/// Stores information required do deserialize meta-elements of nodes.
/// <remarks>Should be internal, but the compiler doesn't like it in
/// <see cref="DeserializerBase{T,H}._deserializerMetaInfo"/>.</remarks>
public class DeserializerMetaInfo(IDeserializerHandler handler)
{
    private readonly Dictionary<Language, INodeFactory> _language2NodeFactory = new();
    private readonly Dictionary<ICompressedId, List<Language>> _languagesByKey = new();
    private readonly Dictionary<CompressedMetaPointer, Classifier> _classifiers = new();
    private readonly Dictionary<CompressedMetaPointer, Feature> _features = new();
    
    internal CompressedIdConfig CompressedIdConfig { get; set; } = new();

    internal void RegisterInstantiatedLanguage(Language language, INodeFactory factory)
    {
        _language2NodeFactory[language] = factory;
        var compressedKey = Compress(language.Key);
        if (!_languagesByKey.TryAdd(compressedKey, [language]))
        {
            _languagesByKey[compressedKey].Add(language);
        }

        foreach (Classifier classifier in language.Entities.OfType<Classifier>())
        {
            _classifiers[Compress(classifier.ToMetaPointer())] = classifier;
            foreach (Feature feature in classifier.Features)
            {
                _features[Compress(feature.ToMetaPointer())] = feature;
            }
        }
    }

    internal INode? Instantiate(string id, MetaPointer metaPointer)
    {
        var compressedMetaPointer = Compress(metaPointer);
        if (!LookupClassifier(compressedMetaPointer, out var classifier))
        {
            classifier =
                handler.UnknownClassifier(compressedMetaPointer, ICompressedId.Create(id, CompressedIdConfig));
            if (classifier == null)
                return null;
        }

        if (!LookupFactory(classifier.GetLanguage(), out var factory))
        {
            return null;
        }

        return factory.CreateNode(id, classifier);
    }

    internal Feature? FindFeature<TFeature>(IReadableNode node, CompressedMetaPointer compressedMetaPointer)
        where TFeature : class, Feature
    {
        Classifier classifier = node.GetClassifier();
        if (!LookupFeature(compressedMetaPointer, out var feature))
        {
            feature = handler.UnknownFeature<TFeature>(compressedMetaPointer, classifier, node);
            if (feature == null)
                return null;
        }

        return feature as TFeature ?? handler.InvalidFeature<TFeature>(compressedMetaPointer, classifier, node);
    }

    private bool LookupClassifier(CompressedMetaPointer compressedMetaPointer,
        [NotNullWhen(true)] out Classifier? classifier) =>
        _classifiers.TryGetValue(compressedMetaPointer, out classifier) ||
        SelectVersion(compressedMetaPointer, out classifier);

    private bool SelectVersion<T>(CompressedMetaPointer compressedMetaPointer, [NotNullWhen(true)] out T? result)
        where T : class, IKeyed
    {
        if (!_languagesByKey.TryGetValue(compressedMetaPointer.Language, out var languages))
        {
            result = null;
            return false;
        }

        result = handler.SelectVersion<T>(compressedMetaPointer, languages);
        return result != null;
    }

    private bool LookupFeature(CompressedMetaPointer compressedMetaPointer,
        [NotNullWhen(true)] out Feature? feature) =>
        _features.TryGetValue(compressedMetaPointer, out feature) || SelectVersion(compressedMetaPointer, out feature);

    internal bool LookupFactory(Language language, [NotNullWhen(true)] out INodeFactory? factory)
    {
        if (_language2NodeFactory.TryGetValue(language, out factory))
            return true;

        factory = language.GetFactory();
        return factory != null;
    }

    internal ICompressedId Compress(string id) =>
        ICompressedId.Create(id, CompressedIdConfig);

    private CompressedMetaPointer Compress(MetaPointer metaPointer) =>
        CompressedMetaPointer.Create(metaPointer, CompressedIdConfig);
}
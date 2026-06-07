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
    private readonly Dictionary<Language, INodeFactory> _language2NodeFactory = [];
    private readonly Dictionary<NodeId, List<Language>> _languagesByKey = [];
    private readonly Dictionary<MetaPointer, Classifier> _classifiers = [];
    private readonly Dictionary<MetaPointer, Feature> _features = [];

    internal void RegisterInstantiatedLanguage(Language language, INodeFactory factory)
    {
        _language2NodeFactory[language] = factory;
        var key = language.Key;
        if (!_languagesByKey.TryAdd(key, [language]))
        {
            _languagesByKey[key].Add(language);
        }

        foreach (var entity in language.Entities)
        {
            if (entity is not Classifier classifier)
                continue;

            _classifiers[classifier.ToMetaPointer()] = classifier;
            foreach (Feature feature in classifier.Features)
            {
                _features[feature.ToMetaPointer()] = feature;
            }
        }
    }

    internal INode? Instantiate(NodeId id, MetaPointer metaPointer)
    {
        if (!LookupClassifier(metaPointer, out var classifier))
        {
            classifier =
                handler.UnknownClassifier(metaPointer, id);
            if (classifier == null)
                return null;
        }

        if (!LookupFactory(classifier.GetLanguage(), out var factory))
        {
            return null;
        }

        return factory.CreateNode(id, classifier);
    }

    internal Feature? FindFeature<TFeature>(IReadableNode node, MetaPointer metaPointer)
        where TFeature : class, Feature
    {
        Classifier classifier = node.GetClassifier();
        if (!LookupFeature(metaPointer, out var feature))
        {
            feature = handler.UnknownFeature<TFeature>(metaPointer, classifier, node);
            if (feature == null)
                return null;
        }

        return feature as TFeature ?? handler.InvalidFeature<TFeature>(metaPointer, classifier, node);
    }

    private bool LookupClassifier(MetaPointer metaPointer,
        [NotNullWhen(true)] out Classifier? classifier) =>
        _classifiers.TryGetValue(metaPointer, out classifier) ||
        SelectVersion(metaPointer, out classifier);

    private bool SelectVersion<T>(MetaPointer metaPointer, [NotNullWhen(true)] out T? result)
        where T : class, IKeyed
    {
        if (!_languagesByKey.TryGetValue(metaPointer.Language, out var languages))
        {
            result = null;
            return false;
        }

        result = handler.SelectVersion<T>(metaPointer, languages);
        return result != null;
    }

    private bool LookupFeature(MetaPointer metaPointer,
        [NotNullWhen(true)] out Feature? feature) =>
        _features.TryGetValue(metaPointer, out feature) || SelectVersion(metaPointer, out feature);

    internal bool LookupFactory(Language language, [NotNullWhen(true)] out INodeFactory? factory)
    {
        if (_language2NodeFactory.TryGetValue(language, out factory))
            return true;

        factory = language.GetFactory();
        if (factory == null)
            return false;

        _language2NodeFactory[language] = factory;
        return true;
    }
}
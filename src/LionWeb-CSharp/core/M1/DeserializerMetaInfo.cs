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
using System.Diagnostics.CodeAnalysis;

public class DeserializerMetaInfo()
{
    private readonly Dictionary<Language, INodeFactory> _language2NodeFactory = new();
    private readonly Dictionary<CompressedId, List<Language>> _languagesByKey = new();
    private readonly Dictionary<CompressedMetaPointer, Classifier> _classifiers = new();
    private readonly Dictionary<CompressedMetaPointer, Feature> _features = new();

    internal IDeserializerHandler Handler { get; set; } = new DeserializerExceptionHandler();
    internal bool StoreUncompressedIds { get; set; } = false;

    public void RegisterInstantiatedLanguage(Language language, INodeFactory factory)
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
                Handler.UnknownClassifier(compressedMetaPointer, CompressedId.Create(id, StoreUncompressedIds));
            if (classifier == null)
                return null;
        }

        if (!LookupFactory(classifier.GetLanguage(), out var factory))
        {
            return null;
        }

        return factory.CreateNode(id, classifier);
    }

    internal Enum? ConvertEnumeration(IWritableNode nodeId, Feature property, Enumeration enumeration, string value)
    {
        var literal = enumeration.Literals.FirstOrDefault(literal => literal.Key == value);

        if (literal != null && LookupFactory(enumeration.GetLanguage(), out var factory))
        {
            Enum? result = factory.GetEnumerationLiteral(literal);
            if (result != null)
            {
                return result;
            }
        }

        return Handler.UnknownEnumerationLiteral(value, enumeration, property, nodeId);
    }

    internal Feature? FindFeature<TFeature>(IWritableNode node, CompressedMetaPointer compressedMetaPointer)
        where TFeature : class, Feature
    {
        Classifier classifier = node.GetClassifier();
        if (!LookupFeature(compressedMetaPointer, out var feature))
        {
            feature = Handler.UnknownFeature<TFeature>(compressedMetaPointer, classifier, node);
            if (feature == null)
                return null;
        }

        return feature as TFeature ?? Handler.InvalidFeature<TFeature>(compressedMetaPointer, classifier, node);
    }

    private bool LookupClassifier(CompressedMetaPointer compressedMetaPointer,
        [MaybeNullWhen(false)] out Classifier classifier) =>
        _classifiers.TryGetValue(compressedMetaPointer, out classifier) ||
        SelectVersion(compressedMetaPointer, out classifier);

    private bool SelectVersion<T>(CompressedMetaPointer compressedMetaPointer, [MaybeNullWhen(false)] out T result)
        where T : class, IKeyed
    {
        var languages = _languagesByKey[compressedMetaPointer.Language];
        if (languages.Count == 0)
        {
            result = default;
            return false;
        }

        result = Handler.SelectVersion<T>(compressedMetaPointer, languages);
        return result != null;
    }

    private bool LookupFeature(CompressedMetaPointer compressedMetaPointer,
        [MaybeNullWhen(false)] out Feature feature) =>
        _features.TryGetValue(compressedMetaPointer, out feature) || SelectVersion(compressedMetaPointer, out feature);

    private bool LookupFactory(Language language, [MaybeNullWhen(false)] out INodeFactory factory) =>
        _language2NodeFactory.TryGetValue(language, out factory);

    private CompressedId Compress(string id) =>
        CompressedId.Create(id, StoreUncompressedIds);

    private CompressedMetaPointer Compress(MetaPointer metaPointer) =>
        CompressedMetaPointer.Create(metaPointer, StoreUncompressedIds);
}
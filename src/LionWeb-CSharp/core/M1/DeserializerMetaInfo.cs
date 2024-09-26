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

public class DeserializerMetaInfo()
{
    private readonly Dictionary<Language, INodeFactory> _language2NodeFactory = new();
    private readonly Dictionary<CompressedMetaPointer, Classifier> _classifiers = new();
    private readonly Dictionary<CompressedMetaPointer, Feature> _features = new();

    internal IDeserializerHandler Handler { get; set; } = new DeserializerExceptionHandler();
    internal bool StoreUncompressedIds { get; set; } = false;

    public void RegisterInstantiatedLanguage(Language language, INodeFactory factory)
    {
        _language2NodeFactory[language] = factory;

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
        if (!_classifiers.TryGetValue(compressedMetaPointer, out var classifier))
        {
            classifier = Handler.UnknownClassifier(id, metaPointer);
            if (classifier == null)
                return null;
        }

        return _language2NodeFactory[classifier.GetLanguage()].CreateNode(id, classifier);
    }

    internal Enum? ConvertEnumeration(string nodeId, Enumeration enumeration, string value)
    {
        var literal = enumeration.Literals.FirstOrDefault(literal => literal.Key == value);

        if (literal != null && _language2NodeFactory.TryGetValue(enumeration.GetLanguage(), out var factory))
            return factory.GetEnumerationLiteral(literal);

        return Handler.UnknownEnumerationLiteral(nodeId, enumeration, value);
    }

    internal TFeature? FindFeature<TFeature>(IWritableNode node, CompressedMetaPointer compressedMetaPointer)
        where TFeature : class, Feature
    {
        Classifier classifier = node.GetClassifier();
        if (!_features.TryGetValue(compressedMetaPointer, out var feature))
        {
            feature = Handler.UnknownFeature(classifier, compressedMetaPointer, node);
            if (feature == null)
                return null;
        }

        return feature is TFeature f
            ? f
            : Handler.InvalidFeature<TFeature>(classifier, compressedMetaPointer, node);
    }

    private CompressedMetaPointer Compress(MetaPointer metaPointer) =>
        CompressedMetaPointer.Create(metaPointer, StoreUncompressedIds);
}
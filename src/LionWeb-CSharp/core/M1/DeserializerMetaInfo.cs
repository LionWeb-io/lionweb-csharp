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
using System.Text.Json;
using System.Text.Json.Nodes;

/// Stores information required do deserialize meta-elements of nodes.
/// <remarks>Should be internal, but the compiler doesn't like it in
/// <see cref="DeserializerBase{T}._deserializerMetaInfo"/>.</remarks>
public class DeserializerMetaInfo
{
    private readonly Dictionary<Language, INodeFactory> _language2NodeFactory = new();
    private readonly Dictionary<CompressedId, List<Language>> _languagesByKey = new();
    private readonly Dictionary<CompressedMetaPointer, Classifier> _classifiers = new();
    private readonly Dictionary<CompressedMetaPointer, Feature> _features = new();

    internal IDeserializerHandler Handler { get; set; } = new DeserializerExceptionHandler();
    internal bool StoreUncompressedIds { get; set; } = false;

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

    private Enum? ConvertEnumeration(IWritableNode nodeId, Feature property, Enumeration enumeration, string value)
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

    internal Feature? FindFeature<TFeature>(IReadableNode node, CompressedMetaPointer compressedMetaPointer)
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

    private CompressedId Compress(string id) =>
        CompressedId.Create(id, StoreUncompressedIds);


    internal object? ConvertDatatype(IWritableNode node, Feature property, LanguageEntity datatype, string? value)
    {
        var convertedValue = (datatype, value) switch
        {
            (_, null) => null,
            (PrimitiveType p, { } v) => ConvertPrimitiveType(node, property, p, v),
            (Enumeration enumeration, { } v) => ConvertEnumeration(node, property, enumeration, v),
            (StructuredDataType sdt, { } v) => ConvertStructuredDataType(node, property, sdt, v),
            var (_, v) => Handler.UnknownDatatype(v, datatype, property, node)
        };
        return convertedValue;
    }

    private object? ConvertPrimitiveType(IWritableNode node, Feature property, PrimitiveType datatype, string value)
    {
        CompressedId compressedId = Compress(node.GetId());
        return datatype switch
        {
            var b when b == BuiltInsLanguage.Instance.Boolean => bool.TryParse(value, out var result)
                ? result
                : Handler.InvalidPropertyValue<bool>(value, property, compressedId),
            var i when i == BuiltInsLanguage.Instance.Integer => int.TryParse(value, out var result)
                ? result
                : Handler.InvalidPropertyValue<int>(value, property, compressedId),
            // leave a String value as a string:
            var s when s == BuiltInsLanguage.Instance.String => value,
            _ => Handler.UnknownDatatype(value, datatype, property, node)
        };
    }


    private IStructuredDataTypeInstance? ConvertStructuredDataType(IWritableNode node, Feature property,
        StructuredDataType sdt, string s)
    {
        try
        {
            JsonObject? jsonObject = JsonSerializer.Deserialize<JsonObject>(s);
            if(jsonObject == null)
                return Handler.UnknownDatatype(s, sdt, property, node) as IStructuredDataTypeInstance;
            return ConvertStructuredDataType(node, property, sdt, jsonObject);
        } catch (Exception e) when (e is JsonException or NotSupportedException)
        {
            return Handler.UnknownDatatype(s, sdt, property, node) as IStructuredDataTypeInstance;
        }
    }

    private IStructuredDataTypeInstance? ConvertStructuredDataType(IWritableNode node, Feature property,
        StructuredDataType sdt, JsonObject jsonObject)
    {
        var fieldValues = new FieldValues();
        
        foreach ((var key, JsonNode? jsonNode) in jsonObject)
        {
            var field = sdt.Fields.FirstOrDefault(f => f.Key == key);
            if (field == null)
            {
                field = Handler.UnknownField(key, sdt, property, node);
                if (field == null)
                    continue;
            }
            var value = (field.Type, jsonNode) switch
            {
                (_, null) => null,
                (not StructuredDataType, JsonValue j) when j.GetValueKind() == JsonValueKind.String =>
                    ConvertDatatype(node, property, field.Type, j.GetValue<string>()),
                (StructuredDataType s, JsonObject o) => ConvertStructuredDataType(node, property, s, o),
                _ => Handler.UnknownDatatype(jsonNode.ToString(), sdt, property, node)
            };
            
            fieldValues.Add(field, value);
        }

        if (_language2NodeFactory.TryGetValue(property.GetLanguage(), out var factory))
        {
            return factory.CreateStructuredDataTypeInstance(sdt, fieldValues);
        }

        return Handler.UnknownDatatype(jsonObject.ToString(), sdt, property, node) as IStructuredDataTypeInstance;
    }
}
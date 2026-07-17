// Copyright 2026 TRUMPF Laser SE and other contributors
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

using M3;
using System.Collections.Immutable;

public class M2Cache : IGlobalM2Cache
{
    private readonly object _lockObject = new();
    private readonly Dictionary<(Language, MetaPointerKey), IKeyed> _keyed = [];
    private readonly Dictionary<Classifier, IImmutableSet<Feature>> _allFeatures = [];
    private readonly Dictionary<Classifier, IImmutableSet<Classifier>> _allGeneralizations = [];
    private readonly Dictionary<Classifier, IImmutableSet<Classifier>> _allSpecializations = [];
    private readonly Dictionary<Classifier, IImmutableSet<Classifier>> _directGeneralizations = [];
    private readonly Dictionary<Classifier, IImmutableSet<Classifier>> _directSpecializations = [];
    private readonly Dictionary<(Classifier, MetaPointerKey), Feature> _features = [];
    private readonly Dictionary<(StructuredDataType, MetaPointerKey), Field> _fields = [];
    private readonly Dictionary<(Enumeration, MetaPointerKey), EnumerationLiteral> _literals = [];

    /// <inheritdoc />
    public T? FindByKey<T>(Language language, MetaPointerKey key) where T : class, IKeyed =>
        _keyed.TryGetValue((language, key), out var result) && result is T t
            ? t
            : null;


    /// <inheritdoc />
    public IImmutableSet<Feature>? AllFeatures(Classifier classifier) =>
        _allFeatures.GetValueOrDefault(classifier);

    /// <inheritdoc />
    public IImmutableSet<Classifier>? AllGeneralizations(Classifier classifier) =>
        _allGeneralizations.GetValueOrDefault(classifier);

    /// <inheritdoc />
    public IImmutableSet<Classifier>? DirectGeneralizations(Classifier classifier) =>
        _directGeneralizations.GetValueOrDefault(classifier);

    /// <inheritdoc />
    public IImmutableSet<Classifier>? DirectSpecializations(Classifier classifier) =>
        _directSpecializations.GetValueOrDefault(classifier);

    /// <inheritdoc />
    public IImmutableSet<Classifier>? AllSpecializations(Classifier classifier) =>
        _allSpecializations.GetValueOrDefault(classifier);

    /// <inheritdoc />
    public Feature? FeatureByKey(Classifier classifier, MetaPointerKey key) =>
        _features.GetValueOrDefault((classifier, key));


    /// <inheritdoc />
    public Field? FieldByKey(StructuredDataType structuredDataType, MetaPointerKey key) =>
        _fields.GetValueOrDefault((structuredDataType, key));

    /// <inheritdoc />
    public EnumerationLiteral? LiteralByKey(Enumeration enumeration, MetaPointerKey key) =>
        _literals.GetValueOrDefault((enumeration, key));


    /// <inheritdoc />
    public void Clear()
    {
        lock (_lockObject)
        {
            _keyed.Clear();
            _allFeatures.Clear();
            _allGeneralizations.Clear();
            _allSpecializations.Clear();
            _directGeneralizations.Clear();
            _directSpecializations.Clear();
            _features.Clear();
            _fields.Clear();
            _literals.Clear();
        }
    }

    /// <inheritdoc />
    public void Register(IEnumerable<Language> languages)
    {
        lock (_lockObject)
        {
            PopulateDirectRelations(languages);
            PopulateAllGeneralizationsAndFeatures();
            PopulateDirectSpecializations();
            PopulateAllSpecializations();
        }
    }

    private void PopulateDirectRelations(IEnumerable<Language> languages)
    {
        HashSet<Language> remainingLanguages = new HashSet<Language>(languages);

        while (remainingLanguages.FirstOrDefault() is { } language)
        {
            remainingLanguages.Remove(language);
            if (!_keyed.TryAdd((language, language.Key), language))
                continue;

            foreach (var entity in language.Entities)
            {
                _keyed[(language, entity.Key)] = entity;

                switch (entity)
                {
                    case Classifier c:
                        var generalizations = c switch
                        {
                            Annotation a => a.DirectGeneralizations(),
                            Concept c1 => c1.DirectGeneralizations(),
                            Interface i => i.DirectGeneralizations(),
                            _ => throw new UnsupportedClassifierException(c)
                        };
                        _directGeneralizations[c] = generalizations;
                        remainingLanguages.UnionWith(generalizations.Select(c => c.GetLanguage()));
                        foreach (var feature in c.Features)
                        {
                            _keyed[(language, feature.Key)] = feature;
                            remainingLanguages.Add(feature.GetFeatureType().GetLanguage());
                        }

                        break;

                    case StructuredDataType s:
                        foreach (var field in s.Fields)
                        {
                            _keyed[(language, field.Key)] = field;
                            _fields[(s, field.Key)] = field;
                            remainingLanguages.Add(field.Type.GetLanguage());
                        }

                        break;

                    case Enumeration e:
                        foreach (var literal in e.Literals)
                        {
                            _keyed[(language, literal.Key)] = literal;
                            _literals[(e, literal.Key)] = literal;
                        }

                        break;
                }
            }
        }
    }

    private void PopulateAllGeneralizationsAndFeatures()
    {
        foreach (var classifier in _directGeneralizations.Keys)
        {
            HashSet<Classifier> generalizations = [];
            CollectGeneralizations(classifier, generalizations);
            _allGeneralizations[classifier] = generalizations.Except([classifier]).ToImmutableHashSet();

            var allFeatures = generalizations.SelectMany(g => g.Features).ToImmutableHashSet();
            _allFeatures[classifier] = allFeatures;

            foreach (var feature in allFeatures)
            {
                _features[(classifier, feature.Key)] = feature;
            }
        }
    }

    private void CollectGeneralizations(Classifier c, HashSet<Classifier> generalizations)
    {
        if (!generalizations.Add(c))
            return;

        var directGeneralizations = _directGeneralizations[c];
        foreach (Classifier generalization in directGeneralizations)
        {
            CollectGeneralizations(generalization, generalizations);
        }
    }

    private void PopulateDirectSpecializations()
    {
        ILookup<Classifier, Classifier> directSpecializations = _directGeneralizations
            .SelectMany(p => p.Value.Select(v => (p.Key, v)))
            .ToLookup(p => p.v, p => p.Key);

        foreach (var grouping in directSpecializations)
        {
            _directSpecializations[grouping.Key] = grouping.ToImmutableHashSet();
        }

        foreach (var classifier in _directGeneralizations.Keys)
        {
            _directSpecializations.TryAdd(classifier, []);
        }
    }

    private void PopulateAllSpecializations()
    {
        foreach (var classifier in _directGeneralizations.Keys)
        {
            HashSet<Classifier> specializations = [];
            CollectSpecializations(classifier, specializations);
            _allSpecializations[classifier] = specializations.Except([classifier]).ToImmutableHashSet();
        }
    }

    private void CollectSpecializations(Classifier c, HashSet<Classifier> specializations)
    {
        if (!specializations.Add(c))
            return;

        if (!_directSpecializations.TryGetValue(c, out var directSpecializations))
            return;

        foreach (Classifier specialization in directSpecializations)
        {
            CollectSpecializations(specialization, specializations);
        }
    }
}
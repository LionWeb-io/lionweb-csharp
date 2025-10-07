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

namespace LionWeb.Protocol.Delta;

using Core.M1;
using Core.M2;
using Core.M3;
using Core.Serialization;

/// <inheritdoc cref="BuildSharedKeyMap"/>
public class SharedKeyedMapBuilder
{
    /// Builds a map of <see cref="CompressedMetaPointer">meta-pointers</see> to
    /// all <see cref="IKeyed">keyed language elements</see> within, or (transitively) used by, <paramref name="languages"/>.
    public static SharedKeyedMap BuildSharedKeyMap(IEnumerable<Language> languages) =>
        new SharedKeyedMapBuilder().Build(languages);

    private readonly SharedKeyedMap _sharedKeyedMap = [];

    private SharedKeyedMap Build(IEnumerable<Language> languages)
    {
        foreach (var language in languages)
            FromLanguage(language);

        return _sharedKeyedMap;
    }


    private void FromLanguage(Language language)
    {
        foreach (var entity in language.Entities)
            FromEntity(entity);
    }

    private void FromEntity(LanguageEntity entity)
    {
        switch (entity)
        {
            case Classifier c:
                FromClassifier(c);
                return;
            case StructuredDataType s:
                FromSdt(s);
                return;
            default:
                var metaPointer = entity.ToMetaPointer();
                TryAdd(metaPointer, entity);
                return;
        }
    }

    private void FromClassifier(Classifier classifier)
    {
        var metaPointer = classifier.ToMetaPointer();
        if (!TryAdd(metaPointer, classifier))
            return;

        foreach (var generalization in classifier.DirectGeneralizations())
            FromClassifier(generalization);

        foreach (var feature in classifier.Features)
            FromFeature(feature);
    }

    private void FromFeature(Feature feature)
    {
        var metaPointer = feature.ToMetaPointer();
        if (!TryAdd(metaPointer, feature))
            return;

        FromEntity(feature.GetFeatureType());
    }

    private void FromSdt(StructuredDataType sdt)
    {
        var metaPointer = sdt.ToMetaPointer();
        if (!TryAdd(metaPointer, sdt))
            return;

        foreach (var field in sdt.Fields)
        {
            if (!TryAdd(field.ToMetaPointer(), field))
                continue;
            FromEntity(field.Type);
        }
    }

    private bool TryAdd(MetaPointer metaPointer, IKeyed keyed) =>
        _sharedKeyedMap.TryAdd(
            CompressedMetaPointer.Create(metaPointer, new CompressedIdConfig(KeepOriginal: true)),
            keyed
        );
}
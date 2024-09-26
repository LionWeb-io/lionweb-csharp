// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

namespace LionWeb.Core.Serialization;

using M2;
using M3;

/// Utilities to arbitrate between M2 and MetaPointers.
public static class MetaPointerExtensions
{
    /// Represents <paramref name="entity"/> as MetaPointer.
    public static MetaPointer ToMetaPointer(this LanguageEntity entity)
    {
        var language = entity.GetLanguage();
        var metaPointer = new MetaPointer(language.Key, language.Version, entity.Key);
        return metaPointer;
    }

    /// Represents <paramref name="feature"/> as MetaPointer.
    public static MetaPointer ToMetaPointer(this Feature feature)
    {
        var language = feature.GetFeatureClassifier().GetLanguage();
        var metaPointer = new MetaPointer(language.Key, language.Version, feature.Key);
        return metaPointer;
    }

    /// Compares <paramref name="metaPointer"/> and <paramref name="classifier"/> in terms of <see cref="IKeyed.Key"/> and <see cref="M2Extensions.GetLanguage">Language</see>.
    public static bool Matches(this MetaPointer metaPointer, Classifier classifier)
        => metaPointer.Language == classifier.GetLanguage().Key
           && metaPointer.Version == classifier.GetLanguage().Version
           && metaPointer.Key == classifier.Key;

    /// Compares <paramref name="metaPointer"/> and <paramref name="feature"/> in terms of <see cref="IKeyed.Key"/> and <see cref="M2Extensions.GetLanguage">Language</see>.
    public static bool Matches(this MetaPointer metaPointer, Feature feature)
        => metaPointer.Language == feature.GetFeatureClassifier().GetLanguage().Key
           && metaPointer.Version == feature.GetFeatureClassifier().GetLanguage().Version
           && metaPointer.Key == feature.Key;
}
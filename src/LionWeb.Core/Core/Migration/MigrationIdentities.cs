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

namespace LionWeb.Core.Migration;

using M2;
using M3;

/// Represents the identifying parts of a <see cref="Language"/>.
public record LanguageIdentity(string Key, string Version)
{
    public static LanguageIdentity FromLanguage(Language language) =>
        new(language.Key, language.Version);
}

/// Represents the identifying parts of a <see cref="Classifier"/>.
[Obsolete]
public record ClassifierIdentity(string Key, LanguageIdentity Language)
{
    public static ClassifierIdentity FromClassifier(Classifier classifier) =>
        new(classifier.Key, LanguageIdentity.FromLanguage(classifier.GetLanguage()));
}

/// Represents the identifying parts of a <see cref="Feature"/>.
[Obsolete]
public record FeatureIdentity(string Key, ClassifierIdentity Classifier)
{
    public static FeatureIdentity FromFeature(Feature feature) =>
        new(feature.Key, ClassifierIdentity.FromClassifier(feature.GetClassifier()));
}

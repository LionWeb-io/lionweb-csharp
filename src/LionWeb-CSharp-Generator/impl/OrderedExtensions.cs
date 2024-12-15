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

namespace LionWeb.CSharp.Generator.Impl;

using Core.M2;
using Core.M3;

public static class OrderedExtensions
{
    private const string _separator = ".";

    public static IEnumerable<Feature> Ordered(this IEnumerable<Feature> features) =>
        features.OrderBy(f => f.GetLanguage().Name + _separator + f.GetFeatureClassifier().Name + _separator + f.Name);

    public static IEnumerable<EnumerationLiteral> Ordered(this IEnumerable<EnumerationLiteral> literals) =>
        literals.OrderBy(f => f.GetEnumeration().Name + _separator + f.Name);

    public static IEnumerable<Field> Ordered(this IEnumerable<Field> fields) =>
        fields.OrderBy(f => f.GetStructuredDataType().Name + _separator + f.Name);

    public static IEnumerable<Interface> Ordered(this IEnumerable<Interface> interfaces) =>
        interfaces.OrderBy(f => f.GetLanguage().Name + _separator + f.Name);

    public static IEnumerable<T> Ordered<T>(this IEnumerable<T> entities) where T : LanguageEntity =>
        entities.OrderBy(f => f.Name);

    public static IEnumerable<Language> Ordered(this IEnumerable<Language> languages) =>
        languages.OrderBy(f => f.Name);
}
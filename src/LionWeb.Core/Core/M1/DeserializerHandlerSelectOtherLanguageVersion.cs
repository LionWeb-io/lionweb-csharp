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

/// Hosts logic to find the closest matching language version in <see cref="IDeserializerHandler.SelectVersion{T}"/>.
public static class DeserializerHandlerSelectOtherLanguageVersion
{
    /// <inheritdoc cref="SelectVersion{T}(LionWeb.Core.M1.CompressedMetaPointer,System.Collections.Generic.List{LionWeb.Core.M3.Language},Comparer{Language})"/>
    public static T? SelectVersion<T>(CompressedMetaPointer metaPointer, List<Language> languages)
        where T : class, IKeyed =>
        SelectVersion<T>(metaPointer, languages, Comparer<Language>.Create(DefaultLanguageComparer));

    /// <summary>
    /// Chooses the <typeparamref name="T"/> with the same key as <paramref name="metaPointer"/>
    /// from the <paramref name="languages">language</paramref> with the
    /// <paramref name="languageComparer">first</paramref> version.
    /// </summary>
    /// <param name="metaPointer">Unresolvable meta-pointer.</param>
    /// <param name="languages">Languages with same key as <paramref name="metaPointer"/>.</param>
    /// <param name="languageComparer">Comparer to select the preferred language.
    /// Make sure to invert the result for choosing the latest version.</param>
    /// <typeparam name="T">Kind of language element we're looking for.</typeparam>
    public static T? SelectVersion<T>(CompressedMetaPointer metaPointer, List<Language> languages,
        Comparer<Language> languageComparer)
        where T : class, IKeyed
    {
        IEnumerable<IKeyed> keyed = typeof(T) switch
        {
            { } f when f == typeof(Feature) => languages
                .SelectMany(l => l.Entities.OfType<Classifier>())
                .SelectMany(c => c.Features),
            { } el when el == typeof(EnumerationLiteral) => languages
                .SelectMany(l => l.Entities.OfType<Enumeration>())
                .SelectMany(e => e.Literals),
            _ => languages
                .SelectMany(l => l.Entities.OfType<T>())
        };

        var candidates = keyed
            .Where(k => CompressedId.Create(k.Key, false).Equals(metaPointer.Key))
            .OrderBy(f => f.GetLanguage(), languageComparer);

        return candidates
            .FirstOrDefault() as T;
    }

    /// Compares <paramref name="a"/>'s and <paramref name="b"/>'s <see cref="Language.Version"/> by means of <see cref="Version"/>,
    /// i.e. dotted-number (<c>11.22.33.44</c>).
    public static int DefaultLanguageComparer(Language a, Language b)
    {
        if (Version.TryParse(a.Version, out Version? aVersion) && Version.TryParse(b.Version, out Version? bVersion))
        {
            return -aVersion.CompareTo(bVersion);
        }

        return -string.Compare(a.Version, b.Version, StringComparison.Ordinal);
    }
}
﻿// Copyright 2025 TRUMPF Laser SE and other contributors
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

using M1;
using M2;
using M3;
using System.Diagnostics.CodeAnalysis;

/// Extensions useful to work with <see cref="LenientNode"/>s during Migration.
public static class MigrationExtensions
{
    /// All <see cref="M1Extensions.Descendants"/> of <paramref name="nodes"/>, including annotations.
    public static IEnumerable<LenientNode> Descendants(this List<LenientNode> nodes) =>
        nodes.SelectMany(Descendants);

    /// All <see cref="M1Extensions.Descendants"/> of <paramref name="node"/>, including annotations.
    public static IEnumerable<LenientNode> Descendants(this LenientNode node) =>
        M1Extensions.Descendants(node, true, true);

    /// Collects all languages used in <paramref name="nodes"/> via their classifier + generalizations, features, types,
    /// and instance values (for <see cref="EnumerationLiteral"/>).  
    public static IEnumerable<DynamicLanguage> CollectUsedLanguages(List<LenientNode> nodes)
    {
        HashSet<Language> result = [];

        foreach (var node in nodes.Descendants())
            CollectUsedLanguages(node.GetClassifier(), result);

        return result
            .Where(l => l is not IBuiltInsLanguage and not ILionCoreLanguage)
            .Cast<DynamicLanguage>();
    }

    private static void CollectUsedLanguages(Classifier? classifier, HashSet<Language> alreadyCollected)
    {
        if (classifier == null || alreadyCollected.Contains(classifier.GetLanguage()))
            return;

        alreadyCollected.Add(classifier.GetLanguage());
        foreach (var feature in classifier.Features)
            CollectUsedLanguages(feature, alreadyCollected);

        switch (classifier)
        {
            case Annotation a:
                CollectUsedLanguages(a.Annotates, alreadyCollected);
                CollectUsedLanguages(a.Extends, alreadyCollected);
                foreach (var iface in a.Implements)
                    CollectUsedLanguages(iface, alreadyCollected);
                break;

            case Concept c:
                CollectUsedLanguages(c.Extends, alreadyCollected);
                foreach (var iface in c.Implements)
                    CollectUsedLanguages(iface, alreadyCollected);
                break;

            case Interface i:
                foreach (var iface in i.Extends)
                    CollectUsedLanguages(iface, alreadyCollected);
                break;
        }
    }

    private static void CollectUsedLanguages(Feature? feature, HashSet<Language> alreadyCollected)
    {
        if (feature == null)
            return;

        alreadyCollected.Add(feature.GetLanguage());

        switch (feature)
        {
            case Property p:
                CollectUsedLanguages(p.Type, alreadyCollected);
                break;
            case Link l:
                CollectUsedLanguages(l.Type, alreadyCollected);
                break;
        }
    }

    private static void CollectUsedLanguages(Datatype? datatype, HashSet<Language> alreadyCollected)
    {
        if (datatype == null)
            return;

        alreadyCollected.Add(datatype.GetLanguage());

        if (datatype is not StructuredDataType s)
            return;

        foreach (var field in s.Fields)
            CollectUsedLanguages(field.Type, alreadyCollected);
    }

    internal static bool TryLookup<T>(string key, Language language, [NotNullWhen(true)] out T? result) where T : IKeyed
    {
        var mapped = M1Extensions
            .Descendants<IKeyed>(language, true)
            .FirstOrDefault(k => k.Key == key);

        if (mapped is T r)
        {
            result = r;
            return true;
        }

        result = default;
        return false;
    }
}
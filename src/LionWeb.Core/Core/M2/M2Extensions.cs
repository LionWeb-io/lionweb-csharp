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

namespace LionWeb.Core.M2;

using M1;
using M3;
using System.Collections;
using System.Collections.Immutable;
using Utilities;

/// <summary>
/// Extension methods for LionCore M3 types of a "query-like" nature.
/// </summary>
public static class M2Extensions
{
    /// <summary>
    /// Searches the things of type <typeparamref name="T"/> within the given <paramref name="ts"/>
    /// by the given <paramref name="key"/> (from <see cref="IKeyed"/>).
    /// </summary>
    /// <param name="ts">Things to search through.</param>
    /// <param name="key">Key of the requested thing.</param>
    /// <returns>A <typeparamref name="T"/> with the given key, or:</returns>
    /// <exception cref="KeyNotFoundException">If the given <paramref name="ts"/> does not contain a thing with the given key.</exception>
    private static T FindByKey<T>(this IEnumerable<T> ts, string key) where T : IKeyed
    {
        T? result = ts.FirstOrDefault(t => t.Key == key);
        if (result != null)
            return result;

        throw new KeyNotFoundException($"could not find element with key=\"{key}\"");
    }

    /// <summary>
    /// Searches the things of type <typeparamref name="T"/> within the given <paramref name="language"/>
    /// by the given <paramref name="key"/> (from <see cref="IKeyed"/>).
    /// </summary>
    /// <param name="language">Language to search through.</param>
    /// <param name="key">Key of the requested thing.</param>
    /// <returns>A <typeparamref name="T"/> with the given key, or:</returns>
    /// <exception cref="KeyNotFoundException">If the given <paramref name="language"/> does not contain a thing with the given key.</exception>
    public static T FindByKey<T>(this Language language, string key) where T : IKeyed =>
        M1Extensions.Descendants<IKeyed>(language, true, true)
            .OfType<T>()
            .FindByKey(key);

    /// <summary>
    /// Returns the classifier with <paramref name="key"/> contained in <paramref name="language"/>.
    /// Does <i>not</i> find classifiers from dependent languages.
    /// </summary>
    /// <param name="language">Language to search through.</param>
    /// <param name="key">Key of the requested classifier.</param>
    /// <returns>Classifier with <paramref name="key"/> contained in <paramref name="language"/>.</returns>
    /// <exception cref="KeyNotFoundException">If <paramref name="language"/> does not contain a classifier with <paramref name="key"/>.</exception>
    public static Classifier ClassifierByKey(this Language language, string key)
        => language
            .Entities
            .OfType<Classifier>()
            .FindByKey(key);

    /// <summary>
    /// Returns the feature with <paramref name="key"/> contained in <paramref name="classifier"/> or any of its generalizations.
    /// </summary>
    /// <param name="classifier">Classifier to search through.</param>
    /// <param name="key">Key of requested feature.</param>
    /// <returns>Feature with <paramref name="key"/> contained in <paramref name="classifier"/> or any of its generalizations.</returns>
    /// <exception cref="InvalidOperationException">If <paramref name="classifier"/> or any of its generalizations does not contain a feature with <paramref name="key"/>.</exception>
    public static Feature FeatureByKey(this Classifier classifier, string key)
        => classifier
            .AllFeatures()
            .FindByKey(key);

    /// <summary>
    /// Enumerates all features of <paramref name="classifier"/> and all its generalizations (aka supertypes).
    /// </summary>
    /// <param name="classifier">Classifier to find features of.</param>
    /// <returns>All features of <paramref name="classifier"/> and all its generalizations.</returns>
    public static ISet<Feature> AllFeatures(this Classifier classifier) =>
        classifier.AllGeneralizations()
            .Prepend(classifier)
            .SelectMany(c => c.Features)
            .ToImmutableHashSet();

    /// <summary>
    /// Enumerates all direct generalizations (aka supertypes) of <paramref name="classifier"/>. 
    /// </summary>
    /// <param name="classifier">Classifier to find generalizations of.</param>
    /// <returns>All direct generalizations of <paramref name="classifier"/>.</returns>
    /// <exception cref="UnsupportedClassifierException">If <paramref name="classifier"/>'s type is unsupported (should not happen).</exception>
    /// <seealso cref="DirectGeneralizations(Annotation)"/>
    /// <seealso cref="DirectGeneralizations(Concept)"/>
    /// <seealso cref="DirectGeneralizations(Interface)"/>
    public static ISet<Classifier> DirectGeneralizations(this Classifier classifier) =>
        classifier switch
        {
            Annotation a => a.DirectGeneralizations(),
            Concept c => c.DirectGeneralizations(),
            Interface i => i.DirectGeneralizations(),
            _ => throw new UnsupportedClassifierException(classifier)
        };

    /// <summary>
    /// Enumerates all direct generalizations (aka supertypes) of <paramref name="annotation"/>. 
    /// </summary>
    /// <param name="annotation">Annotation to find generalizations of.</param>
    /// <returns><paramref name="annotation"/>'s <i>extended</i> Annotation and <i>implemented</i> Interfaces.</returns>
    public static ISet<Classifier> DirectGeneralizations(this Annotation annotation)
    {
        var result = new List<Classifier>();
        if (annotation.Extends != null)
        {
            result.Add(annotation.Extends);
        }

        result.AddRange(annotation.Implements);
        return result.ToImmutableHashSet();
    }

    /// <summary>
    /// Enumerates all direct generalizations (aka supertypes) of <paramref name="concept"/>.
    /// </summary>
    /// <param name="concept">Concept to find generalizations of.</param>
    /// <returns><paramref name="concept"/>'s <i>extended</i> Concept and <i>implemented</i> Interfaces.</returns>
    public static ISet<Classifier> DirectGeneralizations(this Concept concept)
    {
        var result = new List<Classifier>();
        if (concept.Extends != null)
        {
            result.Add(concept.Extends);
        }

        result.AddRange(concept.Implements);
        return result.ToImmutableHashSet();
    }

    /// <summary>
    /// Enumerates all direct generalizations (aka supertypes) of <paramref name="iface"/>.
    /// </summary>
    /// <param name="iface">Interface to find generalizations of.</param>
    /// <returns><paramref name="iface"/>'s <i>extended</i> Interfaces.</returns>
    public static ISet<Classifier> DirectGeneralizations(this Interface iface) =>
        iface.Extends.OfType<Classifier>().ToImmutableHashSet();

    /// <summary>
    /// Enumerates all direct and indirect generalizations (aka supertypes) of <paramref name="classifier"/>.
    /// Optionally includes <paramref name="classifier"/>.
    /// </summary>
    /// <param name="classifier">Classifier to find generalizations of.</param>
    /// <param name="includeSelf">If <c>true</c>, the result includes <paramref name="classifier"/>.</param>
    /// <returns>All direct and indirect generalizations of <paramref name="classifier"/>.</returns>
    /// <exception cref="UnsupportedClassifierException">If <paramref name="classifier"/>'s type is unsupported (should not happen).</exception>
    /// <seealso cref="DirectGeneralizations(Classifier)"/>
    public static ISet<Classifier> AllGeneralizations(this Classifier classifier, bool includeSelf = false)
    {
        IEnumerable<Classifier> result = CollectGeneralizations(classifier, new HashSet<Classifier>());

        if (!includeSelf)
            result = result.Except([classifier]);

        return result.ToImmutableHashSet();
    }

    private static IEnumerable<Classifier> CollectGeneralizations(Classifier basis, ISet<Classifier> processed)
    {
        if (!processed.Add(basis))
        {
            return [];
        }

        return basis.DirectGeneralizations()
            .SelectMany(g => CollectGeneralizations(g, processed))
            .Prepend(basis);
    }

    /// <summary>
    /// Enumerates all direct specializations (aka subtypes) of <paramref name="classifier"/> within <paramref name="languages"/>.
    /// </summary>
    /// <param name="classifier">Classifier to find specializations of.</param>
    /// <param name="languages">Languages to search through for specializations of <paramref name="classifier"/>.</param>
    /// <returns>All direct specializations of <paramref name="classifier"/> within <paramref name="languages"/>.</returns>
    public static ISet<Classifier> DirectSpecializations(this Classifier classifier,
        IEnumerable<Language> languages)
    {
        ILookup<Classifier, Classifier> directSpecializations = MapAllSpecializations(languages);

        return directSpecializations[classifier]
            .Except([classifier])
            .ToImmutableHashSet();
    }

    private static ILookup<Classifier, Classifier> MapAllSpecializations(IEnumerable<Language> languages) =>
        languages
            .SelectMany(l => l
                .Entities
                .OfType<Classifier>())
            .SelectMany(basis => basis
                .DirectGeneralizations()
                .Select(super => (basis, super)))
            .ToLookup(p => p.super, p => p.basis);

    /// <summary>
    /// Enumerates all direct and indirect specializations (aka subtypes) of <paramref name="classifier"/> within <paramref name="languages"/>.
    /// Optionally includes <paramref name="classifier"/>.
    /// </summary>
    /// <param name="classifier">Classifier to find specializations of.</param>
    /// <param name="languages">Languages to search through for specializations of <paramref name="classifier"/>.</param>
    /// <param name="includeSelf">If <c>true</c>, the result includes <paramref name="classifier"/>.</param>
    /// <returns>All direct and indirect specializations of <paramref name="classifier"/> within <paramref name="languages"/>.</returns>
    public static ISet<Classifier> AllSpecializations(this Classifier classifier,
        IEnumerable<Language> languages, bool includeSelf = false)
    {
        ILookup<Classifier, Classifier> directSpecializations = MapAllSpecializations(languages);

        IEnumerable<Classifier> result =
            CollectSpecializations(classifier, directSpecializations, new HashSet<Classifier>());

        if (!includeSelf)
            result = result.Except([classifier]);

        return result.ToImmutableHashSet();
    }

    private static IEnumerable<Classifier> CollectSpecializations(Classifier basis,
        ILookup<Classifier, Classifier> directSpecializations, ISet<Classifier> processed)
    {
        if (!processed.Add(basis))
        {
            return [];
        }

        return directSpecializations[basis]
            .SelectMany(c => CollectSpecializations(c, directSpecializations, processed))
            .Prepend(basis);
    }

    /// <summary>
    /// Enumerates all Enumerations defined by <paramref name="language"/>.
    /// </summary>
    /// <param name="language">Language to search through for Enumerations.</param>
    /// <returns>All Enumerations defined by <paramref name="language"/>.</returns>
    public static IReadOnlyList<Enumeration> Enumerations(this Language language) =>
        language
            .Entities
            .OfType<Enumeration>()
            .ToList()
            .AsReadOnly();

    /// <summary>
    /// Returns the Language that contains <paramref name="keyed"/>.
    /// </summary>
    /// <param name="keyed">Language element to find the language of.</param>
    /// <returns>Language that contains <paramref name="keyed"/>.</returns>
    /// <exception cref="UnsupportedNodeTypeException">If <paramref name="keyed"/> is directly or indirectly contained in an unsupported type is unsupported (should not happen).</exception>
    public static Language GetLanguage(this IKeyed keyed) =>
        keyed switch
        {
            Language l => l,
            _ => keyed.GetParent() is IKeyed k
                ? k.GetLanguage()
                : throw new UnsupportedNodeTypeException(keyed, nameof(keyed))
        };

    /// <summary>
    /// Returns the classifier that defines <paramref name="feature"/>.
    /// </summary>
    /// <param name="feature">Feature to find defining classifier of.</param>
    /// <returns>Classifier that defines <paramref name="feature"/>.</returns>
    public static Classifier GetFeatureClassifier(this Feature feature) =>
        (Classifier)feature.GetParent()!;

    /// <summary>
    /// Returns the Enumeration that contains <paramref name="literal"/>. 
    /// </summary>
    /// <param name="literal">EnumerationLiteral to find Enumeration of.</param>
    /// <returns>Enumeration that contains <paramref name="literal"/>.</returns>
    public static Enumeration GetEnumeration(this EnumerationLiteral literal) =>
        (Enumeration)literal.GetParent()!;

    /// <summary>
    /// Returns the structured datatype with <paramref name="key"/> contained in <paramref name="language"/>.
    /// Does <i>not</i> find structured datatypes from dependent languages.
    /// </summary>
    /// <param name="language">Language to search through.</param>
    /// <param name="key">Key of the requested structured datatype.</param>
    /// <returns>Structured DataType with <paramref name="key"/> contained in <paramref name="language"/>.</returns>
    /// <exception cref="KeyNotFoundException">If <paramref name="language"/> does not contain a structured datatype with <paramref name="key"/>.</exception>
    public static StructuredDataType StructuredDataTypeByKey(this Language language, string key)
        => language
            .Entities
            .OfType<StructuredDataType>()
            .FindByKey(key);

    /// <summary>
    /// Returns the field with <paramref name="key"/> contained in <paramref name="structuredDataType"/>.
    /// </summary>
    /// <param name="structuredDataType">Structured DataType to search through.</param>
    /// <param name="key">Key of requested field.</param>
    /// <returns>Field with <paramref name="key"/> contained in <paramref name="structuredDataType"/>.</returns>
    /// <exception cref="InvalidOperationException">If <paramref name="structuredDataType"/> does not contain a field with <paramref name="key"/>.</exception>
    public static Field FieldByKey(this StructuredDataType structuredDataType, string key)
        => structuredDataType
            .Fields
            .FindByKey(key);
    
    /// <summary>
    /// Returns the StructuredDataType that contains <paramref name="field"/>. 
    /// </summary>
    /// <param name="field">Field to find StructuredDataType of.</param>
    /// <returns>StructuredDataType that contains <paramref name="field"/>.</returns>
    public static StructuredDataType GetStructuredDataType(this Field field) =>
        (StructuredDataType)field.GetParent()!;

    /// <summary>
    /// Returns the type of <paramref name="feature"/>,
    /// i.e. a Datatype if <paramref name="feature"/> is a Property, a Classifier if <paramref name="feature"/> is a Link.
    /// </summary>
    /// <param name="feature">Feature to find type of.</param>
    /// <returns>Type of <paramref name="feature"/>.</returns>
    /// <exception cref="UnsupportedNodeTypeException">If <paramref name="feature"/> is neither a Property nor a Link (should not happen).</exception>
    public static LanguageEntity GetFeatureType(this Feature feature) => feature switch
    {
        Property p => p.Type,
        Link l => l.Type,
        _ => throw new UnsupportedNodeTypeException(feature, nameof(feature))
    };

    /// <summary>
    /// Checks if <paramref name="candidate"/> can have <paramref name="annotation"/> as annotation.
    /// </summary>
    /// <param name="annotation">Annotation to check.</param>
    /// <param name="candidate">Classifier to check.</param>
    /// <returns><c>true</c> if <paramref name="annotation"/> can annotate <paramref name="candidate"/>, <c>false</c> otherwise.</returns>
    public static bool CanAnnotate(this Annotation annotation, Classifier candidate) =>
        ReferenceEquals(annotation.Annotates, annotation.GetLanguage().LionWebVersion.BuiltIns.Node) ||
        candidate.AllGeneralizations(true).Contains(annotation.Annotates, new LanguageEntityIdentityComparer());

    /// <summary>
    /// Checks if <paramref name="candidate"/> is a generalization (aka supertype) of <paramref name="basis"/>.
    /// If <paramref name="includeSelf"/> is <c>true</c>, returns <c>true</c> if <paramref name="candidate"/> == <paramref name="basis"/>. 
    /// </summary>
    /// <param name="basis">Base classifier (aka specialization, aka subtype) to check.</param>
    /// <param name="candidate">Potential generalization (aka supertype) to check.</param>
    /// <param name="includeSelf">If <c>true</c>, <paramref name="basis"/> is considered its own generalization.</param>
    /// <returns><c>true</c> if <paramref name="candidate"/> is a generalization of <paramref name="basis"/>, <c>false</c> otherwise.</returns>
    public static bool IsGeneralization(this Classifier basis, Classifier candidate, bool includeSelf = true) =>
        basis.AllGeneralizations(includeSelf).Contains(candidate, new LanguageEntityIdentityComparer());

    /// <summary>
    /// Re-types <paramref name="value"/> as IEnumerable&lt;<typeparamref name="T"/>&gt;
    /// </summary>
    /// <param name="link"><paramref name="value"/>'s origin Link.</param>
    /// <param name="value">Untyped <paramref name="link"/> value.</param>
    /// <typeparam name="T">Type of nodes in <paramref name="value"/>.</typeparam>
    /// <returns><paramref name="value"/> re-typed as IEnumerable&lt;<typeparamref name="T"/>&gt;.</returns>
    /// <exception cref="InvalidValueException">If <paramref name="value"/> cannot be re-typed as IEnumerable&lt;<typeparamref name="T"/>&gt;</exception>
    public static IEnumerable<T> AsNodes<T>(this Link link, object? value) where T : IReadableNode
        => (link.Multiple, value) switch
        {
            (true, IEnumerable e) => CastIterator<T>(link, e),
            (false, T n) => [n],
            var (_, v) => throw new InvalidValueException(link, v)
        };


    /// <summary>
    /// Re-types <paramref name="value"/> as IEnumerable&lt;<typeparamref name="T"/>&gt;
    /// </summary>
    /// <param name="value">Untyped value, convertible to <typeparamref name="T"/>.</param>
    /// <typeparam name="T">Type of nodes in <paramref name="value"/>.</typeparam>
    /// <returns><paramref name="value"/> re-typed as IEnumerable&lt;<typeparamref name="T"/>&gt;.</returns>
    /// <exception cref="InvalidValueException">If <paramref name="value"/> cannot be re-typed as IEnumerable&lt;<typeparamref name="T"/>&gt;</exception>
    public static IEnumerable<T> AsNodes<T>(object? value) where T : IReadableNode
        => value switch
        {
            IEnumerable e => CastIterator<T>(null, e),
            T n => [n],
            _ => throw new InvalidValueException(null, value)
        };

    private static IEnumerable<T> CastIterator<T>(Link? link, IEnumerable source)
    {
        foreach (var obj in source)
        {
            if (obj is T tt)
            {
                yield return tt;
            } else
            {
                throw new InvalidValueException(link, obj);
            }
        }
    }

    public static bool AreAllReadableNodes(IEnumerable enumerable) =>
        enumerable.Cast<object?>().All(o => o is IReadableNode and not INode);

    /// <summary>
    /// Checks whether all entries in <paramref name="enumerable"/> are of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="enumerable">Enumerable to check entries of.</param>
    /// <typeparam name="T">Type all entries of <paramref name="enumerable"/> should conform to.</typeparam>
    /// <returns><c>true</c> if all entries of <paramref name="enumerable"/> are of type <typeparamref name="T"/>; <c>false</c> otherwise.</returns>
    public static bool AreAll<T>(IEnumerable enumerable) =>
        enumerable.Cast<object?>().All(o => o is T);
}
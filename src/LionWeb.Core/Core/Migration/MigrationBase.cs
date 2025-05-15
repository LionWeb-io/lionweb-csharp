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
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Utilities;

/// Migrates instances from <see cref="OriginLanguageIdentity"/> to <see cref="DestinationLanguage"/>.
/// Implementers MUST use the setters in this class instead of <see cref="IWritableNode.Set"/> to avoid mixing languages of different migration rounds.
public abstract class MigrationBase<TDestinationLanguage> : IMigration where TDestinationLanguage : Language
{
    /// Language we migrate <i>from</i>
    /// <seealso cref="DestinationLanguage"/>
    protected readonly LanguageIdentity OriginLanguageIdentity;

    /// Language we migrate <i>to</i>
    /// <seealso cref="OriginLanguageIdentity"/> 
    protected readonly TDestinationLanguage DestinationLanguage;

    private ILanguageRegistry? _languageRegistry;

    /// Access to <see cref="IModelMigrator"/>'s <see cref="ILanguageRegistry"/>.
    /// <exception cref="IllegalMigrationStateException">If not set yet (i.e. before registering to an <see cref="IModelMigrator"/>).</exception>
    protected ILanguageRegistry LanguageRegistry
    {
        get => _languageRegistry ?? throw new IllegalMigrationStateException("LanguageRegistry is null");
        set => _languageRegistry = value;
    }

    /// <inheritdoc />
    public virtual int Priority { get; init; } = IMigration.DefaultPriority;

    /// Migrates instances from <paramref name="originLanguage"/> to <paramref name="destinationLanguage"/>.
    protected MigrationBase(LanguageIdentity originLanguage, TDestinationLanguage destinationLanguage)
    {
        OriginLanguageIdentity = originLanguage;
        DestinationLanguage = destinationLanguage;
    }

    /// Migrates instances from <tt>{ Key = <paramref name="destinationLanguage"/>.key, Version = <paramref name="originVersion"/> }</tt> to <paramref name="destinationLanguage"/>.
    protected MigrationBase(string originVersion, TDestinationLanguage destinationLanguage)
        : this(new LanguageIdentity(destinationLanguage.Key, originVersion), destinationLanguage)
    {
    }

    /// <inheritdoc />
    public virtual void Initialize(ILanguageRegistry languageRegistry)
    {
        LanguageRegistry = languageRegistry;
        RegisterDestinationLanguage();
    }

    private void RegisterDestinationLanguage()
    {
        if (OriginLanguageIdentity.Key != DestinationLanguage.Key ||
            OriginLanguageIdentity.Version == DestinationLanguage.Version)
            return;

        var destinationIdentity = LanguageIdentity.FromLanguage(DestinationLanguage);

        if (LanguageRegistry.TryGetLanguage(destinationIdentity, out _))
            return;

        if (DestinationLanguage is DynamicLanguage dynamicLang)
        {
            LanguageRegistry.RegisterLanguage(dynamicLang, OriginLanguageIdentity);
            return;
        }

        var dynamicDestination = new DynamicLanguageCloner(
                DestinationLanguage.LionWebVersion,
                LanguageRegistry.KnownLanguages.Where(l =>
                    l.Key != DestinationLanguage.Key || l.Version != DestinationLanguage.Version)
            )
            .Clone(DestinationLanguage);
        LanguageRegistry.RegisterLanguage(dynamicDestination);
    }

    /// <inheritdoc />
    /// <returns><c>true</c> if <paramref name="languageIdentities"/> includes <see cref="OriginLanguageIdentity"/>.</returns>
    public virtual bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
        languageIdentities.Contains(OriginLanguageIdentity);

    /// <inheritdoc />
    /// Executes <see cref="MigrateInternal"/> and afterward changes all usages of <see cref="OriginLanguageIdentity"/> to <see cref="DestinationLanguage"/>.
    public MigrationResult Migrate(List<LenientNode> inputRootNodes)
    {
        var result = MigrateInternal(inputRootNodes);
        if (!LanguageRegistry.TryGetLanguage(OriginLanguageIdentity, out var originLanguage))
            return result;

        if (originLanguage.Key == DestinationLanguage.Key && originLanguage.Version == DestinationLanguage.Version)
            return result;

        originLanguage.Key = DestinationLanguage.Key;
        originLanguage.Version = DestinationLanguage.Version;
        result = result with { Changed = true };

        return result;
    }

    /// Executes the actual migration.
    protected abstract MigrationResult MigrateInternal(List<LenientNode> inputRootNodes);

    #region LanguageEntity helpers

    /// <inheritdoc cref="MigrationExtensions.AllInstancesOf"/>
    protected IEnumerable<LenientNode> AllInstancesOf(List<LenientNode> nodes, Classifier classifier) =>
        nodes
            .Descendants()
            .Where(n => IsInstanceOf(n, classifier));

    /// Whether <paramref name="node"/> is an instance of <paramref name="destinationClassifier"/>.
    /// Handles specialization correctly as long as all involved languages are <see cref="ILanguageRegistry.KnownLanguages">known</see>.
    /// This works better if <paramref name="destinationClassifier"/> is part of <see cref="DestinationLanguage"/>.
    protected bool IsInstanceOf(LenientNode node, Classifier destinationClassifier)
    {
        Classifier lookup = LookupAsDestination(node.GetClassifier());

        var languages = LanguageRegistry.KnownLanguages;
        var allSpecializations = LookupAsDestination(destinationClassifier).AllSpecializations(languages, true);

        return allSpecializations.Any(c => c.EqualsIdentity(lookup));
    }

    /// Sets <paramref name="node"/>'s <paramref name="property"/> to <paramref name="value"/>
    /// while taking care of different <see cref="DynamicLanguageCloner">language variants</see> during migration.
    protected void SetProperty(LenientNode node, Property property, object? value) =>
        node.Set(LookupAsOriginOrDestination(property), value);

    /// Tries to get <paramref name="property"/> from <paramref name="node"/> and returns it in <paramref name="value"/>.
    /// <returns><c>true</c> if <paramref name="property"/> is set in <paramref name="node"/>; <c>false</c> otherwise.</returns>
    protected bool TryGetProperty(LenientNode node, Property property, [NotNullWhen(true)] out object? value) =>
        node.TryGet(LookupAsOriginOrDestination(property), out value);

    /// Sets <paramref name="node"/>'s <paramref name="containment"/> to <paramref name="child"/>
    /// while taking care of different <see cref="DynamicLanguageCloner">language variants</see> and
    /// <see cref="ConvertSubtreeToLenient">node representations</see> during migration.
    protected void SetChild(LenientNode node, Containment containment, IWritableNode child) =>
        node.Set(LookupAsOriginOrDestination(containment), ConvertSubtreeToLenient(child));

    /// Tries to get <paramref name="containment"/> from <paramref name="node"/> and returns it in <paramref name="value"/>.
    /// <returns><c>true</c> if <paramref name="containment"/> is set with single node in <paramref name="node"/>; <c>false</c> otherwise.</returns>
    protected bool TryGetChild(LenientNode node, Containment containment, [NotNullWhen(true)] out LenientNode? value)
    {
        if (node.TryGet(LookupAsOriginOrDestination(containment), out var v))
        {
            switch (v)
            {
                case LenientNode n:
                    value = n;
                    return true;

                case IEnumerable e:
                    var nodes = e.Cast<LenientNode>().ToList();
                    if (nodes.Count == 1)
                    {
                        value = nodes.First();
                        return true;
                    }

                    value = null;
                    return false;
            }
        }

        value = null;
        return false;
    }

    /// Sets <paramref name="node"/>'s <paramref name="containment"/> to <paramref name="children"/>
    /// while taking care of different <see cref="DynamicLanguageCloner">language variants</see> and
    /// <see cref="ConvertSubtreeToLenient">node representations</see> during migration.
    protected void SetChildren(LenientNode node, Containment containment, IEnumerable<IWritableNode> children) =>
        node.Set(LookupAsOriginOrDestination(containment), children.Select(ConvertSubtreeToLenient));

    /// Tries to get <paramref name="containment"/> from <paramref name="node"/> and returns it in <paramref name="value"/>.
    /// <returns><c>true</c> if <paramref name="containment"/> is set with at least one node in <paramref name="node"/>; <c>false</c> otherwise.</returns>
    protected bool TryGetChildren(LenientNode node, Containment containment,
        [NotNullWhen(true)] out List<LenientNode>? value)
    {
        if (node.TryGet(LookupAsOriginOrDestination(containment), out var v))
        {
            switch (v)
            {
                case LenientNode n:
                    value = [n];
                    return true;

                case IEnumerable e:
                    value = e.Cast<LenientNode>().ToList();
                    return value.Count != 0;
            }
        }

        value = null;
        return false;
    }

    /// Sets <paramref name="node"/>'s <paramref name="reference"/> to <paramref name="target"/>
    /// while taking care of different <see cref="DynamicLanguageCloner">language variants</see> during migration.
    protected void SetReference(LenientNode node, Reference reference, IReadableNode target) =>
        node.Set(LookupAsOriginOrDestination(reference), target);

    /// Tries to get <paramref name="reference"/> from <paramref name="node"/> and returns it in <paramref name="value"/>.
    /// <returns><c>true</c> if <paramref name="reference"/> is set with single node in <paramref name="node"/>; <c>false</c> otherwise.</returns>
    protected bool TryGetReference(LenientNode node, Reference reference, [NotNullWhen(true)] out IReadableNode? value)
    {
        if (node.TryGet(LookupAsOriginOrDestination(reference), out var v))
        {
            switch (v)
            {
                case IReadableNode n:
                    value = n;
                    return true;

                case IEnumerable e:
                    var nodes = e.Cast<IReadableNode>().ToList();
                    if (nodes.Count == 1)
                    {
                        value = nodes.First();
                        return true;
                    }

                    value = null;
                    return false;
            }
        }

        value = null;
        return false;
    }

    /// Sets <paramref name="node"/>'s <paramref name="reference"/> to <paramref name="targets"/>
    /// while taking care of different <see cref="DynamicLanguageCloner">language variants</see> during migration.
    protected void SetReferences(LenientNode node, Reference reference, IEnumerable<IReadableNode> targets) =>
        node.Set(LookupAsOriginOrDestination(reference), targets);

    /// Tries to get <paramref name="reference"/> from <paramref name="node"/> and returns it in <paramref name="value"/>.
    /// <returns><c>true</c> if <paramref name="reference"/> is set with at least one node in <paramref name="node"/>; <c>false</c> otherwise.</returns>
    protected bool TryGetReferences(LenientNode node, Reference reference,
        [NotNullWhen(true)] out List<IReadableNode>? value)
    {
        if (node.TryGet(LookupAsOriginOrDestination(reference), out var v))
        {
            switch (v)
            {
                case IReadableNode n:
                    value = [n];
                    return true;

                case IEnumerable e:
                    value = e.Cast<IReadableNode>().ToList();
                    return value.Count != 0;
            }
        }

        value = null;
        return false;
    }

    /// Converts <paramref name="node"/> and all <see cref="MigrationExtensions.Descendants(LionWeb.Core.LenientNode)"/> to <see cref="LenientNode"/>s.
    protected LenientNode ConvertSubtreeToLenient(IReadableNode node) => node switch
    {
        LenientNode l => l,
        var n => ConvertToLenient(n)
    };

    /// Converts <paramref name="node"/> to a <see cref="LenientNode"/>, including all properties/property values, containments/children, and references.
    protected LenientNode ConvertToLenient(IReadableNode node)
    {
        var result = new LenientNode(node.GetId(), LookupAsOriginOrDestination(node.GetClassifier()));
        result.AddAnnotations(node.GetAnnotations().Select(ConvertSubtreeToLenient));
        foreach (Feature feature in node.CollectAllSetFeatures())
        {
            var value = node.Get(feature);
            switch (feature)
            {
                case Property:
                    result.Set(feature, value);
                    break;

                case Containment:
                    switch (value)
                    {
                        case IEnumerable enumerable:
                            result.Set(feature, enumerable.Cast<IWritableNode>().Select(ConvertSubtreeToLenient));
                            break;
                        case IWritableNode writableNode:
                            result.Set(feature, ConvertSubtreeToLenient(writableNode));
                            break;
                    }

                    break;

                case Reference:
                    result.Set(feature, value);
                    break;
            }
        }

        return result;
    }

    #endregion

    #region Lookup

    /// Finds the equivalent of <paramref name="keyed"/>
    /// <list type="bullet">
    /// <item>within <see cref="OriginLanguageIdentity"/>, if <paramref name="keyed"/>'s language's key matches <see cref="OriginLanguageIdentity"/>'s key;</item>
    /// <item>otherwise, within <see cref="ILanguageRegistry.KnownLanguages"/>.</item>
    /// </list>
    /// <exception cref="UnknownLookupException">If no equivalent can be found for <paramref name="keyed"/>.</exception>
    protected T LookupAsOrigin<T>(T keyed) where T : IKeyed
    {
        if (keyed.GetLanguage().Key == OriginLanguageIdentity.Key &&
            LanguageRegistry.TryLookup<T>(keyed.Key, OriginLanguageIdentity, out var originResult))
            return originResult;

        return LanguageRegistry.Lookup(keyed);
    }

    /// Finds the equivalent of <paramref name="keyed"/>
    /// <list type="bullet">
    /// <item>within <see cref="DestinationLanguage"/>, if <paramref name="keyed"/>'s language's key matches <see cref="DestinationLanguage"/>'s key;</item>
    /// <item>otherwise, within <see cref="ILanguageRegistry.KnownLanguages"/>.</item>
    /// </list>
    /// <exception cref="UnknownLookupException">If no equivalent can be found for <paramref name="keyed"/>.</exception>
    protected T LookupAsDestination<T>(T keyed) where T : IKeyed
    {
        var destinationIdentity = LanguageIdentity.FromLanguage(DestinationLanguage);
        if (keyed.GetLanguage().Key == DestinationLanguage.Key &&
            LanguageRegistry.TryLookup<T>(keyed.Key, destinationIdentity, out var destinationResult))
            return destinationResult;

        return LanguageRegistry.Lookup(keyed);
    }

    /// Finds the equivalent of <paramref name="keyed"/>
    /// <list type="number">
    /// <item>within <see cref="OriginLanguageIdentity"/>, if <paramref name="keyed"/>'s language's key matches <see cref="OriginLanguageIdentity"/>'s key;</item>
    /// <item>within <see cref="DestinationLanguage"/>, if <paramref name="keyed"/>'s language's key matches <see cref="DestinationLanguage"/>'s key;</item>
    /// <item>otherwise, within <see cref="ILanguageRegistry.KnownLanguages"/>.</item>
    /// </list>
    /// <exception cref="UnknownLookupException">If no equivalent can be found for <paramref name="keyed"/>.</exception>
    protected T LookupAsOriginOrDestination<T>(T keyed) where T : IKeyed
    {
        var keyedLanguage = keyed.GetLanguage();

        if (keyedLanguage.Key == OriginLanguageIdentity.Key &&
            LanguageRegistry.TryLookup<T>(keyed.Key, OriginLanguageIdentity, out var originResult))
            return originResult;

        var destinationIdentity = LanguageIdentity.FromLanguage(DestinationLanguage);
        if (keyedLanguage.Key == DestinationLanguage.Key &&
            LanguageRegistry.TryLookup<T>(keyed.Key, destinationIdentity, out var destinationResult))
            return destinationResult;

        return LanguageRegistry.Lookup(keyed);
    }

    #endregion

    /// <inheritdoc cref="IdUtils.NewId"/>
    protected virtual string NewId() =>
        IdUtils.NewId();

    /// <inheritdoc cref="CreateNode(DynamicClassifier,string?)"/>
    protected virtual LenientNode CreateNode(Classifier classifier, string? id = null)
    {
        if (classifier is not DynamicClassifier)
            classifier = LookupAsOriginOrDestination(classifier);

        if (classifier is not DynamicClassifier dynamicClassifier)
            throw new UnknownLookupException(classifier);

        return CreateNode(dynamicClassifier, id);
    }

    /// Creates a new node with classifier <paramref name="classifier"/> and node id <paramref name="id"/>,
    /// or <see cref="NewId"/>.
    protected virtual LenientNode CreateNode(DynamicClassifier classifier, string? id = null) =>
        new LenientNode(id ?? NewId(), classifier);
}
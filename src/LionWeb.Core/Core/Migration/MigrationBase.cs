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

using M3;
using System.Collections;
using Utilities;

/// Migrates instances from <paramref name="originLanguage"/> to <paramref name="targetLanguage"/>.
/// Implementers MUST use the setters in this class instead of <see cref="IWritableNode.Set"/> to avoid mixing languages of different migration rounds.
public abstract class MigrationBase<TTargetLanguage>(LanguageIdentity originLanguage, TTargetLanguage targetLanguage) : IMigration
    where TTargetLanguage : Language
{
    protected readonly LanguageIdentity OriginLanguageIdentity = originLanguage;
    protected readonly TTargetLanguage _targetLang = targetLanguage;

    private ILanguageRegistry? _languageRegistry;

    protected LionWebVersions LionWebVersion { get; init; } = LionWebVersions.Current;

    protected ILanguageRegistry LanguageRegistry
    {
        get => _languageRegistry ?? throw new ApplicationException("LanguageRegistry is null");
        set => _languageRegistry = value;
    }

    /// <inheritdoc />
    public virtual int Priority => IMigration.DefaultPriority;

    /// <inheritdoc />
    public virtual void Initialize(ILanguageRegistry languageRegistry) =>
        LanguageRegistry = languageRegistry;


    /// <inheritdoc />
    /// <returns><c>true</c> if <paramref name="languageIdentities"/> includes <see cref="originLanguage"/>.</returns>
    public virtual bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
        languageIdentities.Contains(OriginLanguageIdentity);

    /// <inheritdoc />
    /// Executes <see cref="MigrateInternal"/> and afterward changes all usages of <see cref="originLanguage"/> to <see cref="targetLanguage"/>.
    public MigrationResult Migrate(List<LenientNode> inputRootNodes)
    {
        var result = MigrateInternal(inputRootNodes);
        if (!LanguageRegistry.TryGetLanguage(OriginLanguageIdentity, out var originLanguage))
        {
            return result;
        }

        if (originLanguage.Key == _targetLang.Key && originLanguage.Version == _targetLang.Version)
        {
            return result;
        }

        originLanguage.Key = _targetLang.Key;
        originLanguage.Version = _targetLang.Version;
        result = result with { Changed = true };

        return result;
    }

    /// Executes the actual migration.
    protected abstract MigrationResult MigrateInternal(List<LenientNode> inputRootNodes);

    #region Keyed identity helpers

    protected bool TryGetProperty(LenientNode node, FeatureIdentity featureIdentity, out object? value)
    {
        var property = TempProperty(featureIdentity);

        if (node.CollectAllSetFeatures().Contains(property, new FeatureIdentityComparer()))
        {
            value = node.Get(property);
            return true;
        }

        value = null;
        return false;
    }

    protected void SetProperty(LenientNode node, FeatureIdentity featureIdentity, object? value)
    {
        var property = TempProperty(featureIdentity);
        node.Set(property, value);
    }

    protected bool TryGetContainment(LenientNode node, FeatureIdentity featureIdentity, out List<LenientNode> children)
    {
        var containment = TempContainment(featureIdentity);

        if (node.CollectAllSetFeatures().Contains(containment, new FeatureIdentityComparer()))
        {
            children = (node.Get(containment) as IEnumerable)?.Cast<LenientNode>().ToList()!;
            return true;
        }

        children = [];
        return false;
    }

    protected void SetContainment(LenientNode node, FeatureIdentity featureIdentity, List<LenientNode> children)
    {
        var containment = TempContainment(featureIdentity);
        node.Set(containment, children);
    }

    protected bool TryGetReference(LenientNode node, FeatureIdentity featureIdentity, out List<IReadableNode> targets)
    {
        var reference = TempReference(featureIdentity);

        if (node.CollectAllSetFeatures().Contains(reference, new FeatureIdentityComparer()))
        {
            targets = (node.Get(reference) as IEnumerable)?.Cast<IReadableNode>().ToList()!;
            return true;
        }

        targets = [];
        return false;
    }

    protected void SetReference(LenientNode node, FeatureIdentity featureIdentity, List<IReadableNode> targets)
    {
        var reference = TempReference(featureIdentity);
        node.Set(reference, targets);
    }

    private DynamicProperty TempProperty(FeatureIdentity featureIdentity)
    {
        var classifier = TempClassifier(featureIdentity.Classifier, featureIdentity.Key + "-Concept");

        var existing = classifier
            .Features
            .OfType<DynamicProperty>()
            .FirstOrDefault(f => f.Key == featureIdentity.Key);
        if (existing != null)
            return existing;

        return new(featureIdentity.Key, LionWebVersion, classifier)
        {
            Key = featureIdentity.Key, Name = "Reference-" + featureIdentity.Key, Optional = true
        };
    }

    private DynamicContainment TempContainment(FeatureIdentity featureIdentity)
    {
        var classifier = TempClassifier(featureIdentity.Classifier, featureIdentity.Key + "-Concept");

        var existing = classifier
            .Features
            .OfType<DynamicContainment>()
            .FirstOrDefault(f => f.Key == featureIdentity.Key);
        if (existing != null)
            return existing;

        return new(featureIdentity.Key, LionWebVersion, classifier)
        {
            Key = featureIdentity.Key, Name = "Reference-" + featureIdentity.Key, Optional = true
        };
    }

    private DynamicReference TempReference(FeatureIdentity featureIdentity)
    {
        var classifier = TempClassifier(featureIdentity.Classifier, featureIdentity.Key + "-Concept");

        var existing = classifier
            .Features
            .OfType<DynamicReference>()
            .FirstOrDefault(f => f.Key == featureIdentity.Key);
        if (existing != null)
            return existing;

        return new(featureIdentity.Key, LionWebVersion, classifier)
        {
            Key = featureIdentity.Key, Name = "Reference-" + featureIdentity.Key, Optional = true
        };
    }

    protected DynamicClassifier TempClassifier(ClassifierIdentity classifierIdentity, string? id = null)
    {
        var language = TempLanguage(classifierIdentity.Language, id != null ? id + "-Language" : null);

        var existing = language
            .Entities
            .OfType<DynamicClassifier>()
            .FirstOrDefault(e => e.Key == classifierIdentity.Key);
        if (existing != null)
            return existing;

        return new DynamicConcept(id ?? NewId(), LionWebVersion, language) { Key = classifierIdentity.Key };
    }

    protected DynamicLanguage TempLanguage(LanguageIdentity languageIdentity, string? id = null)
    {
        if (LanguageRegistry.TryGetLanguage(languageIdentity, out var language))
            return language;

        DynamicLanguage dynamicLanguage =
            new(id ?? NewId(), LionWebVersion) { Key = languageIdentity.Key, Version = languageIdentity.Version };
        LanguageRegistry.RegisterLanguage(dynamicLanguage);
        return dynamicLanguage;
    }

    protected LenientNode CreateNode(ClassifierIdentity classifierIdentity, string? id = null) =>
        CreateNode(TempClassifier(classifierIdentity), id);

    #endregion

    #region LanguageEntity helpers

    /// <inheritdoc cref="MigrationExtensions.AllInstancesOf"/>
    protected IEnumerable<LenientNode> AllInstancesOf(List<LenientNode> nodes, Classifier classifier) =>
        nodes
            .Descendants()
            .Where(n => classifier.EqualsIdentity(Lookup(n.GetClassifier())));

    /// <inheritdoc cref="ILanguageRegistry.Lookup{T}"/>
    protected T Lookup<T>(T keyed) where T : IKeyed =>
        LanguageRegistry.Lookup(keyed);

    /// Sets <paramref name="node"/>'s <paramref name="property"/> to <paramref name="value"/>
    /// while taking care of different <see cref="DynamicLanguageCloner">language variants</see> during migration.
    protected void SetProperty(IWritableNode node, Property property, object? value) =>
        node.Set(Lookup(property), value);

    /// Sets <paramref name="node"/>'s <paramref name="containment"/> to <paramref name="child"/>
    /// while taking care of different <see cref="DynamicLanguageCloner">language variants</see> and
    /// <see cref="ConvertSubtreeToLenient">node representations</see> during migration.
    protected void SetChild(LenientNode node, Containment containment, IWritableNode child) =>
        node.Set(Lookup(containment), ConvertSubtreeToLenient(child));

    /// Sets <paramref name="node"/>'s <paramref name="containment"/> to <paramref name="children"/>
    /// while taking care of different <see cref="DynamicLanguageCloner">language variants</see> and
    /// <see cref="ConvertSubtreeToLenient">node representations</see> during migration.
    protected void SetChildren(LenientNode node, Containment containment, IEnumerable<IWritableNode> children) =>
        node.Set(Lookup(containment), children.Select(ConvertSubtreeToLenient));

    /// Sets <paramref name="node"/>'s <paramref name="reference"/> to <paramref name="target"/>
    /// while taking care of different <see cref="DynamicLanguageCloner">language variants</see> during migration.
    protected void SetReference(LenientNode node, Reference reference, IReadableNode target) =>
        node.Set(Lookup(reference), target);

    /// Sets <paramref name="node"/>'s <paramref name="reference"/> to <paramref name="targets"/>
    /// while taking care of different <see cref="DynamicLanguageCloner">language variants</see> during migration.
    protected void SetReferences(LenientNode node, Reference reference, IEnumerable<IReadableNode> targets) =>
        node.Set(Lookup(reference), targets);

    
    /// Converts <paramref name="node"/> and all <see cref="MigrationExtensions.Descendants"/> to <see cref="LenientNode"/>s.
    protected LenientNode ConvertSubtreeToLenient(IReadableNode node) => node switch
    {
        LenientNode l => l,
        var n => ConvertToLenient(n)
    };

    /// Converts <paramref name="node"/> to a <see cref="LenientNode"/>, including all properties/property values, containments/children, and references.
    protected LenientNode ConvertToLenient(IReadableNode node)
    {
        var result = new LenientNode(node.GetId(), Lookup(node.GetClassifier()));
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

    /// <inheritdoc cref="IdUtils.NewId"/>
    protected virtual string NewId() =>
        IdUtils.NewId();

    /// Creates a new node with classifier <paramref name="classifier"/> and node id <paramref name="id"/>,
    /// or <see cref="NewId"/>.
    protected LenientNode CreateNode(Classifier classifier, string? id = null) =>
        new LenientNode(id ?? NewId(), classifier);
}
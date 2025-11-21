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

// ReSharper disable InconsistentNaming

namespace LionWeb.Core.M3;

using M2;
using Notification.Partition;
using Notification.Pipe;
using System.Diagnostics.CodeAnalysis;
using Utilities;

/// <inheritdoc cref="Language"/>
public abstract class LanguageBase<TNodeFactory>(NodeId id, LionWebVersions lionWebVersion)
    : ReadableNodeBase<IReadableNode>(id, null), Language<TNodeFactory>
    where TNodeFactory : INodeFactory
{
    /// This language's current factory.
    protected TNodeFactory _factory;

    /// <inheritdoc />
    public LionWebVersions LionWebVersion { get; } = lionWebVersion;

    /// <inheritdoc />
    protected override IBuiltInsLanguage _builtIns => new Lazy<IBuiltInsLanguage>(() => LionWebVersion.BuiltIns).Value;

    /// <inheritdoc />
    protected override ILionCoreLanguage _m3 => new Lazy<ILionCoreLanguage>(() => LionWebVersion.LionCore).Value;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
    [
        _builtIns.INamed_name,
        _m3.IKeyed_key,
        _m3.Language_version,
        _m3.Language_entities,
        _m3.Language_dependsOn
    ];

    /// <inheritdoc cref="IConceptInstance.GetClassifier()" />
    public override Classifier GetClassifier() => GetConcept();

    /// <inheritdoc />
    public Concept GetConcept() => _m3.Language;

    /// <inheritdoc />
    public override object Get(Feature feature)
    {
        if (feature == _builtIns.INamed_name)
            return Name;
        if (feature == _m3.IKeyed_key)
            return Key;
        if (feature == _m3.Language_version)
            return Version;
        if (feature == _m3.Language_entities)
            return Entities;
        if (feature == _m3.Language_dependsOn)
            return DependsOn;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }

    /// <inheritdoc />
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        if (_builtIns.INamed_name.EqualsIdentity(feature) && ((Language)this).TryGetName(out var name))
        {
            value = name;
            return true;
        }

        if (_m3.IKeyed_key.EqualsIdentity(feature) && ((Language)this).TryGetKey(out var key))
        {
            value = key;
            return true;
        }

        if (_m3.Language_version.EqualsIdentity(feature) && ((Language)this).TryGetVersion(out var version))
        {
            value = version;
            return true;
        }

        if (_m3.Language_entities.EqualsIdentity(feature) && ((Language)this).TryGetEntities(out var entities))
        {
            value = entities;
            return true;
        }

        if (_m3.Language_dependsOn.EqualsIdentity(feature) && ((Language)this).TryGetDependsOn(out var dependsOn))
        {
            value = dependsOn;
            return true;
        }

        value = null;
        return false;
    }

    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public bool TryGetName([NotNullWhen(true)] out string? name)
    {
        name = Name;
        return name != null;
    }

    /// <inheritdoc />
    public abstract MetaPointerKey Key { get; }

    /// <inheritdoc />
    public bool TryGetKey([NotNullWhen(true)] out MetaPointerKey? key)
    {
        key = Key;
        return key != null;
    }

    /// <inheritdoc />
    public abstract string Version { get; }

    /// <inheritdoc />
    public bool TryGetVersion([NotNullWhen(true)] out string? version)
    {
        version = Version;
        return version != null;
    }

    /// <inheritdoc />
    public abstract IReadOnlyList<LanguageEntity> Entities { get; }

    /// <inheritdoc />
    public abstract IReadOnlyList<Language> DependsOn { get; }

    /// <inheritdoc />
    public virtual TNodeFactory GetFactory() => _factory;

    /// <inheritdoc />
    public virtual void SetFactory(TNodeFactory factory) => _factory = factory;

    /// <inheritdoc />
    public INotificationSender? GetNotificationSender() => null;

    /// <inheritdoc />
    IPartitionNotificationProducer? IPartitionInstance.GetNotificationProducer() => null;
}
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

namespace LionWeb.Core.M3;

using M2;
using Notification;
using Notification.Partition;
using Notification.Partition.Emitter;
using Notification.Pipe;
using System.Collections;
using System.Collections.Immutable;
using Utilities;

/// <inheritdoc cref="Language"/>
public class DynamicLanguage(NodeId id, LionWebVersions lionWebVersion) : DynamicIKeyed(id, lionWebVersion), Language
{
    /// <inheritdoc />
    public LionWebVersions LionWebVersion { get; } = lionWebVersion;

    private string? _version;

    /// <inheritdoc />
    public string Version
    {
        get => _version ?? throw new UnsetFeatureException(_m3.Language_version);
        set => _version = value;
    }

    /// <inheritdoc />
    public bool TryGetVersion(out string? version)
    {
        version = _version;
        return version != null;
    }

    private readonly List<DynamicLanguageEntity> _entities = [];

    /// <inheritdoc />
    public IReadOnlyList<LanguageEntity> Entities => _entities.AsReadOnly();

    /// <inheritdoc cref="Entities"/>
    public void AddEntities(IEnumerable<DynamicLanguageEntity> entities)
    {
        var safeNodes = entities?.ToList();
        AssureNotNull(safeNodes, _m3.Language_entities);
        AssureNotNullMembers(safeNodes, _m3.Language_entities);
        if (_entities.SequenceEqual(safeNodes))
            return;
        foreach (var value in safeNodes)
        {
            ContainmentAddMultipleNotificationEmitter<DynamicLanguageEntity> emitter = new(_m3.Language_entities, this, value, _entities, null);
            emitter.CollectOldData();
            if (AddEntitiesRaw(value))
                emitter.Notify();
        }
    }

    /// <inheritdoc cref="Entities"/>
    public void InsertEntities(Index index, IEnumerable<DynamicLanguageEntity> entities)
    {
        AssureInRange(index, _entities);
        var safeNodes = entities?.ToList();
        AssureNotNull(safeNodes, _m3.Language_entities);
        AssureNoSelfMove(index, safeNodes, _entities);
        AssureNotNullMembers(safeNodes, _m3.Language_entities);
        foreach (var value in safeNodes)
        {
            ContainmentAddMultipleNotificationEmitter<DynamicLanguageEntity> emitter = new(_m3.Language_entities, this, value, _entities, index);
            emitter.CollectOldData();
            if (InsertEntitiesRaw(index++, value))
                emitter.Notify();
        }
    }

    /// <inheritdoc cref="Entities"/>
    public void RemoveEntities(IEnumerable<DynamicLanguageEntity> entities) =>
        RemoveSelfParent(entities?.ToList(), _entities, _m3.Language_entities);
    
    protected internal bool SetEntitiesRaw(List<DynamicLanguageEntity> nodes) => ExchangeChildrenRaw(nodes, _entities);
    protected internal bool AddEntitiesRaw(DynamicLanguageEntity? value) => AddChildRaw(value, _entities);
    private bool InsertEntitiesRaw(int index, DynamicLanguageEntity? value) => InsertChildRaw(index, value, _entities);
    private bool RemoveEntitiesRaw(DynamicLanguageEntity? value) => RemoveChildRaw(value, _entities);
    
    private readonly List<Language> _dependsOn = [];

    /// <inheritdoc />
    public IReadOnlyList<Language> DependsOn => _dependsOn.AsReadOnly();

    /// <inheritdoc cref="DependsOn"/>
    public void AddDependsOn(IEnumerable<Language> languages)
    {
        var safeNodes = languages.ToList();
        AssureNotNull(languages, _m3.Language_dependsOn);
        AssureNotNullMembers(safeNodes, _m3.Language_dependsOn);
        _dependsOn.AddRange(safeNodes);
    }

    /// <inheritdoc cref="DependsOn"/>
    public void InsertDependsOn(Index index, IEnumerable<Language> languages)
    {
        var safeNodes = languages.ToList();
        AssureNotNull(languages, _m3.Language_dependsOn);
        AssureNotNullMembers(safeNodes, _m3.Language_dependsOn);
        _dependsOn.InsertRange(index, safeNodes);
    }
    
    
    /// <inheritdoc cref="DependsOn"/>
    public void RemoveDependsOn(IEnumerable<Language> languages)
    {
        var safeNodes = languages.ToList();
        AssureNotNull(languages, _m3.Language_dependsOn);
        AssureNotNullMembers(safeNodes, _m3.Language_dependsOn);
        RemoveAll(safeNodes, _dependsOn, null);
    }

    /// <inheritdoc cref="IConceptInstance.GetClassifier()" />
    public override Classifier GetClassifier() => GetConcept();

    /// <inheritdoc />
    public Concept GetConcept() => _m3.Language;


    /// <inheritdoc />
    protected override bool DetachChild(INode child)
    {
        if (base.DetachChild(child))
        {
            return true;
        }

        var containment = GetContainmentOf(child);
        if (containment == _m3.Language_entities)
            return _entities.Remove((DynamicLanguageEntity)child);

        return false;
    }

    /// <inheritdoc />
    public override Containment? GetContainmentOf(INode child)
    {
        var result = base.GetContainmentOf(child);
        if (result != null)
            return result;

        if (child is LanguageEntity s && _entities.Contains(s))
            return _m3.Language_entities;

        return null;
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Language_version,
            _m3.Language_entities,
            _m3.Language_dependsOn
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_m3.Language_version == feature)
        {
            result = Version;
            return true;
        }

        if (_m3.Language_entities == feature)
        {
            result = Entities;
            return true;
        }

        if (_m3.Language_dependsOn == feature)
        {
            result = DependsOn;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool TryGetPropertyRaw(Property property, out object? value)
    {
        if (base.TryGetPropertyRaw(property, out value))
            return true;
        
        if (_m3.Language_version.EqualsIdentity(property))
        {
            value = _version;
            return true;
        }

        value = null;
        return false;
    }

    /// <inheritdoc />
    protected internal override bool TryGetContainmentsRaw(Containment containment, out IReadOnlyList<IReadableNode> nodes)
    {  
        if (base.TryGetContainmentsRaw(containment, out nodes))
            return true;

        if (_m3.Language_entities.EqualsIdentity(containment))
        {
            nodes = _entities;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool TryGetReferencesRaw(Reference reference, out IReadOnlyList<IReferenceTarget> targets)
    {
        if (base.TryGetReferencesRaw(reference, out targets))
            return true;

        if (_m3.Language_dependsOn.EqualsIdentity(reference))
        {
            targets = _dependsOn.Select(ReferenceTarget.FromNode).ToImmutableList();
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool SetPropertyRaw(Property property, object? value)
    {
        if (base.SetPropertyRaw(property, value))
            return true;
        
        if (_m3.Language_version.EqualsIdentity(property) && value is null or string)
        {
            _version = (string?)value;
            return true;
        }
        
        return false;
    }

    /// <inheritdoc />
    protected internal override bool AddContainmentsRaw(Containment containment, IWritableNode node)
    {
        if (base.AddContainmentsRaw(containment, node))
            return true;
        
        if (_m3.Language_entities.EqualsIdentity(containment) && node is DynamicLanguageEntity entity)
        {
            return AddEntitiesRaw(entity);
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool AddReferencesRaw(Reference reference, ReferenceTarget target)
    {
        if (base.AddReferencesRaw(reference, target))
            return true;
        
        if (_m3.Language_dependsOn.EqualsIdentity(reference)&& target.Target is Language language)
        {
            _dependsOn.Add(language);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool InsertContainmentsRaw(Containment containment, Index index, IWritableNode node)
    {
        if (base.InsertContainmentsRaw(containment, index, node))
            return true;
        
        if (_m3.Language_entities.EqualsIdentity(containment) && node is DynamicLanguageEntity entity)
        {
            return InsertEntitiesRaw(index, entity);
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool InsertReferencesRaw(Reference reference, Index index, ReferenceTarget target)
    {
        if (base.InsertReferencesRaw(reference, index, target))
            return true;
        
        if (_m3.Language_dependsOn.EqualsIdentity(reference)&& target.Target is Language language)
        {
            _dependsOn.Insert(index, language);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool RemoveContainmentsRaw(Containment containment, IWritableNode node)
    {
        if (base.RemoveContainmentsRaw(containment, node))
            return true;
        
        if (_m3.Language_entities.EqualsIdentity(containment) && node is DynamicLanguageEntity entity)
        {
            return RemoveEntitiesRaw(entity);
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool RemoveReferencesRaw(Reference reference, ReferenceTarget target)
    {
        if (base.RemoveReferencesRaw(reference, target))
            return true;
        
        if (_m3.Language_dependsOn.EqualsIdentity(reference)&& target.Target is Language language)
        {
            return _dependsOn.Remove(language);
        }

        return false;
    }

    /// <inheritdoc />
    protected override bool SetInternal(Feature? feature, object? value, INotificationId? notificationId = null)
    {
        var result = base.SetInternal(feature, value);
        if (result)
        {
            return result;
        }

        if (_m3.Language_version == feature)
        {
            Version = value switch
            {
                string str => str,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (_m3.Language_entities == feature)
        {
            switch (value)
            {
                case IEnumerable e:
                    RemoveSelfParent(_entities?.ToList(), _entities, _m3.Language_entities);
                    AddEntities(e.OfType<DynamicLanguageEntity>().ToArray());
                    return true;
                default:
                    throw new InvalidValueException(feature, value);
            }
        }

        if (_m3.Language_dependsOn == feature)
        {
            switch (value)
            {
                case IEnumerable e:
                    _dependsOn.Clear();
                    _dependsOn.AddRange(e.OfType<Language>());
                    return true;
                default:
                    throw new InvalidValueException(feature, value);
            }
        }

        return false;
    }
    
    /// <inheritdoc />
    protected override bool AddInternal(Link? link, IEnumerable<IReadableNode> nodes)
    {
        if (base.AddInternal(link, nodes))
            return true;

        if (_m3.Language_entities.EqualsIdentity(link))
        {
            AddEntities(_m3.Language_entities.AsNodes<DynamicLanguageEntity>(nodes).ToList());
            return true;
        }

        if (_m3.Language_dependsOn.EqualsIdentity(link))
        {
            AddDependsOn(_m3.Language_dependsOn.AsNodes<Language>(nodes).ToList());
            return true;
        }

        return false;
    }
    
    /// <inheritdoc />
    protected override bool InsertInternal(Link? link, Index index, IEnumerable<IReadableNode> nodes)
    {
        if (base.InsertInternal(link, index, nodes))
            return true;

        if (_m3.Language_entities.EqualsIdentity(link))
        {
            InsertEntities(index, _m3.Language_entities.AsNodes<DynamicLanguageEntity>(nodes).ToList());
            return true;
        }
        
        if (_m3.Language_dependsOn.EqualsIdentity(link))
        {
            InsertDependsOn(index, _m3.Language_dependsOn.AsNodes<Language>(nodes).ToList());
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected override bool RemoveInternal(Link? link, IEnumerable<IReadableNode> nodes)
    {
        if (base.RemoveInternal(link, nodes))
            return true;
        
        if (_m3.Language_entities.EqualsIdentity(link))
        {
            RemoveEntities(_m3.Language_entities.AsNodes<DynamicLanguageEntity>(nodes).ToList());
            return true;
        }
        
        if (_m3.Language_dependsOn.EqualsIdentity(link))
        {
            RemoveDependsOn(_m3.Language_dependsOn.AsNodes<Language>(nodes).ToList());
            return true;
        }
        
        return false;
    }
    

    /// <inheritdoc cref="GetFactory"/>
    public INodeFactory? NodeFactory { private get; set; }

    /// <inheritdoc />
    public INodeFactory GetFactory() => NodeFactory ??= new ReflectiveBaseNodeFactory(this);

    /// <inheritdoc />
    public void SetFactory(INodeFactory factory) => NodeFactory = factory;
    
    /// <inheritdoc />
    public INotificationSender? GetNotificationSender() => null;
    
    /// <inheritdoc />
    IPartitionNotificationProducer? IPartitionInstance.GetNotificationProducer() => null;
}
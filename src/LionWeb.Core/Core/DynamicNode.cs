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

namespace LionWeb.Core;

using M1.Event;
using M1.Event.Partition;
using M2;
using M3;
using System.Collections;
using Utilities;

/// A generic implementation of <see cref="INode"/> that essentially wraps a (hash-)map <see cref="Feature"/> --> value of setting of that feature.
public class DynamicNode : NodeBase
{
    /// <inheritdoc cref="IReadableNode.GetClassifier()"/>
    private readonly Classifier _classifier;

    /// <summary>
    /// Constructs a new node.
    /// </summary>
    /// <param name="id"><c>this</c> node's <see cref="IReadableNode.GetId">id</see>.</param>
    /// <param name="classifier"><c>this</c> node's <see cref="IReadableNode.GetClassifier()">classifier</see>.</param>
    /// <exception cref="InvalidIdException">If <paramref name="id"/> is not a <see cref="IReadableNode.GetId">valid identifier</see>.</exception>
    public DynamicNode(NodeId id, Classifier classifier) : base(id)
    {
        _classifier = classifier;
    }

    /// <inheritdoc/>
    protected override bool DetachChild(INode child)
    {
        if (base.DetachChild(child))
            return true;

        var containment = GetContainmentOf(child);
        if (containment == null)
            return false;

        switch (_settings[containment])
        {
            case INode:
                _settings.Remove(containment);
                return true;
            case List<INode> c:
                var result = c.Remove(child);
                if (c.Count == 0)
                    _settings.Remove(containment);
                return result;
            default:
                return false;
        }
    }

    /// <inheritdoc/>
    public override Containment? GetContainmentOf(INode child) =>
        _settings
            .Keys
            .OfType<Containment>()
            .FirstOrDefault(k => k
                .AsNodes<INode>(_settings[k])
                .Contains(child));

    /// <inheritdoc/>
    public override Classifier GetClassifier() => _classifier;

    /// Contains all settings of features.
    private readonly Dictionary<Feature, object> _settings = new(new FeatureIdentityComparer());

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (feature == null)
        {
            result = GetAnnotations();
            return true;
        }

        if (_settings.TryGetValue(feature, out var setting))
        {
            result = setting;
            return true;
        }

        result = feature switch
        {
            Property { Optional: false } => throw new UnsetFeatureException(feature),
            Property => null,
            Link { Optional: false } => throw new UnsetFeatureException(feature),
            Link link => link.Multiple ? Enumerable.Empty<INode>().ToList().AsReadOnly() : null,
            _ => throw new UnknownFeatureException(this.GetClassifier(), feature)
        };
        return true;
    }

    /// <inheritdoc />
    protected override bool SetInternal(Feature? feature, object? value, IEventId? eventId = null)
    {
        if (base.SetInternal(feature, value))
            return true;

        return feature switch
        {
            Property p => SetProperty(p, value),
            Reference r => SetReference(r, value),
            Containment c => SetContainment(c, value),
            _ => false
        };
    }

    private bool SetProperty(Property property, object? value)
    {
        var commander = GetPartitionCommander();
        _settings.TryGetValue(property, out var oldValue);
        if (value == null && property.Optional)
        {
            _settings.Remove(property);
            if (oldValue != null)
            {
                commander?.Raise(new PropertyDeletedNotification(this, property, oldValue, commander.CreateEventId()));
            }

            return true;
        }

        var newValue = VersionSpecifics.PrepareSetProperty(property, value);
        if (oldValue != null)
        {
            commander?.Raise(new PropertyChangedNotification(this, property, newValue, oldValue, commander.CreateEventId()));
        } else
        {
            commander?.Raise(new PropertyAddedNotification(this, property, newValue, commander.CreateEventId()));
        }

        _settings[property] = newValue;
        return true;
    }

    private IDynamicNodeVersionSpecifics VersionSpecifics => new Lazy<IDynamicNodeVersionSpecifics>(() =>
        IDynamicNodeVersionSpecifics.Create(GetClassifier().GetLanguage().LionWebVersion)).Value;

    private bool SetReference(Reference reference, object? value)
    {
        switch (reference, value)
        {
            case ({ Multiple: true }, IEnumerable e):
                var enumerable = reference.AsNodes<INode>(e).ToList();
                AssureLinkTypeCompatible(enumerable, reference);
                AssureOptionalCount(value, enumerable, reference);
                UpdateSettings(enumerable, reference);
                return true;

            case ({ Multiple: false }, INode):
                _settings[reference] = value;
                return true;

            case ({ Multiple: false, Optional: true }, null):
                _settings.Remove(reference);
                return true;

            case ({ Optional: false }, null):
                throw new InvalidValueException(reference, value);

            default:
                throw new InvalidValueException(reference, value);
        }
    }

    private bool SetContainment(Containment containment, object? value)
    {
        switch (containment, value)
        {
            case ({ Multiple: true }, IEnumerable e):
                var enumerable = containment.AsNodes<INode>(e).ToList();
                AssureLinkTypeCompatible(enumerable, containment);
                AssureOptionalCount(value, enumerable, containment);

                if (_settings.TryGetValue(containment, out var oldValue) && oldValue is List<INode> oldList)
                    RemoveSelfParent(oldList?.ToList(), oldList, containment, null);

                SetSelfParent(enumerable, containment);
                UpdateSettings(enumerable, containment);
                return true;

            case ({ Multiple: false }, INode n):
                if (_settings.TryGetValue(containment, out var oldVal) && oldVal is INode oldNode)
                    SetParentInternal(oldNode, null);

                DetachChildInternal(n);
                SetParentInternal(n, this);
                _settings[containment] = value;
                return true;

            case ({ Multiple: false, Optional: true }, null):
                _settings.Remove(containment);
                return true;

            case ({ Optional: false }, null):
                throw new InvalidValueException(containment, value);

            default:
                throw new InvalidValueException(containment, value);
        }
    }

    private static void AssureOptionalCount(object value, List<INode> enumerable, Link link)
    {
        if (!link.Optional && enumerable.Count == 0)
            throw new InvalidValueException(link, value);
    }

    private static void AssureLinkTypeCompatible(List<INode> enumerable, Link link)
    {
        foreach (INode n in enumerable)
        {
            if (!n.GetClassifier().IsGeneralization(link.Type))
                throw new InvalidValueException(link, n);
        }
    }

    private void UpdateSettings(List<INode> enumerable, Link link)
    {
        if (enumerable.Count != 0)
            _settings[link] = enumerable;
        else
            _settings.Remove(link);
    }

    /// <inheritdoc/>
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        _settings.Keys;
}

/// A generic implementation of <see cref="IAnnotationInstance"/>
/// that essentially wraps a (hash-)map <see cref="Feature"/> --> value of setting of that feature.
public class DynamicAnnotationInstance : DynamicNode, IAnnotationInstance<INode>
{
    /// <inheritdoc />
    public DynamicAnnotationInstance(NodeId id, Annotation annotation) : base(id, annotation) { }

    /// <inheritdoc cref="IAnnotationInstance.GetAnnotation" />
    public Annotation GetAnnotation() => (Annotation)base.GetClassifier();
}

/// A generic implementation of <see cref="IConceptInstance"/>
/// that essentially wraps a (hash-)map <see cref="Feature"/> --> value of setting of that feature.
public class DynamicConceptInstance : DynamicNode, IConceptInstance<INode>
{
    /// <inheritdoc />
    public DynamicConceptInstance(NodeId id, Concept concept) : base(id, concept) { }

    /// <inheritdoc cref="IConceptInstance.GetConcept()" />
    public Concept GetConcept() => (Concept)base.GetClassifier();
}

/// A generic implementation of <see cref="IPartitionInstance"/>
/// that essentially wraps a (hash-)map <see cref="Feature"/> --> value of setting of that feature.
public class DynamicPartitionInstance : DynamicConceptInstance, IPartitionInstance<INode>
{
    private readonly PartitionEventHandler _eventHandler;

    /// <inheritdoc />
    public DynamicPartitionInstance(NodeId id, Concept concept) : base(id, concept)
    {
        _eventHandler = new PartitionEventHandler(this);
    }

    /// <inheritdoc />
    public IPartitionPublisher GetPublisher() => _eventHandler;

    /// <inheritdoc />
    public IPartitionCommander GetCommander() => _eventHandler;
}
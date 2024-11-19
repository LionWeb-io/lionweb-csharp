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

using M2;
using M3;
using System.Collections;
using Utilities;

/// <summary>
/// A generic implementation of <see cref="INode"/> that essentially wraps a (hash-)map <see cref="Feature"/> --> value of setting of that feature.
/// </summary>
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
    public DynamicNode(string id, Classifier classifier) : base(id)
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

    /// <summary>
    /// Contains all settings of features.
    /// </summary>
    private readonly Dictionary<Feature, object> _settings = new();

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
            _ => throw new ArgumentException($"unsupported feature: {feature}", nameof(feature))
        };
        return true;
    }

    /// <inheritdoc />
    protected override bool SetInternal(Feature? feature, object? value)
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
        switch (value)
        {
            case string when property.Type.EqualsIdentity(_builtIns.String):
            case string when property.Type.EqualsIdentity(_builtIns.Json):
            case int when property.Type.EqualsIdentity(_builtIns.Integer):
            case bool when property.Type.EqualsIdentity(_builtIns.Boolean):
                _settings[property] = value;
                return true;

            case Enum when property.Type is Enumeration e:
                try
                {
                    var factory = e.GetLanguage().GetFactory();
                    var enumerationLiteral = e.Literals[0];
                    Enum literal = factory.GetEnumerationLiteral(enumerationLiteral);
                    if (literal.GetType().IsEnumDefined(value))
                    {
                        _settings[property] = value;
                        return true;
                    }
                } catch (ArgumentException)
                {
                    // fall-through
                }

                throw new InvalidValueException(property, value);

            case null when property.Optional:
                _settings.Remove(property);
                return true;

            default:
                throw new InvalidValueException(property, value);
        }
    }

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
                    RemoveSelfParent(oldList?.ToList(), oldList, containment);

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
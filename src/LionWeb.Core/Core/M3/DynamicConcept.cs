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

using System.Collections;
using System.Collections.Immutable;
using Utilities;

/// <inheritdoc cref="Concept"/>
public class DynamicConcept(NodeId id, LionWebVersions lionWebVersion, DynamicLanguage? language)
    : DynamicClassifier(id, lionWebVersion, language), Concept
{
    /// <inheritdoc />
    public bool Abstract { get; set; }

    /// <inheritdoc />
    public bool Partition { get; set; }

    /// <inheritdoc />
    public Concept? Extends { get; set; }

    private readonly List<Interface> _implements = [];

    /// <inheritdoc />
    public IReadOnlyList<Interface> Implements => _implements.AsReadOnly();

    /// <inheritdoc cref="Implements"/>
    public void AddImplements(IEnumerable<Interface> interfaces) => _implements.AddRange(interfaces);

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Concept;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Concept_abstract,
            _m3.Concept_partition,
            _m3.Concept_extends,
            _m3.Concept_implements
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_m3.Concept_abstract == feature)
        {
            result = Abstract;
            return true;
        }

        if (_m3.Concept_partition == feature)
        {
            result = Partition;
            return true;
        }

        if (_m3.Concept_extends == feature)
        {
            result = Extends;
            return true;
        }

        if (_m3.Concept_implements == feature)
        {
            result = Implements;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool TryGetPropertyRaw(Property property, out object? value)
    {
        if (base.TryGetPropertyRaw(property, out value))
            return true;
        
        if (_m3.Concept_abstract.EqualsIdentity(property))
        {
            value = Abstract;
            return true;
        }
        
        if (_m3.Concept_partition.EqualsIdentity(property))
        {
            value = Partition;
            return true;
        }
        
        return false;
    }

    /// <inheritdoc />
    protected internal override bool TryGetReferenceRaw(Reference reference, out IReferenceTarget? target)
    {
        if (base.TryGetReferenceRaw(reference, out target))
            return true;

        if (_m3.Concept_extends.EqualsIdentity(reference))
        {
            target = ReferenceTarget.FromNodeOptional(Extends);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool TryGetReferencesRaw(Reference reference, out IReadOnlyList<IReferenceTarget> targets)
    {
        if (base.TryGetReferencesRaw(reference, out targets))
            return true;
        
        if (_m3.Concept_implements.EqualsIdentity(reference))
        {
            targets = _implements.Select(ReferenceTarget.FromNode).ToImmutableList();
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool SetPropertyRaw(Property property, object? value)
    {
        if (base.SetPropertyRaw(property, value))
            return true;
        
        if (_m3.Concept_abstract.EqualsIdentity(property) && value is bool @abstract)
        {
            Abstract = @abstract;
            return true;
        }
        
        if (_m3.Concept_partition.EqualsIdentity(property) && value is bool partition)
        {
            Partition = partition;
            return true;
        }
        
        return false;
    }

    /// <inheritdoc />
    protected internal override bool SetReferenceRaw(Reference reference, ReferenceTarget? target)
    {
        if (base.SetReferenceRaw(reference, target))
            return true;

        if (_m3.Concept_extends.EqualsIdentity(reference) && target?.Target is Concept extends)
        {
            Extends = extends;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool AddReferencesRaw(Reference reference, ReferenceTarget target)
    {
        if (base.AddReferencesRaw(reference, target))
            return true;
        
        if (_m3.Concept_implements.EqualsIdentity(reference)&& target.Target is Interface t)
        {
            _implements.Add(t);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool InsertReferencesRaw(Reference reference, Index index, ReferenceTarget target)
    {
        if (base.InsertReferencesRaw(reference, index, target))
            return true;
        
        if (_m3.Concept_implements.EqualsIdentity(reference)&& target.Target is Interface t)
        {
            _implements.Insert(index, t);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool RemoveReferencesRaw(Reference reference, ReferenceTarget target)
    {
        if (base.RemoveReferencesRaw(reference, target))
            return true;
        
        if (_m3.Concept_implements.EqualsIdentity(reference)&& target.Target is Interface t)
        {
            return _implements.Remove(t);
        }

        return false;
    }

    /// <inheritdoc />
    protected override bool SetInternal(Feature? feature, object? value)
    {
        var result = base.SetInternal(feature, value);
        if (result)
        {
            return result;
        }

        if (_m3.Concept_abstract == feature)
        {
            Abstract = value switch
            {
                bool bol => bol,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (_m3.Concept_partition == feature)
        {
            Partition = value switch
            {
                bool bol => bol,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (_m3.Concept_extends == feature)
        {
            Extends = value switch
            {
                Concept ct => ct,
                null => null,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (_m3.Concept_implements == feature)
        {
            switch (value)
            {
                case IEnumerable e:
                    _implements.Clear();
                    _implements.AddRange(e.OfType<Interface>());
                    return true;
                default:
                    throw new InvalidValueException(feature, value);
            }
        }

        return false;
    }
}
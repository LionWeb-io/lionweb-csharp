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

using System.Diagnostics.CodeAnalysis;
using Utilities;

/// <inheritdoc cref="Concept"/>
public class ConceptBase<TLanguage>(NodeId id, TLanguage parent) : ClassifierBase<TLanguage>(id, parent), Concept
    where TLanguage : Language
{
    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Concept_abstract,
            _m3.Concept_partition,
            _m3.Concept_extends,
            _m3.Concept_implements
        ]);

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Concept;

    /// <inheritdoc />
    public override object? Get(Feature feature)
    {
        var result = base.Get(feature);
        if (result != null)
            return result;
        if (feature == _m3.Concept_abstract)
            return Abstract;
        if (feature == _m3.Concept_partition)
            return Partition;
        if (feature == _m3.Concept_extends)
            return Extends;
        if (feature == _m3.Concept_implements)
            return Implements;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }

    /// <inheritdoc />
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        if (base.TryGet(feature, out value))
            return true;

        if (_m3.Concept_abstract.EqualsIdentity(feature) && ((Concept)this).TryGetAbstract(out var @abstract))
        {
            value = @abstract;
            return true;
        }

        if (_m3.Concept_partition.EqualsIdentity(feature) && ((Concept)this).TryGetPartition(out var partition))
        {
            value = partition;
            return true;
        }

        if (_m3.Concept_extends.EqualsIdentity(feature) && ((Concept)this).TryGetExtends(out var extends))
        {
            value = extends;
            return true;
        }

        if (_m3.Concept_implements.EqualsIdentity(feature) && ((Concept)this).TryGetImplements(out var implements))
        {
            value = implements;
            return true;
        }

        value = null;
        return false;
    }

    /// <inheritdoc />
    public bool Abstract { get; init; } = false;

    /// <inheritdoc />
    public bool Partition { get; init; } = false;

    /// <inheritdoc />
    public Concept? Extends => ExtendsLazy.Value;

    /// <inheritdoc cref="Extends"/>
    public Lazy<Concept?> ExtendsLazy { protected get; init; } = new((Concept?)null);

    /// <inheritdoc />
    public IReadOnlyList<Interface> Implements => ImplementsLazy.Value;

    /// <inheritdoc cref="Implements"/>
    public Lazy<IReadOnlyList<Interface>> ImplementsLazy { protected get; init; } = new([]);
}
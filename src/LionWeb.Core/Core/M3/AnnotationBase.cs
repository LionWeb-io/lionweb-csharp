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

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Utilities;

/// <inheritdoc cref="Annotation"/>
public class AnnotationBase<TLanguage>(NodeId id, TLanguage parent) : ClassifierBase<TLanguage>(id, parent), Annotation
    where TLanguage : Language
{
    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Annotation_annotates,
            _m3.Annotation_extends,
            _m3.Annotation_implements
        ]);

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Annotation;

    /// <inheritdoc />
    public override object? Get(Feature feature)
    {
        var result = base.Get(feature);
        if (result != null)
            return result;
        if (feature == _m3.Annotation_annotates)
            return Annotates;
        if (feature == _m3.Annotation_extends)
            return Extends;
        if (feature == _m3.Annotation_implements)
            return Implements;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }

    /// <inheritdoc />
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        if (base.TryGet(feature, out value))
            return true;

        if (_m3.Annotation_annotates.EqualsIdentity(feature) && ((Annotation)this).TryGetAnnotates(out var annotates))
        {
            value = annotates;
            return true;
        }

        if (feature == _m3.Annotation_extends && ((Annotation)this).TryGetExtends(out var extends))
        {
            value = extends;
            return true;
        }

        if (feature == _m3.Annotation_implements && ((Annotation)this).TryGetImplements(out var implements))
        {
            value = implements;
            return true;
        }

        value = null;
        return false;
    }

    /// <inheritdoc />
    protected internal override bool TryGetReferenceRaw(Reference reference, out IReferenceTarget? target)
    {
        if (base.TryGetReferenceRaw(reference, out target))
            return true;

        if (_m3.Annotation_annotates.EqualsIdentity(reference))
        {
            target = ReferenceTarget.FromNodeOptional(Annotates);
            return true;
        }

        if (_m3.Annotation_extends.EqualsIdentity(reference))
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
        
        if (_m3.Annotation_implements.EqualsIdentity(reference))
        {
            targets = Implements.Select(ReferenceTarget.FromNode).ToImmutableList();
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public Classifier Annotates => AnnotatesLazy.Value;

    /// <inheritdoc cref="Annotates"/>
    public required Lazy<Classifier> AnnotatesLazy { protected get; init; }

    /// <inheritdoc />
    public Annotation? Extends => ExtendsLazy.Value;

    /// <inheritdoc cref="Extends"/>
    public Lazy<Annotation?> ExtendsLazy { protected get; init; } = new((Annotation?)null);

    /// <inheritdoc />
    public IReadOnlyList<Interface> Implements => ImplementsLazy.Value;

    /// <inheritdoc cref="Implements"/>
    public Lazy<IReadOnlyList<Interface>> ImplementsLazy { protected get; init; } = new([]);
}
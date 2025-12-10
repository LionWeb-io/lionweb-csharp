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
using Utilities;

/// <inheritdoc cref="Classifier"/>
public abstract class DynamicClassifier(NodeId id, LionWebVersions lionWebVersion, DynamicLanguage? language)
    : DynamicLanguageEntity(id, lionWebVersion, language), Classifier
{
    private readonly List<DynamicFeature> _features = [];

    /// <inheritdoc />
    public IReadOnlyList<Feature> Features => _features.AsReadOnly();

    /// <inheritdoc cref="Features"/>
    public void AddFeatures(IEnumerable<DynamicFeature> features) =>
        AddOptionalMultipleContainment(features, _m3.Classifier_features, _features, AddFeaturesRaw);

    /// <inheritdoc cref="Features"/>
    protected internal bool SetFeaturesRaw(List<DynamicFeature> nodes) => ExchangeChildrenRaw(nodes, _features);

    /// <inheritdoc cref="Features"/>
    protected internal bool AddFeaturesRaw(DynamicFeature? value) => AddChildRaw(value, _features);

    /// <inheritdoc cref="Features"/>
    private bool InsertFeaturesRaw(int index, DynamicFeature? value) => InsertChildRaw(index, value, _features);

    /// <inheritdoc cref="Features"/>
    private bool RemoveFeaturesRaw(DynamicFeature? value) => RemoveChildRaw(value, _features);

    /// <inheritdoc />
    protected override bool DetachChild(INode child)
    {
        if (base.DetachChild(child))
        {
            return true;
        }

        var c = GetContainmentOf(child);
        if (c == _m3.Classifier_features)
            return _features.Remove((DynamicFeature)child);

        return false;
    }

    /// <inheritdoc />
    public override Containment? GetContainmentOf(INode child)
    {
        var result = base.GetContainmentOf(child);
        if (result != null)
            return result;

        if (child is Feature s && _features.Contains(s))
            return _m3.Classifier_features;

        return null;
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Classifier_features
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_m3.Classifier_features == feature)
        {
            result = Features;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool TryGetContainmentsRaw(Containment containment,
        out IReadOnlyList<IReadableNode> nodes)
    {
        if (base.TryGetContainmentsRaw(containment, out nodes))
            return true;

        if (_m3.Classifier_features.EqualsIdentity(containment))
        {
            nodes = _features;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool AddContainmentsRaw(Containment containment, IWritableNode node)
    {
        if (base.AddContainmentsRaw(containment, node))
            return true;

        if (_m3.Classifier_features.EqualsIdentity(containment) && node is DynamicFeature feature)
        {
            return AddFeaturesRaw(feature);
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool InsertContainmentsRaw(Containment containment, Index index, IWritableNode node)
    {
        if (base.InsertContainmentsRaw(containment, index, node))
            return true;

        if (_m3.Classifier_features.EqualsIdentity(containment) && node is DynamicFeature feature)
        {
            return InsertFeaturesRaw(index, feature);
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool RemoveContainmentsRaw(Containment containment, IWritableNode node)
    {
        if (base.RemoveContainmentsRaw(containment, node))
            return true;

        if (_m3.Classifier_features.EqualsIdentity(containment) && node is DynamicFeature feature)
        {
            return RemoveFeaturesRaw(feature);
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

        if (_m3.Classifier_features == feature)
        {
            switch (value)
            {
                case IEnumerable e:
                    RemoveSelfParent(_features?.ToList(), _features, _m3.Classifier_features);
                    AddFeatures(e.OfType<DynamicFeature>().ToArray());
                    return true;
                default:
                    throw new InvalidValueException(feature, value);
            }
        }

        return false;
    }
}
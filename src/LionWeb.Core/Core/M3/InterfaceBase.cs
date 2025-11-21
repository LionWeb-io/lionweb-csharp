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

/// <inheritdoc cref="Interface"/>
public class InterfaceBase<TLanguage>(NodeId id, TLanguage parent) : ClassifierBase<TLanguage>(id, parent), Interface
    where TLanguage : Language
{
    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Interface_extends
        ]);

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Interface;

    /// <inheritdoc />
    public override object Get(Feature feature)
    {
        var result = base.Get(feature);
        if (result != null)
            return result;

        if (feature == _m3.Interface_extends)
            return Extends;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }

    /// <inheritdoc />
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        if (base.TryGet(feature, out value))
            return true;

        if (_m3.Interface_extends.EqualsIdentity(feature) && ((Interface)this).TryGetExtends(out var extends))
        {
            value = extends;
            return true;
        }

        value = null;
        return false;
    }

    /// <inheritdoc />
    public IReadOnlyList<Interface> Extends => ExtendsLazy.Value;

    /// <inheritdoc cref="Extends"/>
    public Lazy<IReadOnlyList<Interface>> ExtendsLazy { protected get; init; } = new([]);
}
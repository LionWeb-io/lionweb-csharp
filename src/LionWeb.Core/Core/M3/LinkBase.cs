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

/// <inheritdoc cref="Link"/>
public abstract class LinkBase<TLanguage> : FeatureBase<TLanguage>, Link where TLanguage : Language
{
    internal LinkBase(NodeId id, Classifier parent, TLanguage language) : base(id, parent, language)
    {
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Link_multiple,
            _m3.Link_type
        ]);

    /// <inheritdoc />
    public override object? Get(Feature feature)
    {
        var result = base.Get(feature);
        if (result != null)
            return result;
        if (feature == _m3.Link_multiple)
            return Multiple;
        if (feature == _m3.Link_type)
            return Type;

        return null;
    }

    /// <inheritdoc />
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        if (base.TryGet(feature, out value))
            return true;

        if (_m3.Link_multiple.EqualsIdentity(feature) && ((Link)this).TryGetMultiple(out var multiple))
        {
            value = multiple;
            return true;
        }

        if (_m3.Link_type.EqualsIdentity(feature) && ((Link)this).TryGetType(out var type))
        {
            value = type;
            return true;
        }

        value = null;
        return false;
    }

    /// <inheritdoc />
    protected internal override bool TryGetPropertyRaw(Property property, out object? value)
    {
        if (base.TryGetPropertyRaw(property, out value))
            return true;
        
        if (_m3.Link_multiple.EqualsIdentity(property) && ((Link)this).TryGetMultiple(out var multiple))
        {
            value = multiple;
            return true;
        }
        
        return false;
    }

    /// <inheritdoc />
    protected internal override bool TryGetReferenceRaw(Reference reference, out IReferenceTarget? target)
    {
        if (base.TryGetReferenceRaw(reference, out target))
            return true;
        
        if (_m3.Link_type.EqualsIdentity(reference) && ((Link)this).TryGetType(out var type))
        {
            target = ReferenceTarget.FromNode(type);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public required bool Multiple { get; init; }

    /// <inheritdoc />
    public required Classifier Type { get; init; }

    /// <inheritdoc />
    public bool TryGetType(out Classifier? type)
    {
        type = Type;
        return type != null;
    }
}
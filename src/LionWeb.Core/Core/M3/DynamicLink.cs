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

using Utilities;

/// <inheritdoc cref="Link"/>
public abstract class DynamicLink(NodeId id, DynamicClassifier? classifier, LionWebVersions lionWebVersion)
    : DynamicFeature(id, classifier, lionWebVersion), Link
{
    private Classifier? _type;

    /// <inheritdoc />
    public bool Multiple { get; set; }

    /// <inheritdoc />
    public Classifier Type
    {
        get => _type ?? throw new UnsetFeatureException(_m3.Link_type);
        set => _type = value;
    }

    /// <inheritdoc />
    public bool TryGetType(out Classifier? type)
    {
        type = _type;
        return _type != null;
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Link_multiple,
            _m3.Link_type
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_m3.Link_multiple == feature)
        {
            result = Multiple;
            return true;
        }

        if (_m3.Link_type == feature)
        {
            result = Type;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool TryGetPropertyRaw(Property property, out object? value)
    {
        if (base.TryGetPropertyRaw(property, out value))
            return true;
        
        if (_m3.Link_multiple.EqualsIdentity(property))
        {
            value = Multiple;
            return true;
        }
        
        return false;
    }

    /// <inheritdoc />
    protected internal override bool TryGetReferenceRaw(Reference reference, out IReferenceTarget? target)
    {
        if (base.TryGetReferenceRaw(reference, out target))
            return true;
        
        if (_m3.Link_type.EqualsIdentity(reference))
        {
            target = ReferenceTarget.FromNodeOptional(_type);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool SetPropertyRaw(Property property, object? value)
    {
        if (base.SetPropertyRaw(property, value))
            return true;
        
        if (_m3.Link_multiple.EqualsIdentity(property) && value is bool multiple)
        {
            Multiple = multiple;
            return true;
        }
        
        return false;
    }

    /// <inheritdoc />
    protected internal override bool SetReferenceRaw(Reference reference, ReferenceTarget? target)
    {
        if (base.SetReferenceRaw(reference, target))
            return true;
        
        if (_m3.Link_type.EqualsIdentity(reference) && target?.Target is Classifier type)
        {
            _type = type;
            return true;
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

        if (_m3.Link_multiple == feature)
        {
            Multiple = value switch
            {
                bool bol => bol,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (_m3.Link_type == feature)
        {
            Type = value switch
            {
                Classifier cf => cf,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        return false;
    }
}
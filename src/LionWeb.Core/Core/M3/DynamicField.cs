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

using Notification;

/// <inheritdoc cref="Field"/>
public class DynamicField : DynamicIKeyed, Field
{
    /// <inheritdoc />
    protected override ILionCoreLanguageWithStructuredDataType _m3 => (ILionCoreLanguageWithStructuredDataType)base._m3;

    private Datatype? _type;

    /// <inheritdoc cref="Field"/>
    public DynamicField(NodeId id, LionWebVersions lionWebVersion, DynamicStructuredDataType? structuredDataType)
        : base(id, lionWebVersion)
    {
        structuredDataType?.AddFields([this]);
        _parent = structuredDataType;
    }

    /// <inheritdoc />
    public Datatype Type
    {
        get => _type ?? throw new UnsetFeatureException(_m3.Field_type);
        set => _type = value;
    }

    /// <inheritdoc />
    public bool TryGetType(out Datatype? type)
    {
        type = _type;
        return type != null;
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Field;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Field_type
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_m3.Field_type == feature)
        {
            result = Type;
            return true;
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

        if (_m3.Field_type == feature)
        {
            Type = value switch
            {
                Datatype dt => dt,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        return false;
    }
}
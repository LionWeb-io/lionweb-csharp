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

/// <inheritdoc cref="Feature"/>
public abstract class DynamicFeature : DynamicIKeyed, Feature
{
    /// <inheritdoc />
    public bool Optional { get; set; }

    /// <inheritdoc />
    protected DynamicFeature(NodeId id, DynamicClassifier? parent, LionWebVersions lionWebVersion) :
        base(id, lionWebVersion)
    {
        parent?.AddFeatures([this]);
        _parent = parent;
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Feature_optional
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_m3.Feature_optional == feature)
        {
            result = Optional;
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

        if (_m3.Feature_optional == feature)
        {
            Optional = value switch
            {
                bool bol => bol,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        return false;
    }
}
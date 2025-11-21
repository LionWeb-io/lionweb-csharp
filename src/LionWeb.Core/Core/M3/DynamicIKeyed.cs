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

using M2;
using Notification;
using System.Diagnostics.CodeAnalysis;

/// <inheritdoc cref="IKeyed"/>
public abstract class DynamicIKeyed(NodeId id, LionWebVersions lionWebVersion) : NodeBase(id), IKeyed
{
    private MetaPointerKey? _key;
    private string? _name;

    /// <inheritdoc />
    protected override IBuiltInsLanguage _builtIns =>
        new Lazy<IBuiltInsLanguage>(() => lionWebVersion.BuiltIns).Value;

    /// <inheritdoc />
    protected override ILionCoreLanguage _m3 =>
        new Lazy<ILionCoreLanguage>(() => lionWebVersion.LionCore).Value;

    /// <inheritdoc />
    public MetaPointerKey Key
    {
        get => _key ?? throw new UnsetFeatureException(_m3.IKeyed_key);
        set => _key = value;
    }

    /// <inheritdoc />
    public bool TryGetKey([NotNullWhen(true)] out MetaPointerKey? key)
    {
        key = _key;
        return key != null;
    }

    /// <inheritdoc />
    public string Name
    {
        get => _name ?? throw new UnsetFeatureException(_builtIns.INamed_name);
        set => _name = value;
    }

    /// <inheritdoc />
    public bool TryGetName([NotNullWhen(true)] out string? name)
    {
        name = _name;
        return name != null;
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
    [
        _builtIns.INamed_name,
        _m3.IKeyed_key
    ];

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_builtIns.INamed_name == feature)
        {
            result = Name;
            return true;
        }

        if (_m3.IKeyed_key == feature)
        {
            result = Key;
            return true;
        }

        result = null;
        return false;
    }

    /// <inheritdoc />
    protected override bool SetInternal(Feature? feature, object? value, INotificationId? notificationId = null)
    {
        if (_builtIns.INamed_name == feature)
        {
            Name = value switch
            {
                string str => str,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (_m3.IKeyed_key == feature)
        {
            Key = value switch
            {
                string str => str,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        return false;
    }
}
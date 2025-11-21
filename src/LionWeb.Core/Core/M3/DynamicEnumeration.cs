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
using System.Collections;

/// <inheritdoc cref="Enumeration"/>
public class DynamicEnumeration(NodeId id, LionWebVersions lionWebVersion, DynamicLanguage? language)
    : DynamicDatatype(id, lionWebVersion, language), Enumeration
{
    private readonly List<EnumerationLiteral> _literals = [];

    /// <inheritdoc />
    public IReadOnlyList<EnumerationLiteral> Literals => _literals.AsReadOnly();

    /// <inheritdoc cref="Literals"/>
    public void AddLiterals(IEnumerable<EnumerationLiteral> literals) =>
        _literals.AddRange(SetSelfParent(literals?.ToList(), _m3.Enumeration_literals));

    /// <inheritdoc />
    protected override bool DetachChild(INode child)
    {
        if (base.DetachChild(child))
        {
            return true;
        }

        var c = GetContainmentOf(child);
        if (c == _m3.Enumeration_literals)
            return _literals.Remove((EnumerationLiteral)child);

        return false;
    }

    /// <inheritdoc />
    public override Containment? GetContainmentOf(INode child)
    {
        var result = base.GetContainmentOf(child);
        if (result != null)
            return result;

        if (child is EnumerationLiteral s && _literals.Contains(s))
            return _m3.Enumeration_literals;

        return null;
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Enumeration;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Enumeration_literals
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_m3.Enumeration_literals == feature)
        {
            result = Literals;
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

        if (_m3.Enumeration_literals == feature)
        {
            switch (value)
            {
                case IEnumerable e:
                    RemoveSelfParent(_literals?.ToList(), _literals, _m3.Enumeration_literals);
                    AddLiterals(e.OfType<EnumerationLiteral>().ToArray());
                    return true;
                default:
                    throw new InvalidValueException(feature, value);
            }
        }

        return false;
    }
}
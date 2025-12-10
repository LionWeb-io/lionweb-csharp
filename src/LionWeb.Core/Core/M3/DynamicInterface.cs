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
using System.Collections.Immutable;
using Utilities;

/// <inheritdoc cref="Interface"/>
public class DynamicInterface(NodeId id, LionWebVersions lionWebVersion, DynamicLanguage? language)
    : DynamicClassifier(id, lionWebVersion, language), Interface
{
    private readonly List<Interface> _extends = [];

    /// <inheritdoc />
    public IReadOnlyList<Interface> Extends => _extends.AsReadOnly();

    /// <inheritdoc cref="Extends"/>
    public void AddExtends(IEnumerable<Interface> extends) => _extends.AddRange(extends);

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Interface;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Interface_extends
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_m3.Interface_extends == feature)
        {
            result = Extends;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool TryGetReferencesRaw(Reference reference, out IReadOnlyList<IReferenceTarget> targets)
    {
        if (base.TryGetReferencesRaw(reference, out targets))
            return true;
        
        if (_m3.Interface_extends.EqualsIdentity(reference))
        {
            targets = _extends.Select(ReferenceTarget.FromNode).ToImmutableList();
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool AddReferencesRaw(Reference reference, ReferenceTarget target)
    {
        if (base.AddReferencesRaw(reference, target))
            return true;
        
        if (_m3.Interface_extends.EqualsIdentity(reference)&& target.Target is Interface t)
        {
            _extends.Add(t);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool InsertReferencesRaw(Reference reference, Index index, ReferenceTarget target)
    {
        if (base.InsertReferencesRaw(reference, index, target))
            return true;
        
        if (_m3.Interface_extends.EqualsIdentity(reference)&& target.Target is Interface t)
        {
            _extends.Insert(index, t);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool RemoveReferencesRaw(Reference reference, ReferenceTarget target)
    {
        if (base.RemoveReferencesRaw(reference, target))
            return true;
        
        if (_m3.Interface_extends.EqualsIdentity(reference)&& target.Target is Interface t)
        {
            return _extends.Remove(t);
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

        if (_m3.Interface_extends == feature)
        {
            switch (value)
            {
                case IEnumerable e:
                    _extends.Clear();
                    _extends.AddRange(e.OfType<Interface>());
                    return true;
                default:
                    throw new InvalidValueException(feature, value);
            }
        }

        return false;
    }
}
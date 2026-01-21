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
using System.Diagnostics.CodeAnalysis;
using Utilities;

/// <inheritdoc cref="IKeyed"/>
public abstract class IKeyedBase<TLanguage> : ReadableNodeBase<IReadableNode>, IKeyed where TLanguage : Language
{
    protected readonly TLanguage _language;

    /// <inheritdoc />
    protected override IBuiltInsLanguage _builtIns =>
        new Lazy<IBuiltInsLanguage>(() => _language.LionWebVersion.BuiltIns).Value;

    /// <inheritdoc />
    protected override ILionCoreLanguage _m3 =>
        new Lazy<ILionCoreLanguage>(() => _language.LionWebVersion.LionCore).Value;

    /// <inheritdoc />
    protected IKeyedBase(NodeId id, TLanguage language) : this(id, language, language) { }

    /// <inheritdoc />
    protected IKeyedBase(NodeId id, IKeyed parent, TLanguage language) : base(id, parent)
    {
        _language = language;
    }

    /// <inheritdoc cref="GetAnnotations"/>
    public IReadOnlyList<IAnnotationInstance> Annotations { get; init; } = [];

    /// <inheritdoc cref="ReadableNodeBase{T}.GetAnnotations"/>
    public override IReadOnlyList<IReadableNode> GetAnnotations() => Annotations;

    /// <inheritdoc />
    public override IReadOnlyList<IReadableNode> GetAnnotationsRaw() => Annotations;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
    [
        _builtIns.INamed_name,
        _m3.IKeyed_key
    ];

    /// <inheritdoc />
    public override object? Get(Feature feature)
    {
        if (feature == _builtIns.INamed_name)
            return Name;
        if (feature == _m3.IKeyed_key)
            return Key;

        return null;
    }

    /// <inheritdoc />
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        if (feature is Property p)
            return TryGetPropertyRaw(p, out value);

        value = null;
        return false;
    }

    /// <inheritdoc />
    protected internal override bool TryGetPropertyRaw(Property property, out object? value)
    {
        if (base.TryGetPropertyRaw(property, out value))
            return true;
        
        if (_builtIns.INamed_name.EqualsIdentity(property) && TryGetName(out var name))
        {
            value = name;
            return true;
        }

        if (_m3.IKeyed_key.EqualsIdentity(property) && TryGetKey(out var key))
        {
            value = key;
            return true;
        }
        
        return false;
    }

    /// <inheritdoc />
    public required string Name { get; init; }

    /// <inheritdoc />
    public bool TryGetName([NotNullWhen(true)] out string? name)
    {
        name = Name;
        return name != null;
    }

    /// <inheritdoc />
    public required MetaPointerKey Key { get; init; }

    /// <inheritdoc />
    public bool TryGetKey([NotNullWhen(true)] out MetaPointerKey? key)
    {
        key = Key;
        return key != null;
    }

    /// <inheritdoc cref="M2Extensions.GetLanguage"/>
    protected TLanguage GetLanguage() => _language;

    /// <inheritdoc />
    public override string? ToString() => Name ?? base.ToString();
}
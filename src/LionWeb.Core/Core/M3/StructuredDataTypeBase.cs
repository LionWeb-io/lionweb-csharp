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

/// <inheritdoc cref="StructuredDataType"/>
public class StructuredDataTypeBase<TLanguage> : DatatypeBase<TLanguage>, StructuredDataType where TLanguage : Language
{
    /// <inheritdoc />
    protected override ILionCoreLanguageWithStructuredDataType _m3 =>
        new Lazy<ILionCoreLanguageWithStructuredDataType>(() =>
            (ILionCoreLanguageWithStructuredDataType)_language.LionWebVersion.LionCore).Value;

    /// <inheritdoc />
    public StructuredDataTypeBase(NodeId id, TLanguage parent) : base(id, parent)
    {
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.StructuredDataType_fields
        ]);

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.StructuredDataType;

    /// <inheritdoc />
    public override object Get(Feature feature)
    {
        var result = base.Get(feature);
        if (result != null)
            return result;
        if (feature == _m3.StructuredDataType_fields)
            return Fields;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }

    /// <inheritdoc />
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        if (base.TryGet(feature, out value))
            return true;

        if (_m3.StructuredDataType_fields.EqualsIdentity(feature) && ((StructuredDataType)this).TryGetFields(out var fields))
        {
            value = fields;
            return true;
        }

        value = null;
        return false;
    }

    /// <inheritdoc />
    public IReadOnlyList<Field> Fields => FieldsLazy.Value;

    /// <inheritdoc cref="Fields"/>
    public required Lazy<IReadOnlyList<Field>> FieldsLazy { protected get; init; }
}
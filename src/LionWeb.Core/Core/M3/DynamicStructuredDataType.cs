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
using Utilities;

/// <inheritdoc cref="StructuredDataType"/>
public class DynamicStructuredDataType(NodeId id, LionWebVersions lionWebVersion, DynamicLanguage? language)
    : DynamicDatatype(id, lionWebVersion, language), StructuredDataType
{
    /// <inheritdoc />
    protected override ILionCoreLanguageWithStructuredDataType _m3 => (ILionCoreLanguageWithStructuredDataType)base._m3;

    private readonly List<DynamicField> _fields = [];

    /// <inheritdoc />
    public IReadOnlyList<Field> Fields => _fields.AsReadOnly();

    /// <inheritdoc cref="Fields"/>
    public void AddFields(IEnumerable<DynamicField> fields) =>
        AddOptionalMultipleContainment(fields, _m3.StructuredDataType_fields, _fields, AddFieldsRaw);

    /// <inheritdoc cref="Fields"/>
    protected internal bool SetFieldsRaw(List<DynamicField> nodes) => ExchangeChildrenRaw(nodes, _fields);

    /// <inheritdoc cref="Fields"/>
    protected internal bool AddFieldsRaw(DynamicField? value) => AddChildRaw(value, _fields);

    /// <inheritdoc cref="Fields"/>
    private bool InsertFieldsRaw(int index, DynamicField? value) => InsertChildRaw(index, value, _fields);

    /// <inheritdoc cref="Fields"/>
    private bool RemoveFieldsRaw(DynamicField? value) => RemoveChildRaw(value, _fields);

    /// <inheritdoc />
    protected override bool DetachChild(INode child)
    {
        if (base.DetachChild(child))
        {
            return true;
        }

        var c = GetContainmentOf(child);
        if (c == _m3.StructuredDataType_fields)
            return _fields.Remove((DynamicField)child);

        return false;
    }

    /// <inheritdoc />
    public override Containment? GetContainmentOf(INode child)
    {
        var result = base.GetContainmentOf(child);
        if (result != null)
            return result;

        if (child is Field s && _fields.Contains(s))
            return _m3.StructuredDataType_fields;

        return null;
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.StructuredDataType;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.StructuredDataType_fields
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_m3.StructuredDataType_fields == feature)
        {
            result = Fields;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool TryGetContainmentsRaw(Containment containment,
        out IReadOnlyList<IReadableNode> nodes)
    {
        if (base.TryGetContainmentsRaw(containment, out nodes))
            return true;

        if (_m3.StructuredDataType_fields.EqualsIdentity(containment))
        {
            nodes = _fields;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool AddContainmentsRaw(Containment containment, IWritableNode node)
    {
        if (base.AddContainmentsRaw(containment, node))
            return true;

        if (_m3.StructuredDataType_fields.EqualsIdentity(containment) && node is DynamicField field)
        {
            return AddFieldsRaw(field);
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool InsertContainmentsRaw(Containment containment, Index index, IWritableNode node)
    {
        if (base.InsertContainmentsRaw(containment, index, node))
            return true;

        if (_m3.StructuredDataType_fields.EqualsIdentity(containment) && node is DynamicField field)
        {
            return InsertFieldsRaw(index, field);
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool RemoveContainmentsRaw(Containment containment, IWritableNode node)
    {
        if (base.RemoveContainmentsRaw(containment, node))
            return true;

        if (_m3.StructuredDataType_fields.EqualsIdentity(containment) && node is DynamicField field)
        {
            return RemoveFieldsRaw(field);
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

        if (_m3.StructuredDataType_fields == feature)
        {
            switch (value)
            {
                case IEnumerable e:
                    RemoveSelfParent(_fields?.ToList(), _fields, _m3.StructuredDataType_fields);
                    AddFields(e.OfType<DynamicField>().ToArray());
                    return true;
                default:
                    throw new InvalidValueException(feature, value);
            }
        }

        return false;
    }
}
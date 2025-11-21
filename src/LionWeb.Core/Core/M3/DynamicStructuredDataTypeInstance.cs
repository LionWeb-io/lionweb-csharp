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
using System.Text;
using Utilities;

/// <inheritdoc cref="IStructuredDataTypeInstance" />
public readonly record struct DynamicStructuredDataTypeInstance : IStructuredDataTypeInstance
{
    private readonly StructuredDataType _structuredDataType;
    private readonly (Field field, object? value)[] _fields;

    /// <inheritdoc cref="DynamicStructuredDataTypeInstance"/>
    public DynamicStructuredDataTypeInstance(StructuredDataType structuredDataType, IFieldValues fieldValues)
    {
        _structuredDataType = structuredDataType;
        _fields = fieldValues.ToArray();
    }

    /// <inheritdoc />
    public StructuredDataType GetStructuredDataType() =>
        _structuredDataType;

    /// <inheritdoc />
    public IEnumerable<Field> CollectAllSetFields() =>
        _fields.Where(v => v.value != null).Select(v => v.field);

    /// <inheritdoc />
    public object? Get(Field field) => TryGet(field, out var result) ? result : throw new UnsetFieldException(field);

    /// <summary>
    /// Gets the <paramref name="value"/> of the given <paramref name="field"/> on <c>this</c>.
    /// </summary>
    /// <returns><c>true</c> if <paramref name="field"/> is set, <c>false</c> otherwise.</returns>
    /// <seealso cref="Get"/>
    public bool TryGet(Field field, [NotNullWhen(true)] out object? value)
    {
        value = _fields.FirstOrDefault(v => v.field == field).value;
        return value != null;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var result = new StringBuilder(_structuredDataType.Name);
        result.Append(" {");

        bool first = true;
        foreach (var field in _structuredDataType.Fields.OrderBy(f => f.Name))
        {
            if (!first)
                result.Append(',');
            first = false;

            result.Append(' ');
            result.Append(field.Name);
            result.Append(" = ");
            if (TryGet(field, out var value))
                result.Append(value);
        }

        result.Append(" }");

        return result.ToString();
    }

    /// <inheritdoc />
    public bool Equals(DynamicStructuredDataTypeInstance other) =>
        _structuredDataType.EqualsIdentity(other._structuredDataType) &&
        OrderWithValue(_fields).SequenceEqual(OrderWithValue(other._fields));

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(_structuredDataType.GetHashCodeIdentity());
        foreach (var field in OrderWithValue(_fields))
        {
            hashCode.Add(field.field.GetHashCodeIdentity());
            hashCode.Add(field.value);
        }

        return hashCode.ToHashCode();
    }

    private static IEnumerable<(Field field, object? value)> OrderWithValue(
        IEnumerable<(Field field, object? value)> fields) =>
        fields.Where(f => f.value != null).OrderBy(f => f.field.Key);
}
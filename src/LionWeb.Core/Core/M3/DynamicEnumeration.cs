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
using Notification.Partition.Emitter;
using System.Collections;
using Utilities;

/// <inheritdoc cref="Enumeration"/>
public class DynamicEnumeration(NodeId id, LionWebVersions lionWebVersion, DynamicLanguage? language)
    : DynamicDatatype(id, lionWebVersion, language), Enumeration
{
    private readonly List<DynamicEnumerationLiteral> _literals = [];

    /// <inheritdoc />
    public IReadOnlyList<EnumerationLiteral> Literals => _literals.AsReadOnly();

    /// <inheritdoc cref="Literals"/>
    public void AddLiterals(IEnumerable<DynamicEnumerationLiteral> literals)
    {
        var safeNodes = literals?.ToList();
        AssureNotNull(safeNodes, _m3.Enumeration_literals);
        AssureNotNullMembers(safeNodes, _m3.Enumeration_literals);
        if (_literals.SequenceEqual(safeNodes))
            return;
        foreach (var value in safeNodes)
        {
            ContainmentAddMultipleNotificationEmitter<DynamicEnumerationLiteral> emitter = new(_m3.Enumeration_literals, this, value, _literals, null);
            emitter.CollectOldData();
            if (AddLiteralsRaw(value))
                emitter.Notify();
        }
    }

    protected internal bool SetLiteralsRaw(List<DynamicEnumerationLiteral> nodes) => ExchangeChildrenRaw(nodes, _literals);
    protected internal bool AddLiteralsRaw(DynamicEnumerationLiteral? value) => AddChildRaw(value, _literals);
    private bool InsertLiteralsRaw(int index, DynamicEnumerationLiteral? value) => InsertChildRaw(index, value, _literals);
    private bool RemoveLiteralsRaw(DynamicEnumerationLiteral? value) => RemoveChildRaw(value, _literals);


    /// <inheritdoc />
    protected override bool DetachChild(INode child)
    {
        if (base.DetachChild(child))
        {
            return true;
        }

        var c = GetContainmentOf(child);
        if (c == _m3.Enumeration_literals)
            return _literals.Remove((DynamicEnumerationLiteral)child);

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
    protected internal override bool TryGetContainmentsRaw(Containment containment, out IReadOnlyList<IReadableNode> nodes)
    {
        if (base.TryGetContainmentsRaw(containment, out nodes))
            return true;
        
        if (_m3.Enumeration_literals.EqualsIdentity(containment))
        {
            nodes = _literals;
            return true;
        }

        return false;
    }
    /// <inheritdoc />
    protected internal override bool AddContainmentsRaw(Containment containment, IWritableNode node)
    {
        if (base.AddContainmentsRaw(containment, node))
            return true;
        
        if (_m3.Enumeration_literals.EqualsIdentity(containment)&& node is DynamicEnumerationLiteral literal)
        {
            return AddLiteralsRaw(literal);
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool InsertContainmentsRaw(Containment containment, Index index, IWritableNode node)
    {
        if (base.InsertContainmentsRaw(containment, index, node))
            return true;
        
        if (_m3.Enumeration_literals.EqualsIdentity(containment)&& node is DynamicEnumerationLiteral literal)
        {
            return InsertLiteralsRaw(index, literal);
        }

        return false;
    }

    /// <inheritdoc />
    protected internal override bool RemoveContainmentsRaw(Containment containment, IWritableNode node)
    {
        if (base.RemoveContainmentsRaw(containment, node))
            return true;
        
        if (_m3.Enumeration_literals.EqualsIdentity(containment)&& node is DynamicEnumerationLiteral literal)
        {
            return RemoveLiteralsRaw(literal);
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
                    AddLiterals(e.OfType<DynamicEnumerationLiteral>().ToArray());
                    return true;
                default:
                    throw new InvalidValueException(feature, value);
            }
        }

        return false;
    }
}
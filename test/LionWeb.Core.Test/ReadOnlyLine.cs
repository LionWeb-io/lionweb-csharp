﻿// Copyright 2024 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Test;

using Core.Utilities;
using Languages.Generated.V2024_1.Shapes.M2;
using M2;
using M3;
using System.Diagnostics.CodeAnalysis;

/// Example of a node that implements <see cref="IReadableNode"/>,
/// but neither <see cref="IWritableNode"/> nor <see cref="INode"/>.
/// Effectively provides the same read-API as <see cref="Line"/> 
public class ReadOnlyLine(string id, IReadableNode? parent)
    : ReadableNodeBase<IReadableNode>(id, parent), INamed
{
    private static new readonly IBuiltInsLanguage _builtIns = LionWebVersions.Current.BuiltIns;

    /// <inheritdoc/>
    public override Classifier GetClassifier() => ShapesLanguage.Instance.Line;

    /// <inheritdoc/>
    public override object? Get(Feature feature) => feature switch
    {
        null => GetAnnotations(),
        not null when _builtIns.INamed_name.EqualsIdentity(feature) => Name,
        not null when ShapesLanguage.Instance.IShape_uuid.EqualsIdentity(feature) => Uuid,
        not null when ShapesLanguage.Instance.IShape_fixpoints.EqualsIdentity(feature) => Fixpoints,
        not null when ShapesLanguage.Instance.Shape_shapeDocs.EqualsIdentity(feature) => ShapeDocs,
        not null when ShapesLanguage.Instance.Line_start.EqualsIdentity(feature) => Start,
        not null when ShapesLanguage.Instance.Line_end.EqualsIdentity(feature) => End,
        _ => throw new UnknownFeatureException(GetClassifier(), feature)
    };

    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        switch (feature)
        {
            case not null when _builtIns.INamed_name.EqualsIdentity(feature) && Name != null:
                value = Name;
                return true;
            case not null when ShapesLanguage.Instance.IShape_uuid.EqualsIdentity(feature) && Uuid != null:
                value = Uuid;
                return true;
            case not null when ShapesLanguage.Instance.IShape_fixpoints.EqualsIdentity(feature) &&
                               Fixpoints is { Count: > 0 }:
                value = Fixpoints;
                return true;
            case not null when ShapesLanguage.Instance.Shape_shapeDocs.EqualsIdentity(feature) && ShapeDocs != null:
                value = ShapeDocs;
                return true;
            case not null when ShapesLanguage.Instance.Line_start.EqualsIdentity(feature) && Start != null:
                value = Start;
                return true;
            case not null when ShapesLanguage.Instance.Line_end.EqualsIdentity(feature) && End != null:
                value = End;
                return true;
            default:
                value = null;
                return false;
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<Feature> CollectAllSetFeatures()
    {
        List<Feature> result = [];
        if (Name != default)
            result.Add(_builtIns.INamed_name);
        if (Uuid != default)
            result.Add(ShapesLanguage.Instance.IShape_uuid);
        if (Fixpoints != default && Fixpoints.Any())
            result.Add(ShapesLanguage.Instance.IShape_fixpoints);
        if (ShapeDocs != default)
            result.Add(ShapesLanguage.Instance.Shape_shapeDocs);
        if (Start != default)
            result.Add(ShapesLanguage.Instance.Line_start);
        if (End != default)
            result.Add(ShapesLanguage.Instance.Line_end);
        return result;
    }

    /// <see cref="INamed"/>
    public required string Name { get; init; }

    public bool TryGetName([NotNullWhen(true)] out string? name)
    {
        name = Name;
        return name != null;
    }

    /// <see cref="IShape"/>
    public required string Uuid { get; init; }

    public IReadOnlyList<Coord> Fixpoints { get; init; } = new List<Coord>().AsReadOnly();
    public Documentation? ShapeDocs { get; init; }

    /// <see cref="Line"/>
    public required Coord Start { get; init; }

    public required Coord End { get; init; }
}
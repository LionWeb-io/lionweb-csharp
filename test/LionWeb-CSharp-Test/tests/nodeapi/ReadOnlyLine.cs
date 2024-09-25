// Copyright 2024 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Utils.Tests;

using Core;
using Core.M2;
using Core.M3;
using Core.Utilities;
using Examples.Shapes.M2;

public class ReadOnlyLine(string id, IReadableNode<IReadableNode>? parent)
    : ReadableNodeBase<IReadableNode<IReadableNode>>(id, parent), INamed
{
    /// <inheritdoc/>
    public override Classifier GetClassifier() => ShapesLanguage.Instance.Line;

    /// <inheritdoc/>
    public override object? Get(Feature feature) => feature switch
    {
        null => GetAnnotations(),
        not null when BuiltInsLanguage.Instance.INamed_name.EqualsIdentity(feature) => Name,
        not null when ShapesLanguage.Instance.IShape_uuid.EqualsIdentity(feature) => Uuid,
        not null when ShapesLanguage.Instance.IShape_fixpoints.EqualsIdentity(feature) => Fixpoints,
        not null when ShapesLanguage.Instance.Shape_shapeDocs.EqualsIdentity(feature) => ShapeDocs,
        not null when ShapesLanguage.Instance.Line_start.EqualsIdentity(feature) => Start,
        not null when ShapesLanguage.Instance.Line_end.EqualsIdentity(feature) => End,
        _ => throw new UnknownFeatureException(GetClassifier(), feature)
    };

    /// <inheritdoc/>
    public override IEnumerable<Feature> CollectAllSetFeatures()
    {
        List<Feature> result = [];
        if (Name != default)
            result.Add(BuiltInsLanguage.Instance.INamed_name);
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

    /// <see cref="IShape"/>
    public required string Uuid { get; init; }

    public IReadOnlyList<Coord> Fixpoints { get; init; }
    public Documentation? ShapeDocs { get; init; }

    /// <see cref="Line"/>
    public required Coord Start { get; init; }

    public required Coord End { get; init; }
}
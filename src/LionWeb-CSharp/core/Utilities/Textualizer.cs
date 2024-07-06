// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

namespace LionWeb.Core.Utilities;

using M2;
using M3;

/// <summary>
/// Class whose extension method `AsString` produces a textualization of the given {@link INode node}.
/// </summary>
public static class Textualizer
{
    private const string _oneIndentation = "    ";

    private static IEnumerable<string> Indent(IEnumerable<string> strings)
        => strings.Select((str) => _oneIndentation + str);

    private static string Join(IEnumerable<string> strings, string separator)
        => string.Join(separator, strings.ToArray()) ?? "";


    /// <returns>a textualization of the given {@link INode node}.</returns>
    public static string AsString(this INode node)
        => Join(node.AsStrings(), "\n");


    private static IEnumerable<string> AsStrings(this INode node)
    {
        return
        [
            $"{node.GetClassifier().Name} (id: {node.GetId()}) {{",
            ..Indent([
                ..node.CollectAllSetFeatures().OfType<Property>().Select(PropertyAsString),
                ..node.CollectAllSetFeatures().OfType<Reference>().Select(ReferenceAsString),
                ..node.CollectAllSetFeatures().OfType<Containment>().SelectMany(ContainmentAsStrings),
                ..node.GetAnnotations().SelectMany(AnnotationContainmentAsStrings)
            ]),
            "}"
        ];

        string ValueAsString(object? value)
            => value switch
            {
                string @string => $"\"{@string}\"",
                _ => $"{value}" // TODO  enums? null?
            };

        string PropertyAsString(Property property)
            => $"{property.Name} = {ValueAsString(node.Get(property))}";

        string ReferenceTargetAsString(INode target)
            => $"{target.GetId()}{(target is INamed named ? $" ({named.Name})" : "")}";

        string ReferenceAsString(Reference reference)
        {
            var targets = reference.AsNodes<INode>(node.Get(reference));
            return
                $"{reference.Name} -> {(targets.Any() ? Join(targets.Select(ReferenceTargetAsString), ", ") : " <none>")}";
        }

        IEnumerable<string> ContainmentAsStrings(Containment containment)
        {
            var children = containment.AsNodes<INode>(node.Get(containment));
            return
            [
                $"{containment.Name}:{(children.Any() ? "" : " <none>")}",
                ..Indent(children.SelectMany(child => child.AsStrings()))
            ];
        }

        IEnumerable<string> AnnotationContainmentAsStrings(INode annotation)
        {
            var txt = annotation.AsStrings();
            return
            [
                "@ " + txt.First(),
                ..txt.Skip(1)
            ];
        }
    }
}
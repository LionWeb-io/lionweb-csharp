// Copyright 2026 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.M1;

using Serialization;
using System.Collections;
using System.Collections.Immutable;

/// <inheritdoc cref="ISerializer"/>
/// <summary>
/// Omits any nodes (both the node itself an all links to it) from serialization that don't pass <paramref name="filter"/>.
/// </summary>
public class FilteringSerializer(LionWebVersions lionWebVersion, Func<IReadableNode, bool> filter) : Serializer(lionWebVersion)
{
    /// <inheritdoc />
    protected override SerializedNode? SerializeNode(IReadableNode node) =>
        filter(node) ? base.SerializeNode(node) : null;


    /// <inheritdoc />
    protected override object? AdjustFeatureValue(object? value)
    {
        switch (value)
        {
            case null:
                return null;
            
            case string:
                return value;

            case IReadableNode n:
                return filter(n) ? n : Enumerable.Empty<IReadableNode>();
            
            case ReferenceTarget t:
                return t.Target is null || filter(t.Target) ? t: Enumerable.Empty<ReferenceTarget>();

            case IEnumerable enumerable:
                List<IReadableNode>? nodeList = null;
                List<ReferenceTarget> referenceList = null;
                List<object>? otherList = null;
                foreach (var element in enumerable)
                {
                    switch (element)
                    {
                        case IReadableNode when otherList is not null || referenceList is not null:
                            throw new SerializerException($"both node and other values in one feature: {element}; {otherList}; {referenceList}");
                        case IReadableNode rn:
                            {
                                nodeList ??= [];

                                if (filter(rn))
                                    nodeList.Add(rn);
                                break;
                            }
                        
                        case ReferenceTarget when otherList is not null || nodeList is not null:
                            throw new SerializerException($"both reference and other values in one feature: {element}; {otherList}; {nodeList}");
                        case ReferenceTarget rt:
                            {
                                referenceList ??= [];

                                if (rt.Target is null || filter(rt.Target))
                                    referenceList.Add(rt);
                                break;
                            }
                        
                        case not null when nodeList is not null || referenceList is not null:
                            throw new SerializerException($"both other value and nodes in one feature: {element}; {nodeList}; {referenceList}");
                        case not null:
                            {
                                otherList ??= [];

                                otherList.Add(element);
                                break;
                            }
                    }
                }

                return nodeList ?? referenceList ?? otherList ?? enumerable;

            default:
                return value;
        }
    }

    /// <inheritdoc />
    protected override IReadOnlyList<IReadableNode> CollectAnnotations(IReadableNode node) =>
        base.CollectAnnotations(node).Where(filter).ToImmutableList();
}
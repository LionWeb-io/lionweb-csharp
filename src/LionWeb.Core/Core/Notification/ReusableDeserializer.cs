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

namespace LionWeb.Core.Notification;

using M1;
using M2;
using M3;

/// <summary>
/// Reusable Deserializer to minimize initialization overhead.
///
/// <para/>
/// Workflow:
/// 
/// <list type="number">
/// <item><see cref="Reset"/></item>
/// <item><see cref="Progress{T}"/></item>
/// <item><see cref="Deserializer.Finish"/></item>
/// </list>
/// </summary>
public class ReusableDeserializer : Deserializer
{
    private readonly SharedNodeMap _sharedNodeMap;

    public ReusableDeserializer(SharedNodeMap sharedNodeMap, DeserializerBuilder builder) : base(builder.LionWebVersion, builder.Handler)
    {
        _sharedNodeMap = sharedNodeMap;
        ResolveInfoHandling = builder.ResolveInfoHandling;
        foreach ((Language language, INodeFactory factory) in builder.Languages)
        {
            RegisterInstantiatedLanguage(language, factory);
        }

        if (builder.LanguageReferences)
        {
            base.RegisterDependentNodes(builder.Languages.Keys.SelectMany(k =>
                M1Extensions.Descendants<IReadableNode>(k, true, true)));
        }
    }

    /// <summary>
    /// Resets internal state to be ready for a new round of deserialization.
    /// </summary>
    public void Reset()
    {
        _containmentsByOwnerId.Clear();
        _referencesByOwnerId.Clear();
        _annotationsByOwnerId.Clear();
        _deserializedNodesById.Clear();
    }

    /// <inheritdoc />
    protected override bool IsInDependentNodes(NodeId nodeId) => 
        _sharedNodeMap.ContainsKey(nodeId) || base.IsInDependentNodes(nodeId);

    /// <inheritdoc />
    protected override IReadableNode? LookupDependentNodeByIdOrDefault(NodeId nodeId) => 
        _sharedNodeMap.TryGetValue(nodeId, out var result)
            ? result
            : base.LookupDependentNodeByIdOrDefault(nodeId);

    /// <inheritdoc />
    protected override IEnumerable<IReadableNode> CollectDependentNodes() => 
        _sharedNodeMap.Values.Concat(base.CollectDependentNodes());
}
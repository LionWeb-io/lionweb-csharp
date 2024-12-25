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

namespace LionWeb.Core.M1;

using M3;

/// <summary>
/// Callbacks to customize a <see cref="ISerializer"/>'s behaviour in non-regular situations.
///
/// <para>
/// Each method of this interface is one callback. It should provide all relevant information as parameters.
/// If the method returns non-null, the returned value is used to <i>heal</i> the issue; otherwise, the offender is skipped (if possible).
/// </para>
/// </summary>
public interface ISerializerHandler
{
    /// <summary>
    /// Language <paramref name="a"/> is <see cref="LionWeb.Core.Utilities.LanguageIdentityComparer">semantically identical</see>
    /// to language <paramref name="b"/>, but they are different C# objects. 
    /// </summary>
    /// <param name="a">One semantically identical language.</param>
    /// <param name="b">Other semantically identical language.</param>
    /// <returns>The language to use, if any.</returns>
    Language? DuplicateUsedLanguage(Language a, Language b);

    /// <summary>
    /// Node with same <see cref="IReadableNode.GetId">node id</see> has already been serialized.
    /// </summary>
    /// <param name="n">Node with duplicate id.</param>
    /// <remarks>
    /// It makes no sense to allow "healing" a duplicate id (e.g. by creating a new id).
    /// If we allowed this, we'd need to keep a map of all nodes to their (potentially "healed") id,
    /// in case we wanted to refer to it.
    /// This would clash with streaming nodes with minimal memory overhead.
    /// </remarks>
    void DuplicateNodeId(IReadableNode n);

    /// <summary>
    /// <paramref name="node"/> contains <paramref name="value"/> in <paramref name="property"/>,
    /// and we don't know how to serialize it.
    /// </summary>
    /// <param name="node">Node that contains unserializable <paramref name="value"/>.</param>
    /// <param name="property">Property that contains unserializable <paramref name="value"/>.</param>
    /// <param name="value">The value we don't know how to serialize.</param>
    /// <returns><paramref name="value"/> as string, will be written verbatim to JSON;
    /// or <c>null</c> to skip serializing this property.</returns>
    string? UnknownDatatype(IReadableNode node, Feature property, object? value);
}
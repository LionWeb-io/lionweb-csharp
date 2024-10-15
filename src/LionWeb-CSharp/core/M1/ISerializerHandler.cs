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

public interface ISerializerHandler
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>The language to use, if any.</returns>
    Language? DuplicateUsedLanguage(Language a, Language b);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="n"></param>
    /// <remarks>
    /// It makes no sense to allow "healing" a duplicate id (e.g. by creating a new id).
    /// If we allowed this, we'd need to keep a map of all nodes to their (potentially "healed") id,
    /// in case we wanted to refer to it.
    /// This would clash with streaming nodes with minimal memory overhead.
    /// </remarks>
    void DuplicateNodeId(IReadableNode n);

    string? UnknownDatatype(IReadableNode node, Feature property, object? value);
}

public class SerializerExceptionHandler : ISerializerHandler
{
    /// <inheritdoc />
    public void DuplicateNodeId(IReadableNode n) =>
        throw new ArgumentException($"nodes have same id '{n.GetId()}': {n}");

    /// <inheritdoc />
    public virtual Language? DuplicateUsedLanguage(Language a, Language b) =>
        throw new ArgumentException(
            $"different languages with same key '{a?.Key ?? b?.Key}' / version '{a?.Version ?? b?.Version}': {a}, {b}");

    /// <inheritdoc />
    public string? UnknownDatatype(IReadableNode node, Feature property, object? value) =>
        throw new ArgumentException($"unsupported property: {property}", nameof(property));
}

public class SerializerIgnoringHandler : ISerializerHandler
{
    /// <inheritdoc />
    public void DuplicateNodeId(IReadableNode n) =>
        Console.WriteLine($"nodes have same id '{n.GetId()}': {n}");

    /// <inheritdoc />
    public virtual Language? DuplicateUsedLanguage(Language a, Language b)
    {
        Console.WriteLine(
            $"different languages with same key '{a?.Key ?? b?.Key}' / version '{a?.Version ?? b?.Version}': {a}, {b}");
        return a;
    }

    public string? UnknownDatatype(IReadableNode node, Feature property, object? value)
    {
        Console.WriteLine($"unsupported property: {property}", nameof(property));
        return null;
    }
}
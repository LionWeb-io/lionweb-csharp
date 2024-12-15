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

/// Logs and ignores any kind of callback.
public class SerializerIgnoringHandler : ISerializerHandler
{
    /// <inheritdoc />
    public void DuplicateNodeId(IReadableNode n) =>
        LogMessage($"nodes have same id '{n.GetId()}': {n}");

    /// <inheritdoc />
    public virtual Language? DuplicateUsedLanguage(Language a, Language b)
    {
        LogMessage(
            $"different languages with same key '{a?.Key ?? b?.Key}' / version '{a?.Version ?? b?.Version}': {a}, {b}");
        return a;
    }

    /// <inheritdoc />
    public string? UnknownDatatype(IReadableNode node, Feature property, object? value)
    {
        LogMessage($"unsupported property: {property}");
        return null;
    }

    /// Writes <paramref name="message"/> to the appropriate logging facility.
    protected virtual void LogMessage(string message) =>
        Console.WriteLine(message);
}
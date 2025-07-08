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

/// Throws some variant of <see cref="LionWebExceptionBase"/> for any callback.
public class SerializerExceptionHandler : ISerializerHandler
{
    /// <inheritdoc />
    public void DuplicateNodeId(IReadableNode n) =>
        throw new SerializerException($"nodes have same id '{n.GetId()}': {n}");

    /// <inheritdoc />
    public virtual Language? DuplicateUsedLanguage(Language a, Language b) =>
        throw new SerializerException(
            $"different languages with same key '{a?.Key ?? b?.Key}' / version '{a?.Version ?? b?.Version}': {a}, {b}");

    /// <inheritdoc />
    public PropertyValue? UnknownDatatype(IReadableNode node, Feature property, object? value) =>
        throw new SerializerException($"unsupported property: {property}");
}
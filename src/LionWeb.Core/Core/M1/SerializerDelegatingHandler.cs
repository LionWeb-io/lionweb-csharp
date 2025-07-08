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

namespace LionWeb.Core.M1;

using M3;

/// Delegates all calls to <paramref name="delegateHandler"/>.
public class SerializerDelegatingHandler(ISerializerHandler delegateHandler) : ISerializerHandler
{
    /// <inheritdoc />
    public virtual Language? DuplicateUsedLanguage(Language a, Language b) =>
        delegateHandler.DuplicateUsedLanguage(a, b);

    /// <inheritdoc />
    public virtual void DuplicateNodeId(IReadableNode n) =>
        delegateHandler.DuplicateNodeId(n);

    /// <inheritdoc />
    public virtual PropertyValue? UnknownDatatype(IReadableNode node, Feature property, object? value) =>
        delegateHandler.UnknownDatatype(node, property, value);
}
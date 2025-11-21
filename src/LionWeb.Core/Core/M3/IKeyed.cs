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

namespace LionWeb.Core.M3;

using M2;
using System.Diagnostics.CodeAnalysis;

/// Something with a name that has a key.
public interface IKeyed : INamed
{
    /// A Key must be a valid <see cref="IReadableNode.GetId">identifier</see>.
    /// It must be unique within its language.
    public MetaPointerKey Key { get; }

    /// <summary>
    /// Gets the <see cref="Key"/>.
    /// </summary>
    /// <param name="key">Value of <see cref="Key"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Key"/> is set; <c>false</c> otherwise.</returns>
    bool TryGetKey([NotNullWhen(true)] out MetaPointerKey? key);
}
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

namespace LionWeb.Generator;

/// Configures the LionWeb C# types generator.
public record GeneratorConfig
{
    /// Whether generated interfaces should inherit from <see cref="Core.INode"/> (if set to <c>true</c>)
    /// or <see cref="Core.IReadableNode"/> (if set to <c>false</c>).
    /// Defaults to <c>true</c>.
    public bool WritableInterfaces { get; init; } = true;

    public UnresolvedReferenceHandling UnresolvedReferenceHandling { get; init; } =
        UnresolvedReferenceHandling.ThrowIfPresent;
}

public enum UnresolvedReferenceHandling
{
    ReturnAsNull,
    ThrowIfPresent
}
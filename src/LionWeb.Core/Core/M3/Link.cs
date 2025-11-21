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

using System.Diagnostics.CodeAnalysis;

/// Represent a connection to a <see cref="Classifier"/>.
public interface Link : Feature
{
    /// A <i>multiple</i> link can have several values.
    /// A non-multiple, i.e. <i>single</i> link can have only one value.
    public bool Multiple { get; }

    /// <summary>
    /// Gets the <see cref="Multiple"/>.
    /// </summary>
    /// <param name="multiple">Value of <see cref="Multiple"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Multiple"/> is set; <c>false</c> otherwise.</returns>
    bool TryGetMultiple([NotNullWhen(true)] out bool? multiple)
    {
        multiple = Multiple;
        return multiple != null;
    }

    /// LionWeb type of this link.
    public Classifier Type { get; }

    /// <summary>
    /// Gets the <see cref="Type"/>.
    /// </summary>
    /// <param name="type">Value of <see cref="Type"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Type"/> is set; <c>false</c> otherwise.</returns>
    bool TryGetType([NotNullWhen(true)] out Classifier? type);
}
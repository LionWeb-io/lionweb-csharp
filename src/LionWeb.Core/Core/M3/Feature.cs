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

/// A Feature represents a characteristic or some form of data associated with a particular <see cref="Classifier"/>.
public interface Feature : IKeyed
{
    /// An <i>optional</i> feature can be <c>null</c> (or empty for <see cref="Link.Multiple">multiple links</see>).
    /// A non-optional, i.e. <i>required</i> feature can NOT be <c>null</c> or empty.  
    public bool Optional { get; }

    /// <summary>
    /// Gets the <see cref="Optional"/>.
    /// </summary>
    /// <param name="optional">Value of <see cref="Optional"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Optional"/> is set; <c>false</c> otherwise.</returns>
    bool TryGetOptional([NotNullWhen(true)] out bool? optional)
    {
        optional = Optional;
        return optional != null;
    }
}
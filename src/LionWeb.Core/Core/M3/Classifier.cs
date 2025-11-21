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

/// Something which can own <see cref="Feature">Features</see>.
public interface Classifier : LanguageEntity
{
    /// <see cref="Feature">Features</see> owned by this classifier.
    public IReadOnlyList<Feature> Features { get; }

    /// <summary>
    /// Gets the <see cref="Features"/>.
    /// </summary>
    /// <param name="features">Value of <see cref="Features"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Features"/> is set and not empty; <c>false</c> otherwise.</returns>
    bool TryGetFeatures([NotNullWhen(true)] out IReadOnlyList<Feature>? features)
    {
        features = Features;
        return features is { Count: > 0 };
    }
}
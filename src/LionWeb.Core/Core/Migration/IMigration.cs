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

namespace LionWeb.Core.Migration;

public interface IMigration
{
    int Priority { get; }

    void Initialize(ILanguageRegistry languageRegistry);

    bool IsApplicable(ISet<LanguageIdentity> languageIdentities);

    // TODO: Maybe rename to Execute / Run / Apply?
    // What happens if return=false and outputRootNodes != inputRootNodes?
    MigrationResult Migrate(List<LenientNode> inputRootNodes);
    
    // List<LenientNode> Apply(List<LenientNode> inputRootNodes, out bool changed);

    public const int DefaultPriority = 10_000;
}

public record MigrationResult(bool changed, List<LenientNode> outputRootNodes);

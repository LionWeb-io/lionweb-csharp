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

// ReSharper disable InconsistentNaming

namespace LionWeb.Core.VersionSpecific.V2025_1;

/// <inheritdoc cref="IVersion2025_1" />
internal sealed class Version2025_1 : VersionBase<IBuiltInsLanguage_2025_1, ILionCoreLanguage_2025_1>,
    IVersion2025_1
{
    internal static readonly Version2025_1 Instance = new Lazy<Version2025_1>(() => new()).Value;
    private Version2025_1() { }

    public override string VersionString => "2025.1";

    public override IBuiltInsLanguage_2025_1 BuiltIns => BuiltInsLanguage_2025_1.Instance;

    public override ILionCoreLanguage_2025_1 LionCore => LionCoreLanguage_2025_1.Instance;
}
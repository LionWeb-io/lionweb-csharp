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

namespace LionWeb.Core.VersionSpecific.V2024_1_Compatible;

using V2024_1;

/// <inheritdoc cref="IVersion2024_1_Compatible" />
internal sealed class Version2024_1_Compatible : VersionBase<IBuiltInsLanguage_2024_1, ILionCoreLanguage_2024_1>,
    IVersion2024_1_Compatible
{
    internal static readonly Version2024_1_Compatible Instance = new Lazy<Version2024_1_Compatible>(() => new()).Value;
    private Version2024_1_Compatible() { }

    public override string VersionString => "2024.1";

    public override IBuiltInsLanguage_2024_1 BuiltIns => BuiltInsLanguage_2024_1.Instance;

    public override ILionCoreLanguage_2024_1 LionCore => LionCoreLanguage_2024_1.Instance;

    public override bool IsCompatibleWithInternal(LionWebVersions other) =>
        base.IsCompatibleWithInternal(other) ||
        ReferenceEquals(other, LionWebVersions.v2023_1) ||
        ReferenceEquals(other, LionWebVersions.v2024_1);
}